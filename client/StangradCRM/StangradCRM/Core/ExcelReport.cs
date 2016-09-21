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
			bool result = true;
			try
			{
				prepareData();
	            excelWorkBook.SaveAs(FileName);
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
		
		private void prepareData ()
		{
			for(int i = 0; i < Rows.Count; i++) 
			{
				prepareCell(Rows[i].Cells, i+1);
			}
		}
		
		private void prepareCell (List<ReportCell> cells, int rowIndex)
		{
			for(int i = 0; i < cells.Count; i++)
			{
				excelWorkSheet.Cells[rowIndex, i+1] = cells[i].Content;
			}
		}
		
		private void SetCellStyle (ReportCell cell, int rowIndex, int columnIndex)
		{
			if(cell.Width != 0) excelWorkSheet.Columns[columnIndex].ColumnWidth = cell.Width;
		}
		
	}
}
