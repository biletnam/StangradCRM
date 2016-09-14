/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.09.2016
 * Время: 15:47
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Office.Interop.Excel;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of ExcelReport.
	/// </summary>
	public abstract class ExcelReport : Report
	{
		
		private Application excelApp = new Application();
		private Workbook excelWorkBook;
		private Worksheet excelWorkSheet;
		private object missingValue = System.Reflection.Missing.Value;
		//private int CurrentRowIndex = 1;
		
		public ExcelReport()
		{
			if(excelApp == null)
			{
				throw new Exception("Excel не установлен или используется его устаревшая версия!");
			}
			excelWorkBook = excelApp.Workbooks.Add(missingValue);
			excelWorkSheet = (Worksheet)excelWorkBook.Worksheets[1];
		}
		
		protected bool Create ()
		{
			if(!Directory.Exists(SavePath))
			{
				LastError = "Directory " + SavePath + " not exist!";
				return false;
			}
			if(ReportName == null)
			{
				LastError = "Report name is null";
				return false;
			}
			
			bool result = true;
			
			try
			{
				SetHeader();
				SetTitles();
				SetRows();
				SetFooter();
				
	            foreach(KeyValuePair<int, double> w in ColumnsWidth)
	            {
	                //excelWorkSheet.Columns[w.Key].ColumnWidth = w.Value;
				}
	            
	            excelWorkBook.SaveAs(SavePath + @"\" + ReportName + ".xlsx");
			}
			catch (Exception ex)
			{
				LastError = ex.Message;
				result = false;
			}
			finally
			{
				Close();
			}
			return result;
		}
		
		private void Close ()
		{
            excelWorkBook.Close(true, missingValue, missingValue);
            excelApp.Quit();
            releaseObject(excelWorkSheet);
            releaseObject(excelWorkBook);
			releaseObject(excelApp);
		}
		
		private void releaseObject (object obj)
		{
            try
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw new Exception("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
			}
		}
		
		protected void SetHeader ()
		{
			
		}
		
		protected void SetTitles ()
		{
			
		}
		
		protected void SetRows ()
		{
			
		}
		
		protected void SetFooter ()
		{
			
		}
	}
}
