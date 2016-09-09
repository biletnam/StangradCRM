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
			
			int updateTime = 60000;
			try
			{
				updateTime = int.Parse(CRMSettingViewModel.instance().getByMashineName("bid_update_time").Setting_value);
			}
			catch {}
			 
			Classes.BidUpdateTask.Start(Dispatcher, updateTime, 
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }));
			
			menuOpenEquipmentWindow.Click += delegate
			{
				EquipmentWindow window = new EquipmentWindow();
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
			
			tiCRMSetting.Content = new MainControlCRMSetting();
			tiBid.Content = new MainControlBid();
			tiArchiveBid.Content = new MainControlArchiveBid();
		}
		
		void MiExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}