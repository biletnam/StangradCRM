<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;

/**
 * Description of Div
 *
 * @author Дмитрий
 */
class Div extends HTMLElement {
    public function render() {
        return '<div ' . $this->renderAttributes() . '>' . $this->renderValues() . '</div>';
    }
}
