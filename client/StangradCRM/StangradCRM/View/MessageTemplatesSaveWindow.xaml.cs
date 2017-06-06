/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 15.05.2017
 * Time: 11:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for MessageTemplatesSaveWindow.xaml
	/// </summary>
	public partial class MessageTemplatesSaveWindow : Window
	{
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));		
		
		MessageTemplates messageTemplate = null;
		
		public MessageTemplatesSaveWindow()
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
			
		}
		
		public MessageTemplatesSaveWindow(MessageTemplates messageTemplate)
		{
			InitializeComponent();
			defaultBrush = tbxName.Background;
			
			Title = "Редактирование шаблона сообщения " + messageTemplate.Name;
			tbxName.Text = messageTemplate.Name;
			tbxTheme.Text = messageTemplate.Theme;
			tbxTemplate.Text = messageTemplate.Template;
			
			this.messageTemplate = messageTemplate;
			
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			
			if(messageTemplate == null)
				messageTemplate = new MessageTemplates();
			
			messageTemplate.Name = tbxName.Text;
			messageTemplate.Theme = tbxTheme.Text;
			messageTemplate.Template = tbxTemplate.Text;
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(messageTemplate.save())
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(); } ));
				}
				else 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(); } ));
				}
			});
			
		}
		
		private void successSave()
		{
			Close();
		}
		
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			MessageBox.Show(messageTemplate.LastError);
		}
		
		
		bool validate () 
		{
			if(tbxName.Text == "") {
				tbxName.Background = errorBrush;
				return false;
			}
			
			return true;
		}
		
	}
}