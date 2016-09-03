<?php

namespace core\models;

/**
 * Description of AbstractQueryBuilder
 *
 * @author Дмитрий
 */
abstract class AbstractQueryBuilder {
    
    /*
     * @param $field string or array
     * @out string aliase
     */
    
    protected function select_fields ($fields, array $tables) {
        $fields = ((!is_array($fields)) ? [$fields] : $fields);
        
        $result_fields = $this->prepare_multi_table_select($fields, $tables);
        return $this->select_query($result_fields);
    }

    /*
     * 1. Выполняет разбор массива полей, помещает в отдельные массивы простые поля и
     *    поля, являющиеся вложенными запросами
     * 2. Выполняет цикл по массиву таблиц, если:
     *      количество элементов в массиве таблиц = количеству элементов первого уровня вложенности
     *      массива полей - развернуть все вложенные элементы текущего элемента   
     * 
     */
    
    protected function prepare_multi_table_select (array $fields, array $tables) {
        $updated_fields = [];
        $subquery_fields = [];
        foreach ($fields as $key => $field) {
            if($field instanceof AbstractQueryBuilder) {
                if(is_string($key)) {
                    $subquery_fields[$key] = $field;
                }
                else {
                    $subquery_fields[] = $field;
                }
            }
            else {
                if(is_string($key)) {
                    $updated_fields[$key] = $field;
                }
                else {
                    $updated_fields[] = $field;
                }
            }
        }

        foreach ($tables as $table) {
            if(count($tables) == count($updated_fields)) {
                $current = each($updated_fields);
                $result_fields[$table] = $this->each_table_fields([$current['key'] => $current['value']]);
            }
            else {
                $result_fields[$table] = $this->each_table_fields($updated_fields);
            }
        }
        $result_fields = array_merge($result_fields, ["subquery" => $subquery_fields]);
        return $result_fields;
    }

    protected function each_table_fields ($fields) {
        
        //$iterator = new \RecursiveArrayIterator($fields);
        //$result = iterator_to_array($iterator, true);
        
        
        $storage = [];
        foreach ($fields as $key => $value) {
            if(is_array($value) && !is_string($key)) {
                $storage = array_merge($storage, $this->each_table_fields($value));
            }
            else {
                $storage[$key] = $value;
            }
        }
        return $storage;
    }

    protected function select_query (array $tables_fields) {
        $result_fields = [];
        $result_values = [];
        foreach ($tables_fields as $table => $fields) {
            $queries_array = $this->prepare_select_fields($fields, $table);
            $result_fields = array_merge($result_fields, $queries_array[0]);
            $result_values = array_merge($result_values, $queries_array[1]);
        }
        return [$result_fields, $result_values];
    }
    
    protected function prepare_select_fields (array $fields, $table) {
        $result_fields = [];
        $values = [];
        foreach ($fields as $k => $v) {
            if($v instanceof AbstractQueryBuilder) {
                $v->build();
                $values = $v->queries[0]['values'];
                $result_fields[] = '(' . $v->queries[0]['query'] . ')' 
                        . (is_string($k) ? " as '" . $k . "'" : "");
            }
            else {
                $result_fields[] = $table . '.' .
                        ((is_string($k)) ? $v . " as '" . $k . "'" : $v);
            }
        }        
        return [$result_fields, $values];
    }
}