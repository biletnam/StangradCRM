/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 18:52
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
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
	public class Bid : Core.Model
	{	
		public int Id_seller { get; set; }
		public int Id_buyer { get; set; }
		public int Id_bid_status { get; set; }
		public int Id_payment_status { get; set; }
		public int? Id_transport_company { get; set; }
		public int? Id_manager { get; set; }
		public DateTime Date_created { get; set;	}
		public DateTime? Shipment_date { get; set; }
		public string Account { get; set; }
		public double Amount { get; set; }
		public int Is_archive { get; set; }
		public int Is_shipped { get; set; }
				
		public DateTime Last_modified { get; set; }
		
		//-------------------->
		
		public TSObservableCollection<EquipmentBid> EquipmentBid
		{
			set
			{
				TSObservableCollection<EquipmentBid> equipmentBid = value;
				EquipmentBidViewModel eb_vm = EquipmentBidViewModel.instance();
				for(int i = 0; i < equipmentBid.Count; i++)
				{
					if(eb_vm.getById(equipmentBid[i].Id) == null)
					{
						eb_vm.add(equipmentBid[i]);
					}
				}
			}
		}
		
		public TSObservableCollection<Payment> Payment
		{
			set
			{
				TSObservableCollection<Payment> payment = value;
				PaymentViewModel p_vm = PaymentViewModel.instance();
				for(int i = 0; i < payment.Count; i++)
				{
					if(p_vm.getById(payment[i].Id) == null)
					{
						p_vm.add(payment[i]);
					}
				}
			}
		}
		
		//--------------------<
		
		public Classes.PaymentStatus CurrentPaymentStatus
		{
			get 
			{
				return (Classes.PaymentStatus)Id_payment_status;
			}
		}
		
		public string BuyerName
		{
			get
			{
				Buyer buyer = BuyerViewModel.instance().getById(Id_buyer);
				if(buyer != null)
				{
					return buyer.Name;
				}
				return "<Не выбрано>";
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
				if(Id_manager == null)
				{
					return "<Не назначен>";
				}
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
		
		public string ShippedYesNo
		{
			get
			{
				if(Is_shipped == 0)
				{
					return "Нет";
				}
				return "Да";
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
		
		public string EquipmentBidStringSearch
		{
			get
			{
				string resultString = "";
				foreach(EquipmentBid equipmentBid in EquipmentBidCollection)
				{
					resultString += equipmentBid.EquipmentName + " " 
						+ equipmentBid.ModificationName + " " 
						+ equipmentBid.Serial_number.ToString() + " "
						+ equipmentBid.ComplectationStringSearch;
				}
				return resultString;
			}
		}
		
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
		
		public Bid() 
		{
			
		}
		
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
			if(Id_manager != null)
			{
				http.addParameter("id_manager", (int)Id_manager);
			}
			http.addParameter("account", Account);
			http.addParameter("amount", Amount);
			http.addParameter("date_created", Date_created.ToString("yyyy-MM-dd"));
			http.addParameter("is_archive", Is_archive);
			http.addParameter("is_shipped", Is_shipped);
			
			if(Id != 0)
			{
				http.addParameter("id", Id);
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
				/*int oldPaymentStatus;
				try
				{
					oldPaymentStatus = (int)oldValues["Id_payment_status"];
					if(oldPaymentStatus == (int)Classes.PaymentStatus.NotPaid 
					   && Id_payment_status != (int)Classes.PaymentStatus.NotPaid)
					{
						Id_bid_status = (int)Classes.BidStatus.InWork;
						if(!save())
						{
							return false;
						}
					}
				}
				catch {};
				
				try
				{
					if((int)oldValues["Id_bid_status"] == (int)Classes.BidStatus.New
					   && Id_bid_status == (int)Classes.BidStatus.InWork)
					{
						equipmentBidCollection.ToList().ForEach(x => x.generateSerialNumber());
					}
				}
				catch {}*/
				
				UpdateAllProperties();
			}
			return result;
		}
		
		protected override bool afterRemove(ResponseParser parser, bool soft = false)
		{
			bool result = base.afterRemove(parser);
			if(result)
			{
				EquipmentBidCollection.ToList().ForEach(x => {x.remove(true);});
			}
			return result;
		}
		
		public void UpdateAllProperties()
		{
			if(oldValues.ContainsKey("Id_bid_status") &&
			   oldValues["Id_bid_status"] != null)
			{
				BidViewModel.instance().updateStatus(this, (int)oldValues["Id_bid_status"]);
			}
			
			if(Is_archive != 0)
			{
				BidViewModel.instance().MoveToArchive(this);
			}
			
			RaisePropertyChanged("Id_seller", Id_seller);
			RaisePropertyChanged("SellerName", null);
			RaisePropertyChanged("Id_buyer", Id_buyer);
			RaisePropertyChanged("BuyerName", null);
			RaisePropertyChanged("Id_bid_status", Id_bid_status);
			RaisePropertyChanged("BidStatusName", null);
			RaisePropertyChanged("BidStatusColor", null);
			RaisePropertyChanged("Id_payment_status", Id_payment_status);
			RaisePropertyChanged("PaymentStatusName", null);
			RaisePropertyChanged("PaymentStatusColor", null);
			RaisePropertyChanged("Id_transport_company", Id_transport_company);
			RaisePropertyChanged("Id_manager", Id_manager);
			RaisePropertyChanged("ManagerName", null);
			RaisePropertyChanged("Date_created", Date_created);
			RaisePropertyChanged("Shipment_date", Shipment_date);
			RaisePropertyChanged("Account", Account);
			RaisePropertyChanged("Amount", Amount);
			RaisePropertyChanged("Is_archive", Is_archive);
			RaisePropertyChanged("Is_shipped", Is_shipped);
			RaisePropertyChanged("Debt", null);
			RaisePropertyChanged("ShippedYesNo", null);
		}
		
		public override void loadedItemInitProperty ()
		{
			RaisePropertyChanged("Id_payment_status", Id_payment_status, true);
			RaisePropertyChanged("Id_bid_status", Id_bid_status, true);
		}		
	}
}
