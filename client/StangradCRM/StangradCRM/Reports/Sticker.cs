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

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of Sticker.
	/// </summary>
	public class Sticker : Core.ExcelReport
	{
		EquipmentBid equipmentBid = null;
		public Sticker(EquipmentBid equipmentBid)
		{
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
			
			ReportRow row = new ReportRow();
			ReportCell cell = new ReportCell(buyer.Name);
			row.Cells.Add(cell);
			Rows.Add(row);
			
			return Create();
		}
	}
}
