/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 01.09.2016
 * Время: 9:15
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Net;

namespace StangradCRMLibs
{
	/// <summary>
	/// Description of Updater.
	/// </summary>
	public class Updater
	{
		public string LastError;
		public bool IsError = false;
		private Uri updatePath = null;
		
		public Updater() {}
		
		public bool CheckUpdate (string currentVersion)
		{
			if(!Auth.getInstance().IsAuth)
			{
				LastError = "Для обновления необходимо авторизоваться!";
				IsError = true;
				return false;
			}
			
			HTTPManager.HTTPRequest http = HTTPManager.HTTPRequest.Create(Settings.uri);
			http.addParameter("entity", "Version/check");
			http.addParameter("current", currentVersion);
			if(!http.post())
			{
				LastError = HTTPManager.HTTPRequest.LastError;
				IsError = true;
				return false;				
			}
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			
			if(!parser.NoError)
			{
				LastError = parser.LastError;
				IsError = true;
				return false;
			}
			if(parser.ServerErrorFlag == 1)
			{
				LastError = parser.ToObject<string>();
				IsError = true;
				return false;
			}
			updatePath = parser.ToObject<Uri>();
			if(updatePath == null)
			{
				return false;
			}
			return true;
		}
		
		public bool DownloadUpdate ()
		{
			if(updatePath == null)
			{
				LastError = "Update path is null!";
				IsError = true;
				return false;
			}
			WebClient client = new WebClient();
			try
			{
				client.DownloadFile(updatePath, "StanGradCRM.exe");
				return true;
			}
			catch (Exception ex)
			{
				LastError = ex.Message;
				IsError = true;
				return false;
			}
		}
		
	}
}
