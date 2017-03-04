/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 22.08.2016
 * Время: 18:14
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
	/// Interaction logic for TransportCompanyWindow.xaml
	/// </summary>
	public partial class TransportCompanyWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public TransportCompanyWindow()
		{
			InitializeComponent();
			viewSource.Source = TransportCompanyViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				TransportCompany transportCompany = e.Item as TransportCompany;
				if(transportCompany == null) return;
				e.Accepted = transportCompany.IsVisible;
			};
			viewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			DataContext = new {TransportCompanyCollection = viewSource.View};
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			TransportCompanySaveWindow window = new TransportCompanySaveWindow();
			window.ShowDialog();
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			TransportCompany transportCompany = dgvTransportCompany.SelectedItem as TransportCompany;
			if(transportCompany == null) return;
			TransportCompanySaveWindow window = new TransportCompanySaveWindow(transportCompany);
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
			TransportCompany transportCompany = dgvTransportCompany.SelectedItem as TransportCompany;
			if(transportCompany == null) return;
			if(!transportCompany.remove())
			{
				MessageBox.Show(transportCompany.LastError);
			}
		}
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			TransportCompanyViewModel.instance().search(tbxSearch.Text);
			viewSource.View.Refresh();
		}
		void BtnSearchClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearch.Text = "";
		}
		void BtnRowUp_Click(object sender, RoutedEventArgs e)
		{
			TransportCompany transportCompany = dgvTransportCompany.SelectedItem as TransportCompany;
			if(transportCompany == null) return;
			
			if(transportCompany.rowUp())
			{
				viewSource.View.Refresh();
			}
			else
			{
				MessageBox.Show(transportCompany.LastError);
			}
		}
		void BtnRowDown_Click(object sender, RoutedEventArgs e)
		{
			TransportCompany transportCompany = dgvTransportCompany.SelectedItem as TransportCompany;
			if(transportCompany == null) return;
			
			if(transportCompany.rowDown())
			{
				viewSource.View.Refresh();
			}
			else
			{
				MessageBox.Show(transportCompany.LastError);
			}
		}
	}
}