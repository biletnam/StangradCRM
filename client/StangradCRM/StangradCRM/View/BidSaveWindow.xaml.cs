/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 16.08.2016
 * Время: 12:15
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Classes;
using StangradCRM.Model;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for BidSaveWindow.xaml
	/// </summary>
	public partial class BidSaveWindow : Window
	{
	
		private Bid bid = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));		
		
		private Buyer currentBuyer = null;
		
		private bool saveEnableFirsrPayment = true;
		private bool isNew = true;
		
		public BidSaveWindow ()
		{
			InitializeComponent();
			setBindings();
			setControlsBehavior();
			dgvEquipmentBid.Visibility = Visibility.Collapsed;
			
			bid = new Bid();
			
			//Set bid data context
			DataContext = new 
			{
				EquipmentBidCollection = bid.EquipmentBidCollection
			};
		}
		
		public BidSaveWindow(Bid bid)
		{
			InitializeComponent();
			setBindings();
			setControlsBehavior();
		
			//Set bid data
			tbxAccount.Text = bid.Account;
			tbxAmount.Text = bid.Amount.ToString();
			dpDateCreated.SelectedDate = bid.Date_created;
			cbxSeller.SelectedItem = SellerViewModel.instance().getById(bid.Id_seller);
			cbxBidStatus.SelectedItem = BidStatusViewModel.instance().getById(bid.Id_bid_status);
			cbxPaymentStatus.SelectedItem  = PaymentStatusViewModel.instance().getById(bid.Id_payment_status);

			//Set buyer data
			currentBuyer = BuyerViewModel.instance().getById(bid.Id_buyer);
			if(currentBuyer != null)
			{
				dlcBuyer.Text = currentBuyer.Name;
				tbxBuyerContactPerson.Text = currentBuyer.Contact_person;
				tbxBuyerPhone.Text = currentBuyer.Phone;
				tbxBuyerEmail.Text = currentBuyer.Email;
				tbxBuyerCity.Text = currentBuyer.City;
			}
			//Set bid data context
			DataContext = new 
			{
				EquipmentBidCollection = bid.EquipmentBidCollection
			};			
			this.bid = bid;
			EditBidInitialize();
		}
		
		//Set collections in controls
		private void setBindings ()
		{
			dlcBuyer.ItemsSource = BuyerViewModel.instance().Collection;
			cbxSeller.ItemsSource = SellerViewModel.instance().Collection;
			cbxBidStatus.ItemsSource = BidStatusViewModel.instance().Collection;
			cbxPaymentStatus.ItemsSource = PaymentStatusViewModel.instance().Collection;
			
			cbxBidStatus.SelectedIndex = 0;
		}
		
		//Behaivior controls
		private void setControlsBehavior ()
		{			
			defaultBrush = tbxAccount.Background;
			dpDateCreated.SelectedDateChanged += delegate { dpDateCreated.Background = defaultBrush; };
			tbxAccount.TextChanged += delegate { tbxAccount.Background = defaultBrush; };
			tbxAmount.TextChanged += delegate 
			{ 
				tbxAmount.Background = defaultBrush;
				tbxFirstPayment.Background = defaultBrush;
			};
			cbxSeller.SelectionChanged += delegate { cbxSeller.Background = defaultBrush; };
			dlcBuyer.getTextBoxItem().TextChanged += delegate { dlcBuyer.Background = defaultBrush; };
			tbxFirstPayment.TextChanged += delegate { tbxFirstPayment.Background = defaultBrush; };
		}		
		
		//If bid edit -> init data
		void EditBidInitialize ()
		{
			dgvEquipmentBid.Visibility = Visibility.Visible;
			Title = "Редактирование заявки №" + bid.Id.ToString();
			isNew = false;
			dpDateCreated.IsEnabled = false;
			Payment payment = PaymentViewModel.instance().getFirstByBidId(bid.Id);
			if(payment != null)
			{
				tbxFirstPayment.Text = payment.Paying.ToString();
				tbxFirstPayment.IsEnabled = false;
				saveEnableFirsrPayment = false;
			}
			tbxDebt.Text = bid.Debt.ToString();

			if(bid.Id_bid_status == (int)Classes.BidStatus.InWork)
			{
				InitializeIfIsWork();
			}	
		}
		
		//if bid status = is_work
		void InitializeIfIsWork ()
		{
			cbxBidStatus.IsEnabled = false;
			Model.PaymentStatus paymentStatus = 
				PaymentStatusViewModel.instance().getById((int)Classes.PaymentStatus.NotPaid);
			if(paymentStatus != null)
			{
				List<Model.PaymentStatus> paymentStatuses
					= PaymentStatusViewModel.instance().Collection.ToList();
				if(paymentStatuses.Contains(paymentStatus))
				{
					paymentStatuses.Remove(paymentStatus);
				}
				cbxPaymentStatus.ItemsSource = paymentStatuses;
			}
			btnIsShipped.Visibility = Visibility.Visible;
			btnIsArchive.Visibility = Visibility.Visible;
			setShippedControlsVisibility();
		}
		
		//Close window
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		//Save bid
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			
			if(currentBuyer == null)
			{
				currentBuyer = new Buyer();
			}
			
			//set buyer data
			currentBuyer.Name = dlcBuyer.Text;
			currentBuyer.Contact_person = tbxBuyerContactPerson.Text;
			currentBuyer.Phone = tbxBuyerPhone.Text;
			currentBuyer.Email = tbxBuyerEmail.Text;
			currentBuyer.City = tbxBuyerCity.Text;
			
			//set bid data
			bid.Date_created = (DateTime)dpDateCreated.SelectedDate.Value;
			bid.Account = tbxAccount.Text;
			bid.Amount = double.Parse(tbxAmount.Text);
			bid.Id_seller = (int)cbxSeller.SelectedValue;
			bid.Id_bid_status = (int)cbxBidStatus.SelectedValue;
			bid.Id_payment_status = (int)cbxPaymentStatus.SelectedValue;
			bid.Id_manager = Auth.getInstance().Id;
			
			//set visual effect
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			//start task
			Task.Factory.StartNew(() => {
				//save buyer
              	if(!currentBuyer.save())
				{
					MessageBox.Show("Не удалось сохранить покупателя.\n" + currentBuyer.LastError);
					return;
				}
              	bid.Id_buyer = currentBuyer.Id; //set buyer id in bid
              	//save bid
              	if(bid.save())
				{
              		//Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { firstPaySave(); } ));
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(); } ));
				}
				else 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(); } ));
				}
			});
		}
		
		
		//Success bid save
		private void successSave()
		{
			if(isNew)
			{
				try
				{
					EditBidInitialize();
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
			else Close();
			dgvEquipmentBid.Visibility = Visibility.Visible;
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
		}
		
		//If save bid error
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			MessageBox.Show("Не удалось сохранить заявку.\n" + bid.LastError);
		}
		
		//Set buyer info
		void DlcBuyer_OnSelect(object sender, StangradCRM.Controls.SelectionChanged e)
		{
			if(e.Value != null)
			{
				Buyer buyer = BuyerViewModel.instance().getById((int)e.Value);
				
				tbxBuyerContactPerson.Text = buyer.Contact_person;
				tbxBuyerPhone.Text = buyer.Phone;
				tbxBuyerEmail.Text = buyer.Email;
				tbxBuyerCity.Text = buyer.City;
				
				currentBuyer = buyer;
			}
		}
		
		//Clear Byer info
		void BtnBuyerClear_Click(object sender, RoutedEventArgs e)
		{
			tbxBuyerContactPerson.Text = "";
			tbxBuyerPhone.Text = "";
			tbxBuyerEmail.Text = "";
			tbxBuyerCity.Text = "";
			dlcBuyer.Clear();
		}
		
		//Validate controls values before save
		private bool validate ()
		{
			if(dpDateCreated.SelectedDate == null)
			{
				dpDateCreated.Background = errorBrush;
				return false;
			}
			if(tbxAccount.Text == "")
			{
				tbxAccount.Background = errorBrush;
				return false;
			}		
			if(tbxAmount.Text == "")
			{
				tbxAmount.Background = errorBrush;
				return false;
			}
			try
			{
				double.Parse(tbxAmount.Text);
			}
			catch
			{
				tbxAmount.Background = errorBrush;
				return false;
			}
			if(cbxSeller.SelectedIndex == -1)
			{
				cbxSeller.Background = errorBrush;
				return false;
			}			
			if(dlcBuyer.Text == "")
			{
				dlcBuyer.Background = errorBrush;
				return false;
			}
			if(cbxBidStatus.SelectedIndex == -1)
			{
				cbxBidStatus.Background = errorBrush;
				return false;
			}
			if(cbxPaymentStatus.SelectedIndex == -1)
			{
				cbxPaymentStatus.Background = errorBrush;
				return false;
			}
			if(tbxFirstPayment.Visibility == Visibility.Visible)
			{
				try
				{
					double firstPayment = double.Parse(tbxFirstPayment.Text);
					if(firstPayment > double.Parse(tbxAmount.Text))
					{
						tbxFirstPayment.Background = errorBrush;
						MessageBox.Show("Значение первого платежа не может быть больше всей суммы!");
						return false;
					}
				}
				catch
				{
					tbxFirstPayment.Text = "0";
					return true;
				}
			}

			return true;
		}

		
		//Save first pay
		private void firstPaySave ()
		{
			if(saveEnableFirsrPayment == false) return;
			if((int)cbxPaymentStatus.SelectedValue ==
			   (int)Classes.PaymentStatus.PartiallyPaid)
			{
				double pay = double.Parse(tbxFirstPayment.Text);
				if(pay == 0) return;
				Payment payment = new Payment();
				payment.Id_bid = bid.Id;
				payment.Id_manager = Auth.getInstance().Id;
				payment.Paying = pay;
				payment.Payment_date = (DateTime)dpDateCreated.SelectedDate;
				if(!payment.save())
				{
					MessageBox.Show("Не удалось сохранить значение первого платежа!\n" + payment.LastError);
				}
			}
		}
		
		//Open payments history window
		void BtnShowPaymentHistory_Click(object sender, RoutedEventArgs e)
		{
			if(bid != null)
			{
				PaymentHistoryWindow window = new PaymentHistoryWindow(bid);
				window.ShowDialog();
			}
		}
		
		//Open set shipped status window
		void BtnIsShipped_Click(object sender, RoutedEventArgs e)
		{
			if(bid != null)
			{
				BidShipmentSaveWindow window = new BidShipmentSaveWindow(bid, new Action( () => { setShippedControlsVisibility(); }));
				window.ShowDialog();
			}
		}
		
		//Set visibility shipped controls
		void setShippedControlsVisibility ()
		{
			if(bid != null && bid.Is_shipped == 1)
			{
				btnIsShipped.Visibility = Visibility.Collapsed;
				lblIsShipped.Visibility = Visibility.Visible;					
			}
		}
		
		//Set archive status
		void BtnIsArchive_Click(object sender, RoutedEventArgs e)
		{
			if(bid.Is_shipped == 1
			   && bid.Id_bid_status == (int)Classes.BidStatus.InWork
			   && bid.Id_payment_status == (int)Classes.PaymentStatus.Paid)
			{
				bid.Is_archive = 1;
				if(!bid.save())
				{
					System.Windows.MessageBox.Show(bid.LastError);
				}
				else
				{
					Close();
				}
			}
			else
			{
				System.Windows.MessageBox.Show("Для перевода заявки в архив должен быть установлен статус 'Отгружено' и 'Оплачено'");
			}
		}
		
		//EquipmentBid -------------------->
		//Open add window
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBidSaveWindow window = new EquipmentBidSaveWindow(bid.Id);
			window.ShowDialog();
		}
		//Open edit window
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			if(equipmentBid == null) return;
			EquipmentBidSaveWindow window = new EquipmentBidSaveWindow(equipmentBid);
			window.ShowDialog();
			
		}		
		//Delete row
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			if(equipmentBid == null) return;
			if(MessageBox.Show("Удалить оборудование из заявки?", "Удалить оборудование из заявки?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
			if(!equipmentBid.remove())
			{
				MessageBox.Show(equipmentBid.LastError);
			}
		}
		
		//Bind bid status and payment status
		//handler Bid status changed
		void CbxBidStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(cbxBidStatus.SelectedIndex == -1 
			   || cbxBidStatus.SelectedValue == null) return;
			
			int bidStatus = (e.AddedItems[0] as Model.BidStatus).Id;
			
			if(cbxPaymentStatus.SelectedIndex == -1
			   || cbxPaymentStatus.SelectedValue == null)
			{
				if(bidStatus == (int)Classes.BidStatus.New)
				{
					cbxPaymentStatus.SelectedValue = (int)Classes.PaymentStatus.NotPaid;
				}
				else
				{
					cbxPaymentStatus.SelectedValue = (int)Classes.PaymentStatus.PartiallyPaid;
				}
				return;
			}
			int paymentStatus = (int)cbxPaymentStatus.SelectedValue;
			if(bidStatus == (int)Classes.BidStatus.New 
			   && paymentStatus != (int)Classes.PaymentStatus.NotPaid)
			{
				cbxPaymentStatus.SelectedValue = (int)Classes.PaymentStatus.NotPaid;
				return;
			}
			if(bidStatus == (int)Classes.BidStatus.InWork 
			   && paymentStatus == (int)Classes.PaymentStatus.NotPaid)
			{
				cbxPaymentStatus.SelectedValue = (int)Classes.PaymentStatus.PartiallyPaid;
				return;
			}
		}
		//handler Payment status change
		void CbxPaymentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(cbxPaymentStatus.SelectedIndex == -1
			  || cbxPaymentStatus.SelectedValue == null) return;
			if(cbxBidStatus.SelectedIndex == -1
			   || cbxBidStatus.SelectedValue == null) return;
			
			int paymentStatus = (e.AddedItems[0] as Model.PaymentStatus).Id;
			int bidStatus = (int)cbxBidStatus.SelectedValue;
			
			if(paymentStatus == (int)Classes.PaymentStatus.NotPaid
			   && bidStatus != (int)Classes.BidStatus.New)
			{
				cbxBidStatus.SelectedValue = (int)Classes.BidStatus.New;
				return;
			}
			if(paymentStatus != (int)Classes.PaymentStatus.NotPaid
			   && bidStatus == (int)Classes.BidStatus.New)
			{
				cbxBidStatus.SelectedValue = (int)Classes.BidStatus.InWork;
				return;
			}
			
		}
	}
}