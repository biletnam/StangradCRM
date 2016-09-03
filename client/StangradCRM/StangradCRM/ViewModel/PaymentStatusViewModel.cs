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
		
		private PaymentStatusViewModel()
		{
			TSObservableCollection<PaymentStatus> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<PaymentStatus>>("PaymentStatus");
			
			if(collection != default(TSObservableCollection<PaymentStatus>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
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
			if(paymentStatus != null && !_collection.Contains(paymentStatus))
			{
				_collection.Add(paymentStatus);
				return true;
			}
			paymentStatus.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			PaymentStatus paymentStatus = modelItem as PaymentStatus;
			if(paymentStatus != null && _collection.Contains(paymentStatus))
			{
				_collection.Remove(paymentStatus);
				return true;
			}
			paymentStatus.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public PaymentStatus getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
	}
}
