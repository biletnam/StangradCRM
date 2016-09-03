<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\renders\baseviews;

/**
 * Description of AbstractView
 *
 * @author Дмитрий
 */
abstract class AbstractView {
    protected $dataSource = null;
    protected $view = null;
    protected $params = [];
    protected $renderSettings = [];

    protected $defaultSettings = [
            'aliase' => null,
            'visible' => true,
            'addable' => true,
            'editable' => true,
            'render_as' => null,
            'reference_column' => null];

    public function __construct($dataSource = null) {
        $this->dataSource = $dataSource;
    }
    
    public function setDataSource ($dataSource) {
        $this->dataSource = $dataSource;
    }
    
    public function getDataSource () {
        return $this->dataSource;
    }
    
    public function addParam ($name, $value) {
        $this->params[$name] = $value;
    }
    
    public function getParam ($name) {
        return $this->params[$name];
    }
    
    public function removeParam ($name) {
        if(isset($this->params[$name])) {
            unset($this->params[$name]);
        }
    }

    public function setParams ($params = []) {
        $this->params = $params;
    }
    
    public function getParams () {
        return $this->params;
    }
    
    public function getView () {
        return $this->view;
    }

    public function setRenderSettings ($settings) {
        $this->renderSettings = $settings;
    }

    protected function renderSettings ($key, $value = null) {
        if(!isset($this->renderSettings[$key])) {
            if($value !== null) {
                return $this->defaultSettings[$value];
            }
            return $this->defaultSettings;
        }
        if($value === null) {
            return $this->renderSettings[$key];
        }
        if(!isset($this->renderSettings[$key][$value])) {
            if($value === 'aliase') {
                return $key;
            }
            if(!isset($this->defaultSettings[$value])) {
                return null;
            }
            return $this->defaultSettings[$value];
        }
        return $this->renderSettings[$key][$value];
    }
    
    abstract protected function prepare ();

    abstract public function show ();
}
