/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 20.09.2016
 * Время: 15:57
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

namespace StangradCRM.Core
{

		public enum TextStyle
		{
			Bold,
			Italic
		}
		
		public enum HorizontalAlignment
		{
			Left,
			Center,
			Right
		}
		
		public enum VerticalAlignment
		{
			Top,
			Center,
			Bottom
		}
		
		public enum BorderStyle
		{
			LineStyleNone,
			Continuous,
			Dash,
			DashDot,
			DashDotDot,
			Dot,
			Double,
			SlantDashDot
		}
		
		public enum Border
		{
			All,
			Left,
			Top,
			Right,
			Bottom
		}
		
		public enum BorderWeight
		{
			None,
			Hairline,
			Medium,
			Thick,
			Thin
		}
		
		public enum Format
		{
			Text,
			Date,
			Money
		}
}
