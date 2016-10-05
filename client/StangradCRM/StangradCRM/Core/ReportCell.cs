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
		public string Content { get; set; } // +
		public double Width { get; set; } // +
		public double Height { get; set; } // +
		
		public uint RowSpan { get; set; } // +
		public uint ColumnSpan { get; set; } // +
		
		public uint FontSize { get; set; } // +
		
		public Format Format { get; set; }
		
		public List<TextStyle> TextStyle { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; } // +
		public VerticalAlignment VerticalAlignment { get; set; } // +
		
		private Color? backgroundColor = null;
		public Color? BackgroundColor 
		{ 
			get
			{
				return backgroundColor;
			}
			set
			{
				backgroundColor = value;
			}
		} //+
		
		private Color? textColor = null;
		public Color? TextColor 
		{ 
			get
			{
				return textColor;	
			}
			set 
			{
				textColor = value;	
			}
		} // +
		
		private Color? borderColor = null;
		public Color? BorderColor { 
			get 
			{
				return borderColor;
			}
			set 
			{
				borderColor = value;
			}
		} // +
		
		public BorderStyle BorderStyle { get; set; }
		public BorderWeight BorderWeight { get; set; }
		public List<Border> Border { get; set; }
		
		public ReportCell () {}
		public ReportCell(string content)
		{
			Content = content;
		}
	}
}
