<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;


use PDO;

/**
 * Description of EquipmentBidController
 *
 * @author Дмитрий
 */
class EquipmentBidController extends \system\controllers\ModelController {
    //put your code here
    protected function permission($actionType) {
        return [0, 0];
    }
    
    public static function  generateSerialNumber ($bid_id)
    {
        $equipmentBid = \user\models\EquipmentBid::get()->where('id_bid', $bid_id)->all();
        $equipmentBids = [];
        for($i = 0; $i < count($equipmentBid); $i++)
        {
            if(((int)$equipmentBid[$i]->SerialNumber !== 0)) 
            {
                continue;
            }
            $equipmentBids[] = [
                "Id" => (int)$equipmentBid[$i]->Id, 
                "SerialNumber" => (int)self::getSerialNumber($equipmentBid[$i])
            ];
        }
        return [0, $equipmentBids];
    } 
    
    private static function getSerialNumber ($equipmentBid)
    {
        if($equipmentBid->SerialNumber != 0) return $equipmentBid->SerialNumber;
        $query = "select max(serial_number) as next_serial_number from equipment_bid where id_modification = " . $equipmentBid->IdModification;
        $db = \configs\Connection::db();
        $sth = $db->prepare($query);
        $sth->execute();
        $result = $sth->fetch(PDO::FETCH_ASSOC);
        $next_serial_number = 1;
        if(!isset($result['next_serial_number']))
        {
            $modification = \user\models\Modification::get()->where('id', $equipmentBid->IdModification)->one();
            if($modification == null) return $next_serial_number;
            $equipment = \user\models\Equipment::get()->where('id', $modification->IdEquipment)->one();
            if($equipment == null) return $next_serial_number;
            $next_serial_number = $equipment->SerialNumber + 1;
        }
        else 
        {
            $next_serial_number = (int)$result['next_serial_number']+1;
        }

        $equipmentBid->SerialNumber = $next_serial_number;
        $result = $equipmentBid->save();
        if($result[0] == 1)
        {
            return self::getSerialNumber($equipmentBid);
        }
        return $next_serial_number;
    }
    
}
