/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 03.10.2016
 * Время: 16:58
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

namespace StangradCRM.View.Forms
{
	/// <summary>
	/// Interaction logic for BuyerListWindow.xaml
	/// </summary>
	public partial class BuyerListWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		Action<Buyer> action = null;
		
		public BuyerListWindow(Action<Buyer> action)
		{
			InitializeComponent();
			viewSource.Source = BuyerViewModel.instance().Collection;
			viewSource.Filter += delegate(object sender, FilterEventArgs e)
			{
				Buyer buyer = e.Item as Buyer;
				if(buyer == null) return;
				
				e.Accepted = buyer.IsVisible;
			};
			
			DataContext = new
			{
				BuyerCollection = viewSource.View
			};
			
			this.action = action;
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			BuyerViewModel.instance().fastSearch(tbxSearch.Text);
			viewSource.View.Refresh();
		}
		
		void BtnSearchClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearch.Text = "";
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			Buyer buyer = row.Item as Buyer;
			if(buyer == null) return;
			
			if(action != null)
			{
				action(buyer);
				Close();
			}
		}
	}
}