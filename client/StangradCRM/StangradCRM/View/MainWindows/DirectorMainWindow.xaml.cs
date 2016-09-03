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

using StangradCRM.View.Controls.DirectorControls;
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

			Classes.BidUpdateTask.Start(Dispatcher);
			
			tiMyBid.Content = new MainControlMyBid();
			tiManagerBid.Content = new MainControlManagerBid();
			tiArchiveBid.Content = new MainControlArchiveBid();
		}

		void MiExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}