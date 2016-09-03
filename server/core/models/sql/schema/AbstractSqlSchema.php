<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of AbstractSqlSchema
 *
 * @author Дмитрий
 */
namespace core\models\sql\schema;


abstract class AbstractSqlSchema {
    
    protected $schema = null;
    protected $pk = null;
    protected $fields = [];
    protected $fk = [];
    protected $relations = [];
    protected $references = [];
    
    public function __construct($table, $connection = null) {}
    
    abstract protected function createSchema($table, $connection);
    abstract protected function createReferences ($table, $field, $connection);
    abstract protected function createRelations($field, $connection);

    public function fields() {
        return $this->fields;
    }
    public function pk() {
        return $this->pk;
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
    
}
