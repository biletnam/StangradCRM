<?php

namespace system\controllers;
use www\models\FileSystem;
class FileSystemController {

    function getAction($param) {
        $answer = array();
        $view = new FileSystem();
        $answer[0] = 0;
        $answer[1] = $view->getItems($param);
        return $answer;
    }

    public function saveAction($param) {
        $answer = array();
        $view = new FileSystem();
        $answer[0] = 0;
        $answer[1] = $view->insert($param);
        return $answer;
    }

    public function deleteAction($param) {
        $view = new FileSystem();
        return $view->delete($param);
    }

    /*public function updateAction($param) {
        $view = new Colors();
        echo $view->update($param);
    }*/
}
?>