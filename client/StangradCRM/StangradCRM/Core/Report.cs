/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.09.2016
 * Время: 15:33
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of Report.
	/// </summary>
	public abstract class Report
	{		
		
		public string SavePath { get; set; }
		public string ReportName { get; set; }
		
		public string HeaderData { get; set; }
		public string FooterData { get; set; }
		
		public string LastError { get; set; }
		
		public ReportRow Titles { get; set; }
		public List<ReportRow> Rows { get; set; }
		
		public Dictionary<int, double> ColumnsWidth = new Dictionary<int, double>();
		
		
		public abstract bool Save ();
		
	}
}
