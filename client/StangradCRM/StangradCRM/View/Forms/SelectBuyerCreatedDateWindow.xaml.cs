/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 29.09.2016
 * Время: 9:24
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

using Microsoft.Win32;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Forms
{
	/// <summary>
	/// Interaction logic for SelectBuyerCreatedDateWindow.xaml
	/// </summary>
	public partial class SelectBuyerCreatedDateWindow : Window
	{
		public SelectBuyerCreatedDateWindow()
		{
			InitializeComponent();
			
			rbSelectPeriod.Checked += delegate 
			{
				dpSelectDate.Visibility = Visibility.Visible;
			};
			rbSelectPeriod.Unchecked += delegate 
			{
				dpSelectDate.Visibility = Visibility.Hidden;
			};
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			List<Buyer> buyerList;
			SaveFileDialog sfDialog = new SaveFileDialog();
			if(dpSelectDate.Visibility == Visibility.Visible)
			{
				if(dpSelectDate.SelectedDate == null)
				{
					MessageBox.Show("Дата не выбрана!");
					return;
				}
				buyerList = BuyerViewModel.instance().getByMoreDateCreated((DateTime)dpSelectDate.SelectedDate);
				sfDialog.FileName = "Список покупателей, созданных с " + ((DateTime)dpSelectDate.SelectedDate).ToString("dd.MM.yyyy") + ".xlsx";
			}
			else
			{
				buyerList = BuyerViewModel.instance().Collection.ToList();
				sfDialog.FileName = "Список покупателей за весь период.xlsx";
			}
			
			sfDialog.Filter = "Excel 2007 worksheet (*.xlsx)|*.xlsx";
			if(sfDialog.ShowDialog() != true) return;
			
			Reports.BuyerList buyerListReport = new StangradCRM.Reports.BuyerList(buyerList);
			buyerListReport.FileName = sfDialog.FileName;
			
			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => 
              	{
	              	if(!buyerListReport.Save())
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(buyerListReport); } ));
	              	}
	              	else
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(buyerListReport); } ));
	              	}
				});
		}
		
		void successSave (Reports.BuyerList buyerListReport)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show("Список покупателей сохранен по пути " + buyerListReport.FileName);
			System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(buyerListReport.FileName));
			Close();
		}
		
		void errorSave (Reports.BuyerList buyerListReport)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show(buyerListReport.LastError);
		}
	}
}