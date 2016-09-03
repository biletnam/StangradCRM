/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08.08.2016
 * Время: 17:39
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.IO;
using System.Text;

namespace StangradCRMLibs
{
	/// <summary>
	/// Description of Log.
	/// </summary>
	public class Log
	{
		private Log() {}
		
		public static void Write (string message) {
			message = DateTime.Now.ToString() + ": " + message + "\n\r";
			File.AppendAllText("log.txt", message, Encoding.UTF8);
		}
		
		public static void WriteError (string message) {
			message = DateTime.Now.ToString() + ": " + message + "\n\r";
			File.AppendAllText("error.txt", message, Encoding.UTF8);
		}
	}
}
