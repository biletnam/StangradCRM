/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 01.09.2016
 * Время: 11:25
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Linq;
using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.ViewModel
{
	/// <summary>
	/// Description of CRMSettingViewModel.
	/// </summary>
	public class CRMSettingViewModel : Core.BaseViewModel, Core.IViewModel
	{
		private static CRMSettingViewModel _instance = null;
		private TSObservableCollection<CRMSetting> _collection =
			new TSObservableCollection<CRMSetting>();
		public TSObservableCollection<CRMSetting> Collection
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
		
		private CRMSettingViewModel()
		{
			TSObservableCollection<CRMSetting> collection =
			StangradCRM.Core.Model.load<TSObservableCollection<CRMSetting>>("CRMSetting");
			
			if(collection != default(TSObservableCollection<CRMSetting>))
			{
				_collection = collection;
				
				_collection.ToList().ForEach(x => { x.IsSaved = true; });
			}
		}
		
		public static CRMSettingViewModel instance()
		{
			if(_instance == null)
			{
				_instance = new CRMSettingViewModel();
			}
			return _instance;
		}
		
		public bool @add<T>(T modelItem)
		{
			CRMSetting crmSetting = modelItem as CRMSetting;
			if(crmSetting == null)
			{
				crmSetting.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			CRMSetting exist = getById(crmSetting.Id);
			if(exist != null || _collection.Contains(crmSetting))
			{
				crmSetting.LastError = "Данная запись уже есть в коллекции.";
				return false;
			}
			_collection.Add(crmSetting);
			return true;
		}
		
		public bool @remove<T>(T modelItem)
		{
			CRMSetting crmSetting = modelItem as CRMSetting;
			if(crmSetting == null)
			{
				crmSetting.LastError = "Не удалось преобразовать входные данные.";
				return false;
			}
			if(!_collection.Contains(crmSetting)) return true;
			return _collection.Remove(crmSetting);
		}
		
		public Core.Model getItem(int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
		public CRMSetting getByMashineName (string mashineName)
		{
			return _collection.Where(x => x.Setting_mashine_name == mashineName).FirstOrDefault();
		}
		
		public CRMSetting getById (int id)
		{
			return _collection.Where(x => x.Id == id).FirstOrDefault();
		}
		
	}
}
