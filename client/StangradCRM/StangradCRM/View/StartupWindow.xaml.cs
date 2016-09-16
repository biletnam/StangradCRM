/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 10.08.2016
 * Время: 12:25
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;
using StangradCRM.View.MainWindows;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for StartupWindow.xaml
	/// </summary>
	public partial class StartupWindow : Window
	{
		
		private IniFile settings = new IniFile("Settings.ini");
		private string version = "0.04";
		
		public StartupWindow()
		{
			InitializeComponent();
			string[] args = Environment.GetCommandLineArgs();
			if(args.Length < 3)
			{
				MessageBox.Show("Необходимые аргументы не были получены!");
				if(MessageBox.Show("Запустить приложение на тестовом сервере?", 
				                "Запустить приложение на тестовом сервере?",
				                MessageBoxButton.YesNo) == MessageBoxResult.No) 
				{
					Close();
					return;
				}
				loadTestServer ();
				return;
			}
			Task.Factory.StartNew(() => {  
              	if(!serverAuthorization(args[1], args[2]))
              	{
              		Dispatcher.BeginInvoke(DispatcherPriority.Background,
              		                       new Action(() => {
      		                                  	failSessionRestore();
      		                                  	return;
  		                                   }));
              	}         	
              	if(loadModels()) 
              	{
              		Dispatcher.BeginInvoke(DispatcherPriority.Background,
              		                       new Action( () => {
              		                                  	openMainWindow();
              		                                  }));
              	}
          	});
			
		}
		
		private bool serverAuthorization(string uriString, string cookieString)
		{
			Uri uri = new Uri(uriString);
			Settings.uri = uri;
			string[] cookieData = cookieString.Split('=');

			if(cookieData.Count() != 2) 
			{
				MessageBox.Show("Не удалось восстановить сессию сервера!\nПриложение будет закрыто!");
				return false;
			}
			try {
				Cookie cookie = new Cookie(cookieData[0], cookieData[1], "/");
				string [] urlArray = Settings.uri.ToString().Split('/');
				
				string [] withoutPort = urlArray[2].Split(':');
				
				cookie.Domain = "." + withoutPort[0];
				HTTPManager.HTTPRequest.CurrentCookie.Add(cookie);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.ToString());
				return false;
			}
			Auth auth = Auth.getInstance();
			auth.Entity = "Auth/userInfo";
			if(!auth.loadCurrentAuthUser()) {
				MessageBox.Show("Не удалось восстановить сессию!" + auth.LastError + "\nПриложение будет закрыто!");
				return false;
			}
			return true;
		}
		
		private void failSessionRestore ()
		{
			Close();
		}
		
		
		private void openMainWindow ()
		{
			settings.Write("version", version);
			Settings.version = version;
			
			int idRole = Auth.getInstance().IdRole;
			switch(idRole)
			{
				case 1:
					AdministratorMainWindow administratorWindow =
						new AdministratorMainWindow();
					administratorWindow.Show();
					break;
				case 2: 
					DirectorMainWindow directorWindow =
						new DirectorMainWindow();
					directorWindow.Show();
					break;
				case 3:
					AccountantMainWindow accountantWindow = 
						new AccountantMainWindow();
					accountantWindow.Show();
					break;
				case 4:
					TechnicalMainWindow technicalWindow = 
						new TechnicalMainWindow();
					technicalWindow.Show();
					break;
				case 5:
					ManagerMainWindow managerWindow =
						new ManagerMainWindow();
					managerWindow.Show();
					break;
			}
			Close();
		}
		
		private bool loadModels () 
		{
			
			CRMSettingViewModel.instance();
			
			ComplectationItemViewModel.instance();
			
			EquipmentViewModel.instance();
			ModificationViewModel.instance();
			SellerViewModel.instance();
			BuyerViewModel.instance();
			BidStatusViewModel.instance();
			PaymentStatusViewModel.instance();
			
			RoleViewModel.instance();
			ManagerViewModel.instance();
			
			BidViewModel.instance();
			
			
			
			//EquipmentBidViewModel.instance();
			//ComplectationViewModel.instance();
			
			//ComplectationItemViewModel.instance();
			
			return true;
		}
		
		private void loadTestServer ()
		{
			IniFile testSettings = new IniFile("testsettings.ini");
			if(!testSettings.KeyExists("login") ||
			   testSettings.Read("login") == "" ||
			   !testSettings.KeyExists("password") ||
			   testSettings.Read("password") == "" ||
			   !testSettings.KeyExists("host") ||
			   testSettings.Read("host") == "")
			{
				testSettings.Write("login", "");
				testSettings.Write("password", "");
				testSettings.Write("host", "");
				MessageBox.Show("Файл с настройками подключения к тестовому серверу некорректен.\nПриложение будет закрыто!");
				Close();
				return;
			}
			Task.Factory.StartNew(() => {
                  	Auth auth = Auth.getInstance();
                  	auth.Entity = "Auth/auth";
                  	auth.Login = testSettings.Read("login");
                  	auth.Password = testSettings.Read("password");
                  	try
                  	{
                  		Settings.uri = new Uri(testSettings.Read("host"));
                  	}
                  	catch
                  	{
                  		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {errorTestServer("Некорректный адрес сервера!");}));
                  		return;
                  	}		                      	
	             	if(!auth.authorization())
                  	{
                  		Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {errorTestServer(auth.LastError);}));
                  		return;
                  	}
	              	if(loadModels()) 
	              	{
	              		Dispatcher.BeginInvoke(DispatcherPriority.Background,
		                       new Action( () => {
                                  	openMainWindow();
                               }));
		              	}
		              	return;
           });
		}
		
		private void errorTestServer (string message)
		{
			MessageBox.Show(message + "\nПриложение будет закрыто!");
			Close();
			return;
		}
		
	}
}