/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 17:16
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for PaymentStatusSaveWindow.xaml
	/// </summary>
	public partial class PaymentStatusSaveWindow : Window
	{
		private PaymentStatus paymentStatus = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public PaymentStatusSaveWindow(PaymentStatus paymentStatus = null)
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			if(paymentStatus != null)
			{
				Title = "Редактирование статуса оплаты (" + paymentStatus.Name + ")";
				tbxName.Text = paymentStatus.Name;
				try
				{
					cpRowColor.SelectedColor = (Color)ColorConverter.ConvertFromString(paymentStatus.Record_color);
				}
				catch {}
				this.paymentStatus = paymentStatus;
			}
			else
			{
				this.paymentStatus = new PaymentStatus();
			}
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			paymentStatus.Name = tbxName.Text;
			paymentStatus.Record_color = cpRowColor.SelectedColor.ToString();

			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(paymentStatus.save())
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
			MessageBox.Show(paymentStatus.LastError);
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