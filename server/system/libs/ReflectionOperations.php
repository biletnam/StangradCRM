<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace system\libs;
use ReflectionClass;
use ReflectionException;

/**
 * Description of CreateReflection
 *
 * @author Дмитрий
 */
class ReflectionOperations {
    
    public static function createClass($classname, $paths) {
        for($i = 0; $i < count($paths); $i++) {
            if(class_exists($paths[$i] . $classname)) {
                return new ReflectionClass($paths[$i] . $classname);
            }
        }
        return null;
    }
    
    public static function createMethod($class, $methodname) {
        if($class->hasMethod($methodname)) {
            return $class->getMethod($methodname);
        }
        return null;
    }

    public static function invoke ($class, $method, $params) {
        $instance = $class->newInstance();
        try {
            return $method->invoke($instance, $params);
        }
        catch (ReflectionException $ex) {
            return $ex;
        }
    }
    
}
