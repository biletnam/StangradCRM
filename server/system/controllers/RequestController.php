<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of RequestController
 *
 * @author Дмитрий
 */

namespace system\controllers;
use core\controllers\IController;
use configs\ClassPaths;
use system\libs\ReflectionOperations;
use ReflectionException;

class RequestController implements IController {

    protected function response ($result) {
        $responseUrl = \system\libs\InputData::server('HTTP_REFERER');
        if($result[0] === 0) {
            if(isset($result[1])) {
                $responseUrl[] = '?' . $result[1]['key'] . '=' . $result[1]['value'];
            }
        }
        else {
            $errorMessage = 'Unknown error';
            if(is_array($result[1])) {
                $errorMessage = $result[1][1];
            }
            else if($result[1] instanceof \Exception) {
                $errorMessage = $result[1]->getMessage();
            }
            else {
                $errorMessage = $result[1];
            }
            $responseUrl[] = '?error=1&' . 'message=' . $errorMessage;
        }
        $response = implode('/', $responseUrl);
        header('location: ' . $response);
        return;
    }

    protected function error ($message, $code = null) {
        if($code === null)
        {
            $this->response([1, $message]);
        }
        else 
        {
            $this->response([$code, $message]);
        }
    }

    function indexAction($frontController) {
        $params = $frontController->getParams();        
        if(isset($params['post'])) {
            $params = $params['post'];
        }
        else {
            if(isset($params['get'])) {
                $params = $params['get'];
            }
        }
        if(!array_key_exists('entity', $params))
        {
            return $this->error('Class name not exist');;
        }
        $entity = explode('/', $params['entity']);
        $class = ReflectionOperations::createClass($entity[0] . 'Controller', ClassPaths::paths());
        if($class === null) {
            return $this->error('Class ' . $entity[0] . ' not exist');
            
        }
        $method = ReflectionOperations::createMethod($class, $entity[1] . 'Action');
        if($method === null) {
            return $this->error('Method ' . $entity[1] . ' not exist');
        }
        unset($params['entity']);
        $result = ReflectionOperations::invoke($class, $method, $params);
        if($result instanceof ReflectionException) {
            return $this->error($result->getMessage(), $result->getCode());
        }
        return $this->response($result);
    }
}


