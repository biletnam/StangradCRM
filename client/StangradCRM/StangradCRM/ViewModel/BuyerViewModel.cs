﻿/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:01
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.Core;
using StangradCRM.Model;
using System.Linq;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of BuyerViewModel.
	/// </summary>
	public class BuyerViewModel : BaseViewModel, IViewModel
	{
		
		private static BuyerViewModel _instance = null;
		
		private TSObservableCollection<Buyer> _collection =
			new TSObservableCollection<Buyer>();
		public TSObservableCollection<Buyer> Collection
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
		
		private BuyerViewModel()
		{
			TSObservableCollection<Buyer> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Buyer>>("Buyer");
			
			if(collection != default(TSObservableCollection<Buyer>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
		public static BuyerViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new BuyerViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			Buyer buyer = modelItem as Buyer;
			if(buyer != null && !_collection.Contains(buyer))
			{
				_collection.Add(buyer);
				return true;
			}
			buyer.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Buyer buyer = modelItem as Buyer;
			if(buyer != null && _collection.Contains(buyer))
			{
				_collection.Remove(buyer);
				return true;
			}
			buyer.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Buyer getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
	}
}
