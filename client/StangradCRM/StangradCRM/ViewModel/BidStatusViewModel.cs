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
		
		private BidStatusViewModel() { load(); }
		
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
			if(bidStatus == null)
			{
				bidStatus.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			BidStatus exist = getById(bidStatus.Id);
			if(exist != null || _collection.Contains(bidStatus))
			{
				bidStatus.LastError = "Либо данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(bidStatus);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			BidStatus bidStatus = modelItem as BidStatus;
			if(bidStatus == null) 
			{
				bidStatus.LastError = "Не удалось преобразовать входные данные.";
				return false;				
			}
			if(!_collection.Contains(bidStatus)) return true;
			return _collection.Remove(bidStatus);
		}
		
		public BidStatus getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		protected override void removeAllItems()
		{
			_collection.ToList().ForEach(x => remove(x));
		}
		
		protected override void load()
		{
			TSObservableCollection<BidStatus> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<BidStatus>>("BidStatus");
			
			if(collection != default(TSObservableCollection<BidStatus>))
			{
				collection.ToList().ForEach(x => { x.IsSaved = true; add(x); });
			}
		}
	}
}
