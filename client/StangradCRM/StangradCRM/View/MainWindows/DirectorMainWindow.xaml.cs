﻿/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 24.08.2016
 * Время: 18:58
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows;

using StangradCRM.View.Controls.DirectorControls;
using StangradCRM.View.Forms;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.MainWindows
{
	/// <summary>
	/// Interaction logic for DirectorMainWindow.xaml
	/// </summary>
	public partial class DirectorMainWindow : Window
	{
		public DirectorMainWindow()
		{
			InitializeComponent();
			Title += " v" + Settings.version + ". Пользователь " + Auth.getInstance().Full_name + ". Режим директора.";

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
			
			tiMyBid.Content = new MainControlMyBid();
			tiManagerBid.Content = new MainControlManagerBid();
			tiArchiveBid.Content = new MainControlArchiveBid();
		}

		void MiExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void MiDataRefresh_Click(object sender, RoutedEventArgs e)
		{
			Classes.UpdateTask.StartSingle(Dispatcher, 
		                               new Action (() => {
			                                           	BuyerViewModel.instance().reload();
			                                           	ComplectationItemViewModel.instance().reload();
			                                           	BidViewModel.instance().reload();
			                                           	//BidViewModel.instance().RemoteLoad();
			                                           	//BuyerViewModel.instance().RemoteLoad();
			                                           	//ComplectationItemViewModel.instance().RemoteLoad();
		                                           }),
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }));
		}
		
		void MiBuyerList_Click(object sender, RoutedEventArgs e)
		{
			SelectBuyerCreatedDateWindow window
				= new SelectBuyerCreatedDateWindow();
			window.ShowDialog();
		}
		
		void MiTurnover_Click(object sender, RoutedEventArgs e)
		{
			SelectSellerAndPeriod window = new SelectSellerAndPeriod();
			window.ShowDialog();
		}
		
		void MiEquipmentShipment_Click(object sender, RoutedEventArgs e)
		{
			SelectShipmentPeriodWindow window = new SelectShipmentPeriodWindow();
			window.ShowDialog();
		}
		
		void MiBuyerHistory_Click(object sender, RoutedEventArgs e)
		{
			SelectBuyerPeriodWindow window = new SelectBuyerPeriodWindow();
			window.ShowDialog();
		}
		
		void MiIndicators_Click(object sender, RoutedEventArgs e)
		{
			IndicatorsPeriodWindow window = new IndicatorsPeriodWindow();
			window.ShowDialog();
		}
		void MiTurnoverForAllSeller_Click(object sender, RoutedEventArgs e)
		{
			TurnoverForAllSellerPeriodWindow window = new TurnoverForAllSellerPeriodWindow();
			window.ShowDialog();
		}
		
		void MiOpenBuyerWindow_Click(object sender, RoutedEventArgs e)
		{
			BuyerWindow w = new BuyerWindow();
			w.ShowDialog();
		}
	}
}