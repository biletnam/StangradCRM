/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.09.2016
 * Время: 11:52
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of Receipts.
	/// </summary>
	public class Receipts :ExcelReport
	{
		List<Payment> payment;
		Seller seller;
		DateTime start, end;
		
		public Receipts(List<Payment> payment, Seller seller, DateTime start, DateTime end)
		{
			this.payment = payment;
			this.seller = seller;
			this.start = start;
			this.end = end;
		}
		
		public override bool Save()
		{
			ReportRow headerRow = new ReportRow();
			headerRow.Cells.Add(new ReportCell() { Width = 11.71});
			headerRow.Cells.Add(new ReportCell("Отчет поступлений " + seller.Name + " c " + start.ToString("dd.MM.yyyy") + " по " + end.ToString("dd.MM.yyyy")) { Width = 58.57, HorizontalAlignment = HorizontalAlignment.Center });
			headerRow.Cells.Add(new ReportCell() { Width = 14.43});
			Rows.Add(headerRow);
			
			ReportRow titleRow = new ReportRow();
			titleRow.Cells.Add(new ReportCell("Код заявки"));
			titleRow.Cells.Add(new ReportCell("Покупатель"));
			titleRow.Cells.Add(new ReportCell("Сумма"));
			Rows.Add(titleRow);
			
			createData();
			
			return Create();
		}
		
		public void createData ()
		{
			double sum = 0;
			for(int i = 0; i < payment.Count; i++)
			{
				Bid bid = BidViewModel.instance().getById(payment[i].Id_bid);
				if(bid == null || bid.Id_seller != seller.Id) continue;
				
				Buyer buyer = BuyerViewModel.instance().getById(bid.Id_buyer);
				
				ReportRow row = new ReportRow();
				row.Cells.Add(new ReportCell (bid.Id.ToString()) 
				              {
				              	HorizontalAlignment = HorizontalAlignment.Center,
				              	BorderColor = System.Drawing.Color.Black
				              });
				row.Cells.Add(new ReportCell (buyer.Name) { BorderColor = System.Drawing.Color.Black });
				
				row.Cells.Add(new ReportCell (payment[i].Paying.ToString()) { BorderColor = System.Drawing.Color.Black, HorizontalAlignment = HorizontalAlignment.Right, Format = Format.Money });
				
				Rows.Add(row);
				
				sum += payment[i].Paying;
			}
			
			ReportRow sumRow = new ReportRow();
			sumRow.Add(new ReportCell());
			sumRow.Add(new ReportCell("Итого:") {HorizontalAlignment = HorizontalAlignment.Right});
			sumRow.Add(new ReportCell(sum.ToString().Replace(',', '.')) { HorizontalAlignment = HorizontalAlignment.Right, BorderColor = System.Drawing.Color.Black, Format = Format.Money });
			Rows.Add(sumRow);
		}
	}
}
