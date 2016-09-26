/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 23.09.2016
 * Время: 14:50
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of Months.
	/// </summary>
	public static class Months
	{
		private static string[] months = new string[] 
		{
			"Январь", "Февраль", "Март", "Апрель", "Май", 
			"Июнь", "Июль", "Август", "Сетнябрь", "Октябрь", "Ноябрь", "Декабрь"
		};
		public static string getRuMonthNameByNumber (int monthNumber)
		{
			if(monthNumber > 12 || monthNumber < 1)
			{
				return "";
			}
			return months[monthNumber-1];
		}
	}
}
