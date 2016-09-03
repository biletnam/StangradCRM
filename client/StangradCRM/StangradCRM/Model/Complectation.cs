/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 19:15
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Complectation.
	/// </summary>
	public class Complectation : Core.Model
	{
		public int Complectation_count { get; set; }
		public string Commentary { get; set; }
		public int Id_equipment_bid { get; set; }

		public Complectation() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("complectation_count", Complectation_count);
			http.addParameter("id_equipment_bid", Id_equipment_bid);
			http.addParameter("commentary", Commentary);
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
				return "Complectation";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return ComplectationViewModel.instance();
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				RaisePropertyChanged("Complectation_count", Complectation_count);
				RaisePropertyChanged("Commentary", Commentary);
				RaisePropertyChanged("Id_equipment_bid", Id_equipment_bid);
			}
			return result;
		}
		
		protected override bool afterRemove(StangradCRMLibs.ResponseParser parser, bool soft = false)
		{
			bool result = base.afterRemove(parser);
			if(result)
			{
				EquipmentBid equipmentBid = EquipmentBidViewModel.instance().getById(Id_equipment_bid);
				if(equipmentBid != null && equipmentBid.ComplectationCollection.Contains(this))
				{
					equipmentBid.ComplectationCollection.Remove(this);
				}
			}
			return result;
		}
		
		public override void loadedItemInitProperty()
		{
			base.loadedItemInitProperty();
			RaisePropertyChanged("Complectation_count", Complectation_count, true);
			RaisePropertyChanged("Commentary", Commentary, true);
		}
		
		public void setOldValues ()
		{
			try
			{
				Complectation_count = (int)oldValues["Complectation_count"];
				Commentary = oldValues["Commentary"].ToString();
			}
			catch {}
		}
		
	}
}
