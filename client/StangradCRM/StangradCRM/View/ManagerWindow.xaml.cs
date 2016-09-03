/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 18:18
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
	/// Interaction logic for ManagerWindow.xaml
	/// </summary>
	public partial class ManagerWindow : Window
	{
		public ManagerWindow()
		{
			InitializeComponent();
			DataContext = new {ManagerCollection = ManagerViewModel.instance().Collection};
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			ManagerSaveWindow window = new ManagerSaveWindow();
			window.ShowDialog();
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			Manager manager = dgvManager.SelectedItem as Manager;
			if(manager == null) return;
			ManagerSaveWindow window = new ManagerSaveWindow(manager);
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
			Manager manager = dgvManager.SelectedItem as Manager;
			if(manager == null) return;
			if(!manager.remove())
			{
				MessageBox.Show(manager.LastError);
			}
		}
	}
}