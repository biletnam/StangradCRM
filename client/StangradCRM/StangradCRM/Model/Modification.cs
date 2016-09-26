/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 16:30
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Modification.
	/// </summary>
	public class Modification : Core.Model
	{
		public string Name  { get; set; }
		public int Id_equipment { get; set; }

		public Modification() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
			http.addParameter("id_equipment", Id_equipment);
			http.addParameter("row_order", Row_order);
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
		
		protected override string Entity {
			get {
				return "Modification";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return ModificationViewModel.instance();
			}
		}
		
		public Equipment Equipment
		{
			get
			{
				if(Id_equipment != 0)
				{
					return EquipmentViewModel.instance().getById(Id_equipment);
				}
				return null;
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
			Modification modification = o as Modification;
			if(modification == null) return;
			Name = modification.Name;
			Id_equipment = modification.Id_equipment;
			Row_order = modification.Row_order;
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
			RaisePropertyChanged("Id_equipment", Id_equipment);
			RaisePropertyChanged("Row_order", Row_order);
		}
	}
}
