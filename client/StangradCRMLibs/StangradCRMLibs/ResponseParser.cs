/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08.08.2016
 * Время: 17:40
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using Newtonsoft.Json.Linq;

namespace StangradCRMLibs
{
	/// <summary>
	/// Description of ResponseParser.
	/// </summary>
	public class ResponseParser
	{
		private JArray serverResponse = null;
		
		public string LastError {get; private set; }
		public int ServerErrorFlag {get; private set; }
		public bool NoError {get; private set; }
		
		private ResponseParser (string jsonString)
		{
			try {
				serverResponse = JArray.Parse(jsonString);
				try
				{
					ServerErrorFlag = (int)serverResponse[0];
					NoError = true;
				}
				catch (Exception ex) 
				{
					LastError = ex.Message;
					NoError = false;
				}
			}
			catch (Exception ex) {
				LastError = jsonString + "(" + ex.Message + ")";
				NoError = false;
			}
		}
		
		public static ResponseParser Parse(string jsonString)
		{
			ResponseParser parser = new ResponseParser(jsonString);
			return parser;
		}
		
		public T ToObject <T> () {
			try {
				if(serverResponse == null || serverResponse[1] == null) {
					LastError = "Server response data is null!";
					return default(T);
				}
				return serverResponse[1].ToObject<T>();
			}
			catch (Exception ex) {
				LastError = ex.Message + " (during conversion)";
				return default(T);
			}
		}
	}
}
