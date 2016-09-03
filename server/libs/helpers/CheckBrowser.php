<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of CheckBrowser
 *
 * @author Дмитрий
 */
class CheckBrowser {
    public static function isIE6() {
        $user_agent = $_SERVER['HTTP_USER_AGENT'];
        $browser = false;
        if ( stristr($user_agent, 'MSIE 6.0') ) $browser = true;
        return $browser;
    }
    
    public static function isIE7() {
        $user_agent = $_SERVER['HTTP_USER_AGENT'];
        $browser = false;
        if ( stristr($user_agent, 'MSIE 7.0') ) $browser = true;
        return $browser;
    }
    
}
