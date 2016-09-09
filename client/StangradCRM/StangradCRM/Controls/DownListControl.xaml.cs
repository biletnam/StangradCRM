/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/16/2016
 * Время: 12:50
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRMLibs;

namespace StangradCRM.Controls
{
	/// <summary>
	/// Interaction logic for DownListControl.xaml
	/// </summary>
	/// 
	
	
	public class SelectionChanged
	{
		public object Value
		{
			get;
			private set;
		}
		public SelectionChanged(object v)
		{
			Value = v;
		}
	}
	
	public partial class DownListControl : UserControl
	{
		
		IEnumerable<object> itemsSource = null;
		ObservableCollection<object> ISCollection = new ObservableCollection<object>();
		
		public delegate void SelectedValueChanged (object sender, SelectionChanged e);
		
		public event SelectedValueChanged OnSelect;
		
		
		public DownListControl()
		{
			InitializeComponent();
			lbList.ItemsSource = ISCollection;
			
			tbxInputData.TextChanged += delegate(object sender, TextChangedEventArgs e) 
			{ 
				lbList.IsDropDownOpen = false;
				ISCollection.Clear();
				if(tbxInputData.Text == "") return;
				
				List<object> collection = (itemsSource).ToList();				
				for(int i = 0; i < collection.Count; i++)
				{
					var propertyInfo = collection[i].GetType().GetProperty(lbList.DisplayMemberPath);
					
					string val = propertyInfo.GetValue(collection[i], null).ToString();
					
					if(val.ToLower().IndexOf(tbxInputData.Text.ToLower()) != -1)
					{
						ISCollection.Add(collection[i]);
					}
				}
				if(ISCollection.Count > 0)
				{
					lbList.IsDropDownOpen = true;
				}
			};
			
			tbxInputData.PreviewKeyDown += delegate(object sender, KeyEventArgs e) 
			{ 
				if(e.Key == Key.Down && lbList.IsDropDownOpen == true)
				{
					lbList.Focus();
				}
			};
		}
		
		void LbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnSelect(this, new SelectionChanged(lbList.SelectedValue));
			tbxInputData.Focus();
			tbxInputData.Text = lbList.Text;
			lbList.IsDropDownOpen = false;
		}
	
		
		public IEnumerable<object> ItemsSource
		{
			get
			{
				return itemsSource;
			}
			set
			{
				itemsSource = value;
			}
		}
		
		public string DisplayMemberPath
		{
			get
			{
				return lbList.DisplayMemberPath;
			}
			set
			{
				lbList.DisplayMemberPath = value;
			}
		}
		
		public string SelectedValuePath
		{
			get
			{
				return lbList.SelectedValuePath;
			}
			set
			{
				lbList.SelectedValuePath = value;
			}
		}

		
		public Style TextFieldStyle
		{
			get
			{
				return tbxInputData.Style;
			}
			set
			{
				tbxInputData.Style = value;
			}
		}
		
		public Style ListStyle
		{
			get
			{
				return lbList.Style;
			}
			set
			{
				lbList.Style = value;
			}
		}
		
		public string Text
		{
			get
			{
				return tbxInputData.Text;
			}
			set
			{
				tbxInputData.Text = value;
				lbList.IsDropDownOpen = false;
			}
		}
		
		public bool IsReadOnly
		{
			get
			{
				return tbxInputData.IsReadOnly;
			}
			set
			{
				tbxInputData.IsReadOnly = value;
			}
		}
		

		public new Brush Background
		{
			get
			{
				return tbxInputData.Background;
			}
			set
			{
				tbxInputData.Background = value;
			}
		}
		
		public void Clear ()
		{
			lbList.SelectedIndex = -1;
			ISCollection.Clear();
			tbxInputData.Text = "";
		}
		
		public TextBox getTextBoxItem ()
		{
			return tbxInputData;
		}
	                            
	}
}
