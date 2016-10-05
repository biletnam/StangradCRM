/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08.08.2016
 * Время: 16:03
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

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.View.Controls;
using StangradCRM.View.Controls.ManagerControls;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.MainWindows
{
	/// <summary>
	/// Interaction logic for ManagerMainWindow.xaml
	/// </summary>
	public partial class ManagerMainWindow : Window
	{
		public ManagerMainWindow()
		{
			InitializeComponent();
			Title += " v" + Settings.version + ". Пользователь " + Auth.getInstance().Full_name + ". Режим менеджера-оператора.";
			
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
			
			List<BidStatus> bidStatus = BidStatusViewModel.instance().Collection.ToList();
			for(int i = 0; i < bidStatus.Count; i++)
			{
				try
				{
					TabItem tabItem = new TabItem();
					tabItem.Header = bidStatus[i].Name;
					tcMain.Items.Add(tabItem);
					tabItem.Content = View.Helpers.ManagerMainWindowHelper.GetControl(bidStatus[i].Id);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			TabItem archiveTabItem = new TabItem();
			archiveTabItem.Header = "Архив заявок";
			tcMain.Items.Add(archiveTabItem);
			archiveTabItem.Content = new MainControlArchive();
		}
		
		void MenuItem_Click(object sender, RoutedEventArgs e)
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