/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 24.08.2016
 * Время: 18:42
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

using StangradCRM.View.Controls.AdministratorControls;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.MainWindows
{
	/// <summary>
	/// Interaction logic for AdministratorMainWindow.xaml
	/// </summary>
	public partial class AdministratorMainWindow : Window
	{
		public AdministratorMainWindow()
		{
			InitializeComponent();
			Title += " v" + Settings.version + ". Пользователь " + Auth.getInstance().Full_name + ". Режим администратора.";
			
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
			
			menuOpenEquipmentWindow.Click += delegate
			{
				EquipmentWindow window = new EquipmentWindow();
				window.ShowDialog();
			};
			
			menuOpenComplectationItemWindow.Click += delegate 
			{
				ComplectationItemWindow window = new ComplectationItemWindow();
				window.ShowDialog();
			};
			
			menuOpenBuyerWindow.Click += delegate 
			{
				BuyerWindow window = new BuyerWindow();
				window.ShowDialog();
			};
			
			menuOpenSellerWindow.Click += delegate 
			{
				SellerWindow window = new SellerWindow();
				window.ShowDialog();
			};
			
			menuOpenBidStatusWindow.Click += delegate 
			{
				BidStatusWindow window = new BidStatusWindow();
				window.ShowDialog();
			};
			
			menuOpenPaymentStatusWindow.Click += delegate 
			{
				PaymentStatusWindow window = new PaymentStatusWindow();
				window.ShowDialog();
			};
			
			menuOpenManagerWindow.Click += delegate
			{
				ManagerWindow window = new ManagerWindow();
				window.ShowDialog();
			};
			
			menuOpenTransportCompanyWindow.Click += delegate 
			{
				TransportCompanyWindow window = new TransportCompanyWindow();
				window.ShowDialog();
			};
			
			menuOpenMessageTemplatesWindow.Click += delegate 
			{
				MessageTemplatesWindow window = new MessageTemplatesWindow();
				window.ShowDialog();
			};
			
			tiCRMSetting.Content = new MainControlCRMSetting();
			tiBid.Content = new MainControlBid();
		}
		
		void MiExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void MiDataRefresh_Click(object sender, RoutedEventArgs e)
		{
			Classes.UpdateTask.StartSingle(Dispatcher, 
	                                  	new Action (() => 
		                                           {  
		                                           		BuyerViewModel.instance().reload();
		                                           		ComplectationItemViewModel.instance().reload();
		                                           		BidViewModel.instance().reload();
		                                           }),
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }));
		}
	}
}