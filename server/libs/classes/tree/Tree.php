<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Tree
 *
 * @author Дмитрий
 */
class Tree {
    private static $tmp_array = array();

    public static function create_from_array($id_key, $ref_key, $array) {
        $map = array();
        for($i = 0; $i < count($array); $i++) {
            $map['' . $array[$i][$id_key] . ''] = $array[$i];
            $map['' . $array[$i][$id_key] . '']['childs'] = array();
        }
        foreach($map as $k => $item) {
            if($item[$ref_key] != 0) {
                if(!isset($map[$item[$ref_key]]['childs'])) {
                    $map[$item[$ref_key]]['childs'] = array();
                }
                $map[$item[$ref_key]]['childs'][$k] = & $map[$k];
                unset($map[$k]);
            }
        }
        return $map;
    }
    
    public static function show_item_tree($item) {
        self::$tmp_array = null;
        if($item['childs'] !== null) {
            foreach($item['childs'] as $k => $v) {
                self::setToArray($v, 0);
            }
        }
        return self::$tmp_array;
    }
    
    private static function setToArray($item, $lvl) {
        $childs = null;
        if($item['childs'] !== null) {
            $childs = $item['childs'] ;
            unset($item['childs']);
        }
        self::$tmp_array[] = array('item' => $item, 'level' => $lvl);
        if($childs !== null) {
            $lvl = $lvl + 1;
            foreach($childs as $k => $v) {
                self::setToArray($v, $lvl);
            }
        }
    }
    
}
