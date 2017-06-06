<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

/**
 * Description of BidFilesController
 *
 * @author Дмитрий Строкин
 */
class BidFilesController extends \system\controllers\ModelController {
    //put your code here
    
    //put your code here
    protected function permission($actionType) {
        return [0, 0];
    }
    
    public function saveAction($params) {
        
        $file = $params['file'];
        unset($params['file']);
        
        $fileName = 'files/' . 
                \libs\classes\text\GUID::get_guid() . $params['ext'];
        
        $ifp = fopen(__DIR__ . '/../../' . $fileName, "a+"); 
        fwrite($ifp, base64_decode(str_replace(' ', '+', $file))); 
        fclose($ifp);
        
        $params['path'] = $fileName;
        $saveResult = parent::saveAction($params);            
        if($saveResult[0] === 0) {
            $saveResult[1]['path'] = $fileName;
        }
        return $saveResult;
    }
    
    public function deleteAction($params) {
        $bidFile = \user\models\BidFiles::create((int)$params['id']);
        $path = $bidFile->Path;
        $deleteResult = parent::deleteAction($params);
        
        $fileName = __DIR__ . '/../../' . $path;
        
        if($deleteResult[0] === 0) {
            if(file_exists($fileName)) {
                unlink($fileName);
            }
        }
        return $deleteResult;
    }
    
    public function savechuckAction ($params) {
        if(!isset($params['id']) || !isset($params['chuck'])) {
            return [1, 'Параметры не были получены!'];
        }
        $id = (int)$params['id'];
        $chuck = $params['chuck'];
        if($id === 0) return [1, 'Id некорректен'];
        
        $bidFile = \user\models\BidFiles::create($id);
        $path = $bidFile->Path;
        
        $fileName = __DIR__ . '/../../' . $path;
        
        $ifp = fopen($fileName, "a+"); 
        fwrite($ifp, base64_decode(str_replace(' ', '+', $chuck))); 
        fclose($ifp);
        
        return [0, 0];
        
    }
    
}

