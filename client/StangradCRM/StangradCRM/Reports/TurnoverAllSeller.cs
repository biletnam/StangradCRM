/*
 * Created by SharpDevelop.
 * User: Дмитрий
 * Date: 03/03/2017
 * Time: 12:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of TurnoverAllSeller.
	/// </summary>
	public class TurnoverAllSeller : ExcelReport
	{
		
		List<Payment> payments;
		List<Seller> sellers;
		DateTime start, end;
		
		public TurnoverAllSeller(List<Payment> payments, List<Seller> sellers, DateTime start, DateTime end)
		{
			this.payments = payments;
			this.sellers = sellers;
			this.start = start;
			this.end = end;
		}
		
		public override bool Save()
		{
			ReportRow headerRow = new ReportRow();
			headerRow.Cells.Add(new ReportCell() { Width = 11.71});
			headerRow.Cells.Add(new ReportCell("Отчет поступлений c " + start.ToString("dd.MM.yyyy") + " по " + end.ToString("dd.MM.yyyy")) { Width = 58.57, HorizontalAlignment = HorizontalAlignment.Center });
			headerRow.Cells.Add(new ReportCell() { Width = 14.43});
			Rows.Add(headerRow);
			
			ReportRow titleRow = new ReportRow();
			titleRow.Cells.Add(new ReportCell("№") { BorderColor = System.Drawing.Color.Black });
			titleRow.Cells.Add(new ReportCell("Продавец") { BorderColor = System.Drawing.Color.Black });
			titleRow.Cells.Add(new ReportCell("Сумма") { BorderColor = System.Drawing.Color.Black });
			Rows.Add(titleRow);
			
			createData();
			
			return Create();
		}
		
		public void createData ()
		{
			double sum = 0;
			for(int i = 0; i < sellers.Count; i++)
			{
				
				double currentSellerSum = getSumBySeller(sellers[i]);
				sum += currentSellerSum;
				
				ReportRow row = new ReportRow();
				row.Cells.Add(new ReportCell ((i+1).ToString())
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					BorderColor = System.Drawing.Color.Black
				});
				row.Cells.Add(new ReportCell (sellers[i].Name) { BorderColor = System.Drawing.Color.Black });
				row.Cells.Add(new ReportCell (currentSellerSum.ToString()) { BorderColor = System.Drawing.Color.Black, HorizontalAlignment = HorizontalAlignment.Right, Format = Format.Money });
				Rows.Add(row);
			}
			
			ReportRow sumRow = new ReportRow();
			sumRow.Add(new ReportCell());
			sumRow.Add(new ReportCell("Итого:") {HorizontalAlignment = HorizontalAlignment.Right, BorderColor = System.Drawing.Color.Black});
			sumRow.Add(new ReportCell(sum.ToString()) { HorizontalAlignment = HorizontalAlignment.Right, BorderColor = System.Drawing.Color.Black, Format = Format.Money });
			Rows.Add(sumRow);
		}
		
		private double getSumBySeller (Seller seller) 
		{
			double sum = 0;
			for(int i = 0; i < payments.Count; i++) 
			{
				Bid bid = BidViewModel.instance().getById(payments[i].Id_bid);
				if(bid.Id_seller != seller.Id) continue;
				sum += payments[i].Paying;
			}
			return sum;
		}
		
	}
}
