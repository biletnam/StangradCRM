<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

/**
 * Description of ModificationController
 *
 * @author Дмитрий
 */
class ModificationController extends \system\controllers\ModelController {
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
        $modification = \user\models\Modification::get()->where('id', $id)->one();
        if($modification == null)
        {
            return [1, "Запись с идентификатором " . $id . " не найдена!"];
        }
        $nextModification = null;
        
        if($up) 
        {
            $nextModification = \user\models\Modification::get()
                    ->where('id_equipment', $modification->IdEquipment)
                    ->where('row_order', $modification->RowOrder, '>')
                    ->order(['row_order'])->one();
        }
        else
        {
            $nextModification = \user\models\Modification::get()
                    ->where('id_equipment', $modification->IdEquipment)
                    ->where('row_order', $modification->RowOrder, '<')
                    ->order(['row_order'], "desc")->one();
        }
        if($nextModification == null)
        {
            return [0, 0];
        }
        $currentRowOrder = $modification->RowOrder;
        $nextRowOrder = $nextModification->RowOrder;
        
        $modification->RowOrder = -1;
        $result = $modification->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[2]];
        }
        $nextModification->RowOrder = $currentRowOrder;
        $result = $nextModification->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[2]];
        }
        $modification->RowOrder = $nextRowOrder;
        $result = $modification->save();
        if($result[0] != 0)
        {
            return [1, "Ошибка при выполнении запроса.\n" . $result[2]];
        }
        return [0, $nextModification->Id];
    }
    
}
