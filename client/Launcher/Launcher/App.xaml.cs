using System;
using System.Windows;

using StangradCRMLibs;

namespace Launcher
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App ()
		{
			Dispatcher.UnhandledException += OnDispatcherUnhandledException;
		}
		
	    void OnDispatcherUnhandledException(object sender,
		                                    System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
	    {
	        string errorMessage = string.Format("Необработанное исключение: {0}", e.Exception.Message + "\nПодробности см. в логе ошибок.");
	        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	        Log.WriteError(e.Exception.ToString());
	        e.Handled = true;
	    }
	}
}