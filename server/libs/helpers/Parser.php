<?php

class Parser
{
	public $result;
	private $_lexems;
        private $id = 0;
        private $nid = array();
        public function parse($content, $id=0)
	{
		if($this->_parseLexems($content))
		{
			// var_dump($this->_lexems);
			
			if($this->_checkSyntax())
			{
                                if($id !== 0) {
                                    $this->id = $id;
                                }
				return $this->_compute();
			}
		}
	}

	private function _parseLexems($content)
	{
		/*static $keywords = [["{MODULE", Lexem::TYPE_MODULE_START],
			["{/MODULE", Lexem::TYPE_MODULE_END],
			["{TEMPLATE", Lexem::TYPE_TEMPLATE_START],
			["{/TEMPLATE", Lexem::TYPE_TEMPLATE_END]
		];*/
            
                static $keywords = array(array("{MODULE", Lexem::TYPE_MODULE_START),
                            array("{/MODULE", Lexem::TYPE_MODULE_END),
                            array("{TEMPLATE", Lexem::TYPE_TEMPLATE_START),
                            array("{/TEMPLATE", Lexem::TYPE_TEMPLATE_END)
                    );

		$pos = 0;
		while($pos < strlen($content))
		{
			//ищем все ближайшие ключевые слова:
			$lexems = array();
			for($i = 0; $i < count($keywords); $i++)
			{
				$lexemStartPos = strpos($content, $keywords[$i][0], $pos);
				if($lexemStartPos !== FALSE)
				{
					$lexemEndPos = strpos($content, "}", $lexemStartPos);
					if($lexemEndPos === FALSE)
						$lexemEndPos = strlen($content);
					$lexem = new Lexem();
					$lexem->type = $keywords[$i][1];
					$lexem->data = substr($content, $lexemStartPos, $lexemEndPos - $lexemStartPos);
					$lexem->startPos = $lexemStartPos;
					$lexem->endPos = $lexemEndPos;
					$lexems[] = $lexem;
				}
			}

			if(count($lexems) === 0)
			{
				$lexem = new Lexem();
				$lexem->type = Lexem::TYPE_CONTENT;
				$lexem->data = trim(substr($content, $pos));
				$lexem->startPos = $pos;
				$lexem->endPos = $pos + strlen($lexem->data);
				if(strlen($lexem->data) > 0)
					$this->_lexems[] = $lexem;
				break;
			}

			//выбираем самое первое ключевое слово:
			$minIndex = 0;
			for($i = 1; $i < count($lexems); $i++)
				if($lexems[$i]->startPos < $lexems[$minIndex]->startPos)
					$minIndex = $i;

			$firstLexem = $lexems[$minIndex];
			if($firstLexem->startPos > $pos)	//перед ключевым словом был контент.
			{
				$lexem = new Lexem();
				$lexem->type = Lexem::TYPE_CONTENT;
				$lexem->data = trim(substr($content, $pos, $firstLexem->startPos - $pos));
				$lexem->startPos = $pos;
				$lexem->endPos = $pos + strlen($lexem->data);
				if(strlen($lexem->data) > 0)
					$this->_lexems[] = $lexem;
			}

			$this->_lexems[] = $firstLexem;
			$pos = $firstLexem->endPos + 1;
		}
		return TRUE;
	}

	private function _checkSyntax()
	{
		return TRUE;	//лень.
	}

	private function _compute()
	{
		//сначала ищем конец модуля:
		for($endModuleIndex = 0; $endModuleIndex < count($this->_lexems); $endModuleIndex++)
		{
			$endModuleLexem = $this->_lexems[$endModuleIndex];
			if($endModuleLexem->type == Lexem::TYPE_MODULE_END)
			{
				//потом ищем начало модуля:
				for($startModuleIndex = $endModuleIndex - 1; $startModuleIndex >= 0; $startModuleIndex--)
				{
					$startModuleTemplate = $this->_lexems[$startModuleIndex];
					if($startModuleTemplate->type == Lexem::TYPE_MODULE_START)
					{
						//определяем аттрибуты модуля:
						$moduleName = "";
						if(preg_match("/MODULE=([\.a-z]*)/i", $startModuleTemplate->data, $match))
						{
							$moduleName = $match[1];
						}

						$id = "";
						if(preg_match("/:id=([\.a-z]*)/i", $startModuleTemplate->data, $match))
							$id = $match[1];

                                                
                                                $class = $moduleName;
                                                $method = 'showContent';
                                                if(class_exists($class)) {
                                                    
                                                    $rc = new ReflectionClass($class);
                                                    if($rc->hasMethod($method)) {
                                                        $contr = $rc->newInstance();
                                                        $meth = $rc->getMethod($method);
                                                        
                                                        if($id === 'THIS') {
                                                            $result = json_decode($meth->invoke($contr, $this->id), true);
                                                           
                                                        }
                                                        else {
                                                            $result = json_decode($meth->invoke($contr), true);
                                                        }
                                                    }
                                                    else {
                                                    
                                                    }

                                                }
                                                else if($class === 'TEMPLATE') {
                                                    if(file_exists('application/templates/' . strtolower($id) . '.php')) {
                                                        $this->result = 'FILE::application/templates/' . strtolower($id) . '.php';
                                                    }
                                                }
                                                else {
                                                    
                                                }
                                                
						// по идее где-то здесь ты должен получить рефлексивный класс.

						//обрабатываем контен внутри модуля, запсывая результат в resultLexems:
						$resultLexems = array();
						$intoTemplate = FALSE;
						for($i = $startModuleIndex + 1; $i < $endModuleIndex; $i++)
						{
							$lexem = $this->_lexems[$i];
							if($lexem->type == Lexem::TYPE_TEMPLATE_START)
								$intoTemplate = TRUE;
							else if($lexem->type == Lexem::TYPE_TEMPLATE_END)
								$intoTemplate = FALSE;
							else if($lexem->type == Lexem::TYPE_CONTENT)
							{
								/*$newLexem = new Lexem();
								$newLexem->type = Lexem::TYPE_RESULT;*/
								
                                                                
                                                               
								if($intoTemplate)
								{
                                                                        for($k = 0; $k < count($result); $k++) {
                                                                            $newLexem = new Lexem();
                                                                            $newLexem->type = Lexem::TYPE_RESULT;
                                                                            $newLexem->data = $lexem->data;
                                                                            foreach ($result[$k] as $key => $value) {
                                                                                $newLexem->data = str_replace('{' . strtoupper($key) . '}', $value, $newLexem->data);
                                                                            }
                                                                            $resultLexems[] = $newLexem;
                                                                        }
                                                                        
									// вот здесь необходимо обработать контент внутри шаблона с помощью рефлексивного класcа:
									//$newLexem->data = str_replace("qwe", $id."_", $lexem->data);
								}
								else
								{
                                                                    
                                                                    $newLexem = new Lexem();
                                                                    $newLexem->type = Lexem::TYPE_RESULT;
                                                                    $newLexem->data = $lexem->data;
                                                                    $resultLexems[] = $newLexem;
                                                                        /*for($k = 0; $k < count($result); $k++) {
                                                                            $newLexem = new Lexem();
                                                                            $newLexem->type = Lexem::TYPE_RESULT;
                                                                            $newLexem->data = $lexem->data;
                                                                            foreach ($result[$k] as $key => $value) {
                                                                                $newLexem->data = str_replace('{' . strtoupper($key) . '}', $value, $newLexem->data);
                                                                            }
                                                                            $resultLexems[] = $newLexem;
                                                                        }*/
                                                                        
									// а вот здесь обработать контент внутри модуля с помощью того же класcа:
									//$newLexem->data = str_replace("123", $moduleName, $lexem->data);
                                                                        //var_dump($lexem->data);

								}
                                                                //var_dump($newLexem);
								//$resultLexems[] = $newLexem;
							}
							else if($lexem->type == Lexem::TYPE_RESULT)
								$resultLexems[] = $lexem;	//для сохранения очередности
							else
							{
								echo '<br>ERROR in _compute: WTF?! Syntax error.<br>';
								return FALSE;
							}
						}

						//убираем модуль, вставляя результат:
						array_splice($this->_lexems, $startModuleIndex, $endModuleIndex + 1 - $startModuleIndex, $resultLexems);
						// var_dump($this->_lexems);
						
						$endModuleIndex = 0;

						break;
					}
				}
			}
		}

		//запихиваем результат в result:
		for($i = 0; $i < count($this->_lexems); $i++)
		{
			$lexem = $this->_lexems[$i];
			if($lexem->type == Lexem::TYPE_RESULT
				|| $lexem->type == Lexem::TYPE_CONTENT)
				$this->result .= $lexem->data;
		}

		return TRUE;
	}
}