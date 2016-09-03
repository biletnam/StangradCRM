<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\views\renders;

use system\helpers\html as html;

/**
 * Description of TableView
 *
 * @author Дмитрий
 */
class TableView extends View {
    
    protected $model = null;
    protected $render = null;
    protected $table = null;
    protected $visibleColimns = null;
    protected $aliases = [];
    protected $paginationCurrentOffset = 0;
    protected $paginationLength = 3;


    public function __construct($model, $render) {
        $this->model = $model;
        $this->render = $render;
        $this->table = new html\Table();
        $this->table->addAttribute('id', 'content-table');
    }

    public function setAliases ($aliases) {
        $this->aliases = $aliases;
    }
    
    public function setAddableColumns ($columns) {
        //$this->addableColumns = $columns;
    }
    
    public function setEditableColumns ($columns) {
        //$this->editableColumns = $columns;
    }

    public function addAttribute ($name, $value) {
        $this->table->addAttribute($name, $value);
    }

    public function setPaginationLength ($length) {
        $this->paginationLength = $length;
    }

    public function getTable () {
        return $this->table;
    }
    
    protected function className () {
        $pathArray = explode('\\', $this->model);
        return strtolower($pathArray[count($pathArray)-1]);
    }

    public function setVisibleColumns ($columns) {
        $this->visibleColimns = $columns;
    }
    
    protected function fields () {
        $model = $this->model;
        return ($this->visibleColimns != null) ? $this->visibleColimns : $model::fields();
    }

    protected function prepareTitle () {
        $fields = $this->fields();
        $cells = [];
        for($i = 0; $i < count($fields); $i++) {
            $title = (isset($this->aliases[$fields[$i]])) ? $this->aliases[$fields[$i]] : $fields[$i];
            $cells[] = new html\Th($title);
        }
        $this->table->setThs($cells);
    }

    protected function createRow ($cells = []) {
        if(count($cells) > 0) {
            return new html\Tr($cells);
        }
        return new html\Tr();
    }
    
    protected function createCell ($value) {
        return new html\Td($value);
    }

    protected function items ($offset = null, $length = null) {
        $model = $this->model;
        if ($offset !== null && $length !== null) {
            return $model::get()->limit($offset, $length)->all();
        }
        return $model::get()->all();
    }
    
    protected function row ($item) {
        $fields = $this->fields();
        $values = [];
        for($i = 0; $i < count($fields); $i++) {
            $getter = \system\libs\FormatModelPropertyName::getter($fields[$i]);
            $values[] = $this->createCell($item->$getter());
        }
        return $this->createRow($values);
    }

    protected function prepare () {
        $this->prepareTitle();
        $items = $this->items($this->paginationCurrentOffset, $this->paginationLength);
        for($i = 0; $i < count($items); $i++) {
            $this->table->addValue($this->row($items[$i]));
        }
    }

    public function render () {
        $this->prepare();
        return $this->table->render();
    }
    
}
