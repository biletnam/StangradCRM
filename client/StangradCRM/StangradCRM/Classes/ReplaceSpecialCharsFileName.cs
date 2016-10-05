/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.09.2016
 * Время: 13:55
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of ReplaceSpecialCharsFileName.
	/// </summary>
	public static class ReplaceSpecialCharsFileName
	{
		public static string Replace (string filename)
		{
			filename = filename.Replace('/', '-');
			filename = filename.Replace('\\', '-');
			filename = filename.Replace('|' , '-');
			filename = filename.Replace('<' , '-');
			filename = filename.Replace('>' , '-');
			filename = filename.Replace('?' , '-');
			filename = filename.Replace('[' , '-');
			filename = filename.Replace(']' , '-');
			filename = filename.Replace(':' , '-');
			filename = filename.Replace('"' , ' ');
			return filename.Replace('*' , '-');
		}
	}
}
