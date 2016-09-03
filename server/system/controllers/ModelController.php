<?php

namespace system\controllers;
use Exception;

abstract class ModelController {

    protected static $model = null;

    
    public function __construct($model = null) {
        static::$model = $model;
        if(static::$model === null) {
            $currentClassNamePath = get_called_class();
            $modelName = str_replace('controllers', 'models', $currentClassNamePath);
            $modelName = str_replace('Controller', '', $modelName);
            if(class_exists($modelName)) {
                static::$model = $modelName;
            }
            else {
                throw new Exception('Class not found', '-1', null);
            }
        }
    }

    public function modelName () {
        return static::$model;
    }
    
    public function custom ($params, $fields = []) {
        $model = static::$model;
        $pk = $model::pk();
        if(isset($params[$pk])) {
            //return $model::get()->where;
        }
    }

    public function getAction($params, $fields = []) {
        $permission = $this->permission("get");
        if($permission[0] === 1) {
            return $permission;
        }
        $model = static::$model;
        $query = $model::get();
        foreach ($params as $key => $value) {
            if($model::issetField($key)) {
                $query = $query->where($key, $value);
            }
        }
        $items = $query->all();
        return [0, static::toArray($items, $fields)];
    }
    
    public function saveAction($params) {
        $permission = $this->permission("save");
        if($permission[0] == 1) {
            return $permission;
        }
        $model = static::$model;
        $pk = $model::pk();
        if(isset($params[$pk])) {
            $key = (int)$params[$pk];
            if($key == 0) {
                return array(1, 'Primary key is not valid');
            }
            unset($params[$pk]);
            return $this->update($key, $params);
        }
        else {
            return $this->insert($params);
        }
    }
    public function deleteAction($params) {
        $permission = $this->permission("delete");
        if($permission[0] == 1) {
            return $permission;
        }
        $model = static::$model;
        $pk = $model::pk();
        if(isset($params[$pk])) {
            $key = (int)$params[$pk];
            if($key == 0) {
                return array(1, 'Primary key is not valid');
            }
            $modelItem = $model::create($key);
            return $modelItem->delete();
        }
        else {
            return array(1, 'Primary key is not valid');
        }
    }
    
    public function permissionAction() {
        return $this->permission();
    }

    protected function insert($params) {
        $model = static::$model;
        $modelItem = $model::create();
        foreach ($params as $key => $value) {
            $property = \system\libs\FormatModelPropertyName::format($key);
            $modelItem->$property = $value;
        }
        return $modelItem->save();
    }
    
    protected function update($key, $params) {
        $model = static::$model;
        $modelItem = $model::create($key);
        foreach ($params as $key => $value) {
            $property = \system\libs\FormatModelPropertyName::format($key);
            $modelItem->$property = $value;
        }
        return $modelItem->save();
    }
    
    public static function toArray($items, $fields = []) {
        $model = static::$model;
        if(count($fields) === 0) {
            $fields = $model::fields();
        }
        $result = [];
        for($i = 0; $i < count($items); $i++) {
            $values = [];
            for($j = 0; $j < count($fields); $j++) {
                $property = \system\libs\FormatModelPropertyName::format($fields[$j]);
                $values[$fields[$j]] = $items[$i]->$property;
            }
            $result[] = $values;
        }
        return $result;
    }

    public static function getFieldType ($fieldName) {
        $model = static::$model;
        
        if(!$model::issetField($fieldName)) {
            return null;
        }
        $type = $model::getFieldType($fieldName);
        return preg_split('/[\(\)]/', $type, -1, PREG_SPLIT_NO_EMPTY);
    }

    public static function scheme () {
        $model = static::$model;
        $scheme = [];
        $fields = $model::fields();
        for($i = 0; $i < count($fields); $i++) {
            $scheme[$fields[$i]] = static::getFieldType($fields[$i]);
        }
        return $scheme;
    }
    
    public static function count () {
        $model = static::$model;
        return $model::count();
    }

    
    public static function pk () {
        $model = static::$model;
        return $model::pk();
    }
    
    public static function isPk ($name) {
        $model = static::$model;
        return $model::isPrimaryKey($name);
    }

    public static function fk () {
        return [];
    }
    
    public static function isFk ($name) {
        $model = static::$model;
        return $model::isForeignKey($name);
    }

    protected function permission($actionType) {
        return [1, "Permission denied"];
    }
    
    private function createSetter($name) {        
        return \system\libs\FormatModelPropertyName::setter($name);
    }
    
}
