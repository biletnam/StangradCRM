<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

/**
 * Description of ManagersController
 *
 * @author Дмитрий
 */
class ManagerController extends \system\controllers\ModelController {
    
    public static function validate($login, $password) {
        $passwordHash = self::passwordHash($password);
        return \user\models\Manager::get()->
                where('login', $login)->
                where('passwd', $passwordHash)->one();
    }
    
    public static function passwordHash($password) {
        return sha1(md5($password));
    }
    
    public function saveAction($params) {
        if(isset($params['passwd']))
        {
            $params['passwd'] = self::passwordHash($params['passwd']);
        }
        return parent::saveAction($params);
    }
    
    protected function permission($actionType) {
        $user = \system\auth\Auth::getAuthUser("system_user");
        if(!$user) 
        {
            return [1, "Permission denied"];
        }
        if($user->role() === 1) 
        {
            return [0, 0];
        }
        if ($actionType === 'get') 
        {
            return [0, 0];
        }
        return [1, "Permission denied"];
    }
}