<?php

namespace core\controllers;
use configs\DefaultClassConfig as classConfig;
use configs\Routes as Routes;
use configs\ClassPaths as classPaths;
use system\libs\ReflectionOperations;
use ReflectionException;

class FrontController {
    protected $_controller, $_action, $_params, $_body;
    static $_instance;
    
    public static function getInstance() {
        if(!(self::$_instance instanceof self)) {
            self::$_instance = new self();
        }
        return self::$_instance;
    }
    
    private function requestUri () {
        $uri = urldecode(\system\libs\InputData::server('REQUEST_URI', 0));
        $uriNoGet = explode('?', $uri);
        return explode('/', $uriNoGet[0]);
    }
    private function post () {
        return \system\libs\InputData::post();
    }
    private function get () {
        return \system\libs\InputData::get();
    }
    private function put () {
        return \system\libs\InputData::put();
    }

    private function setDefault ($params = null) {
        $this->_controller = classConfig::controllerName();
        $this->_action = classConfig::methodName();
        if ($params === null) {
            $params = $this->post();
        }
        $this->_params = $params;
    }

    private function __construct() {
        
        $uriArray = $this->requestUri();        
        if(count($uriArray) === 0 || empty($uriArray[0])) {
            $this->setDefault();
            return;
        }
        $route = Routes::route(urldecode(array_shift($uriArray)));
        if($route === null) {
            $this->setDefault($uriArray);
            return;
        }
        if(is_array($route)) {
            $last = array_pop($route);
            $this->_controller = implode('/', $route) . '/' . ucfirst($last) . 'Controller';
        }
        else {
            $this->_controller = ucfirst($route).'Controller';
        }
        $this->_action = 'indexAction';
        $this->_params = $uriArray;
        
        if($this->put() !== null) {
            echo $this->put();
        }
        if($this->post() !== null) {
            $this->_params ['post'] = $this->post();
        }
        if ($this->get() !== null) {
            $this->_params ['get'] = $this->get();
        }        
    }

    public function route() {
        $paths = classPaths::paths();
        $is_find = false;
        for($i = 0; $i < count($paths); $i++) {
            if(file_exists(str_replace('\\', '/', $paths[$i]) . $this->_controller . '.php')) {
                $is_find = true;
                break;
            }
        }
        if(!$is_find) {
            $this->_controller = 'IndexController';
        }
        $class = ReflectionOperations::createClass($this->_controller, $paths);
        if($class === null) {
            throw new Exception('Class ' . $this->_controller . ' not exist.');
        }
        $method = ReflectionOperations::createMethod($class, $this->_action);
        if($method === null) {
            throw new Exception('Method ' . $this->_action . ' not exist.');
        }
        $result = ReflectionOperations::invoke($class, $method, self::$_instance);
        if($result instanceof ReflectionException) {
            throw new Exception($result->getMessage());
        }
    }

    function getParams() {
        return $this->_params;
    }

    function getController() {
        return $this->_controller;
    }

    function getAction() {
        return $this->_action;
    }

    function getBody() {
        return $this->_body;
    }

    public function setBody($body) {
        $this->_body = $body;
    }
}
?>
