/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.09.2016
 * Время: 13:23
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of BuyerPeriod.
	/// </summary>
	public class BuyerPeriod : ExcelReport
	{
		Buyer buyer;
		List<Bid> bid;
		DateTime start, end;
		
		public BuyerPeriod(Buyer buyer, List<Bid> bid, DateTime start, DateTime end)
		{
			this.buyer = buyer;
			this.bid = bid;
			this.start = start;
			this.end = end;
		}
		
		public override bool Save()
		{
			ReportRow headerRow = new ReportRow();
			headerRow.Add(new ReportCell("Анализ покупателя " + buyer.NameWithCity + " c " + start.ToString("dd.MM.yyyy") + " по " + end.ToString("dd.MM.yyyy")) 
			              {
			              	Width = 10.86,
			              	Height = 30,
			              	ColumnSpan = 5
			              });
			headerRow.Add(new ReportCell() { Width = 22.29 });
			headerRow.Add(new ReportCell() { Width = 8.57 });
			headerRow.Add(new ReportCell() { Width = 14.71 });
			headerRow.Add(new ReportCell() { Width = 13.14 });
			headerRow.Add(new ReportCell() { Width = 13.00, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top });
			Rows.Add(headerRow);
			double sum = 0;
			for(int i = 0; i < bid.Count; i++)
			{
				CreateBidList(bid[i]);
				sum += bid[i].Amount;
				Rows.Add(new ReportRow());
			}
			Rows.Add(new ReportRow());
			
			ReportRow bottomRow = new ReportRow();
			bottomRow.Add(new ReportCell());
			bottomRow.Add(new ReportCell());
			bottomRow.Add(new ReportCell());
			bottomRow.Add(new ReportCell("Итоговая сумма:") { ColumnSpan=1 });
			bottomRow.Add(new ReportCell() {HorizontalAlignment = HorizontalAlignment.Right});
			bottomRow.Add(new ReportCell(sum.ToString().Replace(',', '.')) { BorderColor = System.Drawing.Color.Black, Format= Format.Money });
			
			Rows.Add(bottomRow);
			
			return Create();
		}
		
		private void CreateBidList (Bid bid)
		{
			ReportRow row1 = new ReportRow();
			row1.Add(new ReportCell("Код заявки:") 
			         { 
			         	HorizontalAlignment = HorizontalAlignment.Right,
			         	VerticalAlignment = VerticalAlignment.Bottom,
			         	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			         });
			row1.Add(new ReportCell(bid.Id.ToString()) { 
			         	BorderColor = System.Drawing.Color.Black,
			         	VerticalAlignment = VerticalAlignment.Bottom
			         });
			
			row1.Add(new ReportCell("Сумма:") 
			         {
			         	HorizontalAlignment = HorizontalAlignment.Right,
			         	VerticalAlignment = VerticalAlignment.Bottom,
			         	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			         });
			row1.Add(new ReportCell(bid.Amount.ToString()) { 
			         	BorderColor = System.Drawing.Color.Black,
			         	VerticalAlignment = VerticalAlignment.Bottom });
			
			row1.Add(new ReportCell("Дата отгрузки:") 
			         {
			         	HorizontalAlignment = HorizontalAlignment.Right,
			         	VerticalAlignment = VerticalAlignment.Bottom,
			         	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			         });
			if(bid.Planned_shipment_date != null)
			{
				row1.Add(new ReportCell(((DateTime)bid.Planned_shipment_date).ToString("dd.MM.yyyy"))
				         { 
				         	BorderColor = System.Drawing.Color.Black,
				         	VerticalAlignment = VerticalAlignment.Bottom
				         });
			}
			else
			{
				row1.Add(new ReportCell()
				         { 
				         	VerticalAlignment = VerticalAlignment.Bottom
				         });
			}
			
			ReportRow row2 = new ReportRow();
			row2.Add(new ReportCell("Продавец:") 
			         { 
			         	HorizontalAlignment = HorizontalAlignment.Right,
			         	VerticalAlignment = VerticalAlignment.Bottom,
			         	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			         });
			Seller seller = SellerViewModel.instance().getById(bid.Id_seller);
			row2.Add(new ReportCell(seller.Name) { 
			         	BorderColor = System.Drawing.Color.Black,
			         	VerticalAlignment = VerticalAlignment.Bottom });
			
			row2.Add(new ReportCell("Счет:") 
			         {
			         	HorizontalAlignment = HorizontalAlignment.Right,
			         	VerticalAlignment = VerticalAlignment.Bottom,
			         	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			         });
			row2.Add(new ReportCell(bid.Account) { 
			         	BorderColor = System.Drawing.Color.Black,
			         	VerticalAlignment = VerticalAlignment.Bottom });
			
			row2.Add(new ReportCell("Дата заявки:") 
			         {
			         	HorizontalAlignment = HorizontalAlignment.Right,
			         	VerticalAlignment = VerticalAlignment.Bottom,
			         	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			         });

			row2.Add(new ReportCell(bid.Date_created.ToString("dd.MM.yyyy"))
			         { 
			         	BorderColor = System.Drawing.Color.Black,
			         	VerticalAlignment = VerticalAlignment.Bottom });
			
			Rows.Add(row1);
			Rows.Add(row2);
			Rows.Add(new ReportRow());
			
			CreateEquipmentList(bid);
		}
		
		private void CreateEquipmentList(Bid bid)
		{
			ReportRow titleRow = new ReportRow();
			titleRow.Add(new ReportCell("Оборудование") 
			             {
			             	ColumnSpan=1,
			             	BorderColor = System.Drawing.Color.Black,
			             	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			             });
			titleRow.Add(new ReportCell() 
			             {
			             	HorizontalAlignment = HorizontalAlignment.Center,
			             	VerticalAlignment = VerticalAlignment.Bottom
			             });
			titleRow.Add(new ReportCell("Модификация") 
			             {
			             	ColumnSpan=1,
			             	BorderColor = System.Drawing.Color.Black,
			             	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			             });
			titleRow.Add(new ReportCell() 
			             {
			             	HorizontalAlignment = HorizontalAlignment.Center,
			             	VerticalAlignment = VerticalAlignment.Bottom
			             });
			titleRow.Add(new ReportCell("Серийный номер") 
			             {
			             	ColumnSpan=1,
			             	BorderColor = System.Drawing.Color.Black,
			             	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			             });
			titleRow.Add(new ReportCell() 
			             {
			             	HorizontalAlignment = HorizontalAlignment.Center,
			             	VerticalAlignment = VerticalAlignment.Bottom
			             });
			
			Rows.Add(titleRow);
			
			List<EquipmentBid> equipmentBid = bid.EquipmentBidCollection.ToList();
			for(int i = 0; i < equipmentBid.Count; i++)
			{
				ReportRow row = new ReportRow();
				row.Add(new ReportCell(equipmentBid[i].EquipmentName)
				        {
			             	BorderColor = System.Drawing.Color.Black,
				        	ColumnSpan=1
				        });
				row.Add(new ReportCell() 
				        {
			             	HorizontalAlignment = HorizontalAlignment.Center,
			             	VerticalAlignment = VerticalAlignment.Bottom
				        });
				string modificationName = "";
				if(equipmentBid[i].Id_modification != null)
				{
					modificationName = equipmentBid[i].ModificationName;
				}
				row.Add(new ReportCell(modificationName)
				        {
			             	BorderColor = System.Drawing.Color.Black,
				        	ColumnSpan=1
				        });
				row.Add(new ReportCell() 
				        {
			             	HorizontalAlignment = HorizontalAlignment.Center,
			             	VerticalAlignment = VerticalAlignment.Bottom
				        });
				row.Add(new ReportCell(equipmentBid[i].Serial_number.ToString())
				        {
			             	BorderColor = System.Drawing.Color.Black,
				        	ColumnSpan=1
				        });
				row.Add(new ReportCell() 
				        {
			             	HorizontalAlignment = HorizontalAlignment.Center,
			             	VerticalAlignment = VerticalAlignment.Bottom
				        });
				Rows.Add(row);
			}
		}
	}
}
