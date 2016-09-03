/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:41
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

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for SellerWindow.xaml
	/// </summary>
	public partial class SellerWindow : Window
	{
		public SellerWindow()
		{
			InitializeComponent();
			DataContext = new { SellerCollection = SellerViewModel.instance().Collection };
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
	}
}