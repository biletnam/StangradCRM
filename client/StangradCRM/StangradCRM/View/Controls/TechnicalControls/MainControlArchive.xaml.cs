/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 23.09.2016
 * Время: 12:20
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Controls.TechnicalControls
{
	/// <summary>
	/// Interaction logic for MainControlArchive.xaml
	/// </summary>
	public partial class MainControlArchive : UserControl
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public MainControlArchive()
		{
			InitializeComponent();
			viewSource.Source = EquipmentBidViewModel.instance().getCollectionByArchiveStatus(1);
			//Сортировка даты по убыванию
			viewSource.SortDescriptions.Add(new SortDescription("PlannedShipmentDate", ListSortDirection.Descending));
			DataContext = new
			{
				EquipmentBidCollection = viewSource.View
			};
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			EquipmentBid equipmentBid = row.Item as EquipmentBid;
			if(equipmentBid == null) return;
			
			if(MessageBox.Show("Вернуть в работу?", "Вернуть в работу?",
			                   MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
			
			equipmentBid.Is_archive = 0;
			Bid bid = BidViewModel.instance().getById(equipmentBid.Id_bid);
			if(!equipmentBid.save())
			{
				MessageBox.Show(equipmentBid.LastError);
				return;
			}
			else
			{
				if(bid != null)
				{
					bid.Guid = Guid.NewGuid().ToString();
					if(!bid.save())
					{
						MessageBox.Show("Не удалось обновить время модификации заявки!\nДругой специалист тех. отдела не увидит обновление\n" + bid.LastError);
					}
				}
			}
			EquipmentBidViewModel.instance().updateStatus(equipmentBid);			
		}
	}
}