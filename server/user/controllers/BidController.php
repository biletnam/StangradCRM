<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace user\controllers;

use PDO;

/**
 * Description of BidController
 *
 * @author Дмитрий
 */
class BidController extends \system\controllers\ModelController {
    //put your code here
    
    
    public function getAction($params, $fields = array()) {
        $user = \system\auth\Auth::getAuthUser("system_user");
        $query = "select * from bid";
        $equipmentBidSubQuery = "select id from bid";
        /*if($user->role() === 5)
        {
            $query = "select * from bid where id_manager = " . $user->id();
            $equipmentBidSubQuery = "select id from bid where id_manager = " . $user->id();
        }*/
        return $this->loadBid($query, $equipmentBidSubQuery);
    }
    
    private function loadBid ($query, $subquery)
    {
        $db = \configs\Connection::db();
        $sth = $db->prepare($query);
        $sth->execute();
        $bid = $sth->fetchAll(PDO::FETCH_ASSOC);
        $bid = $this->loadPayment($bid, $subquery);
        return [0, $this->loadEquipmentBid($bid, $subquery)];
    }

    private function loadPayment ($parentData, $subquery)
    {
        $db = \configs\Connection::db();
        $query = "select * from payment where id_bid in (" . $subquery . ")";
        $sth = $db->prepare($query);
        $sth->execute();
        $payment = $sth->fetchAll(PDO::FETCH_ASSOC);
        
        for($i = 0; $i < count($parentData); $i++)
        {
            for($j = 0; $j < count($payment); $j++)
            {
                if((int)$parentData[$i]['id'] === (int)$payment[$j]['id_bid'])
                {
                    if(!isset($parentData[$i]['Payment']))
                    {
                        $parentData[$i]['Payment'] = [];
                    }
                    $parentData[$i]['Payment'][] = $payment[$j];
                }
            }
        }
        return $parentData;
    }
    private function loadEquipmentBid ($parentData, $subquery)
    {
        $db = \configs\Connection::db();
        $query = "select * from equipment_bid where equipment_bid.id_bid in (" . $subquery . ")";
        $complectationSubQuery = "select id from equipment_bid where equipment_bid.id_bid in (" . $subquery . ")";
        $sth = $db->prepare($query);
        $sth->execute();
        $equipmentBid = $sth->fetchAll(PDO::FETCH_ASSOC);
        
        $data = $this->loadComplectation($equipmentBid, $complectationSubQuery);
        
        for($i = 0; $i < count($parentData); $i++)
        {
            for($j = 0; $j < count($data); $j++)
            {
                if((int)$parentData[$i]['id'] === (int)$data[$j]['id_bid'])
                {
                    if(!isset($parentData[$i]['EquipmentBid']))
                    {
                        $parentData[$i]['EquipmentBid'] = [];
                    }
                    $parentData[$i]['EquipmentBid'][] = $data[$j];
                }
            }
        }
        return $parentData;
        
    }
    
    private function loadComplectation ($parendData, $subquery)
    {
        $db = \configs\Connection::db();
        $query = "select * from complectation where complectation.id_equipment_bid in (" . $subquery . ")";
        $sth = $db->prepare($query);
        $sth->execute();
        $complectation = $sth->fetchAll(PDO::FETCH_ASSOC);
        
        for($i = 0; $i < count($parendData); $i++)
        {
            for($j = 0; $j < count($complectation); $j++)
            {
                if((int)$parendData[$i]['id'] === (int)$complectation[$j]['id_equipment_bid'])
                {
                    if(!isset($parendData[$i]['Complectation']))
                    {
                        $parendData[$i]['Complectation'] = [];
                    }
                    $parendData[$i]['Complectation'][] = $complectation[$j];
                }
            }
        }
        return $parendData;
    }   
    
    public function saveAction($params) {
        $params['last_modified'] = null;
        $result = parent::saveAction($params);
        if(((int)$result[0]) === 1) return $result;
        if(isset($params['id']) && (int)$params['id'] != 0)
        {
            $bid = \user\models\Bid::get()->where('id', (int)$params['id'])->one();
        }
        else 
        {
            $bid = \user\models\Bid::get()->where('id', $result[1]['id'])->one();
        }
        if($bid === null)
        {
            return [1, "Server Error! Bid not found!"];
        }
        $result[1]['last_modified'] = $bid->LastModified;
        return $result;
    }

    protected function permission($actionType) {
        return [0, 0];
    }
    
    
    public function lastModifiedAction ($params)
    {
        $user = \system\auth\Auth::getAuthUser("system_user");
        if($user === false) {
            return [1, "Permission denied"];
        }
        if(!isset($params['last_modified']))
        {
            return [0, []];
        }

        $query = "select * from bid where last_modified > '" . $params['last_modified'] . "'";
        $equipmentBidSubQuery = "select id from bid where last_modified > '" . $params['last_modified'] . "'";
        return $this->loadBid($query, $equipmentBidSubQuery);
    }
    
    public function generateEquipmentBidSerialNumberAction ($params)    
    {
        if(!isset($params['id']) || (int)$params['id'] == null)
        {
            return [1, 'ID is not found or incorrect'];
        }
        $id = (int)$params['id'];
        return EquipmentBidController::generateSerialNumber($id);
    }
    
}