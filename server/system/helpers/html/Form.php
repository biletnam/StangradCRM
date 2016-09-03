<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;

/**
 * Description of Form
 *
 * @author Дмитрий
 */
class Form extends HTMLElement {
    public function render() {
        $table = '<form ' . $this->renderAttributes() . '>';
        $table .= $this->renderValues() . '</form>';
        return $table;
    }
}
