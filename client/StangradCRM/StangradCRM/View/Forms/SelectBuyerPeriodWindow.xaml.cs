/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.09.2016
 * Время: 13:25
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Forms
{
	/// <summary>
	/// Interaction logic for SelectBuyerPeriodWindow.xaml
	/// </summary>
	public partial class SelectBuyerPeriodWindow : Window
	{
		
		Buyer selectedBuyer = null;
		
		public SelectBuyerPeriodWindow()
		{
			InitializeComponent();
			DataContext = new
			{
				BuyerCollection = BuyerViewModel.instance().Collection
			};
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(selectedBuyer == null) 
			{
				MessageBox.Show("Выберите покупателя!");
				return;
			}
			if(dpSelectDateStart.SelectedDate == null 
			   || dpSelectDateEnd.SelectedDate == null)
			{
				MessageBox.Show("Выберите дату с и по");
				return;
			}
			
			List<Bid> bids = BidViewModel.instance().GetByPeriod((DateTime)dpSelectDateStart.SelectedDate,
			                                                     (DateTime)dpSelectDateEnd.SelectedDate)
				.Where(x => (x.Id_buyer == selectedBuyer.Id) && (x.Id_bid_status != (int)Classes.BidStatus.New)).ToList();
				
			
			Microsoft.Win32.SaveFileDialog sfDialog = new Microsoft.Win32.SaveFileDialog();
			sfDialog.FileName = Classes.ReplaceSpecialCharsFileName.Replace("Анализ покупателя " + selectedBuyer.NameWithCity + " "
				+ ((DateTime)dpSelectDateStart.SelectedDate).ToString("dd.MM.yyyy")
				+ " по " 
				+ ((DateTime)dpSelectDateEnd.SelectedDate).ToString("dd.MM.yyyy")
				+ ".xlsx");
			sfDialog.Filter = "Excel 2007 worksheet (*.xlsx)|*.xlsx";
			
			if(sfDialog.ShowDialog() != true) return;
			
			StangradCRM.Reports.BuyerPeriod buyerPeriod 
				= new StangradCRM.Reports.BuyerPeriod(selectedBuyer, bids,
				                                   (DateTime)dpSelectDateStart.SelectedDate,
				                                   (DateTime)dpSelectDateEnd.SelectedDate);
			
			buyerPeriod.FileName = sfDialog.FileName;

			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => 
              	{
	              	if(!buyerPeriod.Save())
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(buyerPeriod); } ));
	              	}
	              	else
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(buyerPeriod); } ));
	              	}
				});
		}
		
		void successSave (Reports.BuyerPeriod buyerPeriod)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show("Список покупателей сохранен по пути " + buyerPeriod.FileName);
			System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(buyerPeriod.FileName));
			Close();
		}
		
		void errorSave (Reports.BuyerPeriod buyerPeriod)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show(buyerPeriod.LastError);
		}
		
		void BtnSelectBuyer_Click(object sender, RoutedEventArgs e)
		{
			BuyerListWindow window = new BuyerListWindow(new Action<Buyer>( (buyer) => { selectedBuyer = buyer; tbxBuyer.Text = buyer.NameWithCity; }));
			window.ShowDialog();
		}
	}
}