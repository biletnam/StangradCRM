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
			if(bid != null && !_collection.Contains(bid))
			{
				_collection.Add(bid);
				
				getCollectionByStatus(bid.Id_bid_status).Add(bid);
				
				return true;
			}
			bid.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Bid bid = modelItem as Bid;
			if(bid != null && _collection.Contains(bid))
			{
				_collection.Remove(bid);
				
				getCollectionByStatus(bid.Id_bid_status).Remove(bid);
				
				return true;
			}
			bid.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;
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
			return DateTime.Now;
		}
		
		
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
				Log.WriteError(parser.LastError);
				return null;
			}
			return parser.ToObject<List<Bid>>();
		}
		
		public void RemoteLoad()
		{
			List<Bid> bids = loadLastModified();
			if(bids == null) return;
			
			Auth auth = Auth.getInstance();
			
			for(int i = 0; i < bids.Count; i++)
			{
				Bid newBid = bids[i];
				Bid oldBid = getById(newBid.Id);
				
				if(oldBid == null)
				{
					if(auth.IdRole == (int)Classes.Role.Manager)
					{
						if(newBid.Id_manager == auth.Id)
						{
							add(newBid);
						}
					}
					else
					{
						add(newBid);
					}
				}
				else
				{
					if(auth.IdRole == (int)Classes.Role.Manager)
					{
						if(newBid.Id_manager == auth.Id)
						{
							oldBid.replace(newBid);
						}
						else
						{
							oldBid.remove(true);
						}
					}
					else
					{
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
