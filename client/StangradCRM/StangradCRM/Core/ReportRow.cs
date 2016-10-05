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
using System.Drawing;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of ReportRow.
	/// </summary>
	public class ReportRow
	{
		public double Height { get; set; }
		
		public List<TextStyle> TextStyle { get; set; }
		
		public Color BackgroundColor { get; set; }
		public Color TextColor { get; set; }
		public Color BorderColor { get; set; }
		
		public BorderStyle BorderStyle { get; set; }
		public List<Border> Border { get; set; }
		
		public List<ReportCell> Cells = new List<ReportCell>();
		
		public ReportRow () {}
		
		public ReportRow (List<ReportCell> cells)
		{
			Cells = cells;
		}
		
		public void Add (ReportCell cell)
		{
			Cells.Add(cell);
		}
	}
}
