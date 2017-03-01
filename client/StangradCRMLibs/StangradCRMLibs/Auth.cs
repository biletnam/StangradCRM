/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08.08.2016
 * Время: 17:42
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using HTTPManager;

namespace StangradCRMLibs
{
	/// <summary>
	/// Description of Auth.
	/// </summary>
	public class Auth
	{
		private Auth() {}
		
		public static Auth _instance = null;
		private bool isAuth = false;
		public bool IsAuth {
			get {return this.isAuth;}
		}
		private int id = 0;
		private string full_name = null;
		private string login = null;
		private int id_role = 0;

		private string inputLogin = null;
		private string inputPassword = null;
		private string inputEntity = null;
		
		public int IdRole {
			get {return this.id_role;}
		}
		public int Id {
			get {return this.id;}
		}
		public string Full_name {
			get {return this.full_name;}
		}
		public string Login {
			get {return this.inputLogin;}
			set {this.inputLogin = value;}
		}
		
		public string Password {
			get {return this.inputPassword;}
			set {this.inputPassword = value;}
		}
		
		public string Entity {
			get {return this.inputEntity;}
			set {this.inputEntity = value;}
		}
		
		public string LastError {
			get;
			private set;
		}
		
		public static Auth getInstance () {
			if(Auth._instance == null) {
				Auth._instance = new Auth();
			}
			return Auth._instance;
		}
		
		public bool loadCurrentAuthUser () {
			if(Settings.uri == null)
			{
				LastError = "URI сервера не был получен или является некорректным.";
				return false;				
			}
			HTTPRequest http = HTTPRequest.Create(Settings.uri);
			http.UseCookie = true;
			http.addParameter("entity", this.inputEntity);
			if(!http.post()) 
			{
				LastError = "Запрос не может быть выполнен: " + HTTPRequest.LastError;
				return false;
			}
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			if(!parser.NoError)
			{
				LastError = parser.LastError;
				return false;
			}
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<String>();
				return false;
			}
			Dictionary<string, string> userData = 
				parser.ToObject<Dictionary<string, string>>();
			
			id = Int32.Parse(userData["id"]);
			full_name = userData["name"];
			login = userData["login"];
			id_role = Int32.Parse(userData["role"]);
			
			isAuth = true;
			return true;
		}
		
		public bool authorization () {
			if(Settings.uri == null) {
				LastError = "Пустой URL сервера данных.";
				return false;
			}
			if(inputEntity == null) {
				LastError = "Необходимый параметр (метод-контроллер) не был получен.";
				return false;
			}
			if(inputLogin == null) {
				LastError = "Необходимый параметр (логин) не был получен.";
				return false;
			}
			if(inputPassword == null) {
				LastError = "Необходимый параметр (пароль) не был получен.";
				return false;
			}
			if(Settings.uri == null)
			{
				LastError = "URI сервера не был получен или является некорректным.";
				return false;	
			}
			HTTPRequest http = HTTPRequest.Create(Settings.uri);
			http.UseCookie = true;
			http.addParameter("entity", inputEntity);
			http.addParameter("login", inputLogin);
			http.addParameter("password", inputPassword);
			if(!http.post()) 
			{
				LastError = "Запрос не может быть выполнен: " + HTTPRequest.LastError;
				return false;
			}
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			
			if(!parser.NoError)
			{
				LastError = parser.LastError;
				return false;
			}
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<String>();
				return false;
			}
			Dictionary<string, string> userData =
				parser.ToObject<Dictionary<string, string>>();
			
			id = Int32.Parse(userData["id"]);
			full_name = userData["name"];
			login = userData["login"];
			id_role = Int32.Parse(userData["role"]);
			isAuth = true;
			
			return true;
		}
	}
}
