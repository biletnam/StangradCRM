/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 11:02
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace StangradCRMLibs.Controls
{
	/// <summary>
	/// Interaction logic for Loading.xaml
	/// </summary>
	public partial class Loading : UserControl
	{		
		public Loading()
		{
			InitializeComponent();
			BitmapImage source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri(@"/Images/loading.png", UriKind.Relative);
			source.EndInit();
			img.Source = source;
		}
	}
}