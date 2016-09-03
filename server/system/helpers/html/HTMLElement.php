<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;

/**
 * Description of HTMLElement
 *
 * @author Дмитрий
 */

abstract class HTMLElement {
    protected $attributes = [];
    protected $values = [];
    protected $itemData = null;

    public function __construct($value = null) {
        if($value !== null) {
            if(is_array($value)) {
                $this->values = $value;
            }
            else {
                $this->values[] = $value;
            }
        }
    }
    
    public function addAttribute ($name, $value) {
        if(isset($this->attributes[$name])) {
            if($name === 'style' || $name === 'class') {
                $this->attributes[$name] .= ' ' . $value;
            }
            else {
                $this->attributes[$name] = $value;
            }
        }
        else {
            $this->attributes[$name] = $value;
        }
    }

    public function setAttributes ($attributes = []) {
        $this->attributes = $attributes;
    }
    
    public function getAttribute ($name) {
        return (isset($this->attributes[$name])) ? $this->attributes[$name] : null;
    }
    
    public function removeAttribute ($name) {
        if(isset($this->attributes[$name])) {
            unset($this->attributes[$name]);
        }
    }

    protected function renderAttributes () {
        $attributes = null;
        foreach ($this->attributes as $key => $value) {
            $attributes .= ' ' . $key . '="' . $value . '"';
        }
        return $attributes;
    }

    public function addValue ($value) {
        $this->values[] = $value;
    }
    
    public function setValues ($values = []) {
        $this->values = $values;
    }
    
    public function getValue ($pos) {
        return $this->values[$pos];
    }
    
    public function getValues () {
        return $this->values;
    }

    public function removeValue ($pos) {
        array_splice($this->values, $pos, 1);
    }
    
    protected function renderValues () {
        $value = null;
        for($i = 0; $i < count($this->values); $i++) {
            if($this->values[$i] instanceof HTMLElement) {
                $value .= $this->values[$i]->render();
            }
            else {
                $value .= $this->values[$i];
            }
        }
        return $value;
    }

    public function setItemData ($value) {
        $this->itemData = $value;
    }
    
    public function getItemData () {
        return $this->itemData;
    }

    abstract public function render ();
    
}
