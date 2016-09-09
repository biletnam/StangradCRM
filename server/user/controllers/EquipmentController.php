<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

use PDO;

/**
 * Description of EquipmentController
 *
 * @author Дмитрий
 */
class EquipmentController extends \system\controllers\ModelController {
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
        $equipment = \user\models\Equipment::get()->where('id', $id)->one();        
        if($equipment == null)
        {
            return [1, "Запись с идентификатором " . $id . " не найдена!"];
        }
        
        $nextEquipment = null;
        if($up) 
        {
            $nextEquipment = \user\models\Equipment::get()->where('row_order', $equipment->RowOrder, '>')->order(['row_order'])->one();
        }
        else
        {
            $nextEquipment = \user\models\Equipment::get()->where('row_order', $equipment->RowOrder, '<')->order(['row_order'], "desc")->one();
        }
        if($nextEquipment == null)
        {
            return [0, 0];
        }
        $currentRowOrder = $equipment->RowOrder;
        $nextRowOrder = $nextEquipment->RowOrder;
        
        $equipment->RowOrder = -1;
        $result = $equipment->save();       
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[1]];
        }
        $nextEquipment->RowOrder = $currentRowOrder;
        $result = $nextEquipment->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[1]];
        }
        $equipment->RowOrder = $nextRowOrder;
        $result = $equipment->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[1]];
        }
        return [0, $nextEquipment->Id];
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
        $query = "select max(row_order) as max_row_order from equipment";
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
