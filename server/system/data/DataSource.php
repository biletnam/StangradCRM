<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\data;

use system\controllers\ModelController;
/**
 * Description of DataSource
 *
 * @author Дмитрий
 */
class DataSource {
    
    private $controller = null;
    private $params = null;

    final public function __construct($controller, $params = null) {
        if(!($controller instanceof ModelController)) {
            throw new Exception('Class ' . $controller . ' is not a subclass ModelController', -1, null);
        }
        $this->controller = $controller;
        $this->params = $params;
    }

    public function get ($fields = []) {
        $data = $this->controller->getAction($this->params, $fields);
        if($data[0] === 0) {
            return $data[1];
        }
        return [];
    }
    
    public function modelName () {
        $modelName = $this->controller->modelName();
        $modelNameArray = explode('\\', $modelName);
        return $modelNameArray[count($modelNameArray)-1];
    }
    
    public function count () {
        return $this->controller->count();
    }
    
    public function toArray ($modelArray, $fields = []) {
        return $this->controller->toArray($modelArray, $fields);
    }

    public function model () {
        return $this->controller->modelName();
    }
    
    public function isParam ($name) {
        return isset($this->params[$name]);
    }
    
    public function getParam ($name) {
        if($this->isParam($name)) {
            return $this->params[$name];
        }
        return null;
    }
    
    public function removeParam ($name) {
        if($this->isParam($name)) {
            unset($this->params[$name]);
        }
    }

    public function scheme () {
        return $this->controller->scheme();
    }
    
    public function isPk ($name) {
        return $this->controller->isPk($name);
    }

    public function pk() {
        return $this->controller->pk();
    }
    
    public function isFk ($name) {
        return $this->controller->isFk($name);
    }

    public function fk() {
        return $this->controller->fk();
    }
    
    public function referenceModel($key, $referenceColumn) {
        $model = $this->model();
        $referenceModel = str_replace('_', '',  $model::referenceModel($key));
        if($referenceModel === null) {
            return null;
        }
        $pk = $referenceModel::pk();
        $dataArray = $referenceModel::get()->select([$pk, $referenceColumn])->all();
        return [$this->toArray($dataArray, [$pk, $referenceColumn]), $pk];
    }
    
    
}
