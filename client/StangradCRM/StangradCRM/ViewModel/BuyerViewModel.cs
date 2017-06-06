/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:01
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
		
		private BuyerViewModel() { load(); }
		
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
			if(buyer == null)
			{
				buyer.LastError = "Не удалось преобразовать входные данные.";
				return false;				
			}
			Buyer exist = getById(buyer.Id);
			if(exist != null || _collection.Contains(buyer))
			{
				buyer.LastError = "Данная запись уже есть в коллекции.";
				return false;	
			}
			_collection.Add(buyer);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			Buyer buyer = modelItem as Buyer;
			if(buyer == null)
			{
				buyer.LastError = "Не удалось преобразовать входные данные";
				return false;
			}
			if(!_collection.Contains(buyer)) return true;
			return _collection.Remove(buyer);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public Buyer getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public List<Buyer> getByMoreDateCreated (DateTime date_created)
		{
			return _collection.Where(x => x.Date_created >= date_created).ToList();
		}
		
		public void fastSearch (string searchString)
		{
			searchString = searchString.ToLower();
			string[] properties = new string[] 
			{
				"Name", "Contact_person", "Phone", "Email", "City", "Passport_serial_number", "Inn"
			};
            _collection.ToList().ForEach(x => x.setFilters(properties, false));
            
            _collection.Where(x => (x.Name.ToLower().IndexOf(searchString) != -1) |
                             (x.Contact_person.ToLower().IndexOf(searchString) != -1) |
                             	(x.Phone.ToString().ToLower().IndexOf(searchString) != -1) |
                             	(x.Email.ToLower().IndexOf(searchString) != -1 ) |
                             	(x.City.ToLower().IndexOf(searchString) != -1 ) |
                             	(x.Passport_serial_number.ToLower().IndexOf(searchString) != -1 ) |
                             	(x.Inn.ToLower().IndexOf(searchString) != -1 )
                             ).ToList().ForEach(y => y.setFilters(properties, true));
		}
		
		//Ф-я загрузки данных с сервера
		private List<Buyer> loadData ()
		{
			HTTPManager.HTTPRequest http = HTTPManager.HTTPRequest.Create(Settings.uri);
			http.UseCookie = true;
			http.addParameter("entity", "Buyer/get");
			
			if(!http.post()) return null;
			
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			if(!parser.NoError)
			{
				return null;
			}
			return parser.ToObject<List<Buyer>>();
		}
		
		//Ф-я удаленной загрузки
		public void RemoteLoad()
		{
			//Список покупателей
			List<Buyer> buyers = null;
			try
			{
				buyers = loadData();
			}
			catch
			{
				return;
			}
			if(buyers == null) return;

			//Цикл по покупателям			
			for(int i = 0; i < buyers.Count; i++)
			{
				Buyer buyer = getById(buyers[i].Id);
				if(buyer == null)
				{
					add(buyers[i]);
				}
				else
				{
					buyer.replace(buyers[i]);
				}
			}
		}
		
		protected override void removeAllItems()
		{
			_collection.ToList().ForEach(x => remove(x));
		}
		
		protected override void load()
		{
			TSObservableCollection<Buyer> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<Buyer>>("Buyer");
			
			if(collection != default(TSObservableCollection<Buyer>))
			{
				collection.ToList().ForEach(x => { x.IsSaved = true; add(x); });
			}
		}
		
		public void search (string search_string)
		{
			_collection.ToList().ForEach(x => x.setFilter("BuyerInfo", false));
			_collection.Where(x => x.BuyerInfo.ToLower().IndexOf(search_string.ToLower()) != -1)
				.ToList().ForEach(y => y.setFilter("BuyerInfo", true));
		}
	}
}
