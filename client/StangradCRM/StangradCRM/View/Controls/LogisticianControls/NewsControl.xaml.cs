/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 04.05.2017
 * Time: 11:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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

namespace StangradCRM.View.Controls.LogisticianControls
{
	/// <summary>
	/// Interaction logic for NewsControl.xaml
	/// </summary>
	public partial class NewsControl : UserControl
	{
		CollectionViewSource viewSource;
		CollectionViewSource equipmentBidViewSource = new CollectionViewSource();
		CollectionViewSource complectationViewSource = new CollectionViewSource();
		CollectionViewSource buyerViewSource = new CollectionViewSource();	
		
		CollectionViewSource inWorkViewSource;
		
		public NewsControl(CollectionViewSource newViewSource, CollectionViewSource inWorkViewSource)
		{
			InitializeComponent();
			
			viewSource = newViewSource;
			this.inWorkViewSource = inWorkViewSource;
			
			viewSource.Source = BidViewModel.instance().Collection;

			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Bid bid = e.Item as Bid;
				if(bid == null) return;
				if(bid.Logistician_status == null 
				   && bid.Id_bid_status == 2 && bid.Is_archive == 0 && bid.IsShipped == false)
				{
					e.Accepted = bid.IsVisible;
				}
				else {
					e.Accepted = false;
				}
			};
			
			//Установка сортировки ---->
			//1: По статусу оплаты в порядке убывания (по id статусов)
			viewSource.SortDescriptions.Add(new SortDescription("Id_payment_status", ListSortDirection.Descending));
			//2: По дате отгрузки в порядке убывания
			viewSource.SortDescriptions.Add(new SortDescription("Shipment_date", ListSortDirection.Descending));
			//3: По дате создания в порядке убывания
			viewSource.SortDescriptions.Add(new SortDescription("Date_created", ListSortDirection.Descending));
			SetViewSources();
			
			DataContext = new
			{
				BidCollection = viewSource.View,
				EquipmentBidCollection = equipmentBidViewSource.View,
				ComplectationCollection = complectationViewSource.View,
				BuyerCollection = buyerViewSource.View
			};
			
		}
		
		
		//Фильтры отображени/сокрытия строк таблиц
		private void SetViewSources ()
		{		
			equipmentBidViewSource.Source = EquipmentBidViewModel.instance().Collection;
			equipmentBidViewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				EquipmentBid equipmentBid = e.Item as EquipmentBid;
				if(equipmentBid == null) return;
				
				Bid bid = dgvBid.SelectedItem as Bid;
				if(bid == null)
				{
					e.Accepted = false;
					return;
				}
				if(bid.Id == equipmentBid.Id_bid)
				{
					e.Accepted = true;
				}
				else
				{
					e.Accepted = false;
				}
			};
			
			complectationViewSource.Source = ComplectationViewModel.instance().Collection;
			complectationViewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Complectation complectation = e.Item as Complectation;
				if(complectation == null) return;
				
				EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
				if(equipmentBid == null)
				{
					e.Accepted = false;
					return;
				}
				if(complectation.Id_equipment_bid == equipmentBid.Id)
				{
					e.Accepted = true;
				}
				else
				{
					e.Accepted = false;
				}
			};
			
			buyerViewSource.Source = BuyerViewModel.instance().Collection;
			buyerViewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Buyer buyer = e.Item as Buyer;
				if(buyer == null) return;
				
				Bid bid = dgvBid.SelectedItem as Bid;
				if(bid == null)
				{
					e.Accepted = false;
					return;
				}
				if(buyer.Id == bid.Id_buyer)
				{
					e.Accepted = true;
				}
				else
				{
					e.Accepted = false;
				}
			};
		}
		
		//Быстрый поиск
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
		
		//Очистка быстрого поиска
		void BtnClearFastSearch_Click(object sender, RoutedEventArgs e)
		{
			tbxFastSearch.Text = "";
		}
		
		//Смена выделения на гриде
		void DgvBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			equipmentBidViewSource.View.Refresh();
			buyerViewSource.View.Refresh();
		}
		
		
		///
		void DgvEquipmentBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			complectationViewSource.View.Refresh();
		}
		
		//
		void ContextCopy_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			TextBlock textBlock = ((ContextMenu)mi.Parent).PlacementTarget as TextBlock;
			if(textBlock == null) return;
			
			Clipboard.SetText(textBlock.Text);
		}
		
		
		// Передача заявки в работу логиста
		void BtnBidTransferLogistToWork_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			bid.Logistician_status = 1;
			
			if(!bid.save()) 
			{
				MessageBox.Show(bid.LastError);
				bid.Logistician_status = null;
			}
			else 
			{
				viewSource.View.Refresh();
				inWorkViewSource.View.Refresh();
			}
		}
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void DgvBid_RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			BidSaveWindow window = new BidSaveWindow(bid);
			window.ShowDialog();
			
			viewSource.View.Refresh();
          	dgvBid.CurrentCell = new DataGridCellInfo(row.Item, dgvBid.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void Dgv_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvBid_RowDoubleClick(sender, null);
				e.Handled = true;
			}
		}
		
		void ContextBidFiles_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Заявка не выбрана!");
				return;
			}
			BidFilesWindow window = new BidFilesWindow(bid);
			window.ShowDialog();
		}
		
	}
}