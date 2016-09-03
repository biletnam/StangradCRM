<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\renders;

/**
 * Description of Render
 *
 * @author Дмитрий
 */
abstract class Render extends AbstractRender {

    public function show ($params = []) {        
        $this->view->setRenderSettings(static::renderSettings());
        $this->view->setParams($params);
        return $this->view->show();
    }
    
}