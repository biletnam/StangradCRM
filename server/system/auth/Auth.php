<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Auth
 *
 * @author Дмитрий
 */

namespace system\auth;

session_start();

class Auth {
    
    private $role;
    public function role()
    {
        return $this->role;
    }
    
    private $id;
    public function id()
    {
        return $this->id;
    }
    
    private  $name;
    public function name()
    {
        return $this->name;
    }
    
    private $login;
    public function login()
    {
        return $this->login;
    }

    private static $_instances = [];

    private function __construct($session_name) {
        $this->id = (int)$_SESSION[$session_name]['id'];
        $this->name = $_SESSION[$session_name]['name'];
        $this->login = $_SESSION[$session_name]['login'];
        $this->role = (int)$_SESSION[$session_name]['role'];
    }

    public static function authentication ($login, $password) {
        $user = \user\controllers\ManagerController::validate($login, $password);
        if($user !== null) {
            $user_data = ["id" => $user->Id,
                "name" => $user->Name,
                "login" => $user->Login,
                "role" => $user->IdRole];
            Auth::setAuthUser("system_user", $user_data);
            return $user_data;
        }
        return false;
    }

    public static function getAuthUser($session_name) {
        if(!isset($_SESSION[$session_name])) {
            return false;
        }
        if(!isset(self::$_instances[$session_name]))
        {
            self::$_instances[$session_name] = new Auth($session_name);
        }
        return self::$_instances[$session_name];
    }
    
    public static function getSession ($session_name)
    {
        if(!isset($_SESSION[$session_name])) {
            return false;
        }
        return $_SESSION[$session_name];
    }

    public static function setAuthUser($session_name, $user_data) {
        $_SESSION[$session_name] = $user_data;
    }

}
