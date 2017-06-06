/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08.08.2016
 * Время: 16:06
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Threading;
using StangradCRMLibs;

namespace Launcher
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	/// 

	public partial class MainWindow : Window
	{
		
		private IniFile settings = new IniFile("Settings.ini");
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));

		private string version = "0.00";
		private string appName = "StangradCRM.exe";
		
		public MainWindow()
		{
			InitializeComponent();
			defaultBrush = tbxLogin.Background;
			
			init();
		}
		
		private void init() {
			if(this.settings.KeyExists("login")) 
				tbxLogin.Text = this.settings.Read("login");
			if(this.settings.KeyExists("version"))
				this.version = this.settings.Read("version");
			if(this.settings.KeyExists("app"))
				this.appName = this.settings.Read("app");				
			if(this.settings.KeyExists("host")) {
				tbxHost.Text = this.settings.Read("host");
			}
			else {
				this.toggleHostSettingsVisible(true);
			}
			if(this.settings.KeyExists("pass")) {
				try
				{
					this.tbxPassword.Password = PasswordDecrypt(this.settings.Read("pass"), tbxHost.Text + "-" + tbxLogin.Text);
				} catch {}
			}
			tbxLogin.TextChanged += 
				delegate { tbxLogin.Background = defaultBrush; };
			tbxPassword.PasswordChanged +=
				delegate { tbxPassword.Background = defaultBrush; };
			tbxHost.TextChanged += 
				delegate { tbxHost.Background = defaultBrush; };
			tbxPassword.KeyUp += 
				(sender, e) => { if(e.Key == Key.Enter) this.login(); };
			
			btnLogin.Click += delegate 
			{
				login();
			};
			
			cbxFullSettingsToggle.Click +=
				delegate { this.toggleHostSettingsVisible((bool)cbxFullSettingsToggle.IsChecked); };
			cbxFullSettingsToggle.Unchecked += 
				delegate { this.toggleHostSettingsVisible((bool)cbxFullSettingsToggle.IsChecked); };
			cbxFullSettingsToggle.Checked += 
				delegate { this.toggleHostSettingsVisible((bool)cbxFullSettingsToggle.IsChecked); };
			lblFullSettingToggle.MouseUp += 
				delegate { this.toggleHostSettingsVisible((bool)cbxFullSettingsToggle.IsChecked); };
		}
		
		private void toggleHostSettingsVisible (bool visible) {
			if(visible) {
				this.rowSettings.Visibility = Visibility.Visible;
			}
			else {
				this.rowSettings.Visibility = Visibility.Collapsed;
			}
		}	

		private bool validateFields() {
			if(tbxLogin.Text == "") {
				tbxLogin.Background = errorBrush;
				return false;
			}
			if(tbxPassword.Password == "") {
				tbxPassword.Background = errorBrush;
				return false;
			}
			if(tbxHost.Text == "") {
				tbxHost.Background = errorBrush;
				return false;
			}
			return true;
		}
		
		private void memberAuthData () {
			settings.Write("save", "true");
			settings.Write("login", tbxLogin.Text);
			settings.Write("pass", PasswordEncrypt(tbxPassword.Password, tbxHost.Text + "-" + tbxLogin.Text));
			settings.Write("host", tbxHost.Text);
			settings.Write("app", this.appName);
		}
		
		private void runApp () {			
			Updater updater = new Updater();
			Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => {lblCurrentStatus.Content = "Проверяю обновление...";}));
			if(!updater.CheckUpdate(version))
			{
				if(updater.IsError)
				{
					MessageBox.Show("Ошибка проверки обновления!\n" + updater.LastError);
				}
			}
			else
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => {lblCurrentStatus.Content = "Выполняю обновление...";}));
				if(!updater.DownloadUpdate() && updater.IsError)
				{
					MessageBox.Show("Ошибка загрузки обновления!\n" + updater.LastError);
				}
			}
		
          	string cookie = HTTPManager.HTTPRequest.CurrentCookie.GetCookies(Settings.uri)[0].ToString();
          	Process process = new Process();
          	process.StartInfo.FileName = appName;			                      	
          	process.StartInfo.Arguments = Settings.uri.ToString() + 
          		" " + cookie;
          	process.Start();
          	Dispatcher.BeginInvoke(DispatcherPriority.Background,
          	                       new Action( () => { Close(); }));
		}
		
		private void login () {
			if(!this.validateFields()) return;
			Uri uri;
			try {
				uri = new Uri(tbxHost.Text);
				Settings.uri = uri;
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			loadingProgress.Visibility = Visibility.Visible;
			
			btnLogin.IsEnabled = false;
			tbxPassword.IsEnabled = false;
			
			Auth auth = Auth.getInstance();
			auth.Login = tbxLogin.Text;
			auth.Password = tbxPassword.Password;
			auth.Entity = "Auth/auth";
			Task.Factory.StartNew(() => {
			                      	Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => {lblCurrentStatus.Visibility = Visibility.Visible;}));
				if(auth.authorization()) 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background,
					                       new Action(() => {memberAuthData();}));
					if(!File.Exists(appName))
					{
						version = "0.0";
					}
					runApp();
				}
				else {
					MessageBox.Show(auth.LastError);
					Dispatcher.BeginInvoke(DispatcherPriority.Background,
					                       new Action( () => {
					                                  	lblCurrentStatus.Visibility = Visibility.Hidden;
					                                  	loadingProgress.Visibility = Visibility.Hidden;
					                                  	btnLogin.IsEnabled = true;
					                                  	tbxPassword.IsEnabled = true;
					                                  }));
				}
            });
		}
		
		private string PasswordEncrypt (string password, string key)
		{
			return Crypto.EncryptStringAES(password, key);
		}
		
		private string PasswordDecrypt (string password, string key)
		{
			return Crypto.DecryptStringAES(password, key);
		}
		
		void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				if(tbxLogin.Text == "") {
					MessageBox.Show("Введите логин пользователя!");
					return;
				}
				if(tbxPassword.Password == "") {
					MessageBox.Show("Введите пароль пользователя!");
					return;
				}
				if(tbxHost.Text == "") {
					MessageBox.Show("Введите адрес сервера!");
					return;
				}
				login();
			}
		}
		
	}
}