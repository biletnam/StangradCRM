/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:41
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

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for SellerWindow.xaml
	/// </summary>
	public partial class SellerWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public SellerWindow()
		{
			InitializeComponent();
			viewSource.Source = SellerViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Seller seller = e.Item as Seller;
				if(seller == null) return;
				e.Accepted = seller.IsVisible;
			};
			viewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			DataContext = new { SellerCollection = viewSource.View };
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			Seller seller = dgvSeller.SelectedItem as Seller;
			if(seller == null) return;
			
			SellerSaveWindow window = new SellerSaveWindow(seller);
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
			Seller seller = dgvSeller.SelectedItem as Seller;
			if(seller == null) return;
			if(!seller.remove())
			{
				MessageBox.Show(seller.LastError);
			}
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			SellerSaveWindow window = new SellerSaveWindow();
			window.ShowDialog();
		}
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			SellerViewModel.instance().search(tbxSearch.Text);
			viewSource.View.Refresh();
		}
		void BtnSearchClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearch.Text = "";
		}
		void BtnRowUp_Click(object sender, RoutedEventArgs e)
		{
			Seller seller = dgvSeller.SelectedItem as Seller;
			if(seller == null) return;
			
			if(seller.rowUp())
			{
				viewSource.View.Refresh();
			}
			else
			{
				MessageBox.Show(seller.LastError);
			}
		}
		void BtnRowDown_Click(object sender, RoutedEventArgs e)
		{
			Seller seller = dgvSeller.SelectedItem as Seller;
			if(seller == null) return;
			
			if(seller.rowDown())
			{
				viewSource.View.Refresh();
			}
			else
			{
				MessageBox.Show(seller.LastError);
			}
		}
	}
}