/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:07
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for BidStatusWindow.xaml
	/// </summary>
	public partial class BidStatusWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public BidStatusWindow()
		{
			InitializeComponent();
			viewSource.Source = BidStatusViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				BidStatus bidStatus = e.Item as BidStatus;
				if(bidStatus == null) return;
				e.Accepted = bidStatus.IsVisible;
			};
			
			DataContext = new {BidStatusCollection = viewSource.View};
		}
		
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			if(MessageBox.Show("Подтвердите удаление",
			                "Удалить выбранную запись?",
			                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			{
				return;
			}
			BidStatus bidStatus = dgvBidStatus.SelectedItem as BidStatus;
			if(bidStatus == null) return;
			if(!bidStatus.remove())
			{
				MessageBox.Show(bidStatus.LastError);
			}
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			BidStatus bidStatus = dgvBidStatus.SelectedItem as BidStatus;
			if(bidStatus == null) return;
			BidStatusSaveWindow window = new BidStatusSaveWindow(bidStatus);
			window.ShowDialog();
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			BidStatusSaveWindow window = new BidStatusSaveWindow();
			window.ShowDialog();
		}
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			BidStatusViewModel.instance().search(tbxSearch.Text);
			viewSource.View.Refresh();
		}
		void BtnSearchClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearch.Text = "";
		}
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			
			BidStatus bidStatus = row.Item as BidStatus;
			if(bidStatus == null) return;
			
			BidStatusSaveWindow window = new BidStatusSaveWindow(bidStatus);
			window.ShowDialog();
			
			viewSource.View.Refresh();
          	dgvBidStatus.CurrentCell = new DataGridCellInfo(row.Item, dgvBidStatus.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void RowPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				RowDoubleClick(sender, null);
				e.Handled = true;
			}
		}
		
		
	}
}