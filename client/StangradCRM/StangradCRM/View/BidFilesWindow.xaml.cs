/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 16.05.2017
 * Time: 16:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;
using StangradCRM.Model;
using StangradCRMLibs;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for BidFilesWindow.xaml
	/// </summary>
	public partial class BidFilesWindow : Window
	{
		public BidFilesWindow(Bid bid)
		{
			InitializeComponent();
			
			Title = "Файлы заявки № " + bid.Id.ToString();
			
			DataContext = new {
				BidFilesCollection = bid.BidFilesCollection
			};
			
		}
		
		void BtnDownload_Click(object sender, RoutedEventArgs e)
		{
			BidFiles bidFile = dgvBidFiles.SelectedItem as BidFiles;
			if(bidFile == null) return;
			
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = bidFile.Ext + " |*" + bidFile.Ext;
			dialog.FileName = bidFile.Name;
			
			if(dialog.ShowDialog() == false) return;

			string url = Settings.uri.GetLeftPart(UriPartial.Authority) + "/" + bidFile.Path;
			
			MessageBox.Show(url);
			
			WebClient webClient = new WebClient();
			webClient.DownloadFile(url, dialog.FileName);
		}
	}
}