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
				TSObservableCollection<Complectation> complectations = value;
				ComplectationViewModel c_vm = ComplectationViewModel.instance();
				for(int i = 0; i < complectations.Count; i++)
				{
					Complectation complectation = c_vm.getById(complectations[i].Id);
					if(complectation == null)
					{
						c_vm.add(complectations[i]);
					}
					else
					{
						complectation.replace(complectations[i]);
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
				raiseAllProperties();
				Bid bid = BidViewModel.instance().getById(Id_bid);
				if(bid == null)
				{
					Log.WriteError("Bid is null");
				}
				else if (bid.EquipmentBidCollection.Contains(this))
				{
					Log.WriteError("Item exist");
				}
				else 
				{
					bid.EquipmentBidCollection.Add(this);
				}
			}
			return result;
		}
		
		protected override bool afterRemove(ResponseParser parser, bool soft = false)
		{
			bool result = base.afterRemove(parser, soft);
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
		
		public void setSerialNumber (int serial_number)
		{
			Serial_number = serial_number;
			RaisePropertyChanged("Serial_number", Serial_number);
		}
		
		public override void replace(object o)
		{
			EquipmentBid equipmentBid = o as EquipmentBid;
			if(equipmentBid == null) return;
			Id_modification = equipmentBid.Id_modification;
			Id_bid = equipmentBid.Id_bid;
			Serial_number = equipmentBid.Serial_number;
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Id_modification", Id_modification);
			RaisePropertyChanged("Id_bid", Id_bid);
			RaisePropertyChanged("Serial_number", Serial_number);
			RaisePropertyChanged("EquipmentName", null);
			RaisePropertyChanged("ModificationName", null);
		}
	}
}
