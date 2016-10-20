/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 04.10.2016
 * Время: 18:13
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

using Microsoft.Win32;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Forms
{
	/// <summary>
	/// Interaction logic for IndicatorsPeriodWindow.xaml
	/// </summary>
	/// 
	
	public partial class IndicatorsPeriodWindow : Window
	{
		
		
		public IndicatorsPeriodWindow()
		{
			InitializeComponent();

			int currentYear = DateTime.Now.Year;
			int minYear = BidViewModel.instance().GetDateCreatedFirsBid().Year;
			for(int i = minYear; i < currentYear + 1; i++)
			{
				cbxYear.Items.Add(i);
			}

			
			cbxMonth.Items.Add("Январь");
			cbxMonth.Items.Add("Февраль");
			cbxMonth.Items.Add("Март");
			cbxMonth.Items.Add("Апрель");
			cbxMonth.Items.Add("Май");
			cbxMonth.Items.Add("Июнь");
			cbxMonth.Items.Add("Июль");
			cbxMonth.Items.Add("Август");
			cbxMonth.Items.Add("Сентябрь");
			cbxMonth.Items.Add("Октябрь");
			cbxMonth.Items.Add("Ноябрь");
			cbxMonth.Items.Add("Декабрь");
						
			cbxYear.SelectedIndex = cbxYear.Items.Count-1;			
			cbxMonth.SelectedIndex = -1;
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			int year = (int)cbxYear.SelectedValue;
			int month = cbxMonth.SelectedIndex+1;

			SaveFileDialog sfDialog = new SaveFileDialog();
			sfDialog.FileName = "Показатели за " + year.ToString() + " г. ";
			if(month != 0)
			{
				sfDialog.FileName += Classes.Months.getRuMonthNameByNumber(month) + ".xlsx";
			}
			else
			{
				sfDialog.FileName += ".xlsx";
			}
			sfDialog.Filter = "Excel 2007 worksheet (*.xlsx)|*.xlsx";
			if(sfDialog.ShowDialog() != true) return;
			
			Reports.Indicators indicators = 
				new StangradCRM.Reports.Indicators(BidViewModel.instance().GetByYearAndMonth(year, month), year, month);
			indicators.FileName = sfDialog.FileName;
			
			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => 
              	{
	              	if(!indicators.Save())
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(indicators); } ));
	              	}
	              	else
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(indicators); } ));
	              	}
				});
		}
		
		void successSave (Reports.Indicators indicators)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show("Анализ показателей сохранен по пути " + indicators.FileName);
			System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(indicators.FileName));
			Close();
		}
		
		void errorSave (Reports.Indicators indicators)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			MessageBox.Show(indicators.LastError);
		}
		
		void BtnClearSelectedMonth_Click(object sender, RoutedEventArgs e)
		{
			cbxMonth.SelectedIndex = -1;
		}
		
	}
}