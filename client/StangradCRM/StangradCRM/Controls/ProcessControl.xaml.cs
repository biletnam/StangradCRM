/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 22.09.2016
 * Время: 11:21
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace StangradCRM.Controls
{
	/// <summary>
	/// Interaction logic for ProcessControl.xaml
	/// </summary>
	public partial class ProcessControl : UserControl
	{
		public ProcessControl()
		{
			InitializeComponent();
		}
		
		
		
		public string Text
		{
			get
			{
				return tbContent.Text;
			}
			set
			{
				tbContent.Text = value;
			}
		}
		
		public VerticalAlignment MessageVerticalAlignment 
		{
			get
			{
				return messageContainer.VerticalAlignment;
			}
			set
			{
				messageContainer.VerticalAlignment = value;
			}
		}
		
		public HorizontalAlignment MessageHorizontalAlignment 
		{
			get
			{
				return messageContainer.HorizontalAlignment;
			}
			set
			{
				messageContainer.HorizontalAlignment = value;
			}
		}
		
		public Thickness MessageMargin
		{
			get
			{
				return messageContainer.Margin;
			}
			set
			{
				messageContainer.Margin = value;
			}
		}
	}
}