/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 15:48
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;
using System.Linq;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Buyer.
	/// </summary>
	public class Buyer : StangradCRM.Core.Model
	{		
		public string Name { get; set; }
		public string Contact_person { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string City { get; set; }
		
		public string SecondColumnValue //Кастыль 
		{
			get
			{
				if(City == "") return "";
				return "(" + City + ")";
			}
		}
		
		public Buyer() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
			http.addParameter("contact_person", Contact_person);
			http.addParameter("phone", Phone);
			http.addParameter("email", Email);
			http.addParameter("city", City);
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
				return "Buyer";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return BuyerViewModel.instance();
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				RaisePropertyChanged("Name", Name);
				RaisePropertyChanged("Contact_person", Contact_person);
				RaisePropertyChanged("Phone", Phone);
				RaisePropertyChanged("Email", Email);
				RaisePropertyChanged("City", City);
				
				BidViewModel.instance().Collection.
					Where(x => x.Id_buyer == Id).All(y => { y.UpdateProperty("BuyerName"); return true; });
			}
			return result;
		}
		
	}
}
