/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 05.10.2016
 * Время: 9:13
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.Reports
{
	/// <summary>
	/// Description of Indicators.
	/// </summary>
	/// 	
	
	public class Indicators : ExcelReport
	{
		List<Bid> bid;
		List<Bid> archiveBid;
		int year, month;
		public Indicators(List<Bid> bid, int year, int month)
		{
			archiveBid = bid.Where(x => x.Is_archive != 0).ToList();
			this.bid = bid;
			this.year = year;
			this.month = month;
		}
		
		public override bool Save()
		{
			ReportRow row = new ReportRow();
			string headerText = "Показатели за период " + year.ToString() + " г.";
			if(month != 0)
			{
				headerText += ", " + Classes.Months.getRuMonthNameByNumber(month);
			}
			row.Add(new ReportCell(headerText) 
			        {
			        	ColumnSpan=3,
			        	Height=18.75,
			        	Width=18.86,
			        	TextStyle = new List<TextStyle>() { TextStyle.Bold }
			        });
			row.Add(new ReportCell() { Width = 35.43});
			row.Add(new ReportCell() { Width = 15.43});
			row.Add(new ReportCell()
			        { 
			        	Width = 15, 
			        	HorizontalAlignment= HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Bottom
			        });
			Rows.Add(row);
			Rows.Add(new ReportRow());
			
			CreateBidsInfo();
			Rows.Add(new ReportRow());
			SellerDistribution();
			Rows.Add(new ReportRow());
			ManagerSales();
			Rows.Add(new ReportRow());
			TransportCompanyDistribution();
			Rows.Add(new ReportRow());
			IsShipped();
			Rows.Add(new ReportRow());
			BuyerStatistic();
			Rows.Add(new ReportRow());
			MiddlePositionCountInBid();
			Rows.Add(new ReportRow());
			ComplectationStatistic();
			
			AllDocumentFontSize = 14;
			
			return Create();
		}
		
		private void CreateBidsInfo ()
		{
			//Кол-во переданных в архив заявок
			ReportRow rowInArchive = new ReportRow();
			rowInArchive.Add(new ReportCell("Передано заявок в архив:") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			rowInArchive.Add(new ReportCell());
			rowInArchive.Add(new ReportCell(archiveBid.Count.ToString()) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black});
			Rows.Add(rowInArchive);
			
			//Кол-во удаленных заявок
			ReportRow rowDeleted = new ReportRow();
			rowDeleted.Add(new ReportCell("Удалено заявок:") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			rowDeleted.Add(new ReportCell());
			
			int minId = bid.Min(x => x.Id);
			int maxId = bid.Max(x => x.Id);
			int deletedCount = (maxId-minId)-bid.Count;
			
			rowDeleted.Add(new ReportCell((deletedCount).ToString()) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black});
			Rows.Add(rowDeleted);
			
			//Пустая строка
			Rows.Add(new ReportRow());
			
			//Сумма заявок
			ReportRow closedSumRow = new ReportRow();
			closedSumRow.Add(new ReportCell("Закрыто заявок на сумму:") 
			                 {
			                 	ColumnSpan=1, BorderColor = System.Drawing.Color.Black
			                 });
			closedSumRow.Add(new ReportCell());
			double sum = archiveBid.Sum(x => x.Amount);
			closedSumRow.Add(new ReportCell(sum.ToString()) 
			                 {
			                 	ColumnSpan=1,
			                 	BorderColor = System.Drawing.Color.Black
			                 });
			
			Rows.Add(closedSumRow);
			
			//Пустая строка
			Rows.Add(new ReportRow());
			
			//Средняя сумма заявок
			ReportRow midSumRow = new ReportRow();
			midSumRow.Add(new ReportCell("Средняя сумма заявки:") 
			                 {
			                 	ColumnSpan=1, BorderColor = System.Drawing.Color.Black
			                 });
			midSumRow.Add(new ReportCell());
			midSumRow.Add(new ReportCell(((int)(sum/archiveBid.Count)).ToString())
			                 {
			              		ColumnSpan=1,
			                 	BorderColor = System.Drawing.Color.Black
			                 });
			
			Rows.Add(midSumRow);
			
			//Пустая строка
			Rows.Add(new ReportRow());
			
			//Средний счет от срока заявки до отгрузки
			ReportRow midPeriodShipmentRow = new ReportRow();
			midPeriodShipmentRow.Add(new ReportCell("Средний срок от счета до отгрузки:") 
			                 {
			                 	ColumnSpan=1, BorderColor = System.Drawing.Color.Black
			                 });
			midPeriodShipmentRow.Add(new ReportCell());
			int daySum = archiveBid.Sum(x => (((DateTime)x.Shipment_date).Subtract(x.Date_created)).Days);
			midPeriodShipmentRow.Add(new ReportCell((daySum/archiveBid.Count).ToString())
			                 {
	                         	ColumnSpan=1,
			                 	BorderColor = System.Drawing.Color.Black
			                 });
			
			Rows.Add(midPeriodShipmentRow);
			
			//Пустая строка
			Rows.Add(new ReportRow());
		}
		
		//распределение между продавцами
		private void SellerDistribution ()
		{
			Dictionary<int, double> sellerDict 
				= new Dictionary<int, double>();
			for(int i = 0; i < archiveBid.Count; i++)
			{
				if(!sellerDict.ContainsKey(archiveBid[i].Id_seller))
				{
					sellerDict.Add(archiveBid[i].Id_seller, archiveBid[i].Amount );
				}
				else
				{
					sellerDict[archiveBid[i].Id_seller] += archiveBid[i].Amount;
				}
			}
			if(sellerDict.Count == 0) return;
			
			ReportRow rowH = new ReportRow();
			rowH.Add(new ReportCell("Распределение между продавцами:"));
			Rows.Add(rowH);
			
			ReportRow rowT = new ReportRow();
			rowT.Add(new ReportCell("Продавец") { BorderColor = System.Drawing.Color.Black, ColumnSpan = 1 });
			rowT.Add(null);
			rowT.Add(new ReportCell("Сумма") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			Rows.Add(rowT);
			
			foreach(KeyValuePair<int, double> kv in sellerDict.OrderByDescending(x => x.Value))
			{
				Seller seller = SellerViewModel.instance().getById(kv.Key);
				if(seller == null) continue;
				
				ReportRow row = new ReportRow();
				row.Add(new ReportCell(seller.Name) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				row.Add(null);
				row.Add(new ReportCell(kv.Value.ToString()) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				Rows.Add(row);
			}
		}
		
		//продажи менеджеров
		private void ManagerSales ()
		{
			//Словарь сумм по менеджерам
			Dictionary<int, double> managerDict 
				= new Dictionary<int, double>();
			//Словарь количества по менеджерам
			Dictionary<int, int> managerBidsCountDict
				= new Dictionary<int, int>();
			
			for(int i = 0; i < archiveBid.Count; i++)
			{
				//Подсчет суммы заявок менеджера ---->
				if(!managerDict.ContainsKey(archiveBid[i].Id_manager))
				{
					managerDict.Add(archiveBid[i].Id_manager, archiveBid[i].Amount );
				}
				else
				{
					managerDict[archiveBid[i].Id_manager] += archiveBid[i].Amount;
				}
				//<----/
				
				//Подсчет количества заявок менеджера ---->
				if(!managerBidsCountDict.ContainsKey(archiveBid[i].Id_manager))
				{
					managerBidsCountDict.Add(archiveBid[i].Id_manager, 1 );
				}
				else 
				{
					managerBidsCountDict[archiveBid[i].Id_manager]++;
				}
				//<----/
			}
			if(managerDict.Count == 0) return;
			
			ReportRow rowH = new ReportRow();
			rowH.Add(new ReportCell("Продажи менеджеров:"));
			Rows.Add(rowH);
			
			ReportRow rowT = new ReportRow();
			rowT.Add(new ReportCell("Менеджер") { BorderColor = System.Drawing.Color.Black, ColumnSpan = 1 });
			rowT.Add(null);
			rowT.Add(new ReportCell("Сумма") { BorderColor = System.Drawing.Color.Black });
			rowT.Add(new ReportCell("Кол-во") { BorderColor = System.Drawing.Color.Black });			
			Rows.Add(rowT);
			
			foreach(KeyValuePair<int, double> kv in managerDict.OrderByDescending(x => x.Value))
			{
				Manager manager = ManagerViewModel.instance().getById(kv.Key);
				if(manager == null) continue;
				
				ReportRow row = new ReportRow();
				row.Add(new ReportCell(manager.Name) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				row.Add(null);
				row.Add(new ReportCell(kv.Value.ToString()) { BorderColor = System.Drawing.Color.Black });
				int managerBidsCount = 0;
				if(managerBidsCountDict.ContainsKey(kv.Key)) managerBidsCount = managerBidsCountDict[kv.Key];
				row.Add(new ReportCell(managerBidsCount.ToString()) { BorderColor = System.Drawing.Color.Black });
				Rows.Add(row);
			}
		}
		
		//распределние между транспортными компаниями
		private void TransportCompanyDistribution ()
		{
			Dictionary<int, int> transportCompanyDict 
				= new Dictionary<int, int>();
			for(int i = 0; i < archiveBid.Count; i++)
			{
				if(archiveBid[i].Id_transport_company == null) continue;
				
				if(!transportCompanyDict.ContainsKey((int)archiveBid[i].Id_transport_company))
				{
					transportCompanyDict.Add((int)archiveBid[i].Id_transport_company, 1);
				}
				else
				{
					transportCompanyDict[(int)archiveBid[i].Id_transport_company]++;
				}
			}
			if(transportCompanyDict.Count == 0) return;
			
			ReportRow rowH = new ReportRow();
			rowH.Add(new ReportCell("Выбор транспортной компании:"));
			Rows.Add(rowH);
			
			ReportRow rowT = new ReportRow();
			rowT.Add(new ReportCell("Наименование") { BorderColor = System.Drawing.Color.Black, ColumnSpan = 1 });
			rowT.Add(null);
			rowT.Add(new ReportCell("Кол-во раз") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			Rows.Add(rowT);
			
			foreach(KeyValuePair<int, int> kv in transportCompanyDict.OrderByDescending(x => x.Value))
			{
				TransportCompany transportCompany = TransportCompanyViewModel.instance().getById(kv.Key);
				if(transportCompany == null) continue;
				
				ReportRow row = new ReportRow();
				row.Add(new ReportCell(transportCompany.Name) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				row.Add(null);
				row.Add(new ReportCell(kv.Value.ToString()) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				Rows.Add(row);
			}
		}
		
		//Отгружено
		private void IsShipped ()
		{
			List<EquipmentBid> equipmentBids = new List<EquipmentBid>();			
			for(int i = 0; i < archiveBid.Count; i++)
			{
				equipmentBids.AddRange(archiveBid[i].EquipmentBidCollection.ToList());
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
			if(equipmentBids.Count == 0) return;
			
			ReportRow rowH = new ReportRow();
			rowH.Add(new ReportCell("Отгружено:"));
			Rows.Add(rowH);
			
			ReportRow rowT = new ReportRow();
			rowT.Add(new ReportCell("Оборудование") { BorderColor = System.Drawing.Color.Black});
			rowT.Add(new ReportCell("Модификация") { BorderColor = System.Drawing.Color.Black });
			rowT.Add(new ReportCell("Количество") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			Rows.Add(rowT);
			
			ebGroup = ebGroup.OrderByDescending(x => x.Count).ToList();
			
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
				        	 ColumnSpan=1, 
				        	VerticalAlignment = VerticalAlignment.Bottom,
				        	BorderColor = System.Drawing.Color.Black
				        });
				Rows.Add(row);
			}
			
		}
		
		//Статистика по клиентам
		private void BuyerStatistic()
		{
			Dictionary<int, int> buyerDict 
				= new Dictionary<int, int>();
			for(int i = 0; i < archiveBid.Count; i++)
			{
				if(!buyerDict.ContainsKey(archiveBid[i].Id_buyer))
				{
					buyerDict.Add(archiveBid[i].Id_buyer, 1);
				}
				else
				{
					buyerDict[archiveBid[i].Id_buyer]++;
				}
			}
			if(buyerDict.Count == 0) return;
			
			ReportRow rowH = new ReportRow();
			rowH.Add(new ReportCell("Статистика по клиентам:"));
			Rows.Add(rowH);
			
			ReportRow rowT = new ReportRow();
			rowT.Add(new ReportCell("Покупатель") { BorderColor = System.Drawing.Color.Black, ColumnSpan = 1 });
			rowT.Add(null);
			rowT.Add(new ReportCell("Кол-во заявок") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			Rows.Add(rowT);
			
			foreach(KeyValuePair<int, int> kv in buyerDict.OrderByDescending(x => x.Value))
			{
				Buyer buyer = BuyerViewModel.instance().getById(kv.Key);
				if(buyer == null) continue;
				
				ReportRow row = new ReportRow();
				row.Add(new ReportCell(buyer.Name) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				row.Add(null);
				row.Add(new ReportCell(kv.Value.ToString()) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				Rows.Add(row);
			}
		}
		
		//Среднее количество позиций в заявке
		private void MiddlePositionCountInBid ()
		{
			int equipmentBidCount = archiveBid.Sum(x => x.EquipmentBidCollection.Count);
			//Кол-во переданных в архив заявок
			ReportRow row = new ReportRow();
			row.Add(new ReportCell("Среднее количество позиций в заявке:") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			row.Add(new ReportCell());
			row.Add(new ReportCell((equipmentBidCount/archiveBid.Count).ToString()) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black});
			Rows.Add(row);
		}
		
		//Статистика комплектаций
		private void ComplectationStatistic ()
		{
			Dictionary<int, int> complectationDict 
				= new Dictionary<int, int>();
			for(int i = 0; i < archiveBid.Count; i++)
			{
				for(int j = 0; j < archiveBid[i].EquipmentBidCollection.Count; j++)
				{
					for(int k = 0; k < archiveBid[i].EquipmentBidCollection[j].ComplectationCollection.Count; k++)
					{
						Complectation complectation = archiveBid[i].EquipmentBidCollection[j].ComplectationCollection[k];
						if(!complectationDict.ContainsKey(complectation.Id_complectation_item))
						{
							complectationDict.Add(complectation.Id_complectation_item, 1);
						}
						else
						{
							complectationDict[complectation.Id_complectation_item] ++;
						}
					}
				}
			}
			if(complectationDict.Count == 0) return;
			
			ReportRow rowH = new ReportRow();
			rowH.Add(new ReportCell("Статистика комплектаций:"));
			Rows.Add(rowH);
			
			ReportRow rowT = new ReportRow();
			rowT.Add(new ReportCell("Наименование") { BorderColor = System.Drawing.Color.Black, ColumnSpan = 1 });
			rowT.Add(null);
			rowT.Add(new ReportCell("Кол-во комплектаций") { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
			Rows.Add(rowT);
			
			foreach(KeyValuePair<int, int> kv in complectationDict.OrderByDescending(x => x.Value))
			{
				ComplectationItem citem = ComplectationItemViewModel.instance().getById(kv.Key);
				if(citem == null) continue;
				
				ReportRow row = new ReportRow();
				row.Add(new ReportCell(citem.Name) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				row.Add(null);
				row.Add(new ReportCell(kv.Value.ToString()) { ColumnSpan=1, BorderColor = System.Drawing.Color.Black });
				Rows.Add(row);
			}
		}
	}
}
