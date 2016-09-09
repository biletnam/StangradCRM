/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 22.08.2016
 * Время: 17:23
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of TransportCompany.
	/// </summary>
	public class TransportCompany : Core.Model
	{
		public string Name { get; set; }
		
		public TransportCompany() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
			if(Id != 0)
			{
				http.addParameter("id", Id);
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
				return "TransportCompany";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return TransportCompanyViewModel.instance();
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				raiseAllProperties();
			}
			return result;
		}
		
		public override void replace(object o)
		{
			throw new NotImplementedException();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
		}
	}
}
