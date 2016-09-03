/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/22/2016
 * Время: 13:02
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using StangradCRM.Classes;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for EquipmentSelectWindow.xaml
	/// </summary>
	public partial class EquipmentSelectWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		private Action<Equipment> callback = null;
		public EquipmentSelectWindow(Action<Equipment> callback)
		{
			InitializeComponent();
			this.callback = callback;
			
			viewSource.Source = EquipmentViewModel.instance().Collection;
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Equipment equipment = e.Item as Equipment;
				if(equipment == null) return;
				e.Accepted = equipment.IsVisible;
			};
			
			DataContext = new 
			{ 
				EquipmentCollection = viewSource.View
			};
			
			EquipmentViewModel.instance().Collection.ToList().ForEach(x => {x.IsSelected = false;});
			
		}
		
		void BtnOk_Click(object sender, RoutedEventArgs e)
		{
			Equipment equipment = EquipmentViewModel.instance().Collection.Where(x => x.IsSelected == true).FirstOrDefault();
			if(equipment == null)
			{
				MessageBox.Show("Элемент не выбран. Выберите элемент!");
				return;
			}
			callback(equipment);
			Close();
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void TbxSearchEquipment_TextChanged(object sender, TextChangedEventArgs e)
		{
			EquipmentViewModel.instance().searchByName(tbxSearchEquipment.Text);
			viewSource.View.Refresh();
		}
		
		void BtnSearchEquipmentClear_Click(object sender, RoutedEventArgs e)
		{
			tbxSearchEquipment.Text = "";
		}
		
		void RbnSelected_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;
			if(radioButton == null) return;
			
			DataGridRow dgrow = FindItem.FindParentItem<DataGridRow>(radioButton);
			if (dgrow == null) return;
			
			Equipment equipment = (Equipment)dgrow.Item;
			if(equipment == null) return;
			equipment.IsSelected = true;
			
		}
		
		void RbnSelected_Unchecked(object sender, RoutedEventArgs e)
		{
			RadioButton radioButton = sender as RadioButton;
			if(radioButton == null) return;
			
			DataGridRow dgrow = FindItem.FindParentItem<DataGridRow>(radioButton);
			if (dgrow == null) return;
			
			Equipment equipment = (Equipment)dgrow.Item;
			if(equipment == null) return;
			equipment.IsSelected = false;
		}
	}
}