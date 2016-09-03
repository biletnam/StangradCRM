<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\renders\baseviews;

/**
 * Description of Pagination
 *
 * @author Дмитрий
 */
class PaginationView extends AbstractView {
    
    protected $offset = 0;
    protected $length = 30;
    protected $buttons = [];

    public function __construct($dataSource = null) {
        parent::__construct($dataSource);
        $this->view = new \system\helpers\html\Div();
    }
    
    public function setOffset ($offset) {
        $this->offset = $offset;
    }
    
    public function setLength ($length) {
        $this->length = $length;
    }

    public function buttons () {
        return $this->buttons;
    }

    protected function prepare() {
        $countAll = $this->dataSource->count();
        $paginationButtonCount = ceil((int)$countAll/(int)$this->length);
        for($i = 0; $i < $paginationButtonCount; $i++) {
            $button = new \system\helpers\html\A($i+1);
            if(($this->offset) === $i) {
                $button->addAttribute('class', 'current');
            }
            $button->addAttribute('class', 'button pagination-button');
            $button->addAttribute('href', '/admin/'. strtolower($this->dataSource->modelName()) .'/show/?page='.($i+1));
            $this->view->addValue($button);
        }
    }
    
    public function show() {
        $this->prepare();
        return $this->view->render();
    }
    
}
