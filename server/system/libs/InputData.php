<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of ValidateInput
 *
 * @author Дмитрий
 */

namespace system\libs;

class InputData {
    
    
    public static function server ($p = 'REQUEST_URI', $as_array = 1) {
        $uri = trim(filter_input(INPUT_SERVER, $p, FILTER_SANITIZE_URL), '/');
        if($as_array === 1) {
            return explode('/', $uri);
        }
        return $uri;
    }
    
    public static function post ($as_array = 1) {
        if(count($_POST) > 0) {
            return $_POST;
        }
        return null;
    }
    
    public static function get ($as_array = 1) {
        if(count($_GET) > 0) {
            return $_GET;
        }
        return null;
    }

    public static function put($as_array = 1) {
        if($_SERVER['REQUEST_METHOD'] == "PUT") {            
            parse_str(file_get_contents("php://input"), $post_data);
            if(count($post_data) > 0) {
                return $post_data;
            }
            return null;
        }
        return null;
    }

    public static function delete($as_array = 1) {
        if(count($_DELETE) > 0) {
            return $_DELETE;
        }
        
        return null;
    }    
    
    public static function request ($as_array = 1) {
        return $_REQUEST;
    }    
}
