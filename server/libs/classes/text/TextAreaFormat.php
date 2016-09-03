<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of TextAreaFormat
 *
 * @author Дмитрий
 */
class TextAreaFormat {
    public static function format($text) {
        return str_replace('\r\n', '<br>', $text);
    }
    public static function parse($text) {
        return str_replace('<br>', '\r\n', $text);
    }
}
