/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 29.08.2016
 * Время: 11:38
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Payment.
	/// </summary>
	public class Payment : Core.Model
	{
		
		public double Paying { get; set; }
		public int Id_bid { get; set; }
		public int Id_manager { get; set; }
		public DateTime Payment_date { get; set; }
		
		public string ManagerName
		{
			get
			{
				Manager manager = ManagerViewModel.instance().getById(Id_manager);
				if(manager != null)
				{
					return manager.Name;
				}
				return "";
			}
		}
		
		public Payment() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("paying", Paying);
			http.addParameter("id_bid", Id_bid);
			http.addParameter("id_manager", Id_manager);
			http.addParameter("payment_date", Payment_date.ToString("yyyy-MM-dd"));
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
				return "Payment";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return PaymentViewModel.instance();
			}
		}
	}
}
