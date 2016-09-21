/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 20.09.2016
 * Время: 15:18
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of ReportCell.
	/// </summary>
	public class ReportCell
	{
		public string Content { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public int RowSpan { get; set; }
		public int ColumnSpan { get; set; }
		
		public List<TextStyle> TextStyle { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }
		
		public Color BackgroundColor { get; set; }
		public Color TextColor { get; set; }
		public Color BorderColor { get; set; }
		
		public BorderStyle BorderStyle { get; set; }
		public List<Border> Border { get; set; }
		
		public ReportCell () {}
		public ReportCell(string content)
		{
			Content = content;
		}
	}
}
