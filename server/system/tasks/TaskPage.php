<?php

namespace system\tasks;

/**
 * Description of TaskPage
 *
 * @author Дмитрий
 */
final class TaskPage extends \core\views\BaseView {
    private $html;
    
    public function __construct($html) {
        $this->html = $html;
    }

    public function render($custom = null) {
        if(!is_null($custom)) {
            parent::render($custom);
        }
        return parent::render($this->html);
    }

    public function add_out ($name, $value) {
        $this->$name = $value;
    }
    
}
