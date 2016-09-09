/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/10/2016
 * Время: 12:54
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using StangradCRM.Core;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Equipment.
	/// </summary>
	public class Equipment : StangradCRM.Core.Model
	{

		public string Name { get; set; }
		public int Serial_number { get; set; }
		
		protected override string Entity 
		{ 
			get
			{
				return "Equipment";
			}
		}
		
		protected override IViewModel CurrentViewModel {
			get {
				return EquipmentViewModel.instance();
			}
		}
		
		public Equipment() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
			http.addParameter("serial_number", Serial_number);
			http.addParameter("row_order", (EquipmentViewModel.instance().getMaxRowOrder()+1));
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
		}
		
		protected override void prepareRemoveData(HTTPManager.HTTPRequest http)
		{
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
		}
		
		
		public TSObservableCollection<Modification> Modification
		{
			get 
			{
				return ModificationViewModel.instance().getByEquipmentId(Id);
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			if(result)
			{
				raiseAllProperties();
			}
			return result;
		}
		
		
		public override void replace(object o)
		{
			Equipment equipment = o as Equipment;
			if(equipment == null) return;
			Name = equipment.Name;
			Serial_number = equipment.Serial_number;
			
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
			RaisePropertyChanged("Serial_number", Serial_number);
		}
	}
}
