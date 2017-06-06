/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 05/03/2017
 * Time: 17:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
	/// Interaction logic for LogisticianMainWindow.xaml
	/// </summary>
	public partial class LogisticianMainWindow : Window
	{

		public LogisticianMainWindow()
		{
			InitializeComponent();
			Title += " v" + Settings.version + ". Пользователь " + Auth.getInstance().Full_name + ". Режим логиста.";
			
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
			
			CollectionViewSource newViewSource = new CollectionViewSource();
			CollectionViewSource inWorkViewSource = new CollectionViewSource();
			CollectionViewSource forShipmentSource = new CollectionViewSource();
			CollectionViewSource archiveViewSource = new CollectionViewSource();
			
			tiNewBid.Content = new View.Controls.LogisticianControls.NewsControl(newViewSource, inWorkViewSource);
			tiInWorkBid.Content = new View.Controls.LogisticianControls.InWorkControl(inWorkViewSource, forShipmentSource);
			tiForShipment.Content = new View.Controls.LogisticianControls.ForShipmentControl(forShipmentSource, archiveViewSource);
			tiArchiveBid.Content = new View.Controls.LogisticianControls.ArchiveControl(archiveViewSource);
			
		}
		
		void MenuItem_Click(object sender, RoutedEventArgs e)
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
		                                           }),
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }));
		}		
		
	}
}