/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 24.08.2016
 * Время: 18:58
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.MainWindows
{
	/// <summary>
	/// Interaction logic for TechnicalMainWindow.xaml
	/// </summary>
	public partial class TechnicalMainWindow : Window
	{
		public TechnicalMainWindow()
		{
			InitializeComponent();
			Title += " v" + Settings.version + ". Пользователь " + Auth.getInstance().Full_name + ". Режим специалиста технического отдела.";

			int updateTime = 60000;
			try
			{
				updateTime = int.Parse(CRMSettingViewModel.instance().getByMashineName("bid_update_time").Setting_value) * 1000;
			}
			catch {}
			 
			Classes.UpdateTask.Start(Dispatcher, 
			                         	new Action (() => { BidViewModel.instance().RemoteLoad(); }),
			                         	updateTime,
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }));
			
			tiEquipmentBid.Content = new View.Controls.TechnicalControls.MainControlInWork();
			tiArchiveEquipmentBid.Content = new View.Controls.TechnicalControls.MainControlArchive();
		}
		
		void MiExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void MiDataRefresh_Click(object sender, RoutedEventArgs e)
		{
			Classes.UpdateTask.StartSingle(Dispatcher, 
	                                  	new Action (() => { 
			                                           	BidViewModel.instance().RemoteLoad();
			                                           	BuyerViewModel.instance().RemoteLoad();
			                                           	ComplectationItemViewModel.instance().RemoteLoad();
		                                           }),
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }));
		}
	}
}