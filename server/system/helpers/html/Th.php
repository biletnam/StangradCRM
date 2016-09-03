<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;

/**
 * Description of Th
 *
 * @author Дмитрий
 */
class Th extends HTMLElement {
    
    public function render() {
        return '<th ' . $this->renderAttributes() . '>' . $this->renderValues() . '</th>';
    }
}
