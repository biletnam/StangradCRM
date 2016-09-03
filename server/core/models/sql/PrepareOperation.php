<?php

namespace core\models\sql;

class PrepareOperation {
    private $fields = [];
    private $values = [];
    private $delimiter = [];

    private $tables = [];
    private $type = 'select';

    public function __construct($type) {
        $this->type = $type;
    }
    
    public function add($fields, $values = null, $delimiter = ', ') {
        if(is_array($fields)) {
            for($i = 0; $i < count($fields); $i++) {
                $this->addValue($fields[$i], $values, $delimiter);
                $values = null;
            }
        }
        else {
            $this->addValue($fields, $values, $delimiter);
        }
    }
    
    public function addTable($table) {
        if(!in_array($table, $this->tables)) {
            $this->tables[] = $table;
        }
    }

    private function addValue($field, $values, $delimiter) {
        $this->fields[] = $field;
        $this->delimiter[] = $delimiter;
        if($values !== null) {
            if(is_array($values)) {
                $this->values = array_merge($this->values, $values);
            }
            else {
                $this->values[] = $values;
            }
        }
    }
    
    public function build() {
        $query = '';
        if($this->type == 'select') {
            $query = 'select ' . $this->buildQuery() . ' from ' . implode(', ', $this->tables) . ' ';
        }
        if($this->type == 'where') {
            $query = 'where ' . $this->buildQuery() . ' ';
        }
        if($this->type == 'insert') {
            $query = 'insert into ' . $this->tables[0] . '(' . $this->buildQuery() . ') values(' . str_repeat('?, ', count($this->fields)-1) . ' ?) ';
        }
        if($this->type == 'update') {
            $query = 'update ' . $this->tables[0] . ' set ' . $this->buildQuery() . ' ';
        }
        if($this->type == 'delete') {
            $query = 'delete from ' . $this->tables[0] . ' ';
        }
        if($this->type == 'order') {
            $query = 'order by ' . $this->buildQuery() . ' ';
        }
        if($this->type == 'group') {
            $query = 'group by ' . $this->buildQuery() . ' ';
        }
        if($this->type == 'limit') {
            $query = 'limit ' . $this->buildQuery();
        }
        return [$query, $this->values];
    }
    
    public function buildNoQuery() {
        return [$this->buildQuery(), $this->values];
    }
    
    private function buildQuery() {
        $query = '';
        for($i = 0; $i < count($this->fields); $i++) {
            if($i == 0) {
                $query .= $this->fields[$i];
            }
            else {
                $query .= ' ' . $this->delimiter[$i] . ' ' . $this->fields[$i] . ' '; 
            }
        }
        return $query;
    }
}