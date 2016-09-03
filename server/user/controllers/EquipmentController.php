<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

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
            return [1, "Ошибка при выполнении запроса.\n" . $result[2]];
        }
        $nextEquipment->RowOrder = $currentRowOrder;
        $result = $nextEquipment->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[2]];
        }
        $equipment->RowOrder = $nextRowOrder;
        $result = $equipment->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[2]];
        }
        return [0, $nextEquipment->Id];
    }
    
}
