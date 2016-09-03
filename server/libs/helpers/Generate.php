<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Generate
 *
 * @author Дмитрий
 */

namespace libs\helpers;

use libs\helpers\Alphabet;

class Generate {
    
    private static $numbers = [1,2,3,4,5,6,7,8,9,0];

    public static function generateRandomText($len = 10, $lang = 'en') {
        $result = null;
        $symbolsArray = Alphabet::en();
        if($lang === 'ru') {
            $symbolsArray = Alphabet::ru();
        }
        for($i = 0; $i < $len; $i++) {
            $randNum = rand(0, count($symbolsArray)-1);
            $result = $result . $symbolsArray[$randNum];
        }
        return $result;
    }
    
    public static function generateRandomMixed($len = 10, $lang = 'en') {
        $result = null;
        $symbolsArray = Alphabet::en();
        if($lang === 'ru') {
            $symbolsArray = Alphabet::ru();
        }
        $symbolsArray = array_merge($symbolsArray, self::$numbers);
        for($i = 0; $i < $len; $i++) {
            $randNum = rand(0, count($symbolsArray)-1);
            $result = $result . $symbolsArray[$randNum];
        }
        return $result;
    }

    public static function generateRandomNumber() {
        
    }
}
