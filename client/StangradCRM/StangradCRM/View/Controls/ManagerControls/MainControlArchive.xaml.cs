/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 31.08.2016
 * Время: 12:50
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
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
	/// Interaction logic for MainControlArchive.xaml
	/// </summary>
	public partial class MainControlArchive : UserControl
	{
		CollectionViewSource viewSource;
		CollectionViewSource equipmentBidViewSource = new CollectionViewSource();
		CollectionViewSource complectationViewSource = new CollectionViewSource();
		CollectionViewSource buyerViewSource = new CollectionViewSource();
		
		public MainControlArchive()
		{
			InitializeComponent();
			viewSource = new CollectionViewSource();
			viewSource.Source = BidViewModel.instance().getArchiveCollection();
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Bid bid = e.Item as Bid;
				if(bid == null) return;
				if(bid.Id_manager != Auth.getInstance().Id)
				{
					e.Accepted = false;
					return;
				}
				e.Accepted = bid.IsVisible;
			};
			
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
		
		void DgvBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			equipmentBidViewSource.View.Refresh();
			buyerViewSource.View.Refresh();
		}
		
		void DgvEquipmentBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			complectationViewSource.View.Refresh();
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
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void DgvBid_RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			BidShipmentSaveWindow window = new BidShipmentSaveWindow(bid);
			window.ShowDialog();
			
			viewSource.View.Refresh();
          	dgvBid.CurrentCell = new DataGridCellInfo(row.Item, dgvBid.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void DgvBid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvBid_RowDoubleClick(sender, null);
				e.Handled = true;
			}
		}
		
		void ContextPaymentHistory_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Заявка не выбрана!");
				return;
			}
			PaymentHistoryWindow window = new PaymentHistoryWindow(bid);
			window.ShowDialog();
		}
		
		void ContextCopy_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			TextBlock textBlock = ((ContextMenu)mi.Parent).PlacementTarget as TextBlock;
			if(textBlock == null) return;
			
			Clipboard.SetText(textBlock.Text);
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