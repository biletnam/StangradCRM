/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 02.09.2016
 * Время: 16:23
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace StangradCRM.View.Controls.DirectorControls
{
	/// <summary>
	/// Interaction logic for ManagerBidControlID2.xaml
	/// </summary>
	public partial class ManagerBidControlID2 : UserControl
	{
		static CollectionViewSource viewSource;
		public ManagerBidControlID2(CollectionViewSource viewSource)
		{
			InitializeComponent();
			ManagerBidControlID2.viewSource = viewSource;
			
			ManagerBidControlID2.viewSource.SortDescriptions.Add(new SortDescription("Id_payment_status", ListSortDirection.Descending));
			ManagerBidControlID2.viewSource.SortDescriptions.Add(new SortDescription("Shipment_date", ListSortDirection.Descending));
			
			DataContext = new
			{
				BidCollection = ManagerBidControlID2.viewSource.View
			};
		}
		
		public static void UpdateViewSource ()
		{
			viewSource.View.Refresh();
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
		
	}
}