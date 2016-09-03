<?php

/**
 * Description of BaseTask
 *
 * @author Дмитрий
 */

namespace system\tasks;

abstract class BaseTask implements ITask {
    protected $subtask_params = [];
    protected $subtask_subtask = null;
    
    protected $indexPage = null;
    protected $pages = [];

    protected $before_exec_callback = null;
    protected $after_exec_callback = null;
    
    protected $model = null;
            
    function run(array $params, ITask $subtask = null) {
        if(is_callable($this->before_exec_callback)) {
            $this->before_exec_callback($this);
        }
        if(!is_null($subtask)) {
            $subtask->run($this->subtask_params, $this->subtask_subtask);
        }
        $this->exec($params);
        if(is_callable($this->after_exec_callback)) {
            return $this->after_exec_callback($this);
        }
    }
    
    abstract protected function exec(array $params);
    
    abstract protected function render (TaskPage $page);
    
    protected function renderIndex () {
        if(!is_null($this->indexPage) 
                && ($this->indexPage instanceof TaskPage)) {
            return $this->indexPage->render();
        }
        else {            
            throw new \Exception("Index page is null or not TaskPage");
        }
    }
    
    protected function renderPage ($name) {
        if(array_key_exists($name, $this->pages) 
                && ($this->pages[$name] instanceof TaskPage)) {
            $this->render($this->pages[$name]);
        }
        else {
            throw new \Exception("Page is null or not TaskPage");
        }
    }
    
    public function view () {
        return $this->renderIndex();
    }
}
