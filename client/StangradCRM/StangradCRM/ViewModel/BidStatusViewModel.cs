/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:31
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
	/// Description of BidStatusViewModel.
	/// </summary>
	public class BidStatusViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static BidStatusViewModel _instance = null;
		
		private TSObservableCollection<BidStatus> _collection
			= new TSObservableCollection<BidStatus>();
		public TSObservableCollection<BidStatus> Collection
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
		
		private BidStatusViewModel()
		{
			TSObservableCollection<BidStatus> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<BidStatus>>("BidStatus");
			
			if(collection != default(TSObservableCollection<BidStatus>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
		public static BidStatusViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new BidStatusViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			BidStatus bidStatus = modelItem as BidStatus;
			if(bidStatus != null && !_collection.Contains(bidStatus))
			{
				_collection.Add(bidStatus);
				return true;
			}
			bidStatus.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			BidStatus bidStatus = modelItem as BidStatus;
			if(bidStatus != null && _collection.Contains(bidStatus))
			{
				_collection.Remove(bidStatus);
				return true;
			}
			bidStatus.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;
		}
		
		public BidStatus getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
	}
}
