/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 15.05.2017
 * Time: 11:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of MessageTemplatesViewModel.
	/// </summary>
	public class MessageTemplatesViewModel : Core.BaseViewModel, Core.IViewModel
	{
		
		private static MessageTemplatesViewModel _instance = null;
		
		private TSObservableCollection<MessageTemplates> _collection
			= new TSObservableCollection<MessageTemplates>();
		public TSObservableCollection<MessageTemplates> Collection
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
		
		private MessageTemplatesViewModel()
		{
			load();
		}
		
		public static MessageTemplatesViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new MessageTemplatesViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			MessageTemplates messageTemplate = modelItem as MessageTemplates;
			if(messageTemplate == null)
			{
				messageTemplate.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			MessageTemplates exist = getById(messageTemplate.Id);
			if(exist != null || _collection.Contains(messageTemplate))
			{
				//bidFiles.LastError = "Данная запись уже есть в коллекции.";
				return true;
			}
			_collection.Add(messageTemplate);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			MessageTemplates messageTemplate = modelItem as MessageTemplates;
			if(messageTemplate == null)
			{
				messageTemplate.LastError = "Не удалось преобразовать входные данные.";
				return false;				
			}
			if(!_collection.Contains(messageTemplate)) return true;
			return _collection.Remove(messageTemplate);
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
			TSObservableCollection<MessageTemplates> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<MessageTemplates>>("MessageTemplates");
			
			if(collection != default(TSObservableCollection<MessageTemplates>))
			{
				collection.ToList().ForEach(x => { x.IsSaved = true; add(x); });
			}
		}
		
		public MessageTemplates getById(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public void search (string search_string)
		{
			_collection.ToList().ForEach(x => x.setFilter("Name", false));
			_collection.Where(x => x.Name.ToLower().IndexOf(search_string.ToLower()) != -1)
				.ToList().ForEach(y => y.setFilter("Name", true));
		}
		
	}
}
