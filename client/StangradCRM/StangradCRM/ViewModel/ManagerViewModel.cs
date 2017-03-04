/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 18:12
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.ObjectModel;
using StangradCRM.Core;
using StangradCRM.Model;
using System.Linq;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of ManagerViewModel.
	/// </summary>
	public class ManagerViewModel : Core.BaseViewModel, Core.IViewModel
	{
		
		private static ManagerViewModel _instance = null;
		
		private TSObservableCollection<Manager> _collection = 
			new TSObservableCollection<Manager>();
		
		public TSObservableCollection<Manager> Collection
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
		
		private ManagerViewModel() { load(); }
		
		public static ManagerViewModel instance ()
		{
			if(_instance == null)
			{
				_instance = new ManagerViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			Manager manager = modelItem as Manager;
			if(manager == null)
			{
				manager.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			Manager exist = getById(manager.Id);
			if(exist != null || _collection.Contains(manager))
			{
				manager.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(manager);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Manager manager = modelItem as Manager;
			if(manager == null)
			{
				manager.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(manager)) return true;
			return _collection.Remove(manager);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Manager getById (int managerId)
		{
			return _collection.Where(x => x.Id == managerId).FirstOrDefault();
		}
		
		protected override void removeAllItems()
		{
			_collection.ToList().ForEach(x => remove(x));
		}
		
		protected override void load()
		{
			TSObservableCollection<Manager> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Manager>>("Manager");
			
			if(collection != default(TSObservableCollection<Manager>))
			{
				collection.ToList().ForEach(x => { x.loadedItemInitProperty(); x.IsSaved = true; add(x); });
			}
		}
		
		public void search (string search_string)
		{
			_collection.ToList().ForEach(x => x.setFilter("Name", false));
			_collection.Where(x => x.Name.ToLower().IndexOf(search_string.ToLower()) != -1)
				.ToList().ForEach(y => y.setFilter("Name", true));
		}
	}
}
