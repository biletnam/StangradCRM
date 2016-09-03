<?php

/**
 * PHP QueryBuilder v 0.1
 * Все запросы экземпляра выполнятся в одной транзакции.
 * Функции insert, update, delete завершают формирование текущего запроса и начинают формировать новый
 * Формирование нового запроса происходит при вызове функций insert, update, delete. Перед вызовом 
 * этих функций необходимо указать таблицу методом forTable
 * Если функция forTable не была вызвана - запрос будет формироваться для первой таблицы
 * end завершает формирование текущего запроса
 * Select запросы необходимо разделять функцией end
 * Результаты выполнения запросов сохраняются в кэше, получить результаты можно по индексу выполнения запроса
 * Реузльтат выполнения одного запроса можно передать в другой запрос функцией ...
 * Функции one, all, exec завершают формирование текущего запроса и отпраляют на выполнение
 */

namespace core\models\sql;
use core\models\sql\PrepareOperation;
use Exception;

class QueryBuilder extends \core\models\AbstractQueryBuilder {

    private $caller = null; //Класс модели (для ORM), для которого формируется запрос
    private $connection = null;


    private $firstTable = null; //Первая указанная таблица (указывается в конструкторе)
    private $currentTable = null; //Таблица, с которой ведется работа на настоящий момент
    protected $queries = []; //Массив запросов
    
    private $resultCache = [];

    private $isGroup = false;
    private $groups = [];

    private $tables = [];
    private $operations = [
        'select' => null,
        'insert' => null,
        'update' => null,
        'delete' => null,
        'join' => null,
        'where' => null,
        'order' => null,
        'group' => null,
        'limit' => null
    ];

    //Конструктор
    public function __construct($table, $caller = null, $connection = null) {
        $this->firstTable = $table;
        $this->caller = $caller;
        $this->connection = $connection;
    }
    
    /*Формирование полей для выборки
     * 
     * 
    */

    
    public function test_select ($fields, array $tables = null) {
        $tables = ((is_null($tables)) ? [$this->currentTable()] : $tables);  
        $result = $this->select_fields($fields, $tables);
        $this->prepareSelect($result[0], $tables, $result[1]);
        return $this;
    }

    private function prepareSelect($fields, array $tables, array $values = []) {
        $operation = $this->operation('select');
        $operation->add($fields, $values);
        for($i = 0; $i < count($tables); $i++) {
            $operation->addTable($tables[$i]);
        }
        return $this;
    }

    public function select($fields, $tables = null, $aliases = []) {
        $tables = (is_null($tables)) ? $this->currentTable() : $tables;  
        
        if(is_array($fields)) {

            if(is_array($tables)) {
                if(count($tables) != count($fields)) {
                    throw new Exception('Размерность массива полей не совпадает с размерностью массива таблиц.');
                }
                for($i = 0; $i < count($fields); $i++) {
                    $field = is_array($fields[$i]) ? $fields[$i] : [$fields[$i]];
                    
                    
                    $prevCount = ($i == 0) ? 0 : count($fields[$i-1]);
                    $a = array_slice($aliases, $prevCount, count($field));
                    
                    //$aliase = $this->parse_aliases($field);
                    
                    $this->_select($field, $tables[$i], $a);
                }
            }
            else {
                
                $this->_select($fields, $tables, $aliases);
            }
        }
        else {
            if(is_array($tables)) {
                for($i = 0; $i < count($tables); $i++) {
                    $this->_select([$fields], $tables[$i], [$aliases[$i]]);
                }
            }
            else {
                $this->_select([$fields], $tables, $aliases);
            }
        }
        return $this;
    }
    
    

    private function _select($fields, $table, $aliases) {
        $values = null;
        for($i = 0; $i < count($fields); $i++) {
            if($fields[$i] instanceof QueryBuilder) {
                $query = $fields[$i]->queries[0][0];
                $val = $fields[$i]->queries[0][1];
                $fields[$i] = '(' . $query . ')';
                $values = ($values == null) ? $val : array_merge($values, $val);
            }
            else {
                $fields[$i] = $table . '.' . $fields[$i];
            }
            
            if(isset($aliases[$i]) && $aliases[$i] != null) {
                $fields[$i] .= " as '" . $aliases[$i] . "'";
            }
        }
        $this->tables[$table] = $table;
        $operation = $this->operation('select');
        $operation->add($fields, $values);
        $operation->addTable($table);
    }

    //Формирование полей для добавления
    public function insert($fields, $values) {
        $table = $this->currentTable();
        //array_walk($fields, 'core\model\sql\QueryBuilder::addPrefix', $table . '.');
        $operation = $this->operation('insert');
        $operation->add($fields, $values);
        $operation->addTable($table);
        return $this;
    }
    
