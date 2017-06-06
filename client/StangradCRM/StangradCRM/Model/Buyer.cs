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
		public string Passport_serial_number { get; set; }
		public string Passport_issue_date { get; set; }
		public string Inn { get; set; }
		public DateTime? Date_created { get; set; }

		public string SecondColumnValue //Кастыль 
		{
			get
			{
				if(City == "") return "";
				return "(" + City + ")";
			}
		}
		
		public string NameWithCity
		{
			get
			{
				if(City == "") return Name;
				return Name + " (" + City + ")";
			}
		}
		
		public string BuyerInfo
		{
			get
			{
				return Name + " " + Contact_person + " " + Phone
					+ " " + Email + " " + City + " " + Passport_serial_number
					+ " " + Passport_issue_date + " " + Inn;
			}
		}
		
		public string PassportData {
			get {
				return Passport_serial_number + " " + Passport_issue_date;
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
			http.addParameter("passport_serial_number", Passport_serial_number);
			http.addParameter("passport_issue_date", Passport_issue_date);
			http.addParameter("inn", Inn);
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
				raiseAllProperties();
				BidViewModel.instance().Collection.
					Where(x => x.Id_buyer == Id).All(y => { y.UpdateProperty("BuyerName"); return true; });
			}
			return result;
		}
		
		
		public override void replace(object o)
		{
			Buyer buyer = o as Buyer;
			if(buyer == null) return;
			
			Name = buyer.Name;
			Contact_person = buyer.Contact_person;
			Phone = buyer.Phone;
			Email = buyer.Email;
			City = buyer.City;
			Passport_serial_number = buyer.Passport_serial_number;
			Passport_issue_date = buyer.Passport_issue_date;
			Inn = buyer.Inn;
			
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
			RaisePropertyChanged("Contact_person", Contact_person);
			RaisePropertyChanged("Phone", Phone);
			RaisePropertyChanged("Email", Email);
			RaisePropertyChanged("City", City);
			RaisePropertyChanged("Passport_serial_number", Passport_serial_number);
			RaisePropertyChanged("Passport_issue_date", Passport_issue_date);
			RaisePropertyChanged("Inn", Inn);
			
			RaisePropertyChanged("PassportData", null);
		}
	}
}
