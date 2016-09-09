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
				updateTime = int.Parse(CRMSettingViewModel.instance().getByMashineName("bid_update_time").Setting_value);
			}
			catch {}
			 
			Classes.BidUpdateTask.Start(Dispatcher, updateTime, 
			                            new Action (() => { updateNotification.Visibility = Visibility.Hidden; }), 
			                            new Action (() => { updateNotification.Visibility = Visibility.Visible; }));
			
		}
	}
}