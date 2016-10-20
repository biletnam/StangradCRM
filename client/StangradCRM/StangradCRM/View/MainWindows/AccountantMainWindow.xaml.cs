/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 24.08.2016
 * Время: 18:59
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRM.Model;
using StangradCRM.View.Controls.AccountantControls;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.MainWindows
{
	/// <summary>
	/// Interaction logic for AccountantMainWindow.xaml
	/// </summary>
	public partial class AccountantMainWindow : Window
	{
		public AccountantMainWindow()
		{
			InitializeComponent();
			Title += " v" + Settings.version + ". Пользователь " + Auth.getInstance().Full_name + ". Режим бухгалтера.";
			
			try
			{
				int updateTime = int.Parse(CRMSettingViewModel.instance().getByMashineName("bid_update_time").Setting_value) * 1000;
				if(updateTime != 0)
					Classes.UpdateTask.Start(Dispatcher,
					                            new Action (() => { BidViewModel.instance().reload(); }),
					                            updateTime,
					                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }),
					                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }));
			}
			catch {}
			
			tiNewBid.Content = new MainControlNewBid();
			tiInWorkBid.Content = new MainControlInWork();
			tiArchiveBid.Content = new MainControlArchiveBid();
			
		}
		void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void MiDataRefresh_Click(object sender, RoutedEventArgs e)
		{
			Classes.UpdateTask.StartSingle(Dispatcher, 
	                                  	new Action (() => { 

			                                           	BidViewModel.instance().reload();
			                                           	BuyerViewModel.instance().reload();
			                                           	ComplectationItemViewModel.instance().reload();
		                                           }),
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }));
		}
	}
}