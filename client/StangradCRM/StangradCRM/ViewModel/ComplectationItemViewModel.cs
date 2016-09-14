/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 14.09.2016
 * Время: 10:42
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Linq;
using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of ComplectationItemViewModel.
	/// </summary>
	public class ComplectationItemViewModel : Core.BaseViewModel, Core.IViewModel
	{
		
		private static ComplectationItemViewModel _instance = null;
		
		private TSObservableCollection<ComplectationItem> _collection
			= new TSObservableCollection<ComplectationItem>();
		public TSObservableCollection<ComplectationItem> Collection
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
		
		private ComplectationItemViewModel()
		{
			TSObservableCollection<ComplectationItem> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<ComplectationItem>>("ComplectationItem");
			
			if(collection != default(TSObservableCollection<ComplectationItem>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
		public static ComplectationItemViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new ComplectationItemViewModel();
			}
			return _instance;
		}
		
		
		public bool @add<T>(T modelItem)
		{
			ComplectationItem complectationItem = modelItem as ComplectationItem;
			if(complectationItem != null && !_collection.Contains(complectationItem))
			{
				_collection.Add(complectationItem);
				return true;
			}
			complectationItem.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			ComplectationItem complectationItem = modelItem as ComplectationItem;
			if(complectationItem != null && _collection.Contains(complectationItem))
			{
				_collection.Remove(complectationItem);
				return true;
			}
			complectationItem.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;		
		}
		
		public StangradCRM.Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public ComplectationItem getById (int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
	}
}
