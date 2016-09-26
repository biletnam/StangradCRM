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
using System.Drawing;
using System.Linq;

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
				SetPageMargin();
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
		
		private void SetPageMargin ()
		{
			if(LeftFieldMargin != null)
			{
				excelWorkSheet.PageSetup.LeftMargin = 
					excelApp.CentimetersToPoints((double)LeftFieldMargin);
			}
			if(RightFieldMargin != null)
			{
				excelWorkSheet.PageSetup.RightMargin = 
					excelApp.CentimetersToPoints((double)RightFieldMargin);
			}
			if(TopFieldMargin != null)
			{
				excelWorkSheet.PageSetup.TopMargin = 
					excelApp.CentimetersToPoints((double)TopFieldMargin);
			}
			if(BottomFieldMargin != null)
			{
				excelWorkSheet.PageSetup.BottomMargin = 
					excelApp.CentimetersToPoints((double)BottomFieldMargin);
			}
			if(HeaderFieldMargin != null)
			{
				excelWorkSheet.PageSetup.HeaderMargin = 
					excelApp.CentimetersToPoints((double)HeaderFieldMargin);
			}
			if(FooterFieldMargin != null)
			{
				excelWorkSheet.PageSetup.FooterMargin = 
					excelApp.CentimetersToPoints((double)FooterFieldMargin);
			}
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
			SetAllDocumentFontSize();
			for(int i = 0; i < Rows.Count; i++) 
			{
				prepareCell(Rows[i].Cells, i+1);
			}
		}
		
		private void prepareCell (List<ReportCell> cells, int rowIndex)
		{
			for(int i = 0; i < cells.Count; i++)
			{
				if(cells[i] != null)
				{
					excelWorkSheet.Cells[rowIndex, i+1] = cells[i].Content;
					SetCellStyle(cells[i], rowIndex, i+1);
				}
			}
		}
		
		private void SetCellStyle (ReportCell cell, int rowIndex, int columnIndex)
		{
			Range range = (Range)excelWorkSheet.Range[
				excelWorkSheet.Cells[rowIndex, columnIndex],
				excelWorkSheet.Cells[rowIndex + cell.RowSpan, columnIndex + cell.ColumnSpan]];
			
			//Ширина ячейки
			if(cell.Width != 0) range.ColumnWidth = cell.Width;
			
			//Высота ячейки
			if(cell.Height != 0) range.RowHeight = cell.Height;
			
			//Объединение ячеек по вертикали
			if(cell.RowSpan != 0) range.Merge();
			
			//Объеинение ячеек по горизонтали
			if(cell.ColumnSpan != 0) range.Merge();
						
			//Цвет фона ячейки
			if(cell.BackgroundColor != null) range.Interior.Color = ColorTranslator.ToOle((Color)cell.BackgroundColor);

			//Цвет текста ячейки
			if(cell.TextColor != null) range.Font.Color = ColorTranslator.ToOle((Color)cell.TextColor);

			//Размер шрифта ячейки
			if(cell.FontSize != 0) range.Font.Size = cell.FontSize;			
			
			//Горизонтальное положение текста в ячейке
			switch (cell.HorizontalAlignment)
			{	
				case HorizontalAlignment.Left:
					range.Cells.HorizontalAlignment = XlHAlign.xlHAlignLeft; break;
				
				case HorizontalAlignment.Center:
		            range.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter; break;

				case HorizontalAlignment.Right:
		            range.Cells.HorizontalAlignment = XlHAlign.xlHAlignRight; break;	
				
	           default:
		            range.Cells.HorizontalAlignment = XlHAlign.xlHAlignLeft; break;
			}
			
			//Вертикальное положение текста в ячейке
			switch (cell.VerticalAlignment)
			{	
				case VerticalAlignment.Top:
		            range.Cells.VerticalAlignment = XlVAlign.xlVAlignTop; break;
				
				case VerticalAlignment.Center:
		            range.Cells.VerticalAlignment = XlVAlign.xlVAlignCenter; break;

				case VerticalAlignment.Bottom:
		            range.Cells.VerticalAlignment = XlVAlign.xlVAlignBottom; break;	
				
		           default:
		            range.Cells.VerticalAlignment = XlVAlign.xlVAlignBottom; break;	
			}
			
			//Стили текста
			if(cell.TextStyle != null && cell.TextStyle.Count > 0)
			{
				for(int i = 0; i < cell.TextStyle.Count; i++)
				{
					switch (cell.TextStyle[i])
					{	
						case TextStyle.Bold:
				            range.Font.Bold = true; break;
						
						case TextStyle.Italic:
				            range.Font.Italic = true; break;
					}
				}
			}
			
			//Границы ячейки
			if(cell.Border != null && cell.Border.Count > 0)
			{
				for(int i = 0; i < cell.Border.Count; i++)
				{
					switch (cell.Border[i])
					{	
						case Border.All:
							if(cell.BorderColor != null)
								range.Borders.Color = ColorTranslator.ToOle((Color)cell.BorderColor);
							PrepareCellBorderWeight(cell, range);
							PrepareCellBorderStyle(cell, range);
							
							break;
						
						case Border.Left:
							if(cell.BorderColor != null)
								range.Borders[XlBordersIndex.xlEdgeLeft].Color = ColorTranslator.ToOle((Color)cell.BorderColor);
							PrepareCellBorderWeight(cell, range, XlBordersIndex.xlEdgeLeft);
							PrepareCellBorderStyle(cell, range, XlBordersIndex.xlEdgeLeft);
							
							break;

						case Border.Top:
							if(cell.BorderColor != null)
								range.Borders[XlBordersIndex.xlEdgeTop].Color = ColorTranslator.ToOle((Color)cell.BorderColor);
							PrepareCellBorderWeight(cell, range, XlBordersIndex.xlEdgeTop);
							PrepareCellBorderStyle(cell, range, XlBordersIndex.xlEdgeTop);
							
							break;

						case Border.Right:
							if(cell.BorderColor != null)
								range.Borders[XlBordersIndex.xlEdgeRight].Color = ColorTranslator.ToOle((Color)cell.BorderColor);
							PrepareCellBorderWeight(cell, range, XlBordersIndex.xlEdgeRight);
							PrepareCellBorderStyle(cell, range, XlBordersIndex.xlEdgeRight);
							
							break;

						case Border.Bottom:
							if(cell.BorderColor != null)
								range.Borders[XlBordersIndex.xlEdgeBottom].Color = ColorTranslator.ToOle((Color)cell.BorderColor);
							PrepareCellBorderWeight(cell, range, XlBordersIndex.xlEdgeBottom);
							PrepareCellBorderStyle(cell, range, XlBordersIndex.xlEdgeBottom);
							
							break;							
					}
				}
			}
			else
			{
				if(cell.BorderColor != null)
					range.Borders.Color = ColorTranslator.ToOle((Color)cell.BorderColor);
				PrepareCellBorderStyle(cell, range);
				PrepareCellBorderWeight(cell, range);
			}
			
		}
		
		private void SetAllDocumentFontSize ()
		{
			if(AllDocumentFontSize == 0) return;
			int rowCount = Rows.Count;
			int columnCount = Rows.Max(x => x.Cells.Count);
			((Range)excelWorkSheet.Range[
					excelWorkSheet.Cells[1, 1],
					excelWorkSheet.Cells[rowCount, columnCount]
				]).Font.Size = AllDocumentFontSize;
		}
		
		private void PrepareCellBorderStyle (ReportCell cell, Range range, XlBordersIndex? border = null)
		{
			switch(cell.BorderStyle)
			{
				//case BorderStyle.LineStyleNone:
					//CellBorderStyle(range, XlLineStyle.xlLineStyleNone, border); break;
				
				case BorderStyle.Continuous:
					CellBorderStyle(range, XlLineStyle.xlContinuous, border); break;
				
				case BorderStyle.Dash:
					CellBorderStyle(range, XlLineStyle.xlDash, border); break;
				
				case BorderStyle.DashDot:
					CellBorderStyle(range, XlLineStyle.xlDashDot, border); break;
				
				case BorderStyle.DashDotDot:
					CellBorderStyle(range, XlLineStyle.xlDashDotDot, border); break;
				
				case BorderStyle.Dot:
					CellBorderStyle(range, XlLineStyle.xlDot, border); break;
				
				case BorderStyle.Double:
					CellBorderStyle(range, XlLineStyle.xlDouble, border); break;
				
				case BorderStyle.SlantDashDot:
					CellBorderStyle(range, XlLineStyle.xlSlantDashDot, border); break;					
			}
		}
		
		private void CellBorderStyle (Range range, XlLineStyle style, XlBordersIndex? border = null)
		{
			if(border == null)
			{
				range.Borders.LineStyle = style;
			}
			else
			{
				range.Borders[(XlBordersIndex)border].LineStyle = style;
			}
		}
		
		private void PrepareCellBorderWeight (ReportCell cell, Range range, XlBordersIndex? border = null)
		{
			switch(cell.BorderWeight)
			{
				case BorderWeight.Hairline:
					CellBorderWeight(range, XlBorderWeight.xlHairline, border); break;
				case BorderWeight.Medium:
					CellBorderWeight(range, XlBorderWeight.xlMedium, border); break;
				case BorderWeight.Thick:
					CellBorderWeight(range, XlBorderWeight.xlThick, border); break;
				case BorderWeight.Thin:
					CellBorderWeight(range, XlBorderWeight.xlThin, border); break;
			}
		}
		
		private void CellBorderWeight (Range range, XlBorderWeight borderWeight, XlBordersIndex? border = null)
		{
			if(border == null)
			{
				range.Borders.Weight = borderWeight;
			}
			else
			{
				range.Borders[(XlBordersIndex)border].Weight = borderWeight;
			}
		}
	}
}
