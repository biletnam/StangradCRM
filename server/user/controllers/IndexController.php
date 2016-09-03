<?php

namespace user\controllers;
use core\controllers\IController;
use core\views\View;


class IndexController implements IController{
    
    function indexAction($frontController) {
        $view = new View();        
        $result = $view->render('user/site/index.php');
        $frontController->setBody($result);
    }
}