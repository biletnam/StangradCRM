/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 29.08.2016
 * Время: 12:04
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;
using StangradCRMLibs;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for PaymentSaveWindow.xaml
	/// </summary>
	public partial class PaymentSaveWindow : Window
	{
		private Bid bid;
		private double Debt = 0;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));		
		
		private Payment payment;
		
		public PaymentSaveWindow(Bid bid)
		{
			InitializeComponent();
			defaultBrush = tbxPayment.Background;
			
			Debt = bid.Debt;
			this.bid = bid;
			
			tbxPayment.TextChanged += delegate { tbxPayment.Background = defaultBrush; };
			dpDatePayment.SelectedDateChanged += delegate { dpDatePayment.Background = defaultBrush; };
			
			Title += " (текущий остаток: " + bid.Debt + ")";
			
			lblDebt.Content = bid.Debt.ToString();
			
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			payment = new Payment();
			payment.Id_bid = bid.Id;
			payment.Payment_date = (DateTime)dpDatePayment.SelectedDate;
			payment.Id_manager = Auth.getInstance().Id;
			payment.Paying = double.Parse(tbxPayment.Text);
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(payment.save())
				{
					if((int)Classes.PaymentStatus.NotPaid == bid.Id_payment_status)
					{
						bid.Id_payment_status = (int)Classes.PaymentStatus.PartiallyPaid;
					}
					if(bid.Debt == 0)
					{
						bid.Id_payment_status = (int)Classes.PaymentStatus.Paid;
					}
					if(!bid.save())
					{
						MessageBox.Show(bid.LastError);
						return;
					}
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(); } ));
				}
				else 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(); } ));
				}
			});
		}
		
		private void successSave()
		{
			Close();
		}
		
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			MessageBox.Show(payment.LastError);
		}
		
		private bool validate ()
		{
			try
			{
				double pay = double.Parse(tbxPayment.Text);
				if(pay <= 0)
				{
					tbxPayment.Background = errorBrush;
					return false;
				}
				if(pay > Debt)
				{
					MessageBox.Show("Сумма платежа не может быть больше остатка долга (текущий остаток: " + Debt.ToString() + ")");
					tbxPayment.Background = errorBrush;
					return false;
				}
			}
			catch
			{
				tbxPayment.Background = errorBrush;
				return false;
			}
			if(dpDatePayment.SelectedDate == null)
			{
				dpDatePayment.Background = errorBrush;
				return false;
			}
			return true;
		}
	}
}