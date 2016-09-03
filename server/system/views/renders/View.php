<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\views\renders;

/**
 * Description of View
 *
 * @author Дмитрий
 */
abstract class View {
    protected $model = null;
    protected $render = null;
    protected $params = [];
    protected $aliases = [];
    
    public function __construct($model, $render) {
        $this->model = $model;
        $this->render = $render;
    }
    
    function __call($name, $arguments) {
        return;
    }
            
    function setParams ($params = []) {
        $this->params = $params;
    }
    
    
    
}
