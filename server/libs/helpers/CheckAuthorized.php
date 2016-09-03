<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of CheckAuthorized
 *
 * @author user
 */

namespace libs\helpers;

session_start();
class CheckAuthorized {
    public static function check_authorized($session_name) {
        if(!isset($_SESSION[$session_name])) {
            return false;
        }
        else {
            return $_SESSION[$session_name];
        }
    }
}
