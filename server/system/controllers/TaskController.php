<?php

namespace system\controllers;

/**
 * Description of TaskController
 *
 * @author Дмитрий
 */

use core\controllers\IController;

class TaskController implements IController {
    protected $tasksPath = 'app\\tasks';
    
    public function indexAction ($front) {
        $params = $front->getParams();
        if(count($params) < 1) {
            
        }   
        $taskName = $params[0];
        $taskClass = $this->tasksPath . '\\' . $taskName . '\\' . ucfirst($taskName) . 'Task';
        if(!class_exists($taskClass)) {
            
        }
        $task = new $taskClass();
        $front->setBody($task->view());
    }

    public function delete() {
        
    }

    public function get() {
        
    }

    public function post() {
        
    }

    public function put() {
        
    }

}
