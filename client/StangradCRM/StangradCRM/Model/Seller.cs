/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:22
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;

using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Seller.
	/// </summary>
	public class Seller : Core.Model
	{
		public string Name { get; set; }
		
		public Seller() {}
		
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
				return "Seller";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return SellerViewModel.instance();
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				RaisePropertyChanged("Name", Name);
				BidViewModel.instance().Collection.
					Where(x => x.Id_seller == Id).All(y => { y.UpdateProperty("SellerName"); return true; });
			}
			return result;
		}
		
	}
}
