/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 26.09.2016
 * Время: 10:39
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
	/// Interaction logic for ComplectationItemWindow.xaml
	/// </summary>
	public partial class ComplectationItemWindow : Window
	{
		public ComplectationItemWindow()
		{
			InitializeComponent();
			DataContext = new { ComplectationItemCollection = ComplectationItemViewModel.instance().Collection };
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			ComplectationItemSaveWindow window = new ComplectationItemSaveWindow();
			window.ShowDialog();
		}		
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			ComplectationItem complectationItem = dgvComplectationItem.SelectedItem as ComplectationItem;
			if(complectationItem == null) return;
			
			ComplectationItemSaveWindow window = new ComplectationItemSaveWindow(complectationItem);
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
			ComplectationItem complectationItem = dgvComplectationItem.SelectedItem as ComplectationItem;
			if(complectationItem == null) return;
			if(!complectationItem.remove())
			{
				MessageBox.Show(complectationItem.LastError);
			}
		}
		

	}
}