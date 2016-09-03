<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;

/**
 * Description of Input
 *
 * @author Дмитрий
 */
class Img extends HTMLElement {
    
    public function addValue($value) {
        $this->addAttribute('src', $value);
    }
    
    public function setValues($values = array()) {
        $this->addValue($values);
    }
    
    public function render() {
        return '<img ' . $this->renderAttributes() . '> ';
    }
}
