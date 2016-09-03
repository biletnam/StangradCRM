<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
namespace system\views\renders;

use system\helpers\html as html;
/**
 * Description of FormView
 *
 * @author Дмитрий
 */
class FormView extends View {
    protected $model = null;
    protected $id = null;
    protected $form = null;
    private $values = null;
    protected $aliases = [];
    
    protected $addableColumns = [];
    protected $editableColumns = [];

    protected $render = null;

    public function __construct($model, $render) {
        $this->model = $model;
        
        $this->render = $render;
        
        $this->form = new html\Form();
        $this->form->addAttribute('method', 'post');
        $this->form->addAttribute('enctype', 'multipart/form-data');
        $this->form->addAttribute('action', '/request');
        
        $hidden = new html\Input();
        $hidden->addAttribute('type', 'hidden');
        $hidden->addAttribute('name', 'entity');
        $hidden->addAttribute('value', ucfirst($this->className()) . '/save');
        $this->form->addValue($hidden);
        
        
    }
    
    protected function values () {
        $model = $this->model;
        $pk = $model::pk();
        if($this->id !== null) {
            return $model::get()->where($pk, $this->id)->one();
        }
        return null;
    }    
    
    public function setAliases ($aliases) {
        $this->aliases = $aliases;
    }

    public function setAddableColumns ($columns) {
        $this->addableColumns = $columns;
    }
    
    public function setEditableColumns ($columns) {
        $this->editableColumns = $columns;
    }

    public function setId ($id) {
        $this->id = $id;
    }

    public function addAttribute ($name, $value) {
        $this->form->addAttribute($name, $value);
    }

    public function getForm () {
        return $this->form;
    }
    
    public function setVisibleColumns ($columns) {
        $this->visibleColimns = $columns;
    }
    
    protected function className () {
        $pathArray = explode('\\', $this->model);
        return strtolower($pathArray[count($pathArray)-1]);
    }
    
    protected function createInput ($inputClass, $attributes) {
        $input = new $inputClass();
        $input->setAttributes($attributes);
        return $input;
    }

    protected function pk ($input, $value = null) {
        if($this->values === null) {
            $input->addAttribute('disabled', 'true');
        }
        else {
            $input->addAttribute('readonly', 'true');
        }
    }

    protected function fk ($input, $fieldName, $value = null) {
       $model = $this->model;
       $childModel = $model::referenceModel($fieldName);
       $items = $childModel::get()->all();
       $pk = \system\libs\FormatModelPropertyName::getter($childModel::pk());
       $referenceColumn = $childModel::referenceRenderColumn();
       $referenceColumnRenderMethod = null;
       if($referenceColumn !== null) {
           $referenceColumnRenderMethod = \system\libs\FormatModelPropertyName::getter($referenceColumn);
       }
       for($i = 0; $i < count($items); $i++) {
           $option = new html\Option();
           if($value === $items[$i]->$pk()) {
               $option->addAttribute('selected', 'true');
           }
           $option->addAttribute('value', $items[$i]->$pk());
           if($referenceColumnRenderMethod !== null) {
               $option->addValue($items[$i]->$referenceColumnRenderMethod());
           }
           $input->addValue($option);
       }
    }

    protected function fields () {
        $model = $this->model;
        if($this->id === null) {
            return ($this->addableColumns != null) ? $this->addableColumns : $model::fields();
        }
        else {
            return ($this->editableColumns != null) ? $this->editableColumns : $model::fields();
        }
    }
    
    protected function prepareInput ($fieldName, $fieldType) {
        $model = $this->model;
        $attributes = [];
        $attributes['name'] = $fieldName;
        $attributes['id'] = 'id_' . $fieldName;
        $fielddata = preg_split('/[\(\)]/', $fieldType, -1, PREG_SPLIT_NO_EMPTY);
        $type = $fielddata[0];
        $length = (isset($fielddata[1])) ? $fielddata[1] : null;
        if($length !== null) {
            $attributes['maxlength'] = $length;
        }

        if($model::isPrimaryKey($fieldName)) {
            $type = 'pk';
            $t = $this->rules($type);
            if(isset($t[1])) {
                $attributes['type'] = $t[1];
            }
            $input = $this->createInput($t[0], $attributes);
            $this->pk($input);
        }
        else if($model::isForeignKey($fieldName)) {
            $type = 'fk';
            $t = $this->rules($type);
            if(isset($t[1])) {
                $attributes['type'] = $t[1];
            }
            $input = $this->createInput($t[0], $attributes);
            $value = $this->getFieldValue($fieldName);
            if($value === null) {
                $this->fk($input, $fieldName);
            }
            else {
                $this->fk($input, $fieldName, $value);
            }
        }
        else {
            $t = $this->rules($type);
            if(isset($t[1])) {
                $attributes['type'] = $t[1];
            }
            $input = $this->createInput($t[0], $attributes);
        }
        
        $this->prepareValue($input, $fieldName);
        
        return $input;
    }
    
    protected function rules ($type) {
        switch ($type) {
            case 'varchar' : return ['\\system\\helpers\\html\\Input', 'text'];
            case 'int' : return ['\\system\\helpers\\html\\Input', 'number'];
            case 'date' : return ['\\system\\helpers\\html\\Input', 'date'];
            case 'datetime' : return ['\\system\\helpers\\html\\Input', 'datetime'];
            case 'text' : return ['\\system\\helpers\\html\\TextArea'];
            case 'pk': return ['\\system\\helpers\\html\\Input', 'text'];
            case 'fk' : return ['\\system\\helpers\\html\\Select'];
            case 'option' : return ['\\system\\helpers\\html\\Option'];
            default : return ['\\system\\helpers\\html\\Input', 'text'];
        }
    }

    protected function addItem ($item) {
        $this->form->addValue($item);
    }

    protected function definitionFieldType ($name) {
        
    }

    protected function getFieldValue ($name) {
        if($this->values === null) {
            return null;
        }
        $values = $this->values;
        $getMethod = \system\libs\FormatModelPropertyName::getter($name);
        return $values->$getMethod();
    }

    protected function prepareValue ($input, $name) {
        $value = $this->getFieldValue($name);
        if($value === null) return;
        
        if($input instanceof html\Input) {
            $input->addAttribute('value', $value);
        }
        if($input instanceof html\Textarea ||
                $input instanceof html\Option) {
            $input->addValue($value);
        }
    }
    
    protected function prepareBaseAttribute ($input) {
        
    }
    
    protected function prepare () {
        $model = $this->model;
        $fields = $this->fields();
        if($this->id !== null) {
            $this->values = $model::get()->where($model::pk(), $this->id)->one();
            $this->addItem($this->prepareInput('id', $model::getFieldType('id')));
        }
        for($i = 0; $i < count($fields); $i++) {
            $this->addItem($this->prepareInput($fields[$i], $model::getFieldType($fields[$i])));
        }
        $this->addItem($this->submit());
    }

    protected function submit () {
        $input = new html\Input();
        $input->addAttribute('value', 'Save');
        $input->addAttribute('type', 'submit');
        return $input;
    }

    public function render () {
        $this->prepare();
        return $this->form->render();
    }
    
}
