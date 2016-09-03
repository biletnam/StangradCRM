<?php
namespace configs;

class Routes {
    
    public static function route($route) {
        $routes = array(
            "auth" => "auth",
            "authvalidate" => "authvalidate",
            "logout" => "logout",
            "admin" => "admin",
            "api" => "ajax",
            "index" => "index",
            "request" => "request",

        );
        return (isset($routes[$route])) ? $routes[$route] : null;
    }
}