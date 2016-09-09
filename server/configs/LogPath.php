<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace configs;

/**
 * Description of LogPath
 *
 * @author Дмитрий
 */
class LogPath {
    private function __construct() {
        ;
    }
    
    private function __clone() {
        ;
    }
    
    public static function path ()
    {
        return __DIR__ . '/';
    }
    
}
