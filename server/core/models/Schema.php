<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Schema
 *
 * @author Дмитрий
 */
namespace core\models;
use PDO;

class Schema {

    public static function getSchema ($connection) {
        if($connection instanceof \PDO) {
            $driver = $connection->getAttribute(PDO::ATTR_DRIVER_NAME);
            $method = ucfirst($driver) . 'Schema';
            return self::$method();
        }
    }

    private static function sqliteSchema () {
        return "core\models\sql\schema\SqliteSchema";
    }
    private static function mysqlSchema () {
        return "core\models\sql\schema\MysqlSchema";
    }
    
    public static function __callStatic($method, $arguments) {
        call_user_func_array($this->$method, $arguments);
    }
}
