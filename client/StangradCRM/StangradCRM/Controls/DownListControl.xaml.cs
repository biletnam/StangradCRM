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
		
		public delegate void SelectedValueChanged (object sender, SelectionChanged e);
		public event SelectedValueChanged OnSelect;
		
		public delegate void _TextChanged (object sender, TextChangedEventArgs e);
		public event _TextChanged TextChanged;
		
		IEnumerable<object> itemsSource = null;
		ObservableCollection<object> ISCollection = new ObservableCollection<object>();

		public DownListControl()
		{
			InitializeComponent();
			lbList.ItemsSource = ISCollection;
			
			tbxInputData.TextChanged += delegate(object sender, TextChangedEventArgs e) 
			{ 
				ISCollection.Clear();
				lbList.IsDropDownOpen = false;
				if(tbxInputData.Text == "") return;
				if(FilterItems())
				{
					lbList.IsDropDownOpen = true;
				}
				if(TextChanged != null)
				{
					TextChanged(this, e);
				}
			};
			
			tbxInputData.PreviewKeyDown += delegate(object sender, KeyEventArgs e) 
			{ 
				if(e.Key == Key.Down && lbList.IsDropDownOpen == true)
				{
					lbList.Focus();
				}
			};
			
			lbList.PreviewKeyDown += delegate(object sender, KeyEventArgs e)
			{ 
				if(e.Key == Key.Escape)
				{
					tbxInputData.Focus();
					e.Handled = true;
				}
			};
		}
		
		private bool FilterItems ()
		{
			if(itemsSource == null) return false;
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
				return true;
			}
			return false;
		}
		
		void LbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{		
			if(OnSelect != null)
			{
				OnSelect(this, new SelectionChanged(lbList.SelectedValue));
			}
			tbxInputData.Focus();
			tbxInputData.Text = lbList.Text;
			lbList.IsDropDownOpen = false;
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
			tbxInputData.Text = "";
		}
		
		public TextBox getTextBoxItem ()
		{
			return tbxInputData;
		}

		
		public string CurrentText
		{
			get
			{
				return GetValue(CurrentTextProperty).ToString();
			}
			set
			{
				SetCurrentTextDependencyProperty(CurrentTextProperty, value);
				Text = value;
			}
		}
		
        public static readonly DependencyProperty CurrentTextProperty = DependencyProperty.Register("CurrentText", typeof(string),
                typeof(DownListControl), new FrameworkPropertyMetadata(default(string),
                FrameworkPropertyMetadataOptions.None,
				new PropertyChangedCallback(OnCurrentTextChanged)));
		
        public event PropertyChangedEventHandler CurrentTextChanged;
        
        void SetCurrentTextDependencyProperty (DependencyProperty property, string value)
        {
            SetValue(property, value);
            if(CurrentTextChanged != null)
            {
            	CurrentTextChanged(this, new PropertyChangedEventArgs(property.ToString()));
            }
        }
        
        private static void OnCurrentTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        	DownListControl downListControl = d as DownListControl;
            if (downListControl == null) return;
            downListControl.CurrentText = e.NewValue.ToString();
		}
        
		
		public IEnumerable<object> ItemsSource
		{
			get
			{
				return (IEnumerable<object>)GetValue(ItemsSourceProperty);
			}
			set
			{
				SetItemsSourceDependencyProperty(ItemsSourceProperty, value);
				itemsSource = value;
			}
		}
		
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<object>),
                typeof(DownListControl), new FrameworkPropertyMetadata(default(IEnumerable<object>),
                FrameworkPropertyMetadataOptions.None,
				new PropertyChangedCallback(OnItemsSourceChanged)));
		
        public event PropertyChangedEventHandler ItemsSourceChanged;
        
        void SetItemsSourceDependencyProperty (DependencyProperty property, IEnumerable<object> value)
        {
            SetValue(property, value);
            if(ItemsSourceChanged != null)
            {
            	ItemsSourceChanged(this, new PropertyChangedEventArgs(property.ToString()));
            }
        }
        
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        	DownListControl downListControl = d as DownListControl;
            if (downListControl == null) return;
            downListControl.ItemsSource = (IEnumerable<object>)e.NewValue;
		}
	}
}
