<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\helpers\html;

/**
 * Description of Table
 *
 * @author Дмитрий
 */
class Table extends HTMLElement {
    private $ths = [];

    public function __construct($trs = []) {
        $this->values = $trs;
    }
    
    public function addTh ($value) {
        if($value instanceof Th) {
            $this->ths[] = $value;
        }
        else {
            $this->ths[] = new Th($value);
        }
    }
    
    public function getTh ($pos) {
        return $this->ths[$pos];
    }
    
    public function getThs () {
        return $this->ths;
    }

    public function setThs ($ths = []) {
        $this->ths = $ths;
    }
    
    private function renderThs () {
        if(count($this->ths) > 0) {
            $tr = new Tr();
            $tr->setValues($this->ths);
            return $tr->render();
        }
        return null;
    }

    public function render() {
        $table = '<table ' . $this->renderAttributes() . '>';
        $table .= $this->renderThs();
        $table .= $this->renderValues() . '</table>';
        return $table;
    }
}