    //Формирование полей для обновления
    public function update($fields, $values) {
        $table = $this->currentTable();
        //array_walk($fields, 'core\model\sql\QueryBuilder::addPrefix', $table . '.');
        array_walk($fields, 'core\models\sql\QueryBuilder::addPostfix', ' = ? ');
        $operation = $this->operation('update');
        for($i = 0; $i < count($values); $i++) {
            if($values[$i] == 'NULL') {
                $values[$i] = null;
            }
        }
        $operation->add($fields, $values);
        $operation->addTable($table);
        return $this;
    }

    //Формирование полей для удаления
    public function delete() {
        $table = $this->currentTable();
        $operations = $this->operation('delete');
        $operations->add(null);
        $operations->addTable($table);
        return $this;
    }

    //Подготовка запроса where
    public function where($field, $value, $condition = '=') {
        if(is_array($field)) {
            for($i = 0; $i < count($field); $i++) {
                if(is_array($value)) {
                    if(count($value) != count($field)) {
                        throw new Exception('Размерность массива значений не совпадает с размерностью массива полей');
                    }
                    $this->_where($field[$i], $value[$i], $condition, 'and');
                }
                else {
                    $this->_where($field[$i], $value, $condition, 'and');
                }
            }
            return $this;
        }
        return $this->_where($field, $value, $condition, 'and');
    }
    
    //Подготовка запроса where с логической операцией ИЛИ
    public function orWhere($field, $value, $condition = '=') {
        return $this->_where($field, $value, $condition, 'or');
    }
    
    //Формирование условия WHERE (TODO: Like, Between) 
    private function _where($field, $value, $condition, $lo) {
        $table = $this->currentTable();
        if($this->isGroup != false) {
            $current = $this->groups['where'][count($this->groups['where'])-1];
            if(!($current instanceof PrepareOperation)) {
                $current = new PrepareOperation('where');
            }
            $this->groups['where'][count($this->groups['where'])-1] = $current;
            $operation = $current;
        }
        else {
            $operation = $this->operation('where');
        }
        $condition = strtolower($condition);
        if($value == 'null' || $value == '!null') {
            $value = ($value == 'null') ? $value : 'not null';
            $field = $table . '.' . $field . ' is ' . $value;
            $value = null;
        }
        else if(is_array($value)) {
            $condition = ($condition == '!in') ? 'not in' : 'in';
            $field = $table . '.' . $field . ' ' . $condition . ' (' . str_repeat('?, ', count($value)-1) . ' ?)';
        }
        else if($value instanceof QueryBuilder) {           
            $condition = ($condition == '!in') ? 'not in' : 'in';
            $field = $table . '.' . $field . ' ' . $condition . ' (' . $value->queries[0][0] . ')';
            $value = $value->queries[0][1];
        }
        else if($value instanceof PrepareOperation) {
            $res = $value->buildNoQuery();
            $field = '(' . $res[0] . ')';
            $value = $res[1];
        }
        else {
            $field = $table . '.' . $field . ' ' . $condition . ' ?';
        }
        $operation->add($field, $value, $lo);
        return $this;
    }

    //Формирование порядка сортировки
    public function order($fields, $condition = 'asc') {
        $table = $this->currentTable();
        $condition = ($condition == 'asc') ? 'asc' : 'desc';
        $operation = $this->operation('order');
        if(is_array($fields)) {
            for($i = 0; $i < count($fields); $i++) {
                $field = $table . '.' . $fields[$i] . ' ' . $condition;
                $operation->add($field);
            }
        }
        else {
            $field = $table . '.' . $fields . ' ' . $condition;
            $operation->add($field);
        }
        $operation->addTable($table);
        return $this;
    }
    
    //Формирование группировки
    public function group(array $fields) {
        $table = $this->currentTable();
        $operation = $this->operation('group');
        if(is_array($fields)) {
            for($i = 0; $i < count($fields); $i++) {
                $field = $table . '.' . $fields[$i];
                $operation->add($field);
            }
        }
        else {
            $field = $table . '.' . $fields;
            $operation->add($field);
        }
        $operation->addTable($table);
        return $this;
    }
    
    //Формирование лимита возвращаемых данных
    public function limit($offset, $length) {
        $operation = $this->operation('limit');
        $fields = ['?', '?'];
        $operation->add($fields, [(int)$offset, (int)$length]);
        return $this;
    }
    
    //Формирование функции
    public static function func($func, $field, $table) {
        //return $func . '(' . $table . '.' . $field . ')';
    }
    
    //Произвольный запрос
    public function custom($queryString, $values, $crud = 'select') {
        $this->queries[] = ['query' => $queryString, 'values' => $values, 'type' => $crud];
        return $this;
    }


    //Указывает текующую таблицу
    public function forTable($table) {
        $this->addTable($table);
        $this->currentTable = $table;
        return $this;
    }

    //Удаляет текущую таблицу
    public function end() {
        $this->currentTable = null;
        $this->build();
        return $this;
    }
    
    public function beginGroup($condition = 'and') {
        if($this->isGroup == true) {
            $this->endGroup();
        }
        $this->isGroup = ($condition != 'and') ? 'or' : $condition;
        if(!isset($this->groups['where'])) {
            $this->groups['where'] = [];
        }
        $this->groups['where'][] = null;
        return $this;
    }
    
    public function endGroup() {
        $condition = $this->isGroup;
        $this->isGroup = false;
        if($condition == 'and') {
            $this->where(null, $this->groups['where'][count($this->groups['where'])-1]);
        }
        else {
            $this->orWhere(null, $this->groups['where'][count($this->groups['where'])-1]);
        }
        return $this;
    }

    /*
    * Первые элементы массивов $tables и $keys - главная таблица и первичный ключ соответственно
    * Все остальные таблицы и ключи - подчиненные к главной таблице по указанным ключам
    */
    
    public function relation(array $tables, array $keys) {
        if(count($tables) != count($keys) || count($tables) <= 1) {
            die('Uncorrect relation');
        }
        $this->addTable($tables);
        $primaryTable = $tables[0] . '.' . $keys[0];
        $operation = $this->operation('where');
        for($i = 1; $i < count($tables); $i++) {
            $operation->add($primaryTable . '=' . $tables[$i] . '.' . $keys[$i], null, 'and');
            $operation->addTable($tables[$i]);
        }
        return $this;
    }

    //Отправляет запрос-выборку выполнение. Получение одной (первой) записи
    public function one() {
        $this->build();
        $executor = new SQLExecutor($this->queries, $this->connection);
        return $executor->one($this->caller);
    }
    
    
    //Отправляет запрос-выборку на выполнение. Получение множества записей
    public function all() {
        $this->build();        
        $executor = new SQLExecutor($this->queries, $this->connection);       
        return $executor->all($this->caller);
    }

    //Отправляет запрос на выполнение. 
    public function exec() {        
        $this->build();
        $executor = new SQLExecutor($this->queries, $this->connection);       
        return $executor->exec($this->caller);
    }
    
    public static function lastError () {
        return SQLExecutor::$lastError;
    }

    private static function addPrefix(&$item, $key, $prefix) {
        $item = $prefix . $item;
    }

    private static function addPostfix(&$item, $key, $postfix) {
        $item = $item . $postfix;
    }

    public function build() {
        $query = '';
        $values = [];
        $crud = null;
        foreach($this->operations as $operationType => $operation) {
            if($operation != null) {
                if($crud == null) {
                    $crud = $operationType;
                }
                $result = $operation->build();
                $query .= $result[0];
                $values = array_merge($values, $result[1]);
            }
        }
        if($crud === null) {
            return;
        }
        $this->queries[] = ['query' => $query, 'values' => $values, 'type' => $crud];
        foreach ($this->operations as $key => $value) {
            $this->operations[$key] = null;
        }
    }

    private function currentTable() {
        return ($this->currentTable == null) ? $this->firstTable : $this->currentTable;
    }
    
    private function addTable($table) {
        if(!isset($this->operations['select'])) {
            return;
        }
        if(is_array($table)) {
            for($i = 0; $i < count($table); $i++) {
                $this->operations['select']->addTable($table[$i]);
            }
        }
        else {
            $this->operations['select']->addTable($table);
        }
    }

    private function operation($type) {
        if(!($this->operations[$type] instanceof PrepareOperation)) {
            $this->operations[$type] = new PrepareOperation($type);
        }
        return $this->operations[$type];
    }

    public function getResultCache() {
        $localCache = $this->resultCache;
        $this->clearResultCache();
        return $localCache;
    }
    
    public function clearResultCache() {
        $this->resultCache = [];
    }

    public static function query($table, $caller = null) {
        return new QueryBuilder($table, $caller);
    }

    //Test
    public function showQueries() {
        $this->build();
        for($i = 0; $i < count($this->queries); $i++) {
            echo "query : " . $this->queries[$i]['query'] . '<br>';
            echo "values: ";
            var_dump($this->queries[$i]['values']);
        }
    }    
}

