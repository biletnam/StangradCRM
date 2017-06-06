<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Base64Operations
 *
 * @author Дмитрий
 */

namespace libs\classes\file;

class Base64Operations {
    
    private static $lastError = '';

    public static function getExtension ($base64_string) {
        $pattern = '/^(data:)([^;]+)/i';
        preg_match($pattern, $base64_string, $matches);
        $result = explode('/', $matches[2]);
        return $result[1];
    }
    
    public static function getMime ($base64_string) {
        $pattern = '/^(data:)([^;]+)/i';
        preg_match($pattern, $base64_string, $matches);
        $result = explode('/', $matches[2]);
        return $result[0];
    }

    public static function getBase64String ($file_name) {
        
    }

    public static function save ($base64_string, $file_name, $extension = '') {
        if($extension == '') {
            $extension = self::getExtension($base64_string);
        }
        if(file_exists($file_name . '.' . $extension)) {
            $file_name = Files::getNewFileName($file_name, $extension);
        }
        $ifp = fopen($file_name . '.' . $extension, "wb"); 
        $data = explode(',', $base64_string);
        try {
            fwrite($ifp, base64_decode($data[1])); 
            fclose($ifp);
            return [0, $file_name . '.' . $extension];
        }
        catch (Exception $ex) {
            self::$lastError = $ex;
            return [1, $ex];
        }
    }
    
    
    public static function getLastError() {
        return self::$lastError;
    }
    
}
