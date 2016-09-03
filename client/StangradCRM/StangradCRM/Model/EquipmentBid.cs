/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 19:09
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of EquipmentBid.
	/// </summary>
	public class EquipmentBid : Core.Model
	{
		public int Id_modification { get; set; }
		public int Id_bid { get; set; }
		public int? Serial_number { get; set; }
		
		//-------------------->
		
		public TSObservableCollection<Complectation> Complectation
		{
			set
			{
				TSObservableCollection<Complectation> complectation = value;
				ComplectationViewModel c_wm = ComplectationViewModel.instance();
				for(int i = 0; i < complectation.Count; i++)
				{
					if(c_wm.getById(complectation[i].Id) == null)
					{
						c_wm.add(complectation[i]);
					}
				}
			}
		}
		
		//--------------------<
		
		
		
		public EquipmentBid() {	}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("id_modification", Id_modification);
			http.addParameter("id_bid", Id_bid);
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
			if(Serial_number != null)
			{
				http.addParameter("serial_number", Serial_number);
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
				return "EquipmentBid";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return EquipmentBidViewModel.instance();
			}
		}
		
		
		//EXT
		
		public Modification Modification
		{
			get
			{
				return ModificationViewModel.instance().getById(Id_modification);

			}
		}
		
		public Equipment Equipment
		{
			get
			{
				if(this.Modification == null) return null;
				return EquipmentViewModel.instance().getById(this.Modification.Id_equipment);
			}
		}
		
		public string EquipmentName 
		{
			get
			{
				if(this.Equipment == null) return "<Не выбрано>";
				return this.Equipment.Name;
			}
		}
		
		public string ModificationName
		{
			get
			{
				if(this.Modification == null) return "<Не выбрано>";
				return this.Modification.Name;				
			}
		}
		
		private TSObservableCollection<Complectation> complectationCollection = null;
		public TSObservableCollection<Complectation> ComplectationCollection
		{
			get
			{
				if(complectationCollection == null)
				{
					complectationCollection = ComplectationViewModel.instance().getByEquipmentBidId(Id);
				}
				return complectationCollection;
			}
		}
		
		
		public string ComplectationStringSearch
		{
			get
			{
				string resultString = "";
				foreach(Complectation complectation in ComplectationCollection)
				{
					resultString += complectation.Commentary + " " 
						+ complectation.Complectation_count.ToString();
				}
				return resultString;
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				RaisePropertyChanged("Id_modification", Id_modification);
				RaisePropertyChanged("Id_bid", Id_bid);
				RaisePropertyChanged("Serial_number", Serial_number);
				RaisePropertyChanged("EquipmentName", null);
				RaisePropertyChanged("ModificationName", null);
				Bid bid = BidViewModel.instance().getById(Id_bid);
				if(bid != null && !bid.EquipmentBidCollection.Contains(this))
				{
					bid.EquipmentBidCollection.Add(this);
				}
			}
			return result;
		}
		
		protected override bool afterRemove(ResponseParser parser, bool soft = false)
		{
			bool result = base.afterRemove(parser);
			if(result)
			{
				Bid bid = BidViewModel.instance().getById(Id_bid);
				if(bid != null && bid.EquipmentBidCollection.Contains(this))
				{
					bid.EquipmentBidCollection.Remove(this);
				}
				ComplectationCollection.ToList().ForEach(x => {x.remove(true);});
			}
			return result;
		}
		
		public bool generateSerialNumber ()
		{
			if(Serial_number != null) return true;
			
			if(this.Equipment == null) 
			{
				LastError = "Оборудование не найдено!";
				return false;
			}
			
			List<EquipmentBid> equipmentBid = EquipmentBidViewModel.instance().Collection.Where(x => x.Equipment == this.Equipment).ToList();
			if(equipmentBid.Count == 0)
			{
				Serial_number = Equipment.Serial_number+1;
				return save();
			}
			Serial_number = (equipmentBid.Max(x => x.Serial_number) + 1);
			return save();
		}
	}
}
