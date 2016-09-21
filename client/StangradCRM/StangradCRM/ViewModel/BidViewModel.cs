/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 19:04
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRMLibs;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of BidViewModel.
	/// </summary>
	public class BidViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static BidViewModel _instance = null;
		private TSObservableCollection<Bid> _collection
			= new TSObservableCollection<Bid>();
		
		
		private DateTime loadDateTime = DateTime.Now;
		
		Dictionary<int, TSObservableCollection<Bid>> _collectionByStatus
			= new Dictionary<int, TSObservableCollection<Bid>>();
		
		TSObservableCollection<Bid> _archiveCollection;
		
		public TSObservableCollection<Bid> getCollectionByStatus(int bidStatusId)
		{
			if(!_collectionByStatus.ContainsKey(bidStatusId))
			{
				List<Bid> collection = _collection.Where(x => (x.Id_bid_status == bidStatusId) && (x.Is_archive == 0)).ToList();
				_collectionByStatus.Add(bidStatusId, new TSObservableCollection<Bid>(collection));
			}
			return _collectionByStatus[bidStatusId];
		}
		
		public void updateStatus (Bid bid, int oldStatus)
		{
			if(bid.Id_bid_status == oldStatus) return;
			if(getCollectionByStatus(oldStatus).Contains(bid))
			{
				getCollectionByStatus(oldStatus).Remove(bid);
			}
			
			if(!getCollectionByStatus(bid.Id_bid_status).Contains(bid))
			{
				getCollectionByStatus(bid.Id_bid_status).Add(bid);
			}
		}
		
		public TSObservableCollection<Bid> getArchiveCollection ()
		{
			if(_archiveCollection == null)
			{
				List<Bid> collection = _collection.Where(x => x.Is_archive != 0).ToList();
				_archiveCollection = new TSObservableCollection<Bid>(collection);
			}
			return _archiveCollection;
		}
		
		public void MoveToArchive (Bid bid)
		{
			if(getCollectionByStatus(bid.Id_bid_status).Contains(bid))
			{
				getCollectionByStatus(bid.Id_bid_status).Remove(bid);
				getArchiveCollection().Add(bid);
			}
		}
		
		public TSObservableCollection<Bid> Collection
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
		
		private BidViewModel()
		{
			TSObservableCollection<Bid> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Bid>>("Bid");

			if(collection != default(TSObservableCollection<Bid>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
				_collection.ToList().ForEach(x => { x.loadedItemInitProperty(); });
			}
		}
		
		public static BidViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new BidViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			Bid bid = modelItem as Bid;
			if(bid == null)
			{
				bid.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			Bid exist = getById(bid.Id);
			if(exist != null || _collection.Contains(bid))
			{
				bid.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(bid);
			getCollectionByStatus(bid.Id_bid_status).Add(bid);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Bid bid = modelItem as Bid;
			if(bid == null) 
			{
				bid.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(bid)) return true;
			getCollectionByStatus(bid.Id_bid_status).Remove(bid);
			return _collection.Remove(bid);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Bid getById (int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
	
		public DateTime maxLastModified ()
		{
			if(_collection.Count > 0)
			{
				return _collection.Max(x => x.Last_modified);
			}
			return loadDateTime;
		}
		
		//Ф-я загрузки модифицированных данных с сервера
		private List<Bid> loadLastModified ()
		{
			HTTPManager.HTTPRequest http = HTTPManager.HTTPRequest.Create(Settings.uri);
			http.UseCookie = true;
			
			string lastModified = maxLastModified().ToString("yyyy-MM-dd HH:mm:ss");
			
			http.addParameter("entity", "Bid/lastmodified");
			http.addParameter("last_modified", lastModified);
			
			if(!http.post()) return null;
			
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			if(!parser.NoError)
			{
				return null;
			}
			return parser.ToObject<List<Bid>>();
		}
		
		//Ф-я удаленной загрузки
		public void RemoteLoad()
		{
			//Список заявок
			List<Bid> bids = null;
			try
			{
				bids = loadLastModified();
			}
			catch
			{
				return;
			}
			if(bids == null) return;
			//Аутентификационные данные 
			Auth auth = Auth.getInstance();
			
			//Цикл по новым заявкам			
			for(int i = 0; i < bids.Count; i++)
			{
				//Новая заявка
				Bid newBid = bids[i];
				//Старая заявка
				Bid oldBid = getById(newBid.Id);
				//Если старая заявка = null
				if(oldBid == null)
				{
					//Если текущий пользователь менеджер
					if(auth.IdRole == (int)Classes.Role.Manager)
					{
						//если код менеджера заявки == коду текущего авторизованного менеджер
						if(newBid.Id_manager == auth.Id)
						{
							//добавляем новую заявку в коллекцию
							add(newBid);
						}
						else
						{
							//иначе удаляем заявку из коллекции
							newBid.remove(true);
						}
					}
					else
					{
						//добавляем новую заявку в коллекцию
						add(newBid);
					}
				}
				else //если старая заявка не null
				{
					//Если текущий пользователь менеджер
					if(auth.IdRole == (int)Classes.Role.Manager)
					{
						//если код менеджера заявки == коду текущего авторизованного менеджер
						if(newBid.Id_manager == auth.Id)
						{
							//Заменяем данные старой завки на данные новой заявки
							oldBid.replace(newBid);
						}
						else
						{
							//иначе удаляем заявку
							newBid.remove(true);
						}
					}
					else
					{
						//Заменяем данные старой завки на данные новой заявки
						oldBid.replace(newBid);
					}
				}
			}
		}
		
		
		public void fastSearch (string searchString, TSObservableCollection<Bid> collection = null)
		{
			searchString = searchString.ToLower();
			string[] properties = new string[] 
			{
				"Account", "Amount", "BuyerName", "EquipmentBidStringSearch"
			};
			
			if(collection == null) collection = _collection;
			
            collection.ToList().ForEach(x => x.setFilters(properties, false));
            
            collection.Where(x => (x.Account.ToLower().IndexOf(searchString) != -1) |
                             	(x.Amount.ToString().ToLower().IndexOf(searchString) != -1) |
                             	(x.BuyerInfo.ToLower().IndexOf(searchString) != -1 ) |
                             	(x.EquipmentBidStringSearch.ToLower().IndexOf(searchString) != -1 )
                             ).ToList().ForEach(y => y.setFilters(properties, true));
		}
	}
}
