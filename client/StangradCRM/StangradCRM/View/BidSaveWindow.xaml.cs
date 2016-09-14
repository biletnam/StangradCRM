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
		
		private bool isNew = true;
		
		private Action saveCallback = null;
		
		public BidSaveWindow ()
		{
			InitializeComponent();
			setBindings();
			setControlsBehavior();
			gbxEquipmentBid.Visibility = Visibility.Collapsed;
			
			bid = new Bid();
			
			//Set bid data context
			DataContext = new 
			{
				EquipmentBidCollection = bid.EquipmentBidCollection,
				BuyerCollection = BuyerViewModel.instance().Collection,
				BID = bid
			};
		}
		
		public BidSaveWindow(Bid bid, Action saveCallback = null)
		{
			InitializeComponent();
			setBindings();
			setControlsBehavior();
		
			//Set bid data
			tbxAccount.Text = bid.Account;
			tbxAmount.Text = bid.Amount.ToString();
			dpDateCreated.SelectedDate = bid.Date_created;
			cbxSeller.SelectedItem = SellerViewModel.instance().getById(bid.Id_seller);

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
				EquipmentBidCollection = bid.EquipmentBidCollection,
				BuyerCollection = BuyerViewModel.instance().Collection,
				BID = bid
			};			
			this.bid = bid;
			
			this.saveCallback = saveCallback;
			
			EditBidInitialize();
		}
		
		//Set collections in controls
		private void setBindings ()
		{
			cbxSeller.ItemsSource = SellerViewModel.instance().Collection;
		}
		
		//Behaivior controls
		private void setControlsBehavior ()
		{			
			defaultBrush = tbxAccount.Background;
			dpDateCreated.SelectedDateChanged += delegate { dpDateCreated.Background = defaultBrush; };
			tbxAccount.TextChanged += delegate { tbxAccount.Background = defaultBrush; };
			tbxAmount.TextChanged += delegate { tbxAmount.Background = defaultBrush;};
			cbxSeller.SelectionChanged += delegate { cbxSeller.Background = defaultBrush; };
			dlcBuyer.getTextBoxItem().TextChanged += delegate 
			{
				if(dlcBuyer.Background == errorBrush)
					dlcBuyer.Background = defaultBrush; 
			};
		}		
		
		//If bid edit -> init data
		void EditBidInitialize ()
		{
			gbxEquipmentBid.Visibility = Visibility.Visible;
			Title = "Редактирование заявки №" + bid.Id.ToString();
			isNew = false;

			tbxDebt.Text = bid.Debt.ToString();
			if(bid.Id_bid_status == (int)Classes.BidStatus.InWork)
			{
				InitializeIfIsWork();
			}
			SetReadOnlyBuyersControls(true);
		}
		
		//if bid status = is_work
		void InitializeIfIsWork ()
		{
			btnIsShipped.Visibility = Visibility.Visible;
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
			bid.Id_manager = Auth.getInstance().Id;
			
			//Если новая заявка - устанавливаем статусы "новая" и  "неоплачено"
			if(bid.Id == 0)
			{
				bid.Id_bid_status = (int)Classes.BidStatus.New;
				bid.Id_payment_status = (int)Classes.PaymentStatus.NotPaid;
			}
			
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
			if(saveCallback != null) saveCallback();
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
				
				SetReadOnlyBuyersControls(true);
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
			
			currentBuyer = null;
			
			SetReadOnlyBuyersControls();
		}
		
		//Set enabled/disabled buyer controls
		void SetReadOnlyBuyersControls (bool isReadOnly = false)
		{
			dlcBuyer.IsReadOnly = isReadOnly;
			tbxBuyerContactPerson.IsReadOnly = isReadOnly;
			tbxBuyerPhone.IsReadOnly = isReadOnly;
			tbxBuyerEmail.IsReadOnly = isReadOnly;
			tbxBuyerCity.IsReadOnly = isReadOnly;
			if(!isReadOnly)
			{
				SetBuyerControlsColor();
			}
			else
			{
				SetBuyerControlsColor(new SolidColorBrush(Color.FromRgb(240, 240, 240)));
			}
		}
		
		//Set buyer conrols color
		void SetBuyerControlsColor (Brush brush = null)
		{
			if(brush == null) brush = defaultBrush;
			
			
			
			dlcBuyer.Background = brush;
			tbxBuyerContactPerson.Background = brush;
			tbxBuyerPhone.Background = brush;
			tbxBuyerEmail.Background = brush;
			tbxBuyerCity.Background = brush;
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
				double amount = double.Parse(tbxAmount.Text);
				if(amount < 0)
				{
					tbxAmount.Background = errorBrush;
					MessageBox.Show("Значение суммы должно быть больше 0!");
					return false;
				}
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
			return true;
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
				BidShipmentSaveWindow window = new BidShipmentSaveWindow(bid, new Action( () => 
                 { 
                 	if(bid.Debt == 0)
                 	{
                 		Close();
                 		return;
                 	}
                 	setShippedControlsVisibility(); 
                 }));
				window.ShowDialog();
			}
		}
		
		//Set visibility shipped controls
		void setShippedControlsVisibility ()
		{
			if(bid != null && bid.Is_shipped == 1)
			{
				btnIsShipped.Visibility = Visibility.Collapsed;
				tbIsShipped.Visibility = Visibility.Visible;					
				if(bid.Shipment_date != null && bid.Id_transport_company != null)
				{
					tbIsShipped.Text = "Отгружено, " +
						((DateTime)bid.Shipment_date).ToString("dd.MM.yyyy") + ", " + bid.TransportCompanyName;
				}
				
			}
		}
		
		//EquipmentBid -------------------->
		//Open add window
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBidSaveWindow window = new EquipmentBidSaveWindow(bid.Id);
			window.ShowDialog();
		}

		//Дабл клик по строке таблицы - открывает окно редактирования
		private void DgvEquipmentBid_RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			EquipmentBid equipmentBid = row.Item as EquipmentBid;
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
		
		//Обработка события нажатия клавиш на строке таблице
		void DgvEquipmentBid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvEquipmentBid_RowDoubleClick(sender, null);
				e.Handled = true; //Отмена обработки по умолчанию
			}
		}
	}
}