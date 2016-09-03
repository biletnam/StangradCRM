<!DOCTYPE html>
<html>
    <head></head>
    <body>
        <h1>Stangrad CRM</h1>
    </body>
</html>


<?php
    use debug\FirePHPCore\FirePHP;
    $firephp = FirePHP::getInstance(true);
    $firephp -> fb("message", FirePHP::LOG);

    
    $v = new user\controllers\VersionController();
    
    var_dump($v->checkAction(['current' => '0.00']));