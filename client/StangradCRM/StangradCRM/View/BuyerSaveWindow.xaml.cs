/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:41
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Classes;
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
				tbxPassportSerialNumber.Text = buyer.Passport_serial_number;
				try
				{
					dpPassportIssueDate.SelectedDate = DateTime.Parse(buyer.Passport_issue_date);
				} catch {}
				
				tbxInn.Text = buyer.Inn;
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
			
			if(tbxPassportSerialNumber.Text != "____ ______")
			{
				buyer.Passport_serial_number = tbxPassportSerialNumber.Text;
			}
			else
			{
				buyer.Passport_serial_number = "";
			}
			
			if(dpPassportIssueDate.SelectedDate != null)
			{
				buyer.Passport_issue_date = dpPassportIssueDate.SelectedDate.Value.ToString("dd.MM.yyyy");
			}
			else
			{
				buyer.Passport_issue_date = "";
			}
			
			buyer.Inn = tbxInn.Text;
			
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
			if(tbxEmail.Text != "" && !IsEmail.Valid(tbxEmail.Text)) {
				tbxEmail.Background = errorBrush;
				return false;
			}
			return true;
		}
		
		void TbxPassportSerial_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = !IsNumberic.Valid(e.Text);
		}
		
		void TbxPassportNumber_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = !IsNumberic.Valid(e.Text);
		}
		
		void TbxInn_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = !IsNumberic.Valid(e.Text);
		}
	}
}