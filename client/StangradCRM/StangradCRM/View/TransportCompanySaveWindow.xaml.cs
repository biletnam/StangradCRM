/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 22.08.2016
 * Время: 18:16
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for TransportCompanySaveWindow.xaml
	/// </summary>
	public partial class TransportCompanySaveWindow : Window
	{
		private TransportCompany transportCompany = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));		
		
		public TransportCompanySaveWindow(TransportCompany transportCompany = null)
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			
			cbxMessageTemplate.ItemsSource = MessageTemplatesViewModel.instance().Collection;
			
			if(transportCompany != null)
			{
				Title = "Редактирование транспортной компании (" + transportCompany.Name + ")";
				tbxName.Text = transportCompany.Name;
				tbxSite.Text = transportCompany.Site;
				cbxMessageTemplate.SelectedValue = transportCompany.Id_message_template;
				
				this.transportCompany = transportCompany;
			}
			else
			{
				this.transportCompany = new TransportCompany();
			}
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
			cbxMessageTemplate.SelectionChanged += delegate { cbxMessageTemplate.Background = defaultBrush; };
			
			
			
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			transportCompany.Name = tbxName.Text;
			transportCompany.Id_message_template = (int)cbxMessageTemplate.SelectedValue;
			transportCompany.Site = tbxSite.Text;
			
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(transportCompany.save())
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
			MessageBox.Show(transportCompany.LastError);
		}
		
		private bool validate ()
		{
			if(tbxName.Text == "")
			{
				tbxName.Background = errorBrush;
				return false;
			}
			if(cbxMessageTemplate.SelectedIndex == -1)
			{
				cbxMessageTemplate.Background = errorBrush;
				return false;
			}
			return true;
		}		
	}
}