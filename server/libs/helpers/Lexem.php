<?php

class Lexem
{
	public $type;
	public $data;

	const TYPE_CONTENT = 0;
	const TYPE_MODULE_START = 1;
	const TYPE_MODULE_END = 2;
	const TYPE_TEMPLATE_START = 3;
	const TYPE_TEMPLATE_END = 4;
	const TYPE_RESULT = 10;
}