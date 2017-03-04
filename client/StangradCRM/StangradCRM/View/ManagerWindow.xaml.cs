/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 18:18
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Data;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for ManagerWindow.xaml
	/// </summary>
	public partial class ManagerWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public ManagerWindow()
		{
			InitializeComponent();
			viewSource.Source = ManagerViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Manager manager = e.Item as Manager;
				if(manager == null) return;
				e.Accepted = manager.IsVisible;
			};
			DataContext = new {ManagerCollection = viewSource.View};
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
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			ManagerViewModel.instance().search(tbxSearch.Text);
			viewSource.View.Refresh();
		}
		void BtnSearchClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearch.Text = "";
		}
	}
}