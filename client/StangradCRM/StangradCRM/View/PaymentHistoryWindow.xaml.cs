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

using StangradCRM.Model;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for PaymentHistoryWindow.xaml
	/// </summary>
	public partial class PaymentHistoryWindow : Window
	{
		public PaymentHistoryWindow(Bid bid)
		{
			InitializeComponent();
			CollectionViewSource viewSource = new CollectionViewSource();
			viewSource.Source = bid.PaymentCollection;
			viewSource.SortDescriptions.Add(new SortDescription("Payment_date", ListSortDirection.Descending));
			DataContext = new
			{
				PaymentHistoryCollection = viewSource.View
			};
			Title += bid.Id.ToString();
		}
	}
}