<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

use PDO;
/**
 * Description of SellerController
 *
 * @author Дмитрий
 */
class SellerController extends \system\controllers\ModelController {
    //put your code here
    protected function permission($actionType) {
        return [0, 0];
    }
    
    public function rowUpAction($params)
    {
        if(!isset($params['id']))
        {
            return [1, "Идентификатор не был получен!"];
        }
        if((int)$params['id'] == 0)
        {
            return [1, "Получен некорректный идентификатор!"];
        }
        return $this->changeRowOrder((int)$params['id']);
    }
    
    public function rowDownAction($params)
    {
        if(!isset($params['id']))
        {
            return [1, "Идентификатор не был получен!"];
        }
        if((int)$params['id'] == 0)
        {
            return [1, "Получен некорректный идентификатор!"];
        }
        return $this->changeRowOrder((int)$params['id'], false);
    }
    
    private function changeRowOrder ($id, $up = true)
    {
        $seller = \user\models\Seller::get()->where('id', $id)->one();        
        if($seller == null)
        {
            return [1, "Запись с идентификатором " . $id . " не найдена!"];
        }
        
        $nextSeller = null;
        if($up) 
        {
            $nextSeller = \user\models\Seller::get()->where('row_order', $seller->RowOrder, '>')->order(['row_order'])->one();
        }
        else
        {
            $nextSeller = \user\models\Seller::get()->where('row_order', $seller->RowOrder, '<')->order(['row_order'], "desc")->one();
        }
        if($nextSeller == null)
        {
            return [0, 0];
        }
        $currentRowOrder = $seller->RowOrder;
        $nextRowOrder = $nextSeller->RowOrder;
        
        $seller->RowOrder = -1;
        $result = $seller->save();       
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[1]];
        }
        $nextSeller->RowOrder = $currentRowOrder;
        $result = $nextSeller->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[1]];
        }
        $seller->RowOrder = $nextRowOrder;
        $result = $seller->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[1]];
        }
        return [0, $nextSeller->Id];
    }
    
    public function saveAction($params) {
        if(isset($params['id']) && ((int)$params['id']) != 0)
        {
            return parent::saveAction($params);
        }
        $row_order = $this->generateRowOrder();
        $params['row_order'] = $row_order;
        $result = parent::saveAction($params);
        if($result[0] == 1)
        {
            return $this->saveAction($params);
        }
        $result[1]["row_order"] = $row_order;
        return $result;
    }
    
    private function generateRowOrder ()
    {
        $row_order = 1;
        $query = "select max(row_order) as max_row_order from seller";
        $db = \configs\Connection::db();
        $sth = $db->prepare($query);
        $sth->execute();
        $result = $sth->fetch(PDO::FETCH_ASSOC);
        if(!isset($result['max_row_order']) || ((int)$result['max_row_order']) === 0) 
        {
            return $row_order;
        }
        return ((int)$result['max_row_order']+1);
    }
    
}
