﻿/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 18:52
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using StangradCRM.Core;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of Bid.
	/// </summary>
	/// 
	
	public class TempEquipmentBid
	{
		public int Id { get; set; }
		public int SerialNumber { get; set; }
	}
	
	public class Bid : Core.Model
	{	
		public int Id_seller { get; set; }
		public int Id_buyer { get; set; }
		public int Id_bid_status { get; set; }
		public int Id_payment_status { get; set; }
		public int? Id_transport_company { get; set; }
		public int Id_manager { get; set; }
		public DateTime Date_created { get; set;	}
		public DateTime? Shipment_date { get; set; }
		public DateTime? Planned_shipment_date { get; set; }
		public string Waybill { get; set; }
		public string Account { get; set; }
		public double Amount { get; set; }
		public int Is_archive { get; set; }
		public int Is_shipped { get; set; }
		public string Comment { get; set; }
		public int? Logistician_status { get; set; }
		
		public string Guid { get; set; }
		
		
		//-------------------->
		
		public TSObservableCollection<EquipmentBid> EquipmentBid { 
			set
			{
				equipmentBidCollection = null;
				EquipmentBidViewModel eb_vm = EquipmentBidViewModel.instance();
				for(int i = 0; i < value.Count; i++)
				{
					EquipmentBid equipmentBid = eb_vm.getById(value[i].Id);
					if(equipmentBid == null)
					{
						eb_vm.add(value[i]);
						value[i].loadedItemInitProperty();
					}
					else
					{
						equipmentBid.replace(value[i]);
					}
				}
			}
		}
		
		public TSObservableCollection<Payment> Payment 
		{
			set
			{
				PaymentViewModel p_vm = PaymentViewModel.instance();
				for(int i = 0; i < value.Count; i++)
				{
					Payment payment = p_vm.getById(value[i].Id);
					if(payment == null)
					{
						p_vm.add(value[i]);
					}
					else
					{
						payment.replace(value[i]);
					}
				}				
			}
		}
		
		//--------------------<z
		
		public Classes.PaymentStatus CurrentPaymentStatus
		{
			get 
			{
				return (Classes.PaymentStatus)Id_payment_status;
			}
		}
		
		public Buyer BidBuyer 
		{
			get
			{
				return BuyerViewModel.instance().getById(Id_buyer);
			}
		}
		
		public string BuyerName
		{
			get
			{
				if(BidBuyer != null)
				{
					return BidBuyer.Name;
				}
				return "<Не выбрано>";
			}
		}
		
		public string BuyerInfo
		{
			get
			{
				if(BidBuyer != null)
				{
					return BidBuyer.BuyerInfo;
				}
				return "";
			}
		}
		
		public string SellerName
		{
			get
			{
				Seller seller = SellerViewModel.instance().getById(Id_seller);
				if(seller != null)
				{
					return seller.Name;
				}
				return "<Не выбрано>";
			}
		}		
		
		public string BidStatusName
		{
			get
			{
				BidStatus bidStatus = BidStatusViewModel.instance().getById(Id_bid_status);
				if(bidStatus != null)
				{
					return bidStatus.Name;
				}
				return "<Не выбрано>";
			}
		}
		
		public string BidStatusColor
		{
			get
			{
				BidStatus bidStatus = BidStatusViewModel.instance().getById(Id_bid_status);
				if(bidStatus != null)
				{
					return bidStatus.Record_color;
				}
				return "#ffffff";
			}
		}
		
		public string PaymentStatusName
		{
			get
			{
				PaymentStatus paymentStatus = PaymentStatusViewModel.instance().getById(Id_payment_status);
				if(paymentStatus != null)
				{
					return paymentStatus.Name;
				}
				return "<Не выбрано>";
			}
		}
		
		public string PaymentStatusColor
		{
			get
			{
				PaymentStatus paymentStatus = PaymentStatusViewModel.instance().getById(Id_payment_status);
				if(paymentStatus != null)
				{
					return paymentStatus.Record_color;
				}
				return "#ffffff";
			}
		}
		
		public string ManagerName
		{
			get
			{
				Manager manager = ManagerViewModel.instance().getById((int)Id_manager);
				if(manager != null)
				{
					return manager.Name;
				}
				return "<Не назначен>";
			}
		}
		
		public double Debt 
		{
			get
			{
				return (Amount - PaymentViewModel.instance().getDebtByBidId(Id));
			}
		}
		
		public string TransportCompanyName
		{
			get
			{
				if(Id_transport_company != null
				   && Id_transport_company != 0)
				{
					TransportCompany transportCompany =
						TransportCompanyViewModel.instance().getById((int)Id_transport_company);
					if(transportCompany == null)
					{
						return "";
					}
					return transportCompany.Name;
				}
				return "";
			}
		}
		
		public string TransportCompanyNameWithWaybill
		{
			get
			{
				if(Id_transport_company != null
				   && Id_transport_company != 0)
				{
					TransportCompany transportCompany =
						TransportCompanyViewModel.instance().getById((int)Id_transport_company);
					if(transportCompany == null)
					{
						return "";
					}
					return transportCompany.Name + " " + Waybill;
				}
				return "";
			}
		}
		
		public bool IsDelayShipment
		{
			get
			{
				int dayCount = 0;
				CRMSetting crmSetting = CRMSettingViewModel.instance().getByMashineName("warning_shipment_date_day_count");
				if(crmSetting != null)
				{
					try
					{
						dayCount = int.Parse(crmSetting.Setting_value);
					}
					catch {}
				}
				
				if(Planned_shipment_date != null 
				   && ((DateTime)Planned_shipment_date).AddDays(-dayCount) < DateTime.Now
					&& Is_shipped == 0)
				{
					return true;
				}
				return false;
			}
		}
		
		public bool IsComment
		{
			get {
				if(Comment.Length > 0) return true;
				return false;
			}
		}
		
		public string IsCommentText 
		{
			get
			{
				if(IsComment) return "Да";
				return "Нет";
			}
		}
		
		private TSObservableCollection<EquipmentBid> equipmentBidCollection = null;
		public TSObservableCollection<EquipmentBid> EquipmentBidCollection
		{
			get
			{
				if(equipmentBidCollection == null)
				{
					equipmentBidCollection = EquipmentBidViewModel.instance().getByBidId(Id);
				}
				return equipmentBidCollection;
			}
		}
		
		
		private TSObservableCollection<BidFiles> bidFilesCollection = null;
		public TSObservableCollection<BidFiles> BidFilesCollection
		{
			get 
			{
				if(bidFilesCollection == null)
				{
					bidFilesCollection = BidFilesViewModel.instance().getByBidId(Id);
				}
				return bidFilesCollection;
			}
		}
	
 		
		//For search ---->
		
		public string EquipmentBidStringSearch
		{
			get
			{
				string resultString = "";
				foreach(EquipmentBid equipmentBid in EquipmentBidCollection)
				{
					resultString += equipmentBid.Id.ToString() + " "
						+ equipmentBid.Id_equipment.ToString() + " "
						+ equipmentBid.EquipmentName + " "
						+ equipmentBid.ModificationName + " " 
						+ equipmentBid.Serial_number.ToString() + " "
						+ equipmentBid.ComplectationStringSearch;
				}
				return resultString;
			}
		}
		
		//<----
		
		public TSObservableCollection<Payment> PaymentCollection
		{
			get
			{
				return PaymentViewModel.instance().getByBidId(Id);
			}
		}
		
		public bool PermittedRemoval
		{
			get
			{
				if(Id_bid_status != (int)Classes.BidStatus.New)
				{
					return false;
				}
				CRMSetting setting = CRMSettingViewModel.instance().getByMashineName("new_bid_remove_day_count");
				if(setting == null)
				{
					return false;
				}
				int dayCount = 0;
				try
				{
					dayCount = int.Parse(setting.Setting_value);
					if(Date_created.AddDays(dayCount) <= DateTime.Now)
					{
						return true;
					}
					return false;
				}
				catch
				{
					return false;
				}
			}
		}
		
		public bool IsDuplicate
		{
			get
			{
				return BidViewModel.instance().IsDuplicate(this);
			}
		}
		
		public bool IsShipped 
		{
			get
			{
				return Is_shipped == 1;
			}
		}
		
		public bool IsLogisticianInWork
		{
			get
			{
				return Logistician_status == 1;
			}
		}
		
		public bool IsLogisticianShpped
		{
			get
			{
				return Logistician_status == 2;
			}
		}
		
		public Bid() {}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{			
			http.addParameter("id_seller", Id_seller);			
			http.addParameter("id_buyer", Id_buyer);
			http.addParameter("id_bid_status", Id_bid_status);
			http.addParameter("id_payment_status", Id_payment_status);
			if(Id_transport_company != null)
			{
				http.addParameter("id_transport_company", (int)Id_transport_company);
			}

			http.addParameter("id_manager", (int)Id_manager);
			if(Shipment_date != null)
			{
				http.addParameter("shipment_date", ((DateTime)Shipment_date).ToString("yyyy-MM-dd"));
			}
			if(Planned_shipment_date != null)
			{
				http.addParameter("planned_shipment_date", ((DateTime)Planned_shipment_date).ToString("yyyy-MM-dd"));
			}
			http.addParameter("account", Account);
			http.addParameter("amount", Amount.ToString().Replace(',', '.'));
			http.addParameter("date_created", Date_created.ToString("yyyy-MM-dd"));
			http.addParameter("is_archive", Is_archive);
			http.addParameter("is_shipped", Is_shipped);
			if(Guid != null)
			{
				http.addParameter("guid", Guid);
			}
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
			if(Comment != null)
			{
				http.addParameter("comment", Comment);
			}
			if(Waybill != null) 
			{
				http.addParameter("waybill", Waybill);
			}
			if(Logistician_status == 1 || Logistician_status == 2)
			{
				http.addParameter("logistician_status", Logistician_status);
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
				return "Bid";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return BidViewModel.instance();
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
		
		
		public bool generateSerialNumber ()
		{
			try {
				HTTPManager.HTTPRequest http = HTTPManager.HTTPRequest.Create(Settings.uri);
				http.UseCookie = true;
				http.addParameter("entity", "Bid/generateEquipmentBidSerialNumber");
				http.addParameter("id", Id);
				if(!http.post()) 
				{
					LastError = HTTPManager.HTTPRequest.LastError;
					return false;
				}
				ResponseParser parser = ResponseParser.Parse(http.ResponseData);
				if(!parser.NoError)
				{
					LastError = parser.LastError;
					return false;
				}
				if(parser.ServerErrorFlag != 0)
				{
					LastError = parser.ToObject<string>();
					return false;
				}
				List<TempEquipmentBid> updatedEquipmentBid = parser.ToObject<List<TempEquipmentBid>>();
				for(int i = 0; i < updatedEquipmentBid.Count; i++)
				{
					EquipmentBid equipmentBid = EquipmentBidViewModel.instance().getById(updatedEquipmentBid[i].Id);
					if(equipmentBid == null) continue;
					equipmentBid.setSerialNumber(updatedEquipmentBid[i].SerialNumber);
				}
				return true;
			}
			catch(Exception ex)
			{
				LastError = ex.ToString();
				return false;
			}
		}
		
		protected override bool afterRemove(ResponseParser parser, bool soft = false)
		{
			bool result = base.afterRemove(parser, soft);
			if(result)
			{
				EquipmentBidCollection.ToList().ForEach(x => {x.remove(true);});
				BidFilesCollection.ToList().ForEach(x => {x.remove(true);});
				
				PaymentCollection.ToList().ForEach(x => {x.remove(true);});
				
				equipmentBidCollection = null;
				bidFilesCollection = null;
			}
			return result;
		}
		
		public override void raiseAllProperties()
		{
			if(oldValues.ContainsKey("Id_bid_status") &&
			   (oldValues["Id_bid_status"] as int?) != null &&
			   Id_bid_status != (int)oldValues["Id_bid_status"])
			{
				BidViewModel.instance().UpdateStatus(this, (int)oldValues["Id_bid_status"]);
			}
			
			if(oldValues.ContainsKey("Is_archive") &&
			   (oldValues["Is_archive"] as int?) != null &&
			   Is_archive != (int)oldValues["Is_archive"])
			{
				BidViewModel.instance().UpdateArchive(this);
			}
			
			RaisePropertyChanged("Id_seller", Id_seller);
			RaisePropertyChanged("Id_buyer", Id_buyer);
			RaisePropertyChanged("Id_bid_status", Id_bid_status);
			RaisePropertyChanged("Id_payment_status", Id_payment_status);
			RaisePropertyChanged("Id_transport_company", Id_transport_company);
			RaisePropertyChanged("Id_manager", Id_manager);
			RaisePropertyChanged("Date_created", Date_created);
			RaisePropertyChanged("Shipment_date", Shipment_date);
			RaisePropertyChanged("Account", Account);
			RaisePropertyChanged("Amount", Amount);
			RaisePropertyChanged("Is_archive", Is_archive);
			RaisePropertyChanged("Is_shipped", Is_shipped);
			RaisePropertyChanged("Comment", Comment);
			RaisePropertyChanged("Waybill", Waybill);
			RaisePropertyChanged("Logistician_status", Logistician_status);
			
			
			RaisePropertyChanged("SellerName", null);			
			RaisePropertyChanged("BuyerName", null);
			RaisePropertyChanged("PaymentStatusName", null);
			RaisePropertyChanged("PaymentStatusColor", null);
			RaisePropertyChanged("BidStatusName", null);
			RaisePropertyChanged("BidStatusColor", null);
			RaisePropertyChanged("ManagerName", null);
			RaisePropertyChanged("Debt", null);
			RaisePropertyChanged("TransportCompanyName", null);
			RaisePropertyChanged("TransportCompanyNameWithWaybill", null);
			RaisePropertyChanged("Planned_shipment_date", null);
			RaisePropertyChanged("IsDelayShipment", null);
			RaisePropertyChanged("IsComment", null);
			RaisePropertyChanged("IsCommentText", null);
			
			RaisePropertyChanged("PaymentCollection", null);
		}
		
		public override void loadedItemInitProperty ()
		{
			oldValues["Id_payment_status"] = Id_payment_status;
			oldValues["Id_bid_status"] = Id_bid_status;
			oldValues["Is_archive"] = Is_archive;
		}
		
		public override void replace(object o)
		{
			Bid bid = o as Bid;
			if(bid == null) return;
			
			Id_seller = bid.Id_seller;
			Id_buyer = bid.Id_buyer;
			Id_bid_status = bid.Id_bid_status;
			Id_payment_status = bid.Id_payment_status;
			Id_transport_company = bid.Id_transport_company;
			Id_manager = bid.Id_manager;
			Date_created = bid.Date_created;
			Shipment_date = bid.Shipment_date;
			Planned_shipment_date = bid.Planned_shipment_date;
			Account = bid.Account;
			Amount = bid.Amount;
			Is_archive = bid.Is_archive;
			Is_shipped = bid.Is_shipped;
			Waybill = bid.Waybill;
			Logistician_status = bid.Logistician_status;
			
			raiseAllProperties();
		}
	}
}
