/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:38
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
	/// Description of PaymentStatusViewModel.
	/// </summary>
	public class PaymentStatusViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static PaymentStatusViewModel _instance = null;
		
		private TSObservableCollection<PaymentStatus> _collection
			= new TSObservableCollection<PaymentStatus>();
		public TSObservableCollection<PaymentStatus> Collection
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
		
		private PaymentStatusViewModel() { load(); }
		
		public static PaymentStatusViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new PaymentStatusViewModel();
			}
			return _instance;
		}
			
		public bool @add<T>(T modelItem)
		{
			PaymentStatus paymentStatus = modelItem as PaymentStatus;
			if(paymentStatus == null)
			{
				paymentStatus.LastError = "Не удалось преобразовать входные данные.";
				return false;				
			}
			PaymentStatus exist = getById(paymentStatus.Id);
			if(exist != null || _collection.Contains(paymentStatus))
			{
				paymentStatus.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(paymentStatus);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			PaymentStatus paymentStatus = modelItem as PaymentStatus;
			if(paymentStatus == null)
			{
				paymentStatus.LastError = "Не удалось преобразовать входные данные.";
				return false;				
			}
			if(!_collection.Contains(paymentStatus)) return true;
			return _collection.Remove(paymentStatus);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public PaymentStatus getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}

		protected override void removeAllItems()
		{
			_collection.ToList().ForEach(x => remove(x));
		}
		
		protected override void load()
		{
			TSObservableCollection<PaymentStatus> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<PaymentStatus>>("PaymentStatus");
			
			if(collection != default(TSObservableCollection<PaymentStatus>))
			{
				collection.ToList().ForEach(x => { x.IsSaved = true; add(x); });
			}
		}
		
		public void search (string search_string)
		{
			_collection.ToList().ForEach(x => x.setFilter("Name", false));
			_collection.Where(x => x.Name.ToLower().IndexOf(search_string.ToLower()) != -1)
				.ToList().ForEach(y => y.setFilter("Name", true));
		}
	}
}
