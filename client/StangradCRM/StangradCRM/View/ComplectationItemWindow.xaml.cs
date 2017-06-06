/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 26.09.2016
 * Время: 10:39
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for ComplectationItemWindow.xaml
	/// </summary>
	public partial class ComplectationItemWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public ComplectationItemWindow()
		{
			InitializeComponent();
			viewSource.Source = ComplectationItemViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				ComplectationItem complectationItem = e.Item as ComplectationItem;
				if(complectationItem == null) return;
				e.Accepted = complectationItem.IsVisible;
			};
			DataContext = new { ComplectationItemCollection = viewSource.View };
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
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			ComplectationItemViewModel.instance().search(tbxSearch.Text);
			viewSource.View.Refresh();
		}
		void BtnSearchClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearch.Text = "";
		}
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			
			ComplectationItem complectationItem = row.Item as ComplectationItem;
			if(complectationItem == null) return;
			
			ComplectationItemSaveWindow window = new ComplectationItemSaveWindow(complectationItem);
			window.ShowDialog();

			
			viewSource.View.Refresh();
          	dgvComplectationItem.CurrentCell = new DataGridCellInfo(row.Item, dgvComplectationItem.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void RowPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				RowDoubleClick(sender, null);
				e.Handled = true;
			}
		}

	}
}