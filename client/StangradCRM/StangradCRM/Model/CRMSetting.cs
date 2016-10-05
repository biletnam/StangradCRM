/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 01.09.2016
 * Время: 11:22
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of CRMSetting.
	/// </summary>
	public class CRMSetting : Core.Model
	{
		
		public string Setting_mashine_name { get; set; }
		public string Setting_name { get; set; }
		public string Setting_value { get; set; }
		
		public CRMSetting() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("setting_value", Setting_value);
			http.addParameter("id", Id);
		}
		
		protected override void prepareRemoveData(HTTPManager.HTTPRequest http)
		{
			throw new NotImplementedException();
		}
		
		protected override string Entity {
			get {
				return "CRMSetting";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return CRMSettingViewModel.instance();
			}
		}
		
		public override void replace(object o)
		{
			CRMSetting setting = o as CRMSetting;
			if(o == null) return;
			Setting_mashine_name = setting.Setting_mashine_name;
			Setting_name = setting.Setting_name;
			Setting_value = setting.Setting_value;
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Setting_mashine_name", Setting_mashine_name);
			RaisePropertyChanged("Setting_name", Setting_name);
			RaisePropertyChanged("Setting_value", Setting_value);
		}
	}
}
