/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:58
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Role.
	/// </summary>
	public class Role : Core.Model
	{
		private string name = "";
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				RaisePropertyChanged("Name", value);
			}
		}
		
		public Role() {	}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			throw new NotImplementedException();
		}
		
		protected override void prepareRemoveData(HTTPManager.HTTPRequest http)
		{
			throw new NotImplementedException();
		}
		
		protected override string Entity {
			get {
				return "Role";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return RoleViewModel.instance();
			}
		}
		
		public override void replace(object o)
		{
			throw new NotImplementedException();
		}
		
		public override void raiseAllProperties()
		{
			throw new NotImplementedException();
		}
	}
}
