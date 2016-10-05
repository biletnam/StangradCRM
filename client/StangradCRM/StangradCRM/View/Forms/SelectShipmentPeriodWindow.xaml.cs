/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.09.2016
 * Время: 13:33
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;
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
	/// Interaction logic for SelectShipmentPeriodWindow.xaml
	/// </summary>
	public partial class SelectShipmentPeriodWindow : Window
	{
		public SelectShipmentPeriodWindow()
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
			
			Microsoft.Win32.SaveFileDialog sfDialog = new Microsoft.Win32.SaveFileDialog();
			sfDialog.FileName = "Отгружено с " 
				+ ((DateTime)dpSelectDateStart.SelectedDate).ToString("dd.MM.yyyy")
				+ " по " 
				+ ((DateTime)dpSelectDateEnd.SelectedDate).ToString("dd.MM.yyyy")
				+ ".xlsx";
			sfDialog.Filter = "Excel 2007 worksheet (*.xlsx)|*.xlsx";
			
			if(sfDialog.ShowDialog() != true) return;
			
			List<Bid> bids = BidViewModel.instance()
				.Collection.Where(x => (x.Shipment_date >= (DateTime)dpSelectDateStart.SelectedDate) 
				                  && (x.Shipment_date <= (DateTime)dpSelectDateEnd.SelectedDate)).ToList();
			
			StangradCRM.Reports.ShipmentPeriod shipmentPeriod 
				= new StangradCRM.Reports.ShipmentPeriod(bids,
				                                   (DateTime)dpSelectDateStart.SelectedDate,
				                                   (DateTime)dpSelectDateEnd.SelectedDate);
			
			shipmentPeriod.FileName = sfDialog.FileName;

			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => 
              	{
	              	if(!shipmentPeriod.Save())
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(shipmentPeriod); } ));
	              	}
	              	else
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(shipmentPeriod); } ));
	              	}
				});	
		}
		
		void successSave (Reports.ShipmentPeriod shipmentPeriod)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show("Список покупателей сохранен по пути " + shipmentPeriod.FileName);
			System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(shipmentPeriod.FileName));
			Close();
		}
		
		void errorSave (Reports.ShipmentPeriod shipmentPeriod)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show(shipmentPeriod.LastError);
		}
		
	}
}