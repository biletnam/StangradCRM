/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/10/2016
 * Время: 12:55
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

using HTTPManager;
using StangradCRMLibs;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of Model.
	/// </summary>
	/// 
	
	public class SaveResultData
	{
		public int Id { get; set; }
		public int Row_order { get; set; }
		public DateTime? Last_modified { get; set; }
	}
	
	public abstract class Model : INotifyPropertyChanged
	{
		private int id = 0;
		public int Id 
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
				RaisePropertyChanged("Id", value);
			}
		}
		
		private int row_order;
		public int Row_order
		{
			get
			{
				return row_order;
			}
			set
			{
				row_order = value;
				RaisePropertyChanged("Row_order", value);
			}
		}
		
		private DateTime last_modified;
		public DateTime Last_modified 
		{
			get
			{
				return last_modified;
			}
			set
			{
				last_modified = value;
				RaisePropertyChanged("Last_modified", last_modified);
			}
		}
		
		public string LastError {get; set; }
		public static string LastLoadError {get; private set;}
		
		private bool isVisible = true;
		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				isVisible = value;
				RaisePropertyChanged("IsVisible", value);
			}
		}
		
		private bool isSelected = false;
		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				isSelected = value;
				RaisePropertyChanged("IsSelected", value);
			}
		}
		
		private bool isSaved = false;
		public bool IsSaved
		{
			get
			{
				return isSaved;
			}
			set
			{
				isSaved = value;
				RaisePropertyChanged("IsSaved", value);
			}
		}
		
		private Dictionary<string, bool> filters = new Dictionary<string, bool>();
		
		
		protected Dictionary<string, object> oldValues = new Dictionary<string, object>();
		
		protected abstract string Entity {get;}
		
		protected abstract void prepareSaveData (HTTPRequest http);
		protected abstract void prepareRemoveData (HTTPRequest http);
		
		public abstract void replace (object o);
		public abstract void raiseAllProperties();
		
		protected abstract IViewModel CurrentViewModel { get; }
		
		protected virtual bool afterSave(ResponseParser parser)
		{
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<String>();
				return false;
			}
			
			//Данные, возвращаемые после сохранения
			SaveResultData saveResult = parser.ToObject<SaveResultData>();
			if(saveResult == null)
			{
				LastError = "Данные о добавленной строке не были преобразованы";
				return false;
			}

			if(saveResult.Last_modified != null)
			{
				Last_modified = (DateTime)saveResult.Last_modified;
			}
			
			if(Id == 0)
			{
				Row_order = saveResult.Row_order;
				if(saveResult.Id == 0)
				{
					LastError = "Идентификатор не был получен.";
					return false;
				}
				Id = saveResult.Id;
				return CurrentViewModel.add(this);
			}
			raiseAllProperties();
			return true;
		}
		
		protected virtual bool afterRemove(ResponseParser parser, bool soft = false)
		{
			if(soft) return CurrentViewModel.remove(this);
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<String>();
				return false;
			}
			return CurrentViewModel.remove(this);
		}
		
		public virtual void loadedItemInitProperty ()
		{
			RaisePropertyChanged("Id", Id, true);
		}
		
		public virtual void setOldValues ()
		{
			try
			{
				Id = (int)oldValues["Id"];
			}
			catch {}
		}
		
	    protected void RaisePropertyChanged(string name, object v, bool p = false) {
			if(p) oldValues[name] = v;
	        if (PropertyChanged != null) {
				oldValues[name] = v;
	            PropertyChanged(this, new PropertyChangedEventArgs(name));
	        }
	    }
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		public static T load <T> (string entity, Dictionary<string, object> parameters = null)
		{
			HTTPRequest http = Request();
			http.addParameter("entity", entity + "/get");
			if(parameters != null)
			{
				http.Parameters = parameters;
			}
			if(!exec(http))
			{
				LastLoadError = HTTPRequest.LastError;
				return default(T);
			}
			
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			if(!parser.NoError)
			{
				LastLoadError = parser.LastError;
				return default(T);
			}
			return parser.ToObject<T>();
		}
		
		public bool save ()
		{
			HTTPRequest http = Request();
			http.addParameter("entity", Entity + "/save");
			prepareSaveData(http);
			if(!exec(http, this))
			{
				return false;
			}
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			if(!parser.NoError)
			{
				LastError = parser.LastError;
				return false;
			}
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<string>();
				return false;
			}
			return afterSave(parser);
		}
		
		public bool remove (bool soft = false)
		{
			
			if(soft) return afterRemove(null, soft);
			
			HTTPRequest http = Request();
			http.addParameter("entity", Entity + "/delete");
			prepareRemoveData(http);
			if(!exec(http, this))
			{
				return false;
			}
			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			if(!parser.NoError)
			{
				LastError = parser.LastError;
				return false;
			}
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<string>();
				return false;
			}
			return afterRemove(parser);
		}
		
		private static HTTPRequest Request ()
		{
			HTTPRequest http = HTTPRequest.Create(Settings.uri);
			http.UseCookie = true;
			return http;
		}
		
		private static bool exec (HTTPRequest http, Model model = null)
		{
			if(!http.post())
			{
				if(model != null)
				{
					model.LastError = HTTPRequest.LastError;
				}
				return false;
			}
			return true;
		}
		
		public void UpdateProperty (string propertyName)
		{
			RaisePropertyChanged(propertyName, null);
		}
		
		public void setFilter (string propertyName, bool visible)
		{
            if(filters.ContainsKey(propertyName))
            {
                filters[propertyName] = visible;
            }
            else
            {
                filters.Add(propertyName, visible);
            }
			updateFilter();
		}
		
        public void setFilters(string[] properties, bool visible)
        {
            for(int i = 0; i < properties.Length; i++)
            {
                setFilter(properties[i], visible);
            }
		}
		
		
        private void updateFilter ()
        {
            foreach(KeyValuePair<string, bool> filterValue in filters)
            {
                if (filterValue.Value == false)
                {
                    IsVisible = false;
                    return;
                }
            }
            IsVisible = true;
		}

        public bool rowUp ()
        {
        	return changeRowOrder("rowUp");
        }
        
        public bool rowDown ()
        {
        	return changeRowOrder("rowDown");
        }
		
        
        private bool changeRowOrder(string action)
        {
			HTTPRequest http = Request();
			http.addParameter("entity", Entity + "/" + action);
			http.addParameter("id", Id);
			if(!exec(http, this))
			{
				return false;
			}

			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			
			if(!parser.NoError)
			{
				LastError = parser.LastError;
				return false;
			}
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<string>();
				return false;
			}
			
			int changedRowId = parser.ToObject<int>();
			if(changedRowId == 0) return true;
			return rowSwap(changedRowId);
        }
        
        private bool rowSwap (int changedRowId)
        {
			Model item = CurrentViewModel.getItem(changedRowId);
			if(item == null) 
			{
				LastError = "Item is null";
				return false;
			}
			int itemRowOrder = item.Row_order;
			item.Row_order = Row_order;
			Row_order = itemRowOrder;
			
			return true;
        }
        
	}
}
