/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 16:36
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
	/// Description of ModificationViewModel.
	/// </summary>
	public class ModificationViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static ModificationViewModel _instance = null;
		
		private TSObservableCollection<Modification> _collection =
			new TSObservableCollection<Modification>();
		
		public TSObservableCollection<Modification> Collection
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
		
		private ModificationViewModel() 
		{
			TSObservableCollection<Modification> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Modification>>("Modification");
			
			if(collection != default(TSObservableCollection<Modification>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
		public static ModificationViewModel instance ()
		{
			if(_instance == null)
			{
				_instance = new ModificationViewModel();
			}
			return _instance;
		}
		
		
		public bool @add<T>(T modelItem)
		{
			Modification modification = modelItem as Modification;
			if(modification == null)
			{
				modification.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			Modification exist = getById(modification.Id);
			if(exist != null || _collection.Contains(modification))
			{
				modification.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(modification);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Modification modification = modelItem as Modification;
			if(modification == null)
			{
				modification.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(modification)) return true;
			return _collection.Remove(modification);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Modification getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public TSObservableCollection<Modification> getByEquipmentId(int equipmentId)
		{
			List<Modification> modification = _collection.Where(x => x.Id_equipment == equipmentId).ToList();
			return new TSObservableCollection<Modification>(modification);
		}
	}
}
