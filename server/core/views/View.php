<?php

namespace core\views;

class View extends BaseView {

    private static $js = [];
    private static $css = [];

    public static function addJs($js, $params = []) {
        $params_string = "";
        foreach ($params as $key => $value) {
            $params_string = $params_string . " " . $key . " = '" . $value . "'";
        }
        
        $filePath = 'resource/js/' . $js;
        if(!file_exists($filePath)) {
            return false;
        }
        self::$js[$filePath] = "<script type='text/javascript' src='/".$filePath."' " . $params_string . "></script>";
        return true;
    }

    public static function renderJs() {
        $jsarray = [];
        foreach (self::$js as $value) {
            $jsarray[] = $value;
        }
        return implode(PHP_EOL, $jsarray);
    }

    public static function addCss($css) {
        $filePath = 'resource/css/' . $css;
        if(!file_exists($filePath)) {
            return false;
        }
        self::$css[$filePath] = "<link rel='stylesheet' type='text/css' href='/" . $filePath . "'>";
    }

    public static function renderCss() {
        $cssarray = [];
        foreach (self::$css as $value) {
            $cssarray[] = $value;
        }
        return implode(PHP_EOL, $cssarray);
    }

}