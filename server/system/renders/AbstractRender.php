<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\renders;

/**
 * Description of AbstractRender
 *
 * @author Дмитрий
 */
abstract class AbstractRender implements IRender {
    
    protected $view = null;
    protected $dataSource = null;

    final public function __construct($view, $dataSource) {
        if(!($view instanceof baseviews\AbstractView)) {
            throw new \Exception('View class ' . $view . ' is not a subclass of AbstractView', -1, null);
        }
        if(!($dataSource instanceof \system\data\DataSource)) {
            throw new \Exception('DataSource class ' . $dataSource . ' is not a subclass of DataSource', -1, null);
        }
        $view->setDataSource($dataSource);
        $this->view = $view;
    }
    
    public function getView () {
        return $this->view;
    }

    public function addAttribute ($name, $value) {
        $view = $this->view->getView();
        $view->addAttribute($name, $value);
    }

    abstract public function show ();
}
