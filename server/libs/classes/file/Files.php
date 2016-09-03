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

namespace libs\classes\file;

class Files {
    
    public static function getNewDirName($old_name) {
        if(!file_exists($old_name)) {
            return $old_name;
        }
        else {
            $exit = false;
            $i = 0;
            while (!$exit) {
                $name = $old_name;
                if(!file_exists($name . '_' . $i)) {
                    return $name . '_' . $i;
                }
                $i++;
            }
        }
    }

    public static function getNewFileName($old_name, $ext) {
        if(!file_exists($old_name . '.' . $ext)) {
            return $old_name;
        }
        else {
            $exit = false;
            $i = 0;
            while (!$exit) {
                $name = $old_name;
                if(!file_exists($name . '_' . $i . '.' . $ext)) {
                    return $name . '_' . $i;
                }
                $i++;
            }
        }
    }
    
    public static function createDir($name) {
        $newname = self::getNewDirName($name);
        mkdir($newname);
        return $newname;
    }
    
    public static function getPath($name) {
        $dirs = split('/', $name);
        return $dirs;
    }
    
    public static function removeDirectory($dir) {
      if ($objs = glob($dir."/*")) {
         foreach($objs as $obj) {
           is_dir($obj) ? self::removeDirectory($obj) : unlink($obj);
         }
      }
      rmdir($dir);
    }
    
    public static function renameDirectory($old_dir, $new_dir) {
        if($objs = glob($old_dir."/*")) {
            foreach ($objs as $obj) {
                is_dir($obj) ? self::renameDirectory($old_dir . '/' . $obj, $new_dir . '/' . $obj) : rename($old_dir, $new_dir);
            }
        }
    }
    
}
