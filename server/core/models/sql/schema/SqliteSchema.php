<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of SqliteSchema
 *
 * @author Дмитрий
 */

namespace core\models\sql\schema;

use Exception;
use core\models\sql\QueryBuilder;

class SqliteSchema extends AbstractSqlSchema {

    public function __construct ($table, $connection = null) {
        $schema = $this->createSchema($table, $connection);
        for($i = 0; $i < count($schema); $i++) {
            $this->fields[] = $schema[$i]['name'];
            if($schema[$i]['pk'] == 1) {
                $this->pk = $schema[$i]['name'];
                $this->createRelations($this->pk, $connection);
            }
        }
        $this->createReferences($table, null, $connection);
    }

    protected function createSchema ($table, $connection) {
        $query = new QueryBuilder(null, null, $connection);
        $result = $query->custom("pragma table_info($table)", [])->all();
        if($result == false) {
            throw new Exception($query->lastError());
        }
        return $this->schema = $result[0];
    }
    
    protected function createReferences ($table, $field, $connection) {
        $query = new QueryBuilder(null, null, $connection);
        $result = $query->custom("pragma foreign_key_list($table)", [])->all();
        if($result == false) {
            throw new Exception($query->lastError());
        }
        $result = $result[0];
        for($i = 0; $i < count($result); $i++) {
            $field = $result[$i]['from'];
            $this->references[$field]['table'] = $result[$i]['table'];
            $this->references[$field]['column'] = $result[$i]['to'];
        }
    }
    
    protected function createRelations($field, $connection) {
        $query = new QueryBuilder('sqlite_master', null, $connection);
        $result = $query->select('name')->where('type', 'table')->all();
        if($result == false) {
            throw new Exception($query->lastError());
        }
        $result = $result[0];
        for($i = 0; $i < count($result); $i++) {
            $query = new QueryBuilder(null, null, $connection);
            $tableName = $result[$i]['name'];
            $r = $query->custom("pragma foreign_key_list($tableName)", [])->all();
            if($r == false) {
                throw new Exception($query->lastError());
            }
            $r = $r[0];
            for($j = 0; $j < count($r); $j++) {
                $column = $r[$j]['from'];
                $this->relations[$tableName]['id'] = $field;
                $this->relations[$tableName]['fk'] = $column;
            }
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
    
    public function getScheme () {
        return $this->schema;
    }
    
}
