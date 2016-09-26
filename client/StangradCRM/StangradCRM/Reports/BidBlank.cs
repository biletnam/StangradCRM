/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 21.09.2016
 * Время: 11:57
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of BidBlank.
	/// </summary>
	public class BidBlank : ExcelReport
	{
		Bid bid = null;
		EquipmentBid equipmentBid = null;
		
		protected override Nullable<double> LeftFieldMargin {
			get { return 1; }
		}
		protected override Nullable<double> RightFieldMargin {
			get { return 1; }
		}
		protected override Nullable<double> TopFieldMargin {
			get { return 1; }
		}
		protected override Nullable<double> BottomFieldMargin {
			get { return 1; }
		}
		protected override Nullable<double> HeaderFieldMargin {
			get { return 0; }
		}
		protected override Nullable<double> FooterFieldMargin {
			get { return 0; }
		}
		
		
		public BidBlank(Bid bid, EquipmentBid equipmentBid)
		{
			this.bid = bid;
			this.equipmentBid = equipmentBid;
		}
		
		public override bool Save()
		{
			return createBlank();
		}
		
		private bool createBlank ()
		{
			if(!createNumBidRow())
			{
				return false;
			}
			createEmptyRow(16.50);
			if(!createSellerRow())
			{
				return false;
			}
			if(!createBidDateRow())
			{
				return false;
			}
			if(!createShipmentDateRow())
			{
				return false;
			}
			createEmptyRow(16.50);
			if(!createEquipmentRow())
			{
				return false;
			}
			if(!createModificationRow())
			{
				return false;
			}
			
			createComplectationRows();
			createManagerRow();
			createShipmenntManagerRow();
			
			createDivider();
			createEmptyRow(15.00);
			
			createNumBidRow();
			createEmptyRow(16.50);
			createSellerRow();
			createBidDateRow();
			createShipmentDateRow();
			createEmptyRow(16.50);
			createEquipmentRow();
			createModificationRow();
			createComplectationRows();
			createManagerRow();
			createShipmenntManagerRow();
			
			AllDocumentFontSize = 13;
			return Create();
			
		}
		
		//Пустая строка
		private void createEmptyRow (double height)
		{
			ReportRow empty_row = new ReportRow();
			empty_row.Cells.Add(new ReportCell() { Height = height });
			Rows.Add(empty_row);
		}
		
		//Первая строка - номер бланка заявки
		private bool createNumBidRow ()
		{
			
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("БЛАНК ЗАЯВКИ №") 
			              { 
								ColumnSpan = 9,
								Height = 15,
								Width = 2.86,
								TextStyle = new List<TextStyle> { TextStyle.Bold }
			              });
			
			row.Cells.Add(new ReportCell() { Width = 5});
			row.Cells.Add(new ReportCell() { Width = 6.71});
			row.Cells.Add(new ReportCell() { Width = 5});
			row.Cells.Add(new ReportCell() { Width = 5});
			row.Cells.Add(new ReportCell() { Width = 5});
			row.Cells.Add(new ReportCell() { Width = 6.14});
			row.Cells.Add(new ReportCell() { Width = 5.57});
			row.Cells.Add(new ReportCell() { Width = 5});
			row.Cells.Add(new ReportCell() { 
			              	Width = 5, 
			              	VerticalAlignment = VerticalAlignment.Bottom,
			              	HorizontalAlignment = HorizontalAlignment.Right
			              });
			
			row.Cells.Add(new ReportCell(bid.Id.ToString() + "/" + equipmentBid.Id.ToString())
			              { 
			              	Border = new List<Border>() { Border.All }, 
			              	BorderColor=System.Drawing.Color.Black, 
			              	Width = 5, 
			              	ColumnSpan = 1
			              });
			row.Cells.Add(new ReportCell() { 
			              	Width = 5, 
			              	Border = new List<Border>() { Border.All }, 
			              	BorderColor=System.Drawing.Color.Black, 
			              	VerticalAlignment = VerticalAlignment.Bottom,
			              	HorizontalAlignment = HorizontalAlignment.Center
			              });
			
			row.Cells.Add(new ReportCell() { Width = 5});
			row.Cells.Add(new ReportCell() { Width = 8.57});
			row.Cells.Add(new ReportCell() { Width = 4});
			row.Cells.Add(new ReportCell() {
			              	Width = 4
			              });
			
			Rows.Add(row);
			return true;
		}
		
		//Третья строка (продавец и номер счета)
		private bool createSellerRow ()
		{
			Seller seller = SellerViewModel.instance().getById(bid.Id_seller);
			if(seller == null)
			{
				return false;
			}
			
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Продавец") 
			                {  
			                	Height=16.50,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			row.Cells.Add(new ReportCell(seller.Name) 
			                {
			                	ColumnSpan = 4,
			                	BorderColor = System.Drawing.Color.Black
			                });
			AddEmptyCell(row, 3);
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			row.Cells.Add(new ReportCell("Номер счета") 
			                {  
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			row.Cells.Add(new ReportCell(bid.Account) 
			                {
			                	ColumnSpan = 4,
			                	BorderColor = Color.Black
			                });
			AddEmptyCell(row, 3);
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			Rows.Add(row);
			
			return true;
		}
		
		//Четвертая строка (дата создания и дата счета)
		private bool createBidDateRow ()
		{
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Дата заявки") 
			                {  
			                	Height=16.50,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			row.Cells.Add(new ReportCell(DateTime.Now.ToString("dd.MM.yyyy"))
			                {
			                	ColumnSpan = 4,
			                	BorderColor = System.Drawing.Color.Black
			                });
			AddEmptyCell(row, 3);
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			row.Cells.Add(new ReportCell("Дата счета") 
			                {  
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			row.Cells.Add(new ReportCell(bid.Date_created.ToString("dd.MM.yyyy")) 
			                {
			                	ColumnSpan = 4,
			                	BorderColor = Color.Black
			                });
			AddEmptyCell(row, 3);
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			Rows.Add(row);
			
			return true;
		}
		
		//Пятая строка (дата отгрузки и покупатель)
		private bool createShipmentDateRow ()
		{
			Buyer buyer = BuyerViewModel.instance().getById(bid.Id_buyer);
			if(buyer == null)
			{
				return false;
			}
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Дата отгрузки") 
			                {  
			                	Height=16.50,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			string shipmentDate = "";
			if(bid.Planned_shipment_date != null)
			{
				shipmentDate = ((DateTime)bid.Planned_shipment_date).ToString("dd.MM.yyyy");
			}
			row.Cells.Add(new ReportCell(shipmentDate)
			                {
			                	ColumnSpan = 4,
			                	BorderColor = System.Drawing.Color.Black
			                });
			AddEmptyCell(row, 3);
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			row.Cells.Add(new ReportCell("Покупатель") 
			                {  
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			row.Cells.Add(new ReportCell(buyer.Name) 
			                {
			                	ColumnSpan = 4,
			                	BorderColor = Color.Black
			                });
			AddEmptyCell(row, 3);
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			Rows.Add(row);
			
			return true;
		}
		
		//Седьмая строка (оборудование и серийный номер)
		private bool createEquipmentRow ()
		{
			Equipment equipment = EquipmentViewModel.instance().getById(equipmentBid.Id_equipment);
			if(equipment == null)
			{
				return false;
			}
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Оборудование") 
			                {  
			                	Height=16.50,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			row.Cells.Add(new ReportCell(equipment.Name)
			                {
			                	ColumnSpan = 7,
			                	BorderColor = System.Drawing.Color.Black
			                });
			AddEmptyCell(row, 6);
			
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			row.Cells.Add(new ReportCell("Серийный номер:") 
			                {  
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom,
			                	ColumnSpan=2
			                });
			AddEmptyCell(row, 2);
			string serialNumber = "";
			if(equipmentBid.Serial_number != null)
			{
				serialNumber = equipmentBid.Serial_number.ToString();
			}
			row.Cells.Add(new ReportCell(serialNumber) 
			                {
			                	ColumnSpan = 1,
			                	BorderColor = Color.Black
			                });
			AddEmptyCell(row, 3);
			row.Cells.Add(new ReportCell() { VerticalAlignment = VerticalAlignment.Bottom});
			Rows.Add(row);
			
			return true;
		}
		
		//Восьмая строка (модификации)
		private bool createModificationRow ()
		{
			string modificationName = "";
			if(equipmentBid.Id_modification != null)
			{
				Modification modification = ModificationViewModel.instance().getById((int)equipmentBid.Id_modification);
				if(modification != null)
				{
					modificationName = modification.Name;
				}
			}
			
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Модификация") 
			                {  
			                	Height=16.50,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);
			row.Cells.Add(new ReportCell(modificationName)
			                {
			                	ColumnSpan = 12,
			                	BorderColor = System.Drawing.Color.Black
			                });
			AddEmptyCell(row, 11);
			row.Cells.Add(new ReportCell() 
			              {
			              	VerticalAlignment = VerticalAlignment.Bottom,
			              });
			
			Rows.Add(row);
			
			return true;
		}
		
		//Девятая строка (заголовок "Комплектация:")
		private bool createComplectationTitleRow ()
		{
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Комплектация:") 
			                {  
			                	Height=21.00,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			Rows.Add(row);
			return true;
		}
		
		//Список комплектаций
		private bool createComplectationRows ()
		{
			List<Complectation> complectations = equipmentBid.ComplectationCollection.ToList();
			if(complectations.Count == 0) return true;
			
			createComplectationTitleRow ();
			
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("№") 
			                {  
			                	Height=16.50,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom,
			                	BorderColor = Color.Black
			                });
			row.Cells.Add(new ReportCell("Наименование") 
			              {
			              	ColumnSpan = 12,
			              	BorderColor = Color.Black,
		                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			              });
			AddEmptyCell(row, 11);
			row.Cells.Add(new ReportCell() 
			              {
  		                	VerticalAlignment = VerticalAlignment.Bottom,
		                	HorizontalAlignment = HorizontalAlignment.Center,
		                	BorderColor = Color.Black
			              });
			
			row.Cells.Add(new ReportCell("Кол-во")
			              {
							ColumnSpan = 1,
							BorderColor = Color.Black,
		                	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			              });
			row.Cells.Add(new ReportCell() 
			              {
			              	VerticalAlignment = VerticalAlignment.Bottom,
			              	HorizontalAlignment = HorizontalAlignment.Center
			              });
			
			Rows.Add(row);
			
			for(int i = 0; i < complectations.Count; i++)
			{
				ReportRow complectationRow = new ReportRow();
				complectationRow.Cells.Add(new ReportCell((i+1).ToString())
				                {  
				                	Height=16.50,
				                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
				                	VerticalAlignment = VerticalAlignment.Bottom,
				                	HorizontalAlignment = HorizontalAlignment.Right,
				                	BorderColor = Color.Black
				                });
				complectationRow.Cells.Add(new ReportCell(complectations[i].Name)
				              {
				              	ColumnSpan = 12,
				              	BorderColor = Color.Black,
			                	
				              });
				AddEmptyCell(complectationRow, 11);
				complectationRow.Cells.Add(new ReportCell() 
				              {
	  		                	VerticalAlignment = VerticalAlignment.Bottom,
			                	BorderColor = Color.Black
				              });
				
				complectationRow.Cells.Add(new ReportCell(complectations[i].Complectation_count.ToString())
				              {
								ColumnSpan = 1,
								BorderColor = Color.Black,
				              });
				complectationRow.Cells.Add(new ReportCell() 
				              {
				              	VerticalAlignment = VerticalAlignment.Bottom,
				              	HorizontalAlignment = HorizontalAlignment.Center
				              });
				
				Rows.Add(complectationRow);
			}
			return true;
		}
		
		
		//Данные заявителя
		private bool createManagerRow ()
		{
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Заявитель") 
			                {  
			                	Height=24.00,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 2);

			row.Cells.Add(new ReportCell(Auth.getInstance().Full_name)
			                {
			                	ColumnSpan = 7,
			                	BorderColor = System.Drawing.Color.Black,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			                });
			AddEmptyCell(row, 6);
			row.Cells.Add(new ReportCell() 
			              {
			              	VerticalAlignment = VerticalAlignment.Bottom,
			              	HorizontalAlignment = HorizontalAlignment.Center
			              });
			
			row.Cells.Add(new ReportCell() 
			                {  
			              		BorderColor = System.Drawing.Color.Black,
			                	ColumnSpan = 4
			                });
			AddEmptyCell(row, 4);
			Rows.Add(row);
			
			return true;
		}
		
		//Подпись ответственного за отгрузку
		private bool createShipmenntManagerRow ()
		{
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell("Подпись ответственного за отгрузку") 
			                {  
			                	Height=23.25,
			                	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                	VerticalAlignment = VerticalAlignment.Bottom
			                });
			AddEmptyCell(row, 6);
			
			row.Cells.Add(new ReportCell()
			                {
			                	ColumnSpan = 8,
			                	Border = new List<Border>() { Border.Bottom },
			                	BorderWeight = BorderWeight.Thick,
			                	BorderColor = System.Drawing.Color.Black,
			                });
			Rows.Add(row);
			
			return true;
		}
		
		//Разделитель
		private void createDivider ()
		{
			ReportRow row = new ReportRow();
			row.Cells.Add(new ReportCell() 
			              {
			              	Height=15.00,
			              	ColumnSpan=15,
			              	BorderStyle = BorderStyle.Dot,
			              	BorderWeight = BorderWeight.Thick,
			              	
			              	Border = new List<Border>() { Border.Bottom },
			              });
			Rows.Add(row);
		}
		
		private void AddEmptyCell(ReportRow row, int cellCount = 1)
		{
			for(int i = 0; i < cellCount; i++)
			{
				row.Cells.Add(null);
			}
		}
	}
}
