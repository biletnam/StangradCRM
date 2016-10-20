/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 18:00
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.ObjectModel;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of RoleViewModel.
	/// </summary>
	public class RoleViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static RoleViewModel _instance = null;
		private TSObservableCollection<Role> _collection 
			= new TSObservableCollection<Role>();
		public TSObservableCollection<Role> Collection
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
		
		private RoleViewModel() { load(); }
		
		public static RoleViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new RoleViewModel();
			}
			return _instance;
		}		
		
		public bool @add<T>(T modelItem)
		{
			Role role = modelItem as Role;
			if(role == null)
			{
				role.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			Role exist = getById(role.Id);
			if(exist != null || _collection.Contains(role))
			{
				role.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(role);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Role role = modelItem as Role;
			if(role == null)
			{
				role.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(role)) return true;
			return _collection.Remove(role);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Role getById (int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		protected override void removeAllItems()
		{
			_collection.ToList().ForEach(x => remove(x));
		}
		
		protected override void load()
		{
			TSObservableCollection<Role> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Role>>("Role");
			
			if(collection != default(TSObservableCollection<Role>))
			{
				collection.ToList().ForEach(x => add(x));
			}
		}
	}
}
