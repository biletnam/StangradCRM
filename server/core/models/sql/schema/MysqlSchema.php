<?php

namespace core\models\sql\schema;

use Exception;
use core\models\sql\QueryBuilder;

class MysqlSchema extends AbstractSqlSchema {

    public function __construct ($table, $connection = null) {
        $schema = $this->createSchema($table, $connection);
        for($i = 0; $i < count($schema); $i++) {
            $this->fields[] = $schema[$i]['Field'];
            if($schema[$i]['Key'] == 'PRI') {
                $this->pk = $schema[$i]['Field'];
                $this->createRelations($this->pk, $connection);
            }
            if($schema[$i]['Key'] == 'MUL') {
                $this->fk[] = $schema[$i]['Field'];
                $this->createReferences($table, $schema[$i]['Field'], $connection);
            }
        }
    }

    protected function createSchema ($table, $connection) {
        $query = new QueryBuilder(null, null, $connection);
        $result = $query->custom("show columns from $table", [])->all();
        if($result == false) {
            throw new Exception($query->lastError());
        }
        return $this->schema = $result[0];
    }
    
    protected function createReferences ($table, $field, $connection) {
        $query = new QueryBuilder(null, null, $connection);
        $result = $query->custom("select referenced_column_name, referenced_table_name 
                                    from information_schema.key_column_usage 
                                    where table_name = '$table' and column_name = '$field'", [])->one();
        if($result == false) {
            throw new Exception($query->lastError());
        }
        $this->references[$field]['table'] = $result[0]['referenced_table_name'];
        $this->references[$field]['column'] = $result[0]['referenced_column_name'];
    }
    
    protected function createRelations($field, $connection) {
        $query = new QueryBuilder(null, null, $connection);
        $results = $query->custom("select table_name, column_name from information_schema.key_column_usage 
                                    where referenced_column_name = '$field'", [])->all();
        if($results == false) {
            throw new Exception($query->lastError());
        }
        $result = $results[0];
        for($i = 0; $i < count($result); $i++) {
            $table = $result[$i]['table_name'];
            $column = $result[$i]['column_name'];
            $this->relations[$table]['id'] = $field;
            $this->relations[$table]['fk'] = $column;
        }

    }

    public function relations ($name = null) {
        if($name != null) {
            if(isset($this->relations[$name])) {
                return $this->relations[$name];
            }
            return false;
        }
        else {
            return $this->relations;
        }
    }
    
    public function references ($name = null) {
        if($name != null) {
            if(isset($this->references[$name])) {
                return $this->references[$name];
            }
            return false;
        }
        else {
            return $this->references;
        }
    }
    
    public function getFieldType($fieldName) {
        $schema = $this->schema;
        for($i = 0; $i < count($schema); $i++) {
            if($schema[$i]['Field'] == $fieldName) {
                return $schema[$i]['Type'];
            }
        }
        return null;
    }
    public function isPrimaryKey($fieldName) {
        if($fieldName == $this->pk) {
            return true;
        }
        return false;
    }
    public function isForeignKey($fieldName) {
        for($i = 0; $i < count($this->fk); $i++) {
            if($this->fk[$i] == $fieldName) {
                return true;
            }
        }
        return false;
    }
}