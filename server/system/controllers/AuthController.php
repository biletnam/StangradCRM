<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\controllers;

/**
 * Description of AuthController
 *
 * @author Дмитрий
 */
class AuthController {
    
    public function authAction ($params) {
        if(!isset($params['login']) || !isset($params['password'])) {
            return [1, "Логин или пароль не введены!"];
        }
        
        $login = trim(strip_tags($params['login']));
        $password = trim(strip_tags($params['password']));
        $user = \system\auth\Auth::authentication($login, $password);
        if($user == false) {
            return [1, "Логин или пароль введены не верно!"];
        }
        else {
            return [0, $user];
        }
    }
    public function userInfoAction() {
        $authUser = \system\auth\Auth::getSession("system_user");
        if($authUser == false) {
            return [1, "No authorized user/"];
        }
        else {
            return [0, $authUser];
        }
    }
}
