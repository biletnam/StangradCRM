/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:37
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
	/// Interaction logic for BuyerWindow.xaml
	/// </summary>
	public partial class BuyerWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public BuyerWindow()
		{
			InitializeComponent();
			viewSource.Source = BuyerViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Buyer buyer = e.Item as Buyer;
				if(buyer == null) return;
				e.Accepted = buyer.IsVisible;
			};
			DataContext = new { BuyerCollection =  viewSource.View};
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			BuyerSaveWindow window = new BuyerSaveWindow();
			window.ShowDialog();
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			Buyer buyer = dgvBuyer.SelectedItem as Buyer;
			if(buyer == null) return;
			
			BuyerSaveWindow window = new BuyerSaveWindow(buyer);
			window.ShowDialog();
		}
		
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			if(MessageBox.Show("Подтвердите удаление",
			                "Удалить выбранную запись?",
			                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			{
				return;
			}      
			Buyer buyer = dgvBuyer.SelectedItem as Buyer;
			if(buyer == null) return;
			if(!buyer.remove())
			{
				MessageBox.Show(buyer.LastError);
			}
		}
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			BuyerViewModel.instance().search(tbxSearch.Text);
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
			
			Buyer buyer = row.Item as Buyer;
			if(buyer == null) return;
			
			BuyerSaveWindow window = new BuyerSaveWindow(buyer);
			window.ShowDialog();

			
			viewSource.View.Refresh();
          	dgvBuyer.CurrentCell = new DataGridCellInfo(row.Item, dgvBuyer.CurrentCell.Column);
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