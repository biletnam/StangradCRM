<?php

namespace core\models;

class Model {

    private $caller = null;
    private $properties = [];
    protected static $model = [];
    protected static $schema = [];

    private function __clone() {}

    protected function __construct() {
        $caller = get_called_class();
        $this->caller = $caller;
        $this->createModelFields();
        //$this->prepareRelations();
        //$this->prepareReferences();
    }
    
    public static function create($id = 0) {
        if($id == 0) {
            return new static();
        }
        else {
            if(isset(static::$model[static::table()][$id])) {
                return static::$model[static::table()][$id];
            }
            else {
                return static::get()->where('id', $id)->one();
            }
        }
    }
    
    public static function table() {
        $className = explode('\\', get_called_class());
        return strtolower(array_pop($className));
    }

    public static function connection () {
        return \configs\Connection::db();
    }

    public static function fields() {
        $table = static::table();
        static::createSchema();
        return static::$schema[$table]->fields();
    }

    private static function createSchema() {
        $table = static::table();
        if(!isset(static::$schema[$table])) {
            try {
                $schema  = Schema::getSchema(static::connection());
                static::$schema[$table] = new $schema(static::table(), static::connection());
            }
            catch (Exception $ex) {
                die($ex->getMessage());
            }
        }
    }

    //Experemental functions------------->
    public static function with($table) {
        
    }
    public static function deleteForId($id) {
        $constructor = QueryBuilder::constructor(static::connection());
        $operation = new $constructor(static::table(), static::table(), static::connection());
        return $operation->delete()->where('id', (int)$id)->exec();
    }

    public static function count() {
        $constructor = QueryBuilder::constructor(static::connection());
        $operation = new $constructor(null, null, static::connection());
        $result = $operation->custom('select count(id) as allcount from ' . static::table(), [])->one();
        return $result[0]['allcount'];
    }

    public static function getFieldType($fieldName) {
        static::createSchema();
        return static::$schema[static::table()]->getFieldType($fieldName);
    }

    public static function isPrimaryKey($fieldName) {
        static::createSchema();
        return static::$schema[static::table()]->isPrimaryKey($fieldName);
    }
    
    public static function isForeignKey($fieldName) {
        static::createSchema();
        return static::$schema[static::table()]->isForeignKey($fieldName);
    }

    public static function referenceData($fieldName) {
        static::createSchema();
        return static::$schema[static::table()]->references($fieldName);
    }

    public static function referenceModel($fieldName) {
        $data = static::referenceData($fieldName);
        if(!$data) {
            return null;
        }
        return 'user\\models\\' . ucfirst($data['table']);
    }

    public static function pk() {
        static::createSchema();
        return static::$schema[static::table()]->pk();
    }
    
    public static function issetField ($fieldName) {
        $fields = static::fields();
        $searchResult = array_search($fieldName, $fields);
        if($searchResult !== false) {
            return true;
        }
        return $searchResult;
    }

    public static function referenceRenderColumn () {
        return null;
    }
    
    public static function schema () {
        static::createSchema();
        return static::$schema[static::table()];
    }

    //<---------------------------
    public static function get($fields = null) {
        $caller = get_called_class();
        if($fields == null) {
             $fields = static::fields();
        }        
        $classPath = explode('\\', $caller);
        $constructor = QueryBuilder::constructor(static::connection());
        $operation = new $constructor(static::table(), $caller, static::connection());
        $operation->select($fields);
        return $operation;
    }

    public static function getModel() {
        return static::$model[static::table()];
    }

    private static function addToModel($id, $instance) {
        static::$model[static::table()][(int)$id] = $instance;
    }

    public function save() {
        if($this->Id != null) {
            return $this->update();
        }
        return $this->insert();
    }
    
    private function insert() {
        $data = [];
        for($i = 0; $i < count($this->properties); $i++) {
            $property = \system\libs\FormatModelPropertyName::format($this->properties[$i]);
            $data[$i] = $this->$property;
        }
        
        $constructor = QueryBuilder::constructor(static::connection());
        $operation = new $constructor(static::table(), $this->caller, static::connection());
        $result = $operation->insert($this->properties, $data)->exec();
        if($result == false) {
            $error = $operation::lastError();
            return [1, $error->getMessage()];
        }
        return [0, $result];
    }
    
    
    private function update() {
        $data = [];
        for($i = 0; $i < count($this->properties); $i++) {
            $property = \system\libs\FormatModelPropertyName::format($this->properties[$i]);
            $data[$i] = $this->$property;
        }

        $constructor = QueryBuilder::constructor(static::connection());
        $operation = new $constructor(static::table(), $this->caller, static::connection());
        $result = $operation->update($this->properties, $data)->where('id', $this->Id)->exec();
        if($result == false) {
            $error = $operation::lastError();
            return [1, $error->getMessage()];
        }
        return [0];
    }
    
    public function delete() {
        $constructor = QueryBuilder::constructor(static::connection());
        $operation = new $constructor(static::table(), $this->caller, static::connection());
        $result = $operation->delete()->where('id', $this->Id)->exec();
        if($result == false) {
            $error = $operation::lastError();
            return [1, $error->getMessage()];
        }
        return [0, $result];
    }

    public static function add($row) {
        if($row !== false) {
            $caller = get_called_class();
            $instance = $caller::create();
            self::rowPopulate($instance, $row, static::fields());
            return $instance;
        }
        return null;
    }

    public static function populate($rows) {
        $caller = get_called_class();
        $instances = [];
        for($i = 0; $i < count($rows); $i++) {
            $instance = $caller::create();
            self::rowPopulate($instance, $rows[$i], static::fields());
            $instances[] = $instance;
        }
        return $instances;
    }

    private static function rowPopulate($instance, $row, $fields) {
        for($i = 0; $i < count($fields); $i++) {
            if(!isset($row[$fields[$i]])) continue;
            static::addToModel($row['id'], $instance);
            $property = \system\libs\FormatModelPropertyName::format($fields[$i]);
            $instance->$property = $row[$fields[$i]];
        }
    }
    

    //Create getters setters

    public function __call($method, $args) {
        if(property_exists($this, $method)) {
            if(is_callable($this->$method)) {
                return call_user_func_array($this->$method, $args);
            }
            else {
                //die('<b>Fatal error:</b> ' . get_class($this) . ':' . $method . ' is not callable <i>(dynamically created)</i><br>');
            }
        }
        else {
            //die('<b>Fatal error:</b> Call to undefined method ' . get_class($this) . ':' . $method . ' <i>(dynamically created)</i><br>');
        }
    }

    private function createModelFields() {
        $fields = static::fields();
        for($i = 0; $i < count($fields); $i++) {
            $property = \system\libs\FormatModelPropertyName::format($fields[$i]);
            $this->$property = null;
            $this->properties[] = $fields[$i];
        }
    }

    private function prepareRelations() {
        $schema = static::$schema[static::table()];
        $relations = $schema->relations();
        foreach ($relations as $key => $value) {
            $this->createRelation($key, $value);
        }
    }

    private function createRelation($table, $relation) {
        $this->$table = function () use ($table, $relation) {
            $getId = 'get' . ucfirst($relation['id']);
            $id = $this->$getId();
            $class = 'user\\models\\' . ucfirst($table);
            return $class::get()->where($relation['fk'], $id)->all();
        };
    }

    private function prepareReferences() {
        $schema = static::$schema[static::table()];
        $references = $schema->references();
        foreach ($references as $key => $value) {
            $this->createReference($key, $value);
        }
    }

    private function createReference($key, $referece) {
        $table = $referece['table'];
        if($table == null) {
            return;
        }
        $this->$table = function () use ($key, $referece) {
            $getValue = \system\libs\FormatModelPropertyName::getter($key);
            $value = $this->$getValue();
            $class = 'user\\models\\' . ucfirst($referece['table']);
            return $class::get()->where($referece['column'], $value)->one();
        };
    }
    
    private static $mySqlErrors = 
            [
                23000 => "Ошибка удаления. На удаляемую запись ссылаются другие записи.\n"
            ];
   
}