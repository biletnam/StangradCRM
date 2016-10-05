/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.09.2016
 * Время: 13:24
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of ShipmentPeriod.
	/// </summary>
	/// 
	
	public class EquipmentBidGroup
	{
		public EquipmentBid EquipmentBid { get; set; }
		public int Count { get; set; }
		public int IdEquipment 
		{
			get
			{
				return this.EquipmentBid.Id_equipment;
			}
		}
		public int? IdModification 
		{
			get
			{
				return this.EquipmentBid.Id_modification;
			}
		}
	}
	
	public class ShipmentPeriod : ExcelReport
	{
		List<Bid> bids;
		DateTime start, end;
		public ShipmentPeriod(List<Bid> bids, DateTime start, DateTime end)
		{
			this.bids = bids;
			this.start = start;
			this.end = end;
		}
		
		public override bool Save()
		{
			ReportRow headerRow = new ReportRow();
			headerRow.Add(new ReportCell("Отгружено оборудования c " + start.ToString("dd.MM.yyyy") + " по " + end.ToString("dd.MM.yyyy")) { ColumnSpan = 2 });
			headerRow.Add(new ReportCell());
			headerRow.Add(new ReportCell() { HorizontalAlignment = HorizontalAlignment.Center });
			Rows.Add(headerRow);
			
			ReportRow titleRow = new ReportRow();
			titleRow.Add(new ReportCell("Оборудование") { Width = 20.86} );
			titleRow.Add(new ReportCell("Модификация") { Width = 21.71} );
			titleRow.Add(new ReportCell("Количество") { Width = 18.71} );
			Rows.Add(titleRow);
			
			PrepareEquipmentBid();
			
			return Create();
		}
		
		private void PrepareEquipmentBid ()
		{
			List<EquipmentBid> equipmentBids = new List<EquipmentBid>();			
			for(int i = 0; i < bids.Count; i++)
			{
				equipmentBids.AddRange(bids[i].EquipmentBidCollection.ToList());
			}
			List<EquipmentBidGroup> ebGroup = new List<EquipmentBidGroup>();
			for(int i = 0; i < equipmentBids.Count; i++)
			{
				EquipmentBidGroup equipmentBidGroup = ebGroup.Where(x => (x.IdEquipment == equipmentBids[i].Id_equipment)
				              && (x.IdModification == equipmentBids[i].Id_modification)).FirstOrDefault();
				if(equipmentBidGroup == null)
				{
					ebGroup.Add(new EquipmentBidGroup() { EquipmentBid = equipmentBids[i], Count = 1 });
				}
				else
				{
					equipmentBidGroup.Count++;
				}
			}
			CreateRows(ebGroup);
		}
		
		private void CreateRows(List<EquipmentBidGroup> ebGroup)
		{
			for(int i = 0; i < ebGroup.Count; i++)
			{
				EquipmentBid equipmentBid = ebGroup[i].EquipmentBid;
				ReportRow row = new ReportRow();
				row.Add(new ReportCell(equipmentBid.EquipmentName) 
				        {
				        	VerticalAlignment = VerticalAlignment.Bottom,
				        	BorderColor = System.Drawing.Color.Black
				        });
				string modificationName = "";
				if(equipmentBid.Id_modification != null)
				{
					modificationName= equipmentBid.ModificationName;
				}
				row.Add(new ReportCell(modificationName) 
				        {
				        	VerticalAlignment = VerticalAlignment.Bottom,
				        	BorderColor = System.Drawing.Color.Black
				        });
				row.Add(new ReportCell(ebGroup[i].Count.ToString())
				        {
				        	VerticalAlignment = VerticalAlignment.Bottom,
				        	BorderColor = System.Drawing.Color.Black
				        });
				Rows.Add(row);
			}
		}
		
	}
}
