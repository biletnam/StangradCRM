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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
		CollectionViewSource equipmentBidViewSource = new CollectionViewSource();
		CollectionViewSource complectationViewSource = new CollectionViewSource();
		CollectionViewSource buyerViewSource = new CollectionViewSource();
		
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
			manager.RemoveAll(x => (x.Id_role != (int)Classes.Role.Manager) && (x.Id_role != (int)Classes.Role.Director));
			
			
			List<BidStatus> status = BidStatusViewModel.instance().Collection.ToList();
			status.Remove(BidStatusViewModel.instance().getById((int)Classes.BidStatus.New));
			
			SetViewSources();
			
			DataContext = new
			{
				BidCollection = this.viewSource.View,
				CurrentManagerCollection = manager,
				CurrentStatusCollection = status,
				EquipmentBidCollection = equipmentBidViewSource.View,
				ComplectationCollection = complectationViewSource.View,
				BuyerCollection = buyerViewSource.View,
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

		//Поиск ---->
		
		//Вызывается при вводе данных в строку поиска, осуществляет поиск
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
		
		//Клик по кнопке очистки строки поиска
		void BtnClearFastSearch_Click(object sender, RoutedEventArgs e)
		{
			tbxFastSearch.Text = "";
		}

		
		//Заявки ---->

		//Изменение выделенной строки в таблице заявок
		void DgvBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			equipmentBidViewSource.View.Refresh();
			buyerViewSource.View.Refresh();
			
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			if(!bid.PermittedRemoval) 
			{
				btnBidDelete.IsEnabled = false;
			}
			else 
			{
				btnBidDelete.IsEnabled = true;
			}
		}
		
		//Клик по кнопке добавления заявки, открывает окно добавления заявки
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			BidSaveWindow window = new BidSaveWindow();
			window.ShowDialog();
		}
		
		//Клик по кнопке удаления заявки, удаляет заявку
		void BtnBidDelete_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Выберите удаляемую заявку!");
				return;
			}
			if(bid.BidFilesCollection.Count > 0)
			{
				MessageBox.Show("Перед удалением заявки необходимо удалить прикрепленные файлы!");
				return;
			}
			if(MessageBox.Show("Удалить заявку?", "Удалить заявку?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
			if(!bid.remove())
			{
				MessageBox.Show(bid.LastError);
			}
		}
		
		//Клик по кнопке передачи заявки другому менеджеру, открывает окно передачи заявки другому менеджеру
		void BtnBidTransferToManager_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			TransferToManagerWindow window = new TransferToManagerWindow(bid);
			window.ShowDialog();
			
		}
		
		//Клик по кнопке передачи заявки в другой статус, открывает окно передачи заявки в другой статус
		void BtnBidTransferToStatus_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			PlannedShipmentDateSetWindow window 
				= new PlannedShipmentDateSetWindow(bid, new Action<DateTime>( (planned_shipment_date) => { SetPlannedShipmentDateChangeStatusAndSave(bid, planned_shipment_date); }));
			window.ShowDialog();
		}
		
		//Клик по кнопке просмотра платежей, открывает окно просмотра платежей
		void BtnPaymentHistory_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Выберите заявку!");
				return;
			}
			PaymentHistoryWindow window = new PaymentHistoryWindow(bid);
			window.ShowDialog();
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
		void DgvBid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvBid_RowDoubleClick(sender, null);
				e.Handled = true;
			}
		}

		//Клик по кнопке добавления платежа (в каждой строке), открывает окно добавления платежа
		void BtnAddPayment_Click(object sender, RoutedEventArgs e)
		{
			try
			{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			PaymentSaveWindow window = new PaymentSaveWindow(bid);
			window.ShowDialog();
			
			viewSource.View.Refresh();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		
		//Клик по кнопке удаления заявки (в каждой строке), удаляет заявку
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			if(MessageBox.Show("Удалить заявку?", "Удалить заявку?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
			if(!bid.remove())
			{
				MessageBox.Show(bid.LastError);
			}
		}
		
		//Контекстное меню в таблице заявок ---->
		
		//Клик по элементу меню просмотра истории платежей
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
		
		//Контекстное меню передачи заявки в работу
		void ContextTransferToStatus_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			BidStatus bidStatus = mi.DataContext as BidStatus;
			if(bidStatus == null) return;
			
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Заявка не выбрана!");
				return;
			}
			
			PlannedShipmentDateSetWindow window 
				= new PlannedShipmentDateSetWindow(bid, new Action<DateTime>( (planned_shipment_date) => { SetPlannedShipmentDateChangeStatusAndSave(bid, planned_shipment_date); }));
			window.ShowDialog();
		}
		
		//Контекстное меню передачи заявки другому менеджеру
		void contextMenuItemTransferByManagerClick(object sender, RoutedEventArgs e) 
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			Manager manager = mi.DataContext as Manager;
			if(manager == null) return;
			
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Заявка не выбрана!");
				return;
			}
			if(MessageBox.Show("Передать заявку менеджеру " + manager.Name + "?", 
			                   "Передать заявку другому менеджеру?", 
			                   MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
			bid.Id_manager = manager.Id;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}
			bid.remove(true);
		}
		
		//Оборудование в заявке ---->
		
		//Нажатие на кнопку добавления - открывает окно добавления
		void BtnEquipmentBidAdd_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
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
		
		//Нажатие на кнопку удаления в строке, удаляет запись
		void BtnEquipmentBidRemove_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			EquipmentBid equipmentBid = row.Item as EquipmentBid;
			if(equipmentBid == null) return;
			
			if(MessageBox.Show("Удалить оборудование из заявки?", "Удалить оборудование из заявки?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
			if(!equipmentBid.remove())
			{
				MessageBox.Show(equipmentBid.LastError);
			}
		}

		//Изменение выделенной строки в таблице оборудования в заявке		
		void DgvEquipmentBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			complectationViewSource.View.Refresh();
		}		
		
		//Служебные ---->
		
		//Установка предполагаемой даты отгрузки и сохранение заявки
		private void SetPlannedShipmentDateAndSave (Bid bid, DateTime plannedShipmentDate)
		{
			bid.Planned_shipment_date = plannedShipmentDate;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
			}
		}
		
		//Установка предполагаемой даты отгрузки, смена статуса и сохранение заявки
		private void SetPlannedShipmentDateChangeStatusAndSave (Bid bid, DateTime plannedShipmentDate)
		{
			bid.Id_bid_status = (int)Classes.BidStatus.InWork;
			bid.Planned_shipment_date = plannedShipmentDate;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
			}
		}
		
		
		void ContextCopy_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			TextBlock textBlock = ((ContextMenu)mi.Parent).PlacementTarget as TextBlock;
			if(textBlock == null) return;
			
			Clipboard.SetText(textBlock.Text);
		}
	}
}