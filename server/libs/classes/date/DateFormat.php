<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of DateFormat
 *
 * @author Дмитрий
 */

namespace libs\classes\date;

use DateTime;

class DateFormat {
    private static $ru_months = array (
        "январь",
        "февраль",
        "март",
        "апрель",
        "май",
        "июнь",
        "июль",
        "август",
        "сентябрь",
        "октябрь",
        "ноябрь",
        "декабрь"
    );
    
    public static function toRus($latin_date) {
        $date = new DateTime($latin_date);
        $date_array = explode('.', $date->format("d.m.Y"));
        return $date_array[0] . ' ' . self::$ru_months[(int)$date_array[1]-1] . ' ' . $date_array[2];
    }
}
