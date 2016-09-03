<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\widgets;

/**
 * Description of Widget
 *
 * @author Дмитрий
 */
abstract class Widget  {
    protected $js = [];
    protected $css = [];
    
    public function __construct($js = null, $css = null) {
        if($js !== null) $this->addJs ($js);
        if($css !== null) $this->addCss ($css);
    }
    
    public function addJs ($js) {
        if(is_array($js)) {
            array_merge($this->js, $js);
        }
        else {
            $this->js[] = $js;
        }
    }

    public function addCss ($css) {
        if(is_array($css)) {
            array_merge($this->css, $css);
        }
        else {
            $this->css[] = $css;
        }
    }

    public function renderJs () {
        for($i = 0; $i < count($this->js); $i++) {
            \core\views\View::addJs('widgets/' . $this->js[$i]);
        }
    }
    
    public function renderCss () {
        for($i = 0; $i < count($this->css); $i++) {
            \core\views\View::addCss('widgets/' . $this->css[$i]);
        }
    }

    abstract public function render ($items);
}
