/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 22.08.2016
 * Время: 18:14
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
	/// Interaction logic for TransportCompanyWindow.xaml
	/// </summary>
	public partial class TransportCompanyWindow : Window
	{
		public TransportCompanyWindow()
		{
			InitializeComponent();
			DataContext = new {TransportCompanyCollection = TransportCompanyViewModel.instance().Collection};
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
	}
}