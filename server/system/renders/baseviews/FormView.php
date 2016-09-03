<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\renders\baseviews;

use system\helpers\html as html;
/**
 * Description of FormView
 *
 * @author Дмитрий
 */
class FormView extends AbstractView {
    //put your code here
    
    public function __construct($model = null) {
        parent::__construct($model);
        $this->view = new html\Form();
    }
    
    protected function input ($name, $type, $renderAs = null) {
        if($renderAs === null || !($renderAs instanceof html\HTMLElement)) {
            $input = new html\Input();
        }
        else {
            $input = $renderAs;
        }
        $input->addAttribute('name', $name);
        $input->addAttribute('id', 'id_' . $name);
        return $input;
    }

    protected function buildInput ($key, $type) {
        $renderAs = $this->renderSettings($key, 'render_as');
        if($renderAs !== null) {
            return $this->input($key, $type, $renderAs);
        }
        return $this->input($key, $type);

    }
    
    protected function buildFk ($input, $key, $value = null) {
        $referenceColumn = $this->renderSettings($key, 'reference_column');
        if($referenceColumn === null) {
            return $input;
        }
        $referenceData = $this->dataSource->referenceModel($key, $referenceColumn);
        if($referenceData === null) {
            return $input;
        }
        $referenceDataArray = $referenceData[0];
        $referenceKey = $referenceData[1];
        
        for($i = 0; $i < count($referenceDataArray); $i++) {
            $option = new html\Option($referenceDataArray[$i][$referenceColumn]);
            $option->addAttribute('value', $referenceDataArray[$i][$referenceKey]);
            if($value !== null && $value === $referenceDataArray[$i][$referenceKey]) {
                $option->addAttribute('selected', 'true');
            }
            $input->addValue($option);
        }
        return $input;
    }

    protected function prepareAddable ($key, $type) {
        if($this->renderSettings($key, 'addable') === true) {
            $input = $this->buildInput($key, $type);
            if($this->dataSource->isFk($key)) {
                $this->buildFk($input, $key);
                return $input;
            }
            return $input;
        }
    }
    
    protected function prepareEditable ($key, $type, $data) {
        
        //var_dump($key);
        
        $itemData = $data[0];
        if($this->dataSource->isPk($key)) {
            $input = $this->buildInput($key, $type);
            $input->addAttribute('type', 'hidden');
            $input->addValue($itemData[$key]);
            return $input;
        }
        if($this->renderSettings($key, 'editable') === true) {
            $input = $this->buildInput($key, $type);
            if($this->dataSource->isFk($key)) {
                $this->buildFk($input, $key, $itemData[$key]);
                return $input;
            }
            $input->addValue($itemData[$key]);
            return $input;
        }
        
        $child_model_name = $this->renderSettings($key, 'model');
        
        
        if($child_model_name !== null) {
            echo $child_model_name;
        }
    }

    protected function addInput ($input, $aliase) {
        $input->addAttribute('placeholder', $aliase);
        $this->view->addValue($input);
    } 

    protected function prepare() {
        $scheme = $this->dataSource->scheme();
        foreach ($scheme as $key => $value) {
            $input = ($this->dataSource->isParam($this->dataSource->pk())) ? 
                    $this->prepareEditable($key, $value, $this->dataSource->get()) : 
                    $this->prepareAddable($key, $value);
            if($input !== null) {
                $aliase = $this->renderSettings($key, 'aliase');
                $this->addInput($input, $aliase);
            }
        }
    }

    protected function prepareCancelButton () {
        $cancel = new html\A('Отмена');
        return $cancel;
    }

    protected function prepareOkButton () {
        $ok = new html\Input();
        $ok->addAttribute('type', 'submit');
        $ok->addValue('ОК');
        return $ok;
    }

    public function show() {
        $this->prepare();        
        return $this->view->render();
    }
    
}
