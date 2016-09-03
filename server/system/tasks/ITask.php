<?php

/**
 *
 * @author Дмитрий
 */

namespace system\tasks;

interface ITask {
    function run(array $params, ITask $subtask = null);
}