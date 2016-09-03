<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\widgets;

/**
 * Description of DropDownMenu
 *
 * @author Дмитрий
 */
class DropDownMenu extends Widget {
    
    public function render($items = []) {
        
        $this->addCss('dropdown.css');
        $this->addJs('dropdown.js');
        
        $div = new \system\helpers\html\Div();
        $div->addAttribute('class', 'dropdownmenu');
        
        $drop = new \system\helpers\html\Div();
        $drop->addAttribute('class', 'drop');
        
        $drop->setValues($items);
        $div->addValue($drop);
        
        $this->renderCss();
        $this->renderJs();

        return $div->render();
    }
    
}
