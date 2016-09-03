/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 26.08.2016
 * Время: 17:20
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.Controls.ManagerControls
{
	/// <summary>
	/// Interaction logic for MainControlID1.xaml
	/// </summary>
	public partial class MainControlID1 : UserControl
	{
		CollectionViewSource viewSource;
		public MainControlID1(CollectionViewSource viewSource)
		{
			InitializeComponent();
			this.viewSource = viewSource;
			this.viewSource.SortDescriptions.Add(new SortDescription("Date_created", ListSortDirection.Descending));
			
			List<Manager> manager = ManagerViewModel.instance().Collection.ToList();
			Manager currentManager = ManagerViewModel.instance().getById(Auth.getInstance().Id);
			
			if(manager.Contains(currentManager))
			{
				manager.Remove(currentManager);
			}
			
			List<BidStatus> status = BidStatusViewModel.instance().Collection.ToList();
			status.Remove(BidStatusViewModel.instance().getById((int)Classes.BidStatus.New));
			
			DataContext = new
			{
				BidCollection = this.viewSource.View,
				CurrentManagerCollection = manager,
				CurrentStatusCollection = status
			};
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			BidSaveWindow window = new BidSaveWindow();
			window.ShowDialog();
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			BidSaveWindow window = new BidSaveWindow(bid);
			window.ShowDialog();
		}
		
		void DgvBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetEquipmentBidSource();
			SetBuyerSource();
		}
		
		private void SetEquipmentBidSource ()
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			Binding binding = new Binding();
			if(bid == null)
			{
				binding.Source = null;
			}
			else 
			{
				binding.Source = bid.EquipmentBidCollection;
			}
			dgvEquipmentBid.SetBinding(DataGrid.ItemsSourceProperty, binding);
			
			if(dgvEquipmentBid.Items.Count > 0)
			{
				dgvEquipmentBid.SelectedIndex = 0;
			}
			SetComplectationSource();
		}
		
		private void SetBuyerSource ()
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			Binding binding = new Binding();
			if(bid == null)
			{
				binding.Source = null;
			}
			else 
			{
				List<Buyer> buyerList = new List<Buyer>();
				Buyer buyer = BuyerViewModel.instance().getById(bid.Id_buyer);
				if(buyer != null)
				{
					buyerList.Add(buyer);
					binding.Source = buyerList;
				}
				else
				{
					binding.Source = null;
				}
			}
			dgvBuyer.SetBinding(DataGrid.ItemsSourceProperty, binding);
		}
		
		void BtnEquipmentBidAdd_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			EquipmentBidSaveWindow window = new EquipmentBidSaveWindow(bid.Id);
			window.ShowDialog();
		}
		
		void BtnEquipmentBidEdit_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			if(equipmentBid == null) return;
			EquipmentBidSaveWindow window = new EquipmentBidSaveWindow(equipmentBid);
			window.ShowDialog();
		}
		
		void BtnEquipmentBidRemove_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			if(equipmentBid == null) return;
			if(MessageBox.Show("Удалить оборудование из заявки?", "Удалить оборудование из заявки?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
			if(!equipmentBid.remove())
			{
				MessageBox.Show(equipmentBid.LastError);
			}
		}
		
		void DgvEquipmentBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetComplectationSource();
		}
		
		private void SetComplectationSource ()
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			Binding binding = new Binding();
			if(equipmentBid == null)
			{
				binding.Source = null;
			}
			else
			{
				binding.Source = equipmentBid.ComplectationCollection;
			}
			dgvComplectation.SetBinding(DataGrid.ItemsSourceProperty, binding);
		}
		
		void ContextMenuOpenEditWindow_Click(object sender, RoutedEventArgs e)
		{
			BtnEditRow_Click(null, null);
		}
		
		void ContextMenuRemove_Click(object sender, RoutedEventArgs e)
		{
			BtnDeleteRow_Click(null, null);
		}
		
		void contextMenuItemTransferByManagerClick(object sender, RoutedEventArgs e) 
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null)
			{
				MessageBox.Show("MenuItem is null");
				return;
			}
			
			Manager manager = mi.DataContext as Manager;
			if(manager == null)
			{
				MessageBox.Show("Manager is null");
				return;
			}
			
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Bid is null");
				return;
			}
			
			bid.Id_manager = manager.Id;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}
			BidViewModel.instance().remove(bid);
		}
		
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			if(MessageBox.Show("Удалить заявку?", "Удалить заявку?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
			
			if(!bid.remove())
			{
				MessageBox.Show(bid.LastError);
			}
			
		}
		
		void TbxFastSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			BidViewModel.instance().fastSearch(tbxFastSearch.Text, (TSObservableCollection<Bid>)viewSource.Source);
			viewSource.View.Refresh();
			if(dgvBid.Items.Count > 0)
			{
				dgvBid.SelectedIndex = 0;
			}
			else
			{
				dgvBid.SelectedIndex = -1;
			}
		}
		
		void ContextAddPayment_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			PaymentSaveWindow window = new PaymentSaveWindow(bid);
			window.ShowDialog();
		}
		
		void BtnClearFastSearch_Click(object sender, RoutedEventArgs e)
		{
			tbxFastSearch.Text = "";
		}
		
		void ContextPaymentHistory_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			PaymentHistoryWindow window = new PaymentHistoryWindow(bid);
			window.ShowDialog();
		}
		
		void ContextTransferToInWork_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null)
			{
				MessageBox.Show("MenuItem is null");
				return;
			}
			
			BidStatus bidStatus = mi.DataContext as BidStatus;
			if(bidStatus == null)
			{
				MessageBox.Show("BidStatus is null");
				return;
			}
			
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Bid is null");
				return;
			}
			
			bid.Id_bid_status = bidStatus.Id;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}
		}
		
		void ContextMenuRemove_Loaded(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid ==null) return;
			
			if(bid.PermittedRemoval == false)
			{
				mi.Visibility = Visibility.Collapsed;
			}
			else
			{
				mi.Visibility = Visibility.Visible;
			}
		}
		
		void ContextAddPayment_Loaded(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid ==null) return;
			
			if(bid.Id_payment_status == 3)
			{
				mi.Visibility = Visibility.Collapsed;
			}
			else
			{
				mi.Visibility = Visibility.Visible;
			}
		}
	}
}