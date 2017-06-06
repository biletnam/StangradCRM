<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

use PDO;
/**
 * Description of VersionController
 *
 * @author Дмитрий
 */
class VersionController {
    
    public function checkAction($params) {
        if(!isset($params['current'])) {
            return [1, "Текущая версия программы не была получена!"];
        }
        $version = (floatval($params['current']));
        
        $db = \configs\Connection::db();
        $sth = $db->prepare("select version_number, path from versions where version_number = (select max(version_number) from versions)");
        $sth->execute();
        $result = $sth->fetch(PDO::FETCH_ASSOC);
        $max = floatval($result['version_number']);
        if($max > $version)
        {
            if(file_exists($result['path']))
            {
                return [0, 'http://' . $_SERVER['HTTP_HOST'] . '/' . $result['path']];
            }
            return [1, "Server error. File not exist."];
        }
        return [0, ""];
    }
    
}
