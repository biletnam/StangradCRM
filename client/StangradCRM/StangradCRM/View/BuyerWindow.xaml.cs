/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:37
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
	/// Interaction logic for BuyerWindow.xaml
	/// </summary>
	public partial class BuyerWindow : Window
	{
		public BuyerWindow()
		{
			InitializeComponent();
			DataContext = new { BuyerCollection = BuyerViewModel.instance().Collection };
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
	}
}