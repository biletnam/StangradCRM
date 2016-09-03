<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\renders\baseviews;

use system\helpers\html as html;

/**
 * Description of TableView
 *
 * @author Дмитрий
 */
class TableView extends AbstractView {

    public function __construct($dataSource = null) {
        parent::__construct($dataSource);
        $this->view = new html\Table();
    }

    protected function createRow ($cells = []) {
        if(count($cells) > 0) {
            return new html\Tr($cells);
        }
        return new html\Tr();
    }
    
    protected function createCell ($value) {
        return new html\Td($value);
    }

    protected function row ($items) {
        $cells = [];
        foreach ($this->renderSettings as $key => $value) {
            if($this->renderSettings($key, 'visible') === true) {
                $cells[] = $this->createCell($items[$key]);
            }
        }
        return $this->createRow($cells);
    }

    protected function prepareTitles () {
        $cells = [];
        foreach ($this->renderSettings as $key => $value) {
            if($this->renderSettings($key, 'visible') === true) {
                $aliase = $this->renderSettings($key, 'aliase');
                if($aliase === null) {
                    $cells[] = new html\Th($key);
                }
                else {
                    $cells[] = new html\Th($aliase);
                }
            }
        }
        return $this->createRow($cells);
    }

    protected function data () {
        return $this->dataSource->get();
    }

    protected function prepare () {
        $this->view->addValue($this->prepareTitles());
        $data = $this->data();
        for($i = 0; $i < count($data); $i++) {
            $this->view->addValue($this->row($data[$i]));
        }
    }
    
    public function show () {
        $this->prepare();
        return $this->view->render();
    }
}

