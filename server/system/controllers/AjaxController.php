<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\controllers;

/**
 * Description of AjaxController
 *
 * @author Дмитрий
 */
class AjaxController extends RequestController {
    protected function response($result) {
        echo json_encode($result);
        //echo json_encode([1, "Ведутся технические работы!"]);
    }
}
