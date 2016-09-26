/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 26.09.2016
 * Время: 10:44
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
	/// Interaction logic for ComplectationItemSaveWindow.xaml
	/// </summary>
	public partial class ComplectationItemSaveWindow : Window
	{
		
		ComplectationItem complectationItem = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public ComplectationItemSaveWindow(ComplectationItem complectationItem = null)
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			
			if(complectationItem != null)
			{
				Title = "Редактирование наименования комплектации " + complectationItem.Name;
				tbxName.Text = complectationItem.Name;
				this.complectationItem = complectationItem;
			}
			else
			{
				this.complectationItem = new ComplectationItem();
			}
			
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			
			complectationItem.Name = tbxName.Text;
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(complectationItem.save())
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
			MessageBox.Show(complectationItem.LastError);
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