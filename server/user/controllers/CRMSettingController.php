<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

/**
 * Description of CRMSettingController
 *
 * @author Дмитрий
 */
class CRMSettingController extends \system\controllers\ModelController  {
    //put your code here
    protected function permission($actionType) {
        return [0, 0];
    }
}
