/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 13:27
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of IViewModel.
	/// </summary>
	public interface IViewModel
	{
		bool add<T>(T modelItem);
		bool remove<T>(T modelItem);
		
		Model getItem(int id);
	}
}
