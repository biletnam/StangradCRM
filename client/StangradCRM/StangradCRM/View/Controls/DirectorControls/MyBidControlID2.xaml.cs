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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Win32;
using StangradCRM.Core;
using StangradCRM.Extensions;
using StangradCRM.Model;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.Controls.DirectorControls
{
	/// <summary>
	/// Interaction logic for MainControlID2.xaml
	/// </summary>
	public partial class MyBidControlID2 : UserControl
	{
		CollectionViewSource viewSource;
		CollectionViewSource equipmentBidViewSource = new CollectionViewSource();
		CollectionViewSource complectationViewSource = new CollectionViewSource();
		CollectionViewSource buyerViewSource = new CollectionViewSource();
		
		IniFile iniFile = new IniFile("Settings.ini");
		
		public MyBidControlID2(CollectionViewSource viewSource)
		{
			InitializeComponent();
			this.viewSource = viewSource;
			
			//Список менеджеров
			List<Manager> manager = ManagerViewModel.instance().Collection.ToList();
			//Находим текущего менеджера
			Manager currentManager = ManagerViewModel.instance().getById(Auth.getInstance().Id);
			//Если есть в списке
			if(manager.Contains(currentManager))
			{
				//Удаляем из списка
				manager.Remove(currentManager);
			}
			
			//Установка сортировки ---->
			//1: По статусу оплаты в порядке убывания (по id статусов)
			this.viewSource.SortDescriptions.Add(new SortDescription("Id_payment_status", ListSortDirection.Descending));
			//2: По дате отгрузки в порядке убывания
			this.viewSource.SortDescriptions.Add(new SortDescription("Shipment_date", ListSortDirection.Descending));
			//3: По дате создания в порядке убывания
			this.viewSource.SortDescriptions.Add(new SortDescription("Date_created", ListSortDirection.Descending));
			
			//Список статусов
			List<BidStatus> status = BidStatusViewModel.instance().Collection.ToList();
			//Удаляем из списка статусов текущий
			status.Remove(BidStatusViewModel.instance().getById((int)Classes.BidStatus.InWork));
			
			//Функция установки фильтров
			SetViewSources();
			//Установка контекста данных
			DataContext = new
			{
				BidCollection = this.viewSource.View,
				CurrentManagerCollection = manager,
				CurrentStatusCollection = status,
				EquipmentBidCollection = equipmentBidViewSource.View,
				ComplectationCollection = complectationViewSource.View,
				BuyerCollection = buyerViewSource.View
			};
		}
		
		//Фильтры отображени/сокрытия строк таблиц
		private void SetViewSources ()
		{
			//Установка источника данных оборудования в заявке
			equipmentBidViewSource.Source = EquipmentBidViewModel.instance().Collection;
			//Установка фильтра
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
			
			//Установка источника данных комплектация
			complectationViewSource.Source = ComplectationViewModel.instance().Collection;
			//Установка фильтра
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
			
			//Установка источника данных покупателей
			buyerViewSource.Source = BuyerViewModel.instance().Collection;
			//Установка фильтра
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
		}
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void DgvBid_RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			BidSaveWindow window = new BidSaveWindow(bid);
			window.ShowDialog();						
			
			//обновление viewSource и установка фокуса
			viewSourceRefresh(viewSource, dgvBid, row);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void DgvBid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvBid_RowDoubleClick(sender, null);
				e.Handled = true; //Отмена обработки по умолчанию
			}
		}
		
		//Клик по кнопке добавления платежа (в каждой строке), открывает окно добавления платежа
		void BtnAddPayment_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			Bid bid = row.Item as Bid;
			if(bid == null) return;
			
			PaymentSaveWindow window = new PaymentSaveWindow(bid);
			window.ShowDialog();
			
			//обновление viewSource и установка фокуса
			viewSourceRefresh(viewSource, dgvBid, row);
			
		}
		
		//Клик по кнопке печати бланков
		void BtnPrintBlank_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			Buyer buyer = BuyerViewModel.instance().getById(bid.Id_buyer);
			if(buyer == null) return;
			
			var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			if(iniFile.KeyExists("report_path") && System.IO.Directory.Exists(iniFile.Read("report_path")))
			{
				openFolderDialog.SelectedPath = iniFile.Read("report_path");
			}
			System.Windows.Forms.DialogResult result =
				openFolderDialog.ShowDialog(Classes.OpenDirectoryDialog.GetIWin32Window(this));
			
			if(result == System.Windows.Forms.DialogResult.OK)
			{
				processControl.Visibility = Visibility.Visible;
				Task.Factory.StartNew( () => {              
					int reportCount = bid.EquipmentBidCollection.Count;                     
					for(int i = 0; i < reportCount; i++)
					{
						Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { processControl.Text = "Формирование бланков " + (i+1).ToString() + " из " + reportCount.ToString(); } ));
						EquipmentBid equipmentBid = bid.EquipmentBidCollection[i];
						string fileName = "Бланк заявки №" + bid.Id.ToString() + "-" + equipmentBid.Id.ToString() + " " + bid.Account + " " + buyer.Name + ".xlsx";
						Reports.BidBlank bidBlank = new StangradCRM.Reports.BidBlank(bid, equipmentBid);
						bidBlank.FileName = openFolderDialog.SelectedPath + "/" + ReplaceSpecialCharsFileName(fileName);
						if(!bidBlank.Save())
						{
							MessageBox.Show(bidBlank.LastError);
							Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { processControl.Visibility = Visibility.Hidden; } ));
							return;
						}
						equipmentBid.Is_blank_print = 1;
						if(!equipmentBid.save())
						{
							MessageBox.Show("Не удалось установить статус 'Бланк заявки сформирован' для оборудования в заявке\n" + equipmentBid.LastError);
							Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { processControl.Visibility = Visibility.Hidden; } ));
							return;
						}
					}
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { processControl.Visibility = Visibility.Hidden; } ));
					MessageBox.Show("Все бланки заявок сохранены в директорию '" + openFolderDialog.SelectedPath + "'");
					iniFile.Write("report_path", openFolderDialog.SelectedPath);
					System.Diagnostics.Process.Start(openFolderDialog.SelectedPath);
				});
			}
		}
		
		//Клик по кнопке генерации серийных номеров
		void BtnGenerateSN_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			if(!bid.generateSerialNumber())
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
	
			viewSource.View.Refresh();
		}
		
		//Клик по кнопке передачи заявки в другой статус, открывает окно передачи заявки в другой статус
		void BtnBidTransferToStatus_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			
			bid.Id_bid_status = (int)Classes.BidStatus.New;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}

			viewSource.View.Refresh();
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
			
			viewSource.View.Refresh();
		}
		
		//Клик по кнопке установки статуса 'отгружено', открывает окно установки статуса
		void BtnSetShipment_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Выберите заявку!");
				return;
			}
			BidShipmentSaveWindow window = new BidShipmentSaveWindow(bid);
			window.ShowDialog();
			
			viewSource.View.Refresh();
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
			
			viewSource.View.Refresh();
		}

		//Контекстное меню передачи заявки в статус
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
			
			if(MessageBox.Show("Изменить статус заявки на '" + bidStatus.Name + "'?",
			                   "Изменить статус заявки?",
			                   MessageBoxButton.YesNo) != MessageBoxResult.Yes) return; 
			
			bid.Id_bid_status = bidStatus.Id;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}
			
			viewSource.View.Refresh();
			
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
		
		//Контекстное меню установки статуса "Отгружено"
		void ContextSetIsShipped_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) return;
			BidShipmentSaveWindow window = new BidShipmentSaveWindow(bid);
			window.ShowDialog();
			
			viewSource.View.Refresh();
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
		
		//Обработка события нажатия клавиш на строке таблице
		void DgvEquipmentBid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvEquipmentBid_RowDoubleClick(sender, null);
				e.Handled = true; //Отмена обработки по умолчанию
			}
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
		
		//Клик по контекстному меню печати наклейки
		void MiPrintSticker_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			if(equipmentBid == null) return;
			
			SaveFileDialog sfDialog = new SaveFileDialog();
			sfDialog.FileName = "Наклейка заявки №" + equipmentBid.Id_bid.ToString() + " для оборудования " + equipmentBid.EquipmentName + ", код оборудования в заявке " + equipmentBid.Id.ToString() + " (" + DateTime.Now.ToString("dd.MM.yyyy") + ").xlsx";
			sfDialog.Filter = "Excel 2007 worksheet (*.xlsx)|*.xlsx";
			if(sfDialog.ShowDialog() != true) return;
			
			Reports.Sticker sticker = new StangradCRM.Reports.Sticker(equipmentBid);
			sticker.FileName = sfDialog.FileName;
			if(!sticker.Save())
			{
				MessageBox.Show(sticker.LastError);
			}
			else
			{
				MessageBox.Show("Наклейка сохранена по пути " + sticker.FileName);
				System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(sticker.FileName));
			}
		}
		
		//Клик по контекстному меню печати бланка
		void MiPrintBlank_Click(object sender, RoutedEventArgs e)
		{
			EquipmentBid equipmentBid = dgvEquipmentBid.SelectedItem as EquipmentBid;
			if(equipmentBid == null) return;
			
			Bid bid = BidViewModel.instance().getById(equipmentBid.Id_bid);
			if(bid == null) return;
			
			Buyer buyer = BuyerViewModel.instance().getById(bid.Id_buyer);
			if(buyer == null) return;
			
			var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			if(iniFile.KeyExists("report_path") && System.IO.Directory.Exists(iniFile.Read("report_path")))
			{
				openFolderDialog.SelectedPath = iniFile.Read("report_path");
			}
			System.Windows.Forms.DialogResult result = 
				openFolderDialog.ShowDialog(Classes.OpenDirectoryDialog.GetIWin32Window(this));
			
			if(result == System.Windows.Forms.DialogResult.OK)
			{
				string fileName = "Бланк заявки №" + bid.Id.ToString() + "-" + equipmentBid.Id.ToString() + " " + bid.Account + " " + buyer.Name + ".xlsx";
				Reports.BidBlank bidBlank = new StangradCRM.Reports.BidBlank(bid, equipmentBid);
				bidBlank.FileName = openFolderDialog.SelectedPath + "/" + ReplaceSpecialCharsFileName(fileName);
				
				processControl.Visibility = Visibility.Visible;
				processControl.Text = "Сохранение бланка заявки...";
				
				Task.Factory.StartNew( () => {       
					if(!bidBlank.Save())
					{
						MessageBox.Show(bidBlank.LastError);
						Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { processControl.Visibility = Visibility.Hidden; } ));
						return;
					}
					equipmentBid.Is_blank_print = 1;
					if(!equipmentBid.save())
					{
						MessageBox.Show("Не удалось установить статус 'Бланк заявки сформирован' для оборудования в заявке\n" + equipmentBid.LastError);
						Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { processControl.Visibility = Visibility.Hidden; } ));
						return;
					}
					MessageBox.Show("Бланк заявки сохранен в директорию '" + openFolderDialog.SelectedPath + "'");
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { processControl.Visibility = Visibility.Hidden; } ));
					iniFile.Write("report_path", openFolderDialog.SelectedPath);
					System.Diagnostics.Process.Start(openFolderDialog.SelectedPath);
                });
			}
		}
		
		//Клик по кнопке печати наклейки
		void BtnEquipmentBidPrintSticker_Click(object sender, RoutedEventArgs e)
		{
			MiPrintSticker_Click(null, null);
		}
		
		//Служебные---->
		
		//Обновление viewSource + установка фокуса на строку грида
		void viewSourceRefresh(CollectionViewSource vs,
								DataGrid dg = null,
		                       DataGridRow row = null)
		{
			vs.View.Refresh();
			if(dg != null && row != null)
			{
      			dg.CurrentCell = new DataGridCellInfo(row.Item, dg.CurrentCell.Column);
			}
		}
		
		//Замена спец. символов в имени файла
		string ReplaceSpecialCharsFileName (string filename)
		{
			filename = filename.Replace('/', '-');
			filename = filename.Replace('\\', '-');
			filename = filename.Replace('|' , '-');
			filename = filename.Replace('<' , '-');
			filename = filename.Replace('>' , '-');
			filename = filename.Replace('?' , '-');
			filename = filename.Replace('[' , '-');
			filename = filename.Replace(']' , '-');
			filename = filename.Replace(':' , '-');
			filename = filename.Replace('"' , ' ');
			return filename.Replace('*' , '-');
		}
	}
}