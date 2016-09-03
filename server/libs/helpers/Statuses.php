<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Statuses
 *
 * @author Дмитрий
 */
class Statuses {
    
    private static $purchaseTitleStatusesArray = array("Собирается", "СТОП", "Отправлен", "Получен");
    private static $purchaseColorStatusesArray = array("white", "lightblue", "yellow", "lightgreen", "gray");
    private static $productTitleStatusesArray = array('Ожидает оплаты', 'Частично оплачен', 'Оплачен', 'Товар не пришел', 'Возврат денег', 'Товар отдан заказчику', 'Отказ заказчика');
    private static $productColorStatusesArray = array('white', 'lightblue', 'darkturquoise', 'yellow', 'lightcoral', 'greenyellow', 'chocolate');

    public static function getTitlePurchaseStatusesArray () {
        return self::$purchaseTitleStatusesArray;
    }

    public static function getTitlePurchaseStatus($num) {
        $result = "Неизвестно";
        if(isset(self::$purchaseTitleStatusesArray[$num])) {
            $result = self::$purchaseTitleStatusesArray[$num];
        }
        return $result;
    }
    
    public static function getColorPurchaseStatusesArray () {
        return self::$purchaseColorStatusesArray;
    }

    public static function getColorPurchaseStatus($num) {
        $result = "transparent";
        if(isset(self::$purchaseColorStatusesArray[$num])) {
            $result = self::$purchaseColorStatusesArray[$num];
        }
        return $result;
    }
    
    public static function getTitleProductStatusesArray () {
        return self::$productTitleStatusesArray;
    }
    
    public static function getTitleProductStatus($num) {
        $result = "Неизвестно";
        if(isset(self::$productTitleStatusesArray[$num])) {
            $result = self::$productTitleStatusesArray[$num];
        }
        return $result;
    }
    
    public static function getColorProductStatusesArray () {
        return self::$productColorStatusesArray;
    }    
    
    public static function getColorProductStatus($num) {
        $result = "white";
        if(isset(self::$productColorStatusesArray[$num])) {
            $result = self::$productColorStatusesArray[$num];
        }
        return $result;
    }    
    
}
