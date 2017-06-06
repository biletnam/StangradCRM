/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 24.04.2017
 * Time: 11:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of BidFilesViewModel.
	/// </summary>
	public class BidFilesViewModel : Core.BaseViewModel, Core.IViewModel
	{
		
		private static BidFilesViewModel _instance = null;
		
		private TSObservableCollection<BidFiles> _collection
			= new TSObservableCollection<BidFiles>();
		public TSObservableCollection<BidFiles> Collection
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
		
		private BidFilesViewModel()
		{
			load();
		}
		
		public static BidFilesViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new BidFilesViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			BidFiles bidFiles = modelItem as BidFiles;
			if(bidFiles == null)
			{
				bidFiles.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			BidFiles exist = getById(bidFiles.Id);
			if(exist != null || _collection.Contains(bidFiles))
			{
				//bidFiles.LastError = "Данная запись уже есть в коллекции.";
				return true;
			}
			_collection.Add(bidFiles);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			BidFiles bidFiles = modelItem as BidFiles;
			if(bidFiles == null) 
			{
				bidFiles.LastError = "Не удалось преобразовать входные данные.";
				return false;				
			}
			if(!_collection.Contains(bidFiles)) return true;
			return _collection.Remove(bidFiles);
		}
		
		public BidFiles getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public StangradCRM.Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		protected override void removeAllItems()
		{
			_collection.ToList().ForEach(x => remove(x));
		}
		
		protected override void load()
		{
			TSObservableCollection<BidFiles> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<BidFiles>>("BidFiles");
			
			if(collection != default(TSObservableCollection<BidFiles>))
			{
				collection.ToList().ForEach(x => { x.IsSaved = true; add(x); });
			}
		}
		
		public TSObservableCollection<BidFiles> getByBidId(int id_bid)
		{
			List<BidFiles> bidFiles = _collection.Where(x => x.Id_bid == id_bid).ToList();
			return new TSObservableCollection<BidFiles>(bidFiles);
		}
	}
}
