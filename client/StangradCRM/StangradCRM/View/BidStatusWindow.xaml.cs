/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:07
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
	/// Interaction logic for BidStatusWindow.xaml
	/// </summary>
	public partial class BidStatusWindow : Window
	{
		public BidStatusWindow()
		{
			InitializeComponent();
			DataContext = new {BidStatusCollection = BidStatusViewModel.instance().Collection};
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
	}
}