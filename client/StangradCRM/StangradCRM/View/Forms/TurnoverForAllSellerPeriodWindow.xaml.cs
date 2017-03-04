/*
 * Created by SharpDevelop.
 * User: Дмитрий
 * Date: 03.03.2017
 * Time: 12:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using StangradCRM.Model;
using StangradCRM.ViewModel;
using System.Linq;


namespace StangradCRM.View.Forms
{
	/// <summary>
	/// Interaction logic for TurnoverForAllSellerPeriodWindow.xaml
	/// </summary>
	public partial class TurnoverForAllSellerPeriodWindow : Window
	{
		public TurnoverForAllSellerPeriodWindow()
		{
			InitializeComponent();
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
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
				+ ".xlsx";
			sfDialog.Filter = "Excel 2007 worksheet (*.xlsx)|*.xlsx";
			
			if(sfDialog.ShowDialog() != true) return;
			
			StangradCRM.Reports.TurnoverAllSeller turnoverAllSeller 
				= new StangradCRM.Reports.TurnoverAllSeller(payment, SellerViewModel.instance().Collection.ToList(),
				                                   (DateTime)dpSelectDateStart.SelectedDate,
				                                   (DateTime)dpSelectDateEnd.SelectedDate);
			
			turnoverAllSeller.FileName = sfDialog.FileName;

			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => 
          	{
              	if(!turnoverAllSeller.Save())
              	{
              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(turnoverAllSeller); } ));
              	}
              	else
              	{
              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(turnoverAllSeller); } ));
              	}
			});		
		}
		
		void successSave (Reports.TurnoverAllSeller turnoverAllSeller)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show("Отчет по поступлениям сохранен по пути " + turnoverAllSeller.FileName);
			System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(turnoverAllSeller.FileName));
			Close();
		}
		
		void errorSave (Reports.TurnoverAllSeller turnoverAllSeller)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show(turnoverAllSeller.LastError);
		}
	}
}