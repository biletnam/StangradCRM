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
	/// 
	
	/// <summary>
	/// CRMSetting Mashine Names:
	/// 
	/// new_bid_remove_day_count - Количество дней с момента создания заявки, по истечении которых можно удалять заявку, если она со статусом 'Новая';
	/// bid_update_time - Период обновления заявок (в секундах). Если 0 - обновление отключено;
	/// planned_shipment_day_count - Количество дней, которые добавляются к текущей дате, чтобы получить планируемую дату отгрузки по умолчанию ;
	/// warning_shipment_date_day_count - Количество дней до планируемой даты отгрузги для предупреждения;
	/// smtp_server - SMTP сервер (например smtp.yandex.ru);
	/// smtp_port - SMTP порт
	/// mail_user - Логин пользователя почты;
	/// mail_password - Пароль пользователя почты;
	/// mail_use_ssl - Использовать ssl (1 - использовать, другое значение - не использовать);
	/// email - E-mail отправителя;
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
