/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 28.09.2016
 * Время: 18:39
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of BuyerList.
	/// </summary>
	public class BuyerList : Core.ExcelReport
	{
		List<Buyer> buyerList;
		public BuyerList(List<Buyer> buyerList)
		{
			this.buyerList = buyerList;
		}
		
		public override bool Save()
		{
			if(buyerList == null || buyerList.Count == 0)
			{
				LastError = "Список созданных покупателей за выбранный период пуст!";
				return false;
			}
			ReportRow headerRow = new ReportRow();
			
			headerRow.Cells.Add(new ReportCell ("Организация") { Border = new List<Border>() { Border.All }, 			              	BorderColor=System.Drawing.Color.Black, 
			              			Width = 25,
			              			TextStyle = new List<TextStyle>() { TextStyle.Bold },
			                    	VerticalAlignment = VerticalAlignment.Bottom,
			              			HorizontalAlignment = HorizontalAlignment.Center });
			headerRow.Cells.Add(new ReportCell ("Контактное лицо") { Border = new List<Border>() { Border.All }, 			              	BorderColor=System.Drawing.Color.Black, 
			              			Width = 25,
			                    	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			              			VerticalAlignment = VerticalAlignment.Bottom,
			              			HorizontalAlignment = HorizontalAlignment.Center });
			headerRow.Cells.Add(new ReportCell ("Телефон") { Border = new List<Border>() { Border.All }, 			              	BorderColor=System.Drawing.Color.Black, 
			              			Width = 20,
			                    	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			              			VerticalAlignment = VerticalAlignment.Bottom,
			              			HorizontalAlignment = HorizontalAlignment.Center });
			headerRow.Cells.Add(new ReportCell ("Электронная почта") { Border = new List<Border>() { Border.All }, 			              	BorderColor=System.Drawing.Color.Black, 
			              			Width = 20,
			                    	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			              			VerticalAlignment = VerticalAlignment.Bottom,
			              			HorizontalAlignment = HorizontalAlignment.Center });
			headerRow.Cells.Add(new ReportCell ("Город") { Border = new List<Border>() { Border.All }, 			              	BorderColor=System.Drawing.Color.Black, 
			              			Width = 28,
			                    	TextStyle = new List<TextStyle>() { TextStyle.Bold },
			              			VerticalAlignment = VerticalAlignment.Bottom,
			              			HorizontalAlignment = HorizontalAlignment.Center });
			
			Rows.Add(headerRow);
			
			for(int i = 0; i < buyerList.Count; i++)
			{
				ReportRow row = new ReportRow();
				row.Cells.Add(new ReportCell(buyerList[i].Name));
				row.Cells.Add(new ReportCell(buyerList[i].Contact_person));
				row.Cells.Add(new ReportCell(buyerList[i].Phone));
				row.Cells.Add(new ReportCell(buyerList[i].Email));
				row.Cells.Add(new ReportCell(buyerList[i].City));
				
				Rows.Add(row);
			}
			
			SetLandscapeOrientation();
			
			return Create();
		}
	}
}
