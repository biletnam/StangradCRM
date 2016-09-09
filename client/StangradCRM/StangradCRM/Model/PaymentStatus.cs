/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:37
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;
using System.Linq;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of PaymentStatus.
	/// </summary>
	public class PaymentStatus : Core.Model
	{
		public string Name { get; set; }
		public string Record_color { get; set; }
		
		public PaymentStatus() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
			http.addParameter("record_color", Record_color);
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
				return "PaymentStatus";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return PaymentStatusViewModel.instance();
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				raiseAllProperties();
				BidViewModel.instance().Collection.
					Where(x => x.Id_payment_status == Id).All(y => { 
					                                  	y.UpdateProperty("PaymentStatusColor");
					                                  	y.UpdateProperty("PaymentStatusName");
					                                  	return true; });
			}
			return result;
		}
		
		
		public override void replace(object o)
		{
			PaymentStatus paymentStatus = o as PaymentStatus;
			if(paymentStatus == null) return;
			Name = paymentStatus.Name;
			Record_color = paymentStatus.Record_color;
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
			RaisePropertyChanged("Record_color", Record_color);
		}
	}
}
