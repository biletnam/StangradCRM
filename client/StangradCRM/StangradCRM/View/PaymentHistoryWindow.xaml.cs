/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 31.08.2016
 * Время: 11:34
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRM.Core;
using StangradCRM.Model;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for PaymentHistoryWindow.xaml
	/// </summary>
	public partial class PaymentHistoryWindow : Window
	{
		Bid bid = null;
		CollectionViewSource viewSource = new CollectionViewSource();
		TSObservableCollection<Payment> paymentCollection = null;
		public PaymentHistoryWindow(Bid bid)
		{
			InitializeComponent();
			paymentCollection = bid.PaymentCollection;
			viewSource.Source = paymentCollection;
			viewSource.SortDescriptions.Add(new SortDescription("Payment_date", ListSortDirection.Descending));
			DataContext = new
			{
				PaymentHistoryCollection = viewSource.View
			};
			Title += bid.Id.ToString();
			this.bid = bid;
		}
		
		void BtnPaymentRemove_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			Payment payment = row.Item as Payment;
			if(payment == null) return;
			
			if(MessageBox.Show("Удалить платеж?", "Удалить платеж?", MessageBoxButton.YesNo) 
			   == MessageBoxResult.No) return;
			
			int new_bid_payment_status = bid.Id_payment_status;
			
			if(!payment.remove())
			{
				MessageBox.Show(payment.LastError);
				return;
			}
			if(paymentCollection.Contains(payment))
			{
				paymentCollection.Remove(payment);
			}
			if(bid.Debt > 0)
			{
				new_bid_payment_status = (int)Classes.PaymentStatus.PartiallyPaid;
			}			
			if(bid.Debt == bid.Amount)
			{
				new_bid_payment_status = (int)Classes.PaymentStatus.NotPaid;
			}
			if(bid.Id_payment_status != new_bid_payment_status)
			{
				bid.Id_payment_status = new_bid_payment_status;
				if(!bid.save())
				{
					MessageBox.Show("Не удалось изменить статус оплаты!\n" + bid.LastError);
					return;
				}
				bid.raiseAllProperties();
			}
		}
	}
}