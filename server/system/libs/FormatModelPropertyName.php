<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\libs;

/**
 * Description of FormatModelPropertyName
 *
 * @author Дмитрий
 */
class FormatModelPropertyName {

    public static function format ($name) {
        $props = explode('_', $name);
        for($i = 0; $i < count($props); $i++) {
            $props[$i] = ucfirst($props[$i]);
        }
        return implode('', $props);
    }

    public static function getter ($name) {
        return 'get' . self::format($name);
    }
    public static function setter ($name) {
        return 'set' . self::format($name);
    }
}
