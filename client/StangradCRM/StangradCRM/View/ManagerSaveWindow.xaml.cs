/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 18:23
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for ManagerSaveWindow.xaml
	/// </summary>
	public partial class ManagerSaveWindow : Window
	{
		
		private Manager manager = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public ManagerSaveWindow(Manager manager = null)
		{
			InitializeComponent();
			defaultBrush = tbxLogin.Background;
			
			cbxRole.ItemsSource = RoleViewModel.instance().Collection;
			
			if(manager != null)
			{
				Title = "Редактирование пользователя (" + manager.Name + ")";
				tbxName.Text = manager.Name;
				tbxLogin.Text = manager.Login;
				cbxRole.SelectedItem = RoleViewModel.instance().getById(manager.Id_role);
				this.manager = manager;
				
				tbxPassword.Visibility = Visibility.Collapsed;
				tbxRepeatPassword.Visibility = Visibility.Collapsed;
				
				lbPassword.Visibility = Visibility.Collapsed;
				lbPasswordRepeat.Visibility = Visibility.Collapsed;
				
				chbxChangePassword.Click += delegate 
				{
					if(chbxChangePassword.IsChecked == true)
					{
						tbxPassword.Visibility = Visibility.Visible;
						tbxRepeatPassword.Visibility = Visibility.Visible;
						
						lbPassword.Visibility = Visibility.Visible;
						lbPasswordRepeat.Visibility = Visibility.Visible;
					}
					else
					{
						tbxPassword.Visibility = Visibility.Collapsed;
						tbxRepeatPassword.Visibility = Visibility.Collapsed;
						
						lbPassword.Visibility = Visibility.Collapsed;
						lbPasswordRepeat.Visibility = Visibility.Collapsed;
					}
				};
				
			}
			else
			{
				chbxChangePassword.Visibility = Visibility.Collapsed;
				this.manager = new Manager();
			}
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
			tbxLogin.TextChanged += delegate { tbxLogin.Background = defaultBrush; };
			cbxRole.SelectionChanged += delegate { cbxRole.Background = defaultBrush; };
			tbxPassword.PasswordChanged += delegate { tbxPassword.Background = defaultBrush; };
			tbxRepeatPassword.PasswordChanged += delegate { tbxRepeatPassword.Background = defaultBrush; };
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			manager.Name = tbxName.Text;
			manager.Login = tbxLogin.Text;
			manager.Id_role = (Int32)cbxRole.SelectedValue;
			manager.Password = tbxPassword.Password;
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(manager.save())
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
			MessageBox.Show(manager.LastError);
		}
		
		private bool validate ()
		{
			if(tbxName.Text == "")
			{
				tbxName.Background = errorBrush;
				return false;
			}
			if(tbxLogin.Text == "")
			{
				tbxLogin.Background = errorBrush;
				return false;
			}
			if(cbxRole.SelectedIndex == -1)
			{
				cbxRole.Background = errorBrush;
				return false;
			}
			if(tbxPassword.Password != "" || tbxRepeatPassword.Password != "")
			{
				if(tbxPassword.Password.Length < 6)
				{
					MessageBox.Show("Пароль должен быть 6 или более символов!");
					tbxPassword.Background = errorBrush;
					tbxRepeatPassword.Background = errorBrush;
					return false;
				}
				if(tbxRepeatPassword.Password != tbxPassword.Password)
				{
					MessageBox.Show("Введенные пароли не совпадают!");
					tbxPassword.Background = errorBrush;
					tbxRepeatPassword.Background = errorBrush;
					return false;
				}
			}
			return true;
		}
	}
}