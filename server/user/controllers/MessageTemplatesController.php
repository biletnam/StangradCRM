<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

/**
 * Description of MessageTemplatesController
 *
 * @author Дмитрий Строкин
 */
class MessageTemplatesController extends \system\controllers\ModelController {
    protected function permission($actionType) {
        return [0, 0];
    }
}
