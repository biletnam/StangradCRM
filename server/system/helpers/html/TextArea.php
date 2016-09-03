<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;

/**
 * Description of TextArea
 *
 * @author Дмитрий
 */
class TextArea extends HTMLElement {
    public function render() {
        return '<textarea' . $this->renderAttributes() . '>' . $this->renderValues() . '</textarea>';
    }
}
