/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:41
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
	/// Interaction logic for BuyerSaveWindow.xaml
	/// </summary>
	public partial class BuyerSaveWindow : Window
	{
		private Buyer buyer = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));		
		
		public BuyerSaveWindow(Buyer buyer = null)
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			
			if(buyer != null)
			{
				Title = "Редактирование покупателя (" + buyer.Name + ")";
				tbxName.Text = buyer.Name;
				tbxContactPerson.Text = buyer.Contact_person;
				tbxPhone.Text = buyer.Phone;
				tbxEmail.Text = buyer.Email;
				tbxCity.Text = buyer.City;
				this.buyer = buyer;
			}
			else
			{
				this.buyer = new Buyer();
			}
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			buyer.Name = tbxName.Text;
			buyer.Contact_person = tbxContactPerson.Text;
			buyer.Phone = tbxPhone.Text;
			buyer.Email = tbxEmail.Text;
			buyer.City = tbxCity.Text;
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(buyer.save())
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
			MessageBox.Show(buyer.LastError);
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