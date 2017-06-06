<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of FileSystem
 *
 * @author user
 */

namespace system\models;

use libs\classes\file\Base64Operations;

define('ROOT_PATH_CONTENT', 'files');

class FileSystem {
            
    function getItems ($param) {
        if(!isset($param['id']) || !isset($param['id'][0]) || $param['id'] == '/' || $param['id'][0] == '/') {
            $param['id'] = ROOT_PATH_CONTENT;
        }
        else {
            if(isset($param['name']) && $param['name'] === '..') {
                $tmp_arr = explode('/', $param['id']);
                if(count($tmp_arr) > 2) {
                    array_pop($tmp_arr);
                    array_pop($tmp_arr);
                    $param['id'] = implode('/', $tmp_arr);
                }
                else {
                    $param['id'] = ROOT_PATH_CONTENT;
                }
                
            }
        }
        $files = scandir($param['id']);
        $data = array();
        foreach ($files as $row) {
            if($row != '.') {
                $fullpath = $param['id'];
                if(is_dir($fullpath . '/' .$row)) {
                    $data[] = array('id' => '/' . $fullpath . '/' .$row, 'type' => 'dir', 'name' => $row);
                }
                else {
                    $data[] = array('id' => '/' . $fullpath . '/' .$row, 'type' => pathinfo($row, PATHINFO_EXTENSION), 'name' => $row);
                }
            }
        }
        return $data;
    }
    
    function insert($params) {
        if($params['type'] == null) {
            if(isset($params['id']) && $params['id'] !== '') {
                mkdir($params['id'] . '/' . $params['name']);
                return json_encode(array(array("type" => "dir", "id" => $params['id'] . '/' . $params['name'])));
            }
            else {
                mkdir(ROOT_PATH_CONTENT . '/' . $params['name']);
                return json_encode(array(array("type" => "dir", "id" => ROOT_PATH_CONTENT . '/' . $params['name'])));
            }
        }
        else {
            $ext = Base64Operations::getExtension($params['type']);
            if($ext == 'jpeg') {
                $ext = 'jpg';
            }
            
            if(isset($params['id']) && $params['id'] !== '') {
                $path = $params['id'] . '/' . $params['name'];
                $result = Base64Operations::save($params['type'], $path, $ext);
                if($result[0] == 0) {
                    return [["type" => "file", "id" => $params['id'] . '/' . $result[1]]];
                }
                else {
                    return [1, $result[2]];
                }
            }
            else {
                $path = ROOT_PATH_CONTENT . '/' . $params['name'];
                $result = Base64Operations::save($params['type'], $path, $ext);
                if($result[0] == 0) {
                    return [["type" => "file", "id" => ROOT_PATH_CONTENT . '/' . $result[1]]];
                }
                else {
                    return [1, $result[2]];
                }                
            }
        }
    }
    
    function delete($params) {
        if(is_dir($params['id'])) {
            $this->removeDirectory($params['id']);
        }
        else {
            unlink($params['id']);
        }
    }
    
    function removeDirectory($dir) {
      if ($objs = glob($dir."/*")) {
         foreach($objs as $obj) {
           is_dir($obj) ? $this->removeDirectory($obj) : unlink($obj);
         }
      }
      rmdir($dir);
    }
    
}