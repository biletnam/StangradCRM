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
			
			Classes.BidUpdateTask.Start(Dispatcher);
			
			List<BidStatus> bidStatus = BidStatusViewModel.instance().Collection.ToList();
			for(int i = 0; i < bidStatus.Count; i++)
			{
				try
				{
					TabItem tabItem = new TabItem();
					tabItem.Header = bidStatus[i].Name;
					tcMain.Items.Add(tabItem);
					tabItem.Content = View.Helpers.AccountantMainWindowHelper.GetControl(bidStatus[i].Id);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}
		void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}