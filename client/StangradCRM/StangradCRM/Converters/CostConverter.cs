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
			double v = (double)value;
			if(v == 0) return "0,00";
			
			//Целая часть
			int intPart = (int)v;
			//Дробная часть, округленная до 2-х знаков
			double faction = Math.Round(v - intPart, 2);
			
			string strIntPart = intPart.ToString("# ### ### ###", culture);
			if(faction == 0) return strIntPart;
			string strFaction = (faction * 100).ToString();
			return strIntPart + "," + strFaction;
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
