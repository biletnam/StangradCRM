/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:57
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Manager.
	/// </summary>
	public class Manager : Core.Model
	{
		public string Name { get; set; }
		public string Login { get; set; }
		public int Id_role { get; set; }
		public string Password { get; set; }
		
		public string RoleName
		{
			get
			{
				Role role = RoleViewModel.instance().getById(Id_role);
				if(role != null)
				{
					return role.Name;
				}
				return "<Не назначено>";
			}
		}
		
		public Classes.Role CurrentRole 
		{
			get
			{
				return (Classes.Role)Id_role;
			}
		}
		
		public Manager() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
			http.addParameter("login", Login);
			http.addParameter("id_role", Id_role);
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
			if(Password != "")
			{
				http.addParameter("passwd", Password);
			}
		}
		
		protected override void prepareRemoveData(HTTPManager.HTTPRequest http)
		{
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
		}
		
		protected override string Entity {
			get {
				return "Manager";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return ManagerViewModel.instance();
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			Password = "";
			bool result = base.afterSave(parser);
			if(result)
			{
				raiseAllProperties();
			}
			else
			{
				Name = oldValues["Name"].ToString();
				Login = oldValues["Login"].ToString();
				Id_role = (int)oldValues["Id_role"];
			}
			return result;
		}
		
		public override void loadedItemInitProperty ()
		{
			RaisePropertyChanged("Name", Name, true);
			RaisePropertyChanged("Login", Login, true);
			RaisePropertyChanged("Id_role", Id_role, true);
		}
		
		
		public override void replace(object o)
		{
			Manager manager = o as Manager;
			if(manager == null) return;
			Name = manager.Name;
			Login = manager.Login;
			Id_role = manager.Id_role;
			
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
			RaisePropertyChanged("Login", Login);
			RaisePropertyChanged("Id_role", Id_role);
		}
	}
}
