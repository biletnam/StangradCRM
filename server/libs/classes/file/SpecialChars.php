<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Files
 *
 * @author Дмитрий
 */
class SpecialChars {
    private static $specialCharsArray = array(" ", "\"", "'");


    public static function cut($string) {
        for($i = 0; $i < count(self::$specialCharsArray); $i++) {
            $string = Replaces::_replase($string, self::$specialCharsArray[$i], "");
        }
        return $string;
    }
    
}
