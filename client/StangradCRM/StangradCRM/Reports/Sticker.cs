/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 19.09.2016
 * Время: 17:50
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;
using System.Collections.Generic;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of Sticker.
	/// </summary>
	public class Sticker : Core.ExcelReport
	{
		EquipmentBid equipmentBid = null;
		string serialNumber = "";
		string modificationString = "";
		
		protected override Nullable<double> LeftFieldMargin {
			get { return 2.2; }
		}
		protected override Nullable<double> RightFieldMargin {
			get { return 2.2; }
		}
		protected override Nullable<double> TopFieldMargin {
			get { return 10; }
		}
		protected override Nullable<double> BottomFieldMargin {
			get { return 1.9; }
		}
		protected override Nullable<double> HeaderFieldMargin {
			get { return 0; }
		}
		protected override Nullable<double> FooterFieldMargin {
			get { return 0.8; }
		}
		
		public Sticker(EquipmentBid equipmentBid)
		{
			if(equipmentBid.Serial_number != null)
			{
				serialNumber = equipmentBid.Serial_number.ToString();
			}
			this.equipmentBid = equipmentBid;
		}
		
		public override bool Save()
		{
			Bid bid = BidViewModel.instance().getById(equipmentBid.Id_bid);
			if(bid == null)
			{
				LastError = "Bid not found!";
				return false;
			}
			Buyer buyer = BuyerViewModel.instance().getById(bid.Id_buyer);
			if(buyer == null)
			{
				LastError = "Buyer not found!";
				return false;
			}
			
			Equipment equipment = EquipmentViewModel.instance().getById(equipmentBid.Id_equipment);
			if(equipment == null)
			{
				LastError = "Equipment not found!";
				return false;
			}
			Modification modification = null;
			if(equipmentBid.Id_modification != null)
			{
				modification = ModificationViewModel.instance().getById((int)equipmentBid.Id_modification);
				if(modification != null)
				{
					modificationString = modification.Name;
				}
			}
			
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell(buyer.Name.ToUpper()) 
			              { 
								ColumnSpan = 6,
								Height = 20.25,
								TextStyle = new List<TextStyle>() { TextStyle.Bold },
								
			              });
			row.Cells.Add(new ReportCell());
			row.Cells.Add(new ReportCell());
			row.Cells.Add(new ReportCell());
			row.Cells.Add(new ReportCell());
			row.Cells.Add(new ReportCell());
			row.Cells.Add(new ReportCell() { Width = 0.58, VerticalAlignment = VerticalAlignment.Bottom });
			row.Cells.Add(new ReportCell(buyer.City.ToUpper()) 
			              { 
			              	ColumnSpan = 2,
			              	HorizontalAlignment = HorizontalAlignment.Right,
			              	VerticalAlignment = VerticalAlignment.Bottom,
			              	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			              });
			row.Cells.Add(new ReportCell() 
			              { 
			              	Width = 7.29,
			              	VerticalAlignment = VerticalAlignment.Bottom,
			              	HorizontalAlignment = HorizontalAlignment.Right });
			Rows.Add(row);
			
			ReportRow row_1 = new ReportRow();
			row_1.Cells.Add(new ReportCell(buyer.Contact_person.ToUpper()) 
			                {
				              	ColumnSpan = 6,
				              	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                });
			row_1.Cells.Add(new ReportCell());
			row_1.Cells.Add(new ReportCell());
			row_1.Cells.Add(new ReportCell());
			row_1.Cells.Add(new ReportCell());
			row_1.Cells.Add(new ReportCell());
			row_1.Cells.Add(new ReportCell());
			row_1.Cells.Add(new ReportCell(buyer.Phone.ToUpper()) { 
			                	ColumnSpan = 2, 
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	HorizontalAlignment = HorizontalAlignment.Right});
			Rows.Add(row_1);
			
			Rows.Add(new ReportRow());
			
			ReportRow row_2 = new ReportRow();
			row_2.Cells.Add(new ReportCell(equipment.Name.ToUpper()) 
			                {
				              	ColumnSpan = 6,
				              	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			                });
			row_2.Cells.Add(new ReportCell());
			row_2.Cells.Add(new ReportCell());
			row_2.Cells.Add(new ReportCell());
			row_2.Cells.Add(new ReportCell());
			row_2.Cells.Add(new ReportCell());
			row_2.Cells.Add(new ReportCell());
			row_2.Cells.Add(new ReportCell(serialNumber.ToUpper()) 
			                { 
			                	RowSpan = 1,
			                	ColumnSpan = 2,
			                	HorizontalAlignment = HorizontalAlignment.Right,
			                	VerticalAlignment = VerticalAlignment.Bottom,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			                });
			Rows.Add(row_2);
			
			ReportRow row_3 = new ReportRow();
			row_3.Cells.Add(new ReportCell(modificationString.ToUpper()) 
			                {
				              	ColumnSpan = 6,
				              	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			                });
			Rows.Add(row_3);
			
			AllDocumentFontSize = 14;
			
			return Create();
		}
	}
}
