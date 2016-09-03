/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:21
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
	/// Interaction logic for PaymentStatusWindow.xaml
	/// </summary>
	public partial class PaymentStatusWindow : Window
	{
		public PaymentStatusWindow()
		{
			InitializeComponent();
			DataContext = new {PaymentStatusCollection = PaymentStatusViewModel.instance().Collection};
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
	}
}