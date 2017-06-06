/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:21
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
	/// Interaction logic for PaymentStatusWindow.xaml
	/// </summary>
	public partial class PaymentStatusWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public PaymentStatusWindow()
		{
			InitializeComponent();
			viewSource.Source = PaymentStatusViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				PaymentStatus paymentStatus = e.Item as PaymentStatus;
				if(paymentStatus == null) return;
				e.Accepted = paymentStatus.IsVisible;
			};
			DataContext = new {PaymentStatusCollection = viewSource.View};
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			PaymentStatusSaveWindow window = new PaymentStatusSaveWindow();
			window.ShowDialog();
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			PaymentStatus paymentStatus = dgvPaymentStatus.SelectedItem as PaymentStatus;
			if(paymentStatus == null) return;
			PaymentStatusSaveWindow window = new PaymentStatusSaveWindow(paymentStatus);
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
			PaymentStatus paymentStatus = dgvPaymentStatus.SelectedItem as PaymentStatus;
			if(paymentStatus == null) return;
			if(!paymentStatus.remove())
			{
				MessageBox.Show(paymentStatus.LastError);
			}
		}
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			PaymentStatusViewModel.instance().search(tbxSearch.Text);
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
			
			PaymentStatus paymentStatus = row.Item as PaymentStatus;
			if(paymentStatus == null) return;
			
			PaymentStatusSaveWindow window = new PaymentStatusSaveWindow(paymentStatus);
			window.ShowDialog();

			
			viewSource.View.Refresh();
          	dgvPaymentStatus.CurrentCell = new DataGridCellInfo(row.Item, dgvPaymentStatus.CurrentCell.Column);
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