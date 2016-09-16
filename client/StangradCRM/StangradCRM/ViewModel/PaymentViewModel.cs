/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 29.08.2016
 * Время: 11:44
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRMLibs;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of PaymentViewModel.
	/// </summary>
	public class PaymentViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static PaymentViewModel _instance = null;
		private TSObservableCollection<Payment> _collection = 
			new TSObservableCollection<Payment>();
		public TSObservableCollection<Payment> Collection
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
		
		private PaymentViewModel() {}
		
		public static PaymentViewModel instance ()
		{
			if(_instance == null)
			{
				_instance = new PaymentViewModel();
			}
			return _instance;			
		}
		
		public bool @add<T>(T modelItem)
		{
			Payment payment = modelItem as Payment;
			if(payment != null && !_collection.Contains(payment))
			{
				_collection.Add(payment);
				return true;
			}
			payment.LastError = "Не удалось преобразовать входные данные, либо данная запись уже есть в коллекции.";
			return false;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Payment payment = modelItem as Payment;
			if(payment != null && _collection.Contains(payment))
			{
				_collection.Remove(payment);
				return true;
			}
			payment.LastError = "Не удалось преобразовать входные данные, либо данной записи нет в коллекции.";
			return false;
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Payment getById(int paymentId)
		{
			return _collection.Where(x => x.Id == paymentId).FirstOrDefault();
		}
		
		public TSObservableCollection<Payment> getByBidId (int bidId)
		{
			List<Payment> payment = _collection.Where(x => x.Id_bid == bidId).ToList();
			return new TSObservableCollection<Payment>(payment);
		}
		
		public double getDebtByBidId (int bidId)
		{
			TSObservableCollection<Payment> payment = getByBidId(bidId);
			double Debt = 0;
			for(int i = 0; i < payment.Count; i++)
			{
				Debt += payment[i].Paying;
			}
			return Debt;
		}
		
		public Payment getFirstByBidId (int bidId)
		{
			return _collection.Where(x => x.Id_bid == bidId).FirstOrDefault();
		}
	}
}
