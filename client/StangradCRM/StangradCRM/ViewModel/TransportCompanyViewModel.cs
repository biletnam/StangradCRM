/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 22.08.2016
 * Время: 17:27
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.ObjectModel;
using StangradCRM.Core;
using StangradCRM.Model;
using System.Linq;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of TransportCompanyViewModel.
	/// </summary>
	public class TransportCompanyViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static TransportCompanyViewModel _instance = null;
		private TSObservableCollection<TransportCompany> _collection =
			new TSObservableCollection<TransportCompany>();
		
		public TSObservableCollection<TransportCompany> Collection
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
		
		private TransportCompanyViewModel()
		{
			TSObservableCollection<TransportCompany> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<TransportCompany>>("TransportCompany");
			
			if(collection != default(TSObservableCollection<TransportCompany>))
			{
				_collection = collection;
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
		public static TransportCompanyViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new TransportCompanyViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			TransportCompany transportCompany = modelItem as TransportCompany;
			if(transportCompany == null)
			{
				transportCompany.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			TransportCompany exist = getById(transportCompany.Id);
			if(exist != null || _collection.Contains(transportCompany))
			{
				transportCompany.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(transportCompany);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			TransportCompany transportCompany = modelItem as TransportCompany;
			if(transportCompany == null)
			{
				transportCompany.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(transportCompany)) return true;
			return _collection.Remove(transportCompany);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public TransportCompany getById (int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
	}
}
