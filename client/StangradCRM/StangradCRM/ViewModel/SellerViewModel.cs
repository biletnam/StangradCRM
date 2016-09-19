/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:24
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
	/// Description of SellerViewModel.
	/// </summary>
	public class SellerViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static SellerViewModel _instance = null;
		private TSObservableCollection<Seller> _collection =
			new TSObservableCollection<Seller>();
		public TSObservableCollection<Seller> Collection
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
		
		private SellerViewModel()
		{
			TSObservableCollection<Seller> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Seller>>("Seller");
			
			if(collection != default(TSObservableCollection<Seller>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
		public static SellerViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new SellerViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			Seller seller = modelItem as Seller;
			if(seller == null)
			{
				seller.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			Seller exist = getById(seller.Id);
			if(exist != null || _collection.Contains(seller))
			{
				seller.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(seller);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Seller seller = modelItem as Seller;
			if(seller == null)
			{
				seller.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(seller)) return true;
			return	_collection.Remove(seller);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Seller getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
	}
}
