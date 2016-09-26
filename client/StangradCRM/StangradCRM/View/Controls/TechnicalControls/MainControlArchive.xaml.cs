/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 23.09.2016
 * Время: 12:20
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
			if(!equipmentBid.save())
			{
				MessageBox.Show(equipmentBid.LastError);
				return;
			}
			EquipmentBidViewModel.instance().updateStatus(equipmentBid);			
		}
	}
}