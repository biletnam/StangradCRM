/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:10
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for BidStatusSaveWindow.xaml
	/// </summary>
	public partial class BidStatusSaveWindow : Window
	{
		private BidStatus bidStatus = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public BidStatusSaveWindow(BidStatus bidStatus = null)
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			if(bidStatus != null)
			{
				Title = "Редактирование статуса заявки (" + bidStatus.Name + ")";
				tbxName.Text = bidStatus.Name;
				try
				{
					cpRowColor.SelectedColor = (Color)ColorConverter.ConvertFromString(bidStatus.Record_color);
				}
				catch {}
				this.bidStatus = bidStatus;
			}
			else
			{
				this.bidStatus = new BidStatus();
			}
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			bidStatus.Name = tbxName.Text;
			bidStatus.Record_color = cpRowColor.SelectedColor.ToString();
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(bidStatus.save())
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(); } ));
				}
				else 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(); } ));
				}
			});
		}
		
		private void successSave()
		{
			Close();
		}
		
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			MessageBox.Show(bidStatus.LastError);
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		private bool validate ()
		{
			if(tbxName.Text == "")
			{
				tbxName.Background = errorBrush;
				return false;
			}
			return true;
		}		
		
	}
}