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
		
		private RoleViewModel() 
		{
			TSObservableCollection<Role> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Role>>("Role");
			
			if(collection != default(TSObservableCollection<Role>))
			{
				_collection = collection;
			}
		}
		
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
			if(role != null && !_collection.Contains(role))
			{
				_collection.Add(role);
				return true;
			}
			role.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Role role = modelItem as Role;
			if(role != null && _collection.Contains(role))
			{
				_collection.Remove(role);
				return true;
			}
			role.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Role getById (int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
	}
}
