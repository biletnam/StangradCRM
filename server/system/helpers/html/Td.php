<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;
/**
 * Description of Td
 *
 * @author Дмитрий
 */
class Td extends HTMLElement {
    
    public function render() {
        return '<td ' . $this->renderAttributes() . '>' . $this->renderValues() . '</td>';
    }
}
