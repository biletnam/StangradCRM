<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Log
 *
 * @author Дмитрий
 */

namespace libs\classes\file;

class Log {
    
    private function __construct() {
        ;
    }
    private function __clone() {
        ;
    }
    
    public static function Write ($text)
    {
        $log = fopen(\configs\LogPath::path() . 'log.txt', 'a+');
        fwrite($log, date('d.M.y H:m') . ': ' . $text . PHP_EOL);
        fclose($log);
    }
    
    public static function WriteError ($text)
    {
        $log = fopen(\configs\LogPath::path() . 'error.txt', 'a+');
        fwrite($log, date('d.M.y H:m') . ': ' . $text . PHP_EOL);
        fclose($log);
    }
    
}
