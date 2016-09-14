/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 14.09.2016
 * Время: 10:30
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of ComplectationItem.
	/// </summary>
	public class ComplectationItem : Core.Model
	{
		public string Name { get; set; }
		
		public ComplectationItem() {}
		
		public override void replace(object o)
		{
			ComplectationItem item = o as ComplectationItem;
			if(o == null) return;
			
			Name = item.Name;
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
		}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
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
				return "ComplectationItem";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return ComplectationItemViewModel.instance();
			}
		}
	}
}
