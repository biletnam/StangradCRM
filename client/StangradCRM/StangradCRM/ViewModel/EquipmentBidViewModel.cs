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
		
		private Dictionary<int, TSObservableCollection<EquipmentBid>> _collectionByArchiveStatus
			= new Dictionary<int, TSObservableCollection<EquipmentBid>>();
		
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
		
		public TSObservableCollection<EquipmentBid> getCollectionByArchiveStatus (int archiveStatus)
		{
			if(!_collectionByArchiveStatus.ContainsKey(archiveStatus))
			{
				List<EquipmentBid> list = _collection.Where(x => 
                        (x.Is_archive == archiveStatus) && 
                        (x.Serial_number != null)
                       ).ToList();
				_collectionByArchiveStatus.Add(archiveStatus, new TSObservableCollection<EquipmentBid>(list));
			}
			return _collectionByArchiveStatus[archiveStatus];
		}
		
		public void updateStatus (EquipmentBid equipmentBid)
		{
			if(equipmentBid.Serial_number == null) return;
			int oldStatus = (equipmentBid.Is_archive == 0) ? 1 : 0;
			if(getCollectionByArchiveStatus(oldStatus).Contains(equipmentBid))
			{
				getCollectionByArchiveStatus(oldStatus).Remove(equipmentBid);
			}
			
			if(!getCollectionByArchiveStatus(equipmentBid.Is_archive).Contains(equipmentBid))
			{
				getCollectionByArchiveStatus(equipmentBid.Is_archive).Add(equipmentBid);
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
			if(equipmentBid == null)
			{
				equipmentBid.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			EquipmentBid exist = getById(equipmentBid.Id);
			if(exist != null || _collection.Contains(equipmentBid))
			{
				equipmentBid.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			if(equipmentBid.Serial_number != null)
				getCollectionByArchiveStatus(equipmentBid.Is_archive).Add(equipmentBid);
			_collection.Add(equipmentBid);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			EquipmentBid equipmentBid = modelItem as EquipmentBid;
			if(equipmentBid == null)
			{
				equipmentBid.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(equipmentBid)) return true;
			if(equipmentBid.Serial_number != null)
				getCollectionByArchiveStatus(equipmentBid.Is_archive).Remove(equipmentBid);
			return _collection.Remove(equipmentBid);
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
