/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.09.2016
 * Время: 19:39
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows.Data;

namespace StangradCRM.Converters
{
	/// <summary>
	/// Description of CostConverter.
	/// </summary>
	public class CostConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if((double)value == 0) return "0";
			return ((double)value).ToString("# ### ### ###", culture);
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
            double result;
            if (Double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any,
                         culture, out result))
            {
                return result;
            }
            return value;
		}
	}
}
