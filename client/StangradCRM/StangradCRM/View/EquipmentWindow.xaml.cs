/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 16:44
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	/// Interaction logic for EquipmentWindow.xaml
	/// </summary>
	public partial class EquipmentWindow : Window
	{
		CollectionViewSource viewSource = new CollectionViewSource();
		CollectionViewSource modificationViewSource = new CollectionViewSource();
		
		public EquipmentWindow()
		{
			InitializeComponent();
			
			
			viewSource.Source = EquipmentViewModel.instance().Collection;
			viewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Equipment equipment = e.Item as Equipment;
				if(equipment == null) return;
				e.Accepted = equipment.IsVisible;
			};
			
			modificationViewSource.Source = ModificationViewModel.instance().Collection;
			modificationViewSource.Filter += delegate(object sender, FilterEventArgs e)
			{ 
				Modification modification = e.Item as Modification;
				Equipment equipment = dgvEquipment.SelectedItem as Equipment;
				if(modification == null) return;
				if(equipment == null)
				{
					e.Accepted = false;
					return;
				}
				
				if(modification.Id_equipment == equipment.Id)
				{
					e.Accepted = true;
				}
				else
				{
					e.Accepted = false;
				}
			};
			
			viewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			modificationViewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			DataContext = new
			{ 
				EquipmentCollection = viewSource.View,
				ModificationCollection = modificationViewSource.View
			};
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			EquipmentSaveWindow window = new EquipmentSaveWindow();
			window.ShowDialog();
		}
		
		void BtnEditRow_Click(object sender, RoutedEventArgs e)
		{
			Equipment equipment = dgvEquipment.SelectedItem as Equipment;
			if(equipment == null) return;
			EquipmentSaveWindow window = new EquipmentSaveWindow(equipment);
			window.ShowDialog();
		}
		
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			Button deleteBtn = sender as Button;
			
			if(MessageBox.Show("Подтвердите удаление",
			                "Удалить выбранную запись?",
			                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			{
				return;
			}         			                
			Equipment equipment = dgvEquipment.SelectedItem as Equipment;
			if(equipment == null) return;

			if(!equipment.remove())
			{
				MessageBox.Show(equipment.LastError);
			}

			SetModificationSource();
		}
		
		void BtnAddModification_Click(object sender, RoutedEventArgs e)
		{
			Equipment equipment = dgvEquipment.SelectedItem as Equipment;
			if(equipment == null) 
			{
				MessageBox.Show("Выберите оборудование!");
				return;
			}
			ModificationSaveWindow window = new ModificationSaveWindow(equipment);
			window.ShowDialog();
			SetModificationSource();
		}
		
		void BtnEditModificationRow_Click(object sender, RoutedEventArgs e)
		{
			Modification modification = dgvModification.SelectedItem as Modification;
			if(modification == null) return;
			ModificationSaveWindow window = new ModificationSaveWindow(null, modification);
			window.ShowDialog();
		}
		
		void BtnDeleteModificationRow_Click(object sender, RoutedEventArgs e)
		{
			if(MessageBox.Show("Подтвердите удаление",
			                "Удалить выбранную запись?",
			                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
			{
				return;
			}    
			Modification modification = dgvModification.SelectedItem as Modification;
			if(modification == null) return;
			
			if(!modification.remove())
			{
				MessageBox.Show(modification.LastError);
			}
			
			SetModificationSource();
		}
		
		void DgvEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetModificationSource();
		}
		
		private void SetModificationSource ()
		{
			modificationViewSource.View.Refresh();
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
		
		void BtnRowUp_Click(object sender, RoutedEventArgs e)
		{
			Equipment equipment = dgvEquipment.SelectedItem as Equipment;
			if(equipment == null) return;
			
			if(equipment.rowUp())
			{
				viewSource.View.Refresh();
			}
			else
			{
				MessageBox.Show(equipment.LastError);
			}
		}
		
		void BtnRowDown_Click(object sender, RoutedEventArgs e)
		{
			Equipment equipment = dgvEquipment.SelectedItem as Equipment;
			if(equipment == null) return;
			
			if(equipment.rowDown())
			{
				viewSource.View.Refresh();
			}
			else
			{
				MessageBox.Show(equipment.LastError);
			}
		}
		
		void BtnModificationRowUp_Click(object sender, RoutedEventArgs e)
		{
			Modification modification = dgvModification.SelectedItem as Modification;
			if(modification == null) return;
			
			if(modification.rowUp())
			{
				SetModificationSource();
			}
			else
			{
				MessageBox.Show(modification.LastError);
			}
		}
		
		void BtnModificationRowDown_Click(object sender, RoutedEventArgs e)
		{
			Modification modification = dgvModification.SelectedItem as Modification;
			if(modification == null) return;
			
			if(modification.rowDown())
			{
				SetModificationSource();
			}
			else
			{
				MessageBox.Show(modification.LastError);
			}
		}
		
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void EquipmentRowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			
			Equipment equipment = row.Item as Equipment;
			if(equipment == null) return;
			
			EquipmentSaveWindow window = new EquipmentSaveWindow(equipment);
			window.ShowDialog();
			
			viewSource.View.Refresh();
          	dgvEquipment.CurrentCell = new DataGridCellInfo(row.Item, dgvEquipment.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void EquipmentRowPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				EquipmentRowDoubleClick(sender, null);
				e.Handled = true;
			}
		}
		
		//Дабл клик по строке таблицы - открывает окно редактирования		
		private void ModificationRowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			
			Modification modification = row.Item as Modification;
			if(modification == null) return;
			
			ModificationSaveWindow window = new ModificationSaveWindow(null, modification);
			window.ShowDialog();
			
			modificationViewSource.View.Refresh();
          	dgvModification.CurrentCell = new DataGridCellInfo(row.Item, dgvModification.CurrentCell.Column);
		}
		
		//Обработка события нажатия клавиш на строке таблице
		void ModificationRowPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter) {
				ModificationRowDoubleClick(sender, null);
				e.Handled = true;
			}
		}
		
		
	}
}