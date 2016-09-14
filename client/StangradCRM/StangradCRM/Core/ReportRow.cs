/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.09.2016
 * Время: 15:40
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of ReportRow.
	/// </summary>
	public class ReportRow
	{
		public ReportRow() {}
		
		public enum RowStyle 
		{
            Bold,
            Italic,
            TextAlignLeft,
            TextAlignRight,
            TextAlignCenter,
            Selection,
			Border
		}
		
		public List<string> Cells = new List<string>();
		public List<RowStyle> Styles = new List<RowStyle>();
		
	}
}
