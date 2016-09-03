/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 30.08.2016
 * Время: 15:32
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows.Data;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of PaymentStatusIntConverter.
	/// </summary>
	public class PaymentStatusIntConverter : IValueConverter
	{		
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (int)value;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
