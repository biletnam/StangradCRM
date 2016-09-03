/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 19:19
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of EquipmentBidViewModel.
	/// </summary>
	public class EquipmentBidViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static EquipmentBidViewModel _instance = null;
		private TSObservableCollection<EquipmentBid> _collection
			= new TSObservableCollection<EquipmentBid>();
		public TSObservableCollection<EquipmentBid> Collection
		{
			get
			{
				return _collection;
			}
			set
			{
				_collection = value;
				RaisePropertyChanged("Collection", value);
			}
		}
		
		private EquipmentBidViewModel()
		{
			/*TSObservableCollection<EquipmentBid> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<EquipmentBid>>("EquipmentBid");
			
			if(collection != default(TSObservableCollection<EquipmentBid>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}*/
		}
		
		public static EquipmentBidViewModel instance ()
		{
			if(_instance == null)
			{
				_instance = new EquipmentBidViewModel();
			}
			return _instance;
		}
		
		
		public bool @add<T>(T modelItem)
		{
			EquipmentBid equipmentBid = modelItem as EquipmentBid;
			if(equipmentBid != null && !_collection.Contains(equipmentBid))
			{
				_collection.Add(equipmentBid);
				return true;
			}
			equipmentBid.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			EquipmentBid equipmentBid = modelItem as EquipmentBid;
			if(equipmentBid != null && _collection.Contains(equipmentBid))
			{
				_collection.Remove(equipmentBid);
				return true;
			}
			equipmentBid.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public EquipmentBid getById(int equipmentBidId)
		{
			return _collection.Where(x => x.Id == equipmentBidId).FirstOrDefault();
		}
		
		public TSObservableCollection<EquipmentBid> getByBidId(int bidId)
		{
			List<EquipmentBid> equipmentBid = _collection.Where(x => x.Id_bid == bidId).ToList();
			return new TSObservableCollection<EquipmentBid>(equipmentBid);
		}
	}
}
