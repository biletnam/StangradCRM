/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 05/03/2017
 * Time: 16:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Controls.ManagerControls
{
	/// <summary>
	/// Interaction logic for OtherControl.xaml
	/// </summary>
	public partial class OtherControl : UserControl
	{
		public OtherControl()
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
					return;
				}
			}
			TabItem tabitem = new TabItem();
			tabitem.Header = "Архив";
			tcMain.Items.Add(tabitem);
			tabitem.Content = new OtherManagerArchiveControl();
		}
	}
}