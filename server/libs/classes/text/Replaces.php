<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Replaces
 *
 * @author Дмитрий
 */
class Replaces {
    public static function _replase($string, $old_sumbol, $new_symbol) {
        return str_replace($old_sumbol, $new_symbol, $string);
    }
}
