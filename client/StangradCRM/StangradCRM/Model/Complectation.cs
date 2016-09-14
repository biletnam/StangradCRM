/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 19:15
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.Core;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Complectation.
	/// </summary>
	public class Complectation : Core.Model
	{
		public int Complectation_count { get; set; }
		public int Id_equipment_bid { get; set; }
		public int Id_complectation_item { get; set; }

		public string Name 
		{
			get
			{
				ComplectationItem item = ComplectationItemViewModel.instance().getById(Id_complectation_item);
				if(item == null) return "";
				return item.Name;
			}
		}
		
		public TSObservableCollection<ComplectationItem> ComplectationItemCollection
		{
			get
			{
				return ComplectationItemViewModel.instance().Collection;
			}
		}
		
		//Название нового элемента комплектации
		//Используется в форме добавления комплектация
		//В случае если элемент комплектации не выбран (Id_complectation_item == 0)
		//Будет создан новый элемент комплектации с данным названием
		public string NewComplectationItemName { get; set; }
		
		public Complectation() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("complectation_count", Complectation_count);
			http.addParameter("id_equipment_bid", Id_equipment_bid);
			http.addParameter("id_complectation_item", Id_complectation_item);
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
				raiseAllProperties();
			}
			return result;
		}
		
		protected override bool afterRemove(StangradCRMLibs.ResponseParser parser, bool soft = false)
		{
			bool result = base.afterRemove(parser, soft);
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
			RaisePropertyChanged("Id_complectation_item", Id_complectation_item, true);
		}
		
		public override void setOldValues ()
		{
			base.setOldValues();
			try
			{
				Complectation_count = (int)oldValues["Complectation_count"];
				Id_complectation_item = (int)oldValues["Id_complectation_item"];
			}
			catch {}
		}
		
		
		public override void replace(object o)
		{
			Complectation complectation = o as Complectation;
			if(complectation == null) return;
			
			Complectation_count = complectation.Complectation_count;
			Id_complectation_item = complectation.Id_complectation_item;
			Id_equipment_bid = complectation.Id_equipment_bid;
				
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Complectation_count", Complectation_count);
			RaisePropertyChanged("Id_complectation_item", Id_complectation_item);
			RaisePropertyChanged("Id_equipment_bid", Id_equipment_bid);
			RaisePropertyChanged("Name", null);
		}
	}
}
