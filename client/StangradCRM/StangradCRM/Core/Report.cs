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
		
		public string FileName { get; set; }
		public string LastError { get; set; }
		
		public ReportRow Titles { get; set; }
		public List<ReportRow> Rows = new List<ReportRow>();
		
		public abstract bool Save ();
		
	}
}
