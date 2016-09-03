<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of QueryConstructor
 *
 * @author Дмитрий
 */

namespace core\models;

use PDO;


class QueryBuilder {
    public static function constructor($connection) {
        if($connection instanceof PDO) {
            return "core\models\sql\QueryBuilder";
        }
    }
}
