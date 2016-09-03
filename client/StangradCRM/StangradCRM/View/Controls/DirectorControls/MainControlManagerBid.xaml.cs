/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 02.09.2016
 * Время: 14:51
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

namespace StangradCRM.View.Controls.DirectorControls
{
	/// <summary>
	/// Interaction logic for MainControlManagerBid.xaml
	/// </summary>
	public partial class MainControlManagerBid : UserControl
	{
		public MainControlManagerBid()
		{
			InitializeComponent();
			List<BidStatus> bidStatus = BidStatusViewModel.instance().Collection.ToList();
			for(int i = 0; i < bidStatus.Count; i++)
			{
				try
				{
					TabItem tabItem = new TabItem();
					tabItem.Header = bidStatus[i].Name;
					tcMain.Items.Add(tabItem);
					tabItem.Content = View.Helpers.DirectorManagerBidControlHelper.GetControl(bidStatus[i].Id);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}
	}
}