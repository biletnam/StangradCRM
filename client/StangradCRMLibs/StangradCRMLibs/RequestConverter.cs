/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08.08.2016
 * Время: 17:40
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using Newtonsoft.Json;

namespace StangradCRMLibs
{
	/// <summary>
	/// Description of RequestConverter.
	/// </summary>
	public class RequestConverter
	{
		private RequestConverter() {}
		
		public static string Convert <T> (T t) {
			return JsonConvert.SerializeObject(t, Formatting.Indented);
		}
	}
}
