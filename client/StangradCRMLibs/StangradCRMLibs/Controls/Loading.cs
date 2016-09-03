/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 11:02
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

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