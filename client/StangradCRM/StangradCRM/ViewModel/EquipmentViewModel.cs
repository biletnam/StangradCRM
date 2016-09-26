/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 13:23
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Linq;
using System.Windows.Data;

using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of Equipment.
	/// </summary>
	public class EquipmentViewModel : BaseViewModel, IViewModel
	{
		private static EquipmentViewModel _instance = null;
		
		private TSObservableCollection<Equipment> _collection = 
			new TSObservableCollection<Equipment>();
		
		public TSObservableCollection<Equipment> Collection
		{
			get
			{
				return _collection;
			}
			private set 
			{
				_collection = value;
				RaisePropertyChanged("Collection", value);
			}
		}

		private EquipmentViewModel() 
		{
			TSObservableCollection<Equipment> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Equipment>>("Equipment");
			
			if(collection != default(TSObservableCollection<Equipment>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
			
		}
		
		public static EquipmentViewModel instance ()
		{
			if(_instance == null)
			{
				_instance = new EquipmentViewModel();
			}
			_instance.clearNameFilter();
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			Equipment equipment = modelItem as Equipment;
			if(equipment == null)
			{
				equipment.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			Equipment exist = getById(equipment.Id);
			if(exist != null || _collection.Contains(equipment))
			{
				equipment.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(equipment);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Equipment equipment = modelItem as Equipment;
			if(equipment == null)
			{
				equipment.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(equipment)) return true;
			return _collection.Remove(equipment);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Equipment getById(int equipmentId)
		{
			return _collection.Where(x => x.Id == equipmentId).FirstOrDefault();
		}
		
		public void searchByName(string name)
		{
			_collection.ToList().ForEach(x => x.setFilter("Name", false));
			_collection.Where(x => x.Name.ToLower().IndexOf(name.ToLower()) != -1)
				.ToList().ForEach(y => y.setFilter("Name", true));
		}
		
		public void clearNameFilter ()
		{
			_collection.ToList().ForEach(x => x.setFilter("Name", true));
		}
	}
}
