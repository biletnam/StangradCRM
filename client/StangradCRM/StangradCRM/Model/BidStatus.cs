/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:29
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;
using System.Linq;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of BidStatus.
	/// </summary>
	public class BidStatus : Core.Model
	{
		
		public string Name { get; set; }
		public string Record_color { get; set; }
		
		public Classes.BidStatus CurrentStatus
		{
			get
			{
				return (Classes.BidStatus)Id;
			}
		}
		
		public BidStatus() {}
		
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
				return "BidStatus";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return BidStatusViewModel.instance();
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				RaisePropertyChanged("Name", Name);
				RaisePropertyChanged("Record_color", Record_color);
				BidViewModel.instance().Collection.
					Where(x => x.Id_bid_status == Id).All(y => { 
					                                  	y.UpdateProperty("BidStatusColor");
					                                  	y.UpdateProperty("BidStatusName");
					                                  	return true; });
			}
			return result;
		}
		
		
		
	}
}
