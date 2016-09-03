<?php

namespace core\models\sql;
use configs\Connection;
use PDO;
use PDOException;

class SQLExecutor {

    private $dbh = null;
    private $resultCache = [];
    private $queries = [];
    public static $lastError = null;
    

    public function __construct($queries, $connection = null) {
        if($connection == null) {
            $this->dbh = Connection::db();
        }
        else {
            $this->dbh = $connection;
        }
        $this->dbh->beginTransaction();
        $this->queries = $queries;
    }
    
    function all($caller) {
        for($i = 0; $i < count($this->queries); $i++) {
            $query = $this->queries[$i]['query'];
            $values = $this->queries[$i]['values'];
            $type = $this->queries[$i]['type'];
            $sth = $this->dbh->prepare($query);        
            try {
                $sth->execute($values);
                $this->execRules($sth, $type);
            }
            catch (PDOException $ex) {
                $this->dbh->rollBack();
                $this->resultCache = [];
                self::$lastError = $ex;
                return false;
            }
        }
        $this->dbh->commit();
        if($caller) {
            return $caller::populate($this->resultCache[0]);
        }
        return $this->resultCache;
    }

    function one($caller) {
        for($i = 0; $i < count($this->queries); $i++) {
            $query = $this->queries[$i]['query'];
            $values = $this->queries[$i]['values'];
            $type = $this->queries[$i]['type'];
            $sth = $this->dbh->prepare($query);
            try {
                $sth->execute($values);
                if($i == 0) {
                    $this->execRules($sth, $type, 'one');
                }
                else {
                    $this->execRules($sth, $type);
                }
            }
            catch (PDOException $ex) {
                $this->dbh->rollBack();
                $this->resultCache = [];
                self::$lastError = $ex;
                return false;
            }
        }
        $this->dbh->commit();
        if($caller) {
            return $caller::add($this->resultCache[0]);
        }
        return $this->resultCache;
    }
    
    public function exec($caller) {
        for($i = 0; $i < count($this->queries); $i++) {
            $query = $this->queries[$i]['query'];
            $values = $this->queries[$i]['values'];
            $type = $this->queries[$i]['type'];
            $sth = $this->dbh->prepare($query);
            for($i = 0; $i < count($values); $i++) {
                if($values[$i] == 'NULL') {
                    $sth->bindValue($i+1, NULL, \PDO::PARAM_NULL);
                }
                else if(is_integer($values[$i])) {
                    $sth->bindValue($i+1, $values[$i], \PDO::PARAM_INT);
                }
                else {
                    $sth->bindValue($i+1, $values[$i], \PDO::PARAM_STR);
                }
            }
            try {
                $sth->execute();
                $this->execRules($sth, $type);
            }
            catch (PDOException $ex) {
                $this->dbh->rollBack();
                $this->resultCache = [];
                self::$lastError = $ex;
                return false;
            }
        }
        $this->dbh->commit();
        if($caller) {
            return $this->resultCache[0];
        }
        return $this->resultCache;
    }
    
    private function execRules ($sth, $type, $count = 'all') {
        if($type == 'select') {
            if($count == 'one') {
                $this->resultCache[] = $sth->fetch(PDO::FETCH_ASSOC);
            }
            else {
                $this->resultCache[] = $sth->fetchAll(PDO::FETCH_ASSOC);
            }
        }
        else if($type == 'insert') {
            $this->resultCache[] = $this->dbh->lastInsertId();
        }
        else {
            $this->resultCache[] = true;
        }
    }
    
}
