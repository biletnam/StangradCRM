<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Connection
 *
 * @author Дмитрий
 */

namespace configs;    
use PDO;

class Connection {
   
    private $engine = 'mysql';
    private $host = '';
    private $database = '';
    private $user = '';
    private $pass = '';
    private static $db = null;

    private function __construct(){
        $dns = $this->engine.':dbname='.$this->database.";host=".$this->host;
        self::$db = new PDO($dns, $this->user, $this->pass);
        self::$db->exec('SET CHARACTER SET utf8');
        self::$db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        self::$db->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);
    }
    
    private function __clone() {}
    public static function db()
    {
        if(self::$db === null) {
            new Connection ();
        }
        return self::$db;
    }
}
