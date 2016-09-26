/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 01.09.2016
 * Время: 11:30
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

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Controls.AdministratorControls
{
	/// <summary>
	/// Interaction logic for MainControlCRMSetting.xaml
	/// </summary>
	public partial class MainControlCRMSetting : UserControl
	{
		public MainControlCRMSetting()
		{
			InitializeComponent();
			DataContext = new 
			{
				CRMSettingCollection = CRMSettingViewModel.instance().Collection
			};
		}
		
		
		void BtnSaveRow_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(button == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			CRMSetting crmSetting = (CRMSetting)row.Item;
			if(crmSetting == null) return;
			
			if(!crmSetting.save())
			{
				MessageBox.Show(crmSetting.LastError);
				return;
			}
			crmSetting.IsSaved = true;
		}
		
		void TbxSettingValue_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if(textBox == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(textBox);
			if(row == null) return;
			
			CRMSetting crmSetting = (CRMSetting)row.Item;
			if(crmSetting == null) return;
			
			if(crmSetting.Setting_value != textBox.Text)
			{
				crmSetting.Setting_value = textBox.Text;
				crmSetting.IsSaved = false;
			}
		}
	}
}