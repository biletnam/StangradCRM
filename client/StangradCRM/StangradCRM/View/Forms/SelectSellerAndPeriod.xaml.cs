/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.09.2016
 * Время: 11:55
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
using StangradCRM.ViewModel;

namespace StangradCRM.View.Forms
{
	/// <summary>
	/// Interaction logic for SelectSellerAndPeriod.xaml
	/// </summary>
	public partial class SelectSellerAndPeriod : Window
	{
		public SelectSellerAndPeriod()
		{
			InitializeComponent();
			DataContext = new
			{
				SellerCollection = SellerViewModel.instance().Collection
			};
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(cbxSeller.SelectedIndex == -1) 
			{
				MessageBox.Show("Выберите продавца!");
				return;
			}
			Seller seller = cbxSeller.SelectedItem as Seller;
			if(seller == null)
			{
				MessageBox.Show("Выберите продавца!");
				return;
			}
			if(dpSelectDateStart.SelectedDate == null 
			   || dpSelectDateEnd.SelectedDate == null)
			{
				MessageBox.Show("Выберите дату с и по");
				return;
			}
			List<Payment> payment = PaymentViewModel.instance()
				.GetByPeriod((DateTime)dpSelectDateStart.SelectedDate,
				             (DateTime)dpSelectDateEnd.SelectedDate);
			
			if(payment.Count == 0)
			{
				MessageBox.Show("За выбранный период нет платежей!");
				return;
			}
			
			Microsoft.Win32.SaveFileDialog sfDialog = new Microsoft.Win32.SaveFileDialog();
			sfDialog.FileName = "Поступления с " 
				+ ((DateTime)dpSelectDateStart.SelectedDate).ToString("dd.MM.yyyy")
				+ " по " 
				+ ((DateTime)dpSelectDateEnd.SelectedDate).ToString("dd.MM.yyyy")
				+ " для продавца " + seller.Name + ".xlsx";
			sfDialog.Filter = "Excel 2007 worksheet (*.xlsx)|*.xlsx";
			
			if(sfDialog.ShowDialog() != true) return;
			
			StangradCRM.Reports.Receipts receipts 
				= new StangradCRM.Reports.Receipts(payment, seller,
				                                   (DateTime)dpSelectDateStart.SelectedDate,
				                                   (DateTime)dpSelectDateEnd.SelectedDate);
			
			receipts.FileName = sfDialog.FileName;

			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => 
              	{
	              	if(!receipts.Save())
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(receipts); } ));
	              	}
	              	else
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(receipts); } ));
	              	}
				});			
		}
		
		void successSave (Reports.Receipts receipts)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show("Список покупателей сохранен по пути " + receipts.FileName);
			System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(receipts.FileName));
			Close();
		}
		
		void errorSave (Reports.Receipts receipts)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show(receipts.LastError);
		}
	}
}