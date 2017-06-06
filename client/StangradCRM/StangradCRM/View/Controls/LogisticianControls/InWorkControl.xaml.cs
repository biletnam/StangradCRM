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
using StangradCRM.Model;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.Controls.LogisticianControls
{
	/// <summary>
	/// Interaction logic for InWorkControl.xaml
	/// </summary>
	public partial class InWorkControl : UserControl
	{
		
		CollectionViewSource viewSource;// = new CollectionViewSource();
		CollectionViewSource equipmentBidViewSource = new CollectionViewSource();
		CollectionViewSource complectationViewSource = new CollectionViewSource();
		CollectionViewSource buyerViewSource = new CollectionViewSource();	
		
		CollectionViewSource forShipmentViewSource;
		
		IniFile iniFile = new IniFile("Settings.ini");
		
		public InWorkControl(CollectionViewSource inWorkViewSource, CollectionViewSource forShipmentViewSource)
		{
			InitializeComponent();
		
			viewSource = inWorkViewSource;
			
			this.forShipmentViewSource = forShipmentViewSource;
			
			viewSource.Source = BidViewModel.instance().Collection;

			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Bid bid = e.Item as Bid;
				if(bid == null) return;
				if(bid.Logistician_status == 1 
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
						bidBlank.FileName = openFolderDialog.SelectedPath + "/" + Classes.ReplaceSpecialCharsFileName.Replace(fileName);
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
		
		//Клик по кнопке перевода в статус 'Для отгрузки'
		void BtnSetShipment_Click(object sender, RoutedEventArgs e)
		{
			Bid bid = dgvBid.SelectedItem as Bid;
			if(bid == null) 
			{
				MessageBox.Show("Выберите заявку!");
				return;
			}
			
			bid.Logistician_status = 2;
			if(!bid.save())
			{
				bid.Logistician_status = 1;
				MessageBox.Show(bid.LastError);
			}
			else
			{
				viewSource.View.Refresh();
				forShipmentViewSource.View.Refresh();				
			}
		}
		
		
		
		//Контекстное меню в таблице заявок ---->
		
		//Оборудование в заявке ---->		
		
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
				bidBlank.FileName = openFolderDialog.SelectedPath + "/" + Classes.ReplaceSpecialCharsFileName.Replace(fileName);
				
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
		
		void ContextCopy_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			if(mi == null) return;
			
			TextBlock textBlock = ((ContextMenu)mi.Parent).PlacementTarget as TextBlock;
			if(textBlock == null) return;
			
			Clipboard.SetText(textBlock.Text);
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