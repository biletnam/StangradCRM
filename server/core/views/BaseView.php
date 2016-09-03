<?php

namespace core\views;

/**
 * Description of BaseView
 *
 * @author Дмитрий
 */
abstract class BaseView {
    public function render ($file) {
        ob_start();
        include ($file);
        return ob_get_clean();
    }    
}
