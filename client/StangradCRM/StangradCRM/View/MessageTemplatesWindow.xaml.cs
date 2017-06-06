/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 15.05.2017
 * Time: 11:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for MessageTemplatesWindow.xaml
	/// </summary>
	public partial class MessageTemplatesWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		public MessageTemplatesWindow()
		{
			InitializeComponent();
			
			viewSource.Source = MessageTemplatesViewModel.instance().Collection;
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				MessageTemplates messageTemplate = e.Item as MessageTemplates;
				if(messageTemplate == null) return;
				e.Accepted = messageTemplate.IsVisible;
			};
			DataContext = new {MessageTemplatesCollection = viewSource.View};
		}
		
		void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			MessageTemplatesViewModel.instance().search(tbxSearch.Text);
			viewSource.View.Refresh();
		}
		
		void BtnSearchClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearch.Text = "";
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			MessageTemplates messageTemplate = dgvMessageTemplates.SelectedItem as MessageTemplates;
			if(messageTemplate == null) return;
			MessageTemplatesSaveWindow window = new MessageTemplatesSaveWindow(messageTemplate);
			window.ShowDialog();
		}
		
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			if(MessageBox.Show("Подтвердите удаление",
			                "Удалить выбранную запись?",
			                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			{
				return;
			}
			MessageTemplates messageTemplate = dgvMessageTemplates.SelectedItem as MessageTemplates;
			if(messageTemplate == null) return;
			if(!messageTemplate.remove())
			{
				MessageBox.Show(messageTemplate.LastError);
			}
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			MessageTemplatesSaveWindow w = new MessageTemplatesSaveWindow();
			w.ShowDialog();
		}
		
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void DgvBid_RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			
			MessageTemplates template = row.Item as MessageTemplates;
			if(template == null) return;
			
			MessageTemplatesSaveWindow window = new MessageTemplatesSaveWindow(template);
			window.ShowDialog();
			
			viewSource.View.Refresh();
          	dgvMessageTemplates.CurrentCell = new DataGridCellInfo(row.Item, dgvMessageTemplates.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void DgvBid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				DgvBid_RowDoubleClick(sender, null);
				e.Handled = true;
			}
		}
		
	}
}