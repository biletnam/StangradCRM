/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 16:44
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
	/// Interaction logic for SellerSaveWindow.xaml
	/// </summary>
	public partial class SellerSaveWindow : Window
	{
		private Seller seller = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public SellerSaveWindow(Seller seller = null)
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			
			if(seller != null)
			{
				Title = "Редактирование продавца (" + seller.Name + ")";
				tbxName.Text = seller.Name;
				if(seller.Hidden == 0)
				{
					cbxHidden.SelectedIndex = 0;
				}
				else
				{
					cbxHidden.SelectedIndex = 1;
				}
				this.seller = seller;
			}
			else
			{
				this.seller = new Seller();
				cbxHidden.SelectedIndex = 0;
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
			seller.Hidden = cbxHidden.SelectedIndex;
			seller.Name = tbxName.Text;
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(seller.save())
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
			MessageBox.Show(seller.LastError);
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