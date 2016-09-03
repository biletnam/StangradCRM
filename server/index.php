<?php

ini_set('error_reporting', E_ALL);
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);

use core\controllers\FrontController as FrontController;

function __autoload($class) {
    $classPath = str_replace('\\', '/', $class). '.php';
    $found = stream_resolve_include_path($classPath);
    if($found != false) {
        require_once ($classPath);
    }
}

$front = FrontController::getInstance();
try {
    $front->route();
    echo $front->getBody();
}
catch (Exception $ex) {
    echo $ex->getMessage();
}