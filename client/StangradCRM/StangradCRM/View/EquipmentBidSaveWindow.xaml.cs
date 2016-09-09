/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 22.08.2016
 * Время: 10:16
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Classes;
using StangradCRM.Model;
using StangradCRM.ViewModel;
using Xceed.Wpf.Toolkit;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for EquipmentBidSaveWindow.xaml
	/// </summary>
	public partial class EquipmentBidSaveWindow : Window
	{		
		private EquipmentBid equipmentBid = null;
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		CollectionViewSource equipmentViewSource = new CollectionViewSource();
		CollectionViewSource modificationViewSource = new CollectionViewSource();
		
		bool isNew = false;
		
		public EquipmentBidSaveWindow(int idBid)
		{
			InitializeComponent();
		
			setViewSources();
			
			isNew = true;
			
			equipmentBid = new EquipmentBid();
			equipmentBid.Id_bid = idBid;
			gbxComplectation.Visibility = Visibility.Hidden;
			DataContext = new 
			{
				ComplectationCollection = equipmentBid.ComplectationCollection
			};
			
			Title += idBid.ToString();
		}
		
		public EquipmentBidSaveWindow(EquipmentBid equipmentBid)
		{
			InitializeComponent();
			Title = "Редактирование комплектации в заявке №" + equipmentBid.Id_bid.ToString();
			
			setViewSources();
			
			Modification modification = ModificationViewModel.instance().getById(equipmentBid.Id_modification);
			if(modification == null) return;
			
			Equipment equipment = EquipmentViewModel.instance().getById(modification.Id_equipment);
			if(equipment == null) return;
			
			cbxEquipment.SelectedItem = equipment;
			cbxModification.SelectedItem = modification;
						
			DataContext = new 
			{
				ComplectationCollection = equipmentBid.ComplectationCollection
			};
			
			this.equipmentBid = equipmentBid;

		}
		
		void setViewSources ()
		{
			defaultBrush = cbxEquipment.Background;
			
			equipmentViewSource.Source = EquipmentViewModel.instance().Collection;
			equipmentViewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			
			cbxEquipment.ItemsSource = equipmentViewSource.View;
			
			
			modificationViewSource.Source = ModificationViewModel.instance().Collection;
			modificationViewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			
			modificationViewSource.Filter += delegate(object sender, FilterEventArgs e)
			{
				Modification modification = e.Item as Modification;
				if(modification == null) return;
				if(cbxEquipment.SelectedIndex == -1)
				{
					e.Accepted = false;
					return;
				}
				Equipment equipment = cbxEquipment.SelectedItem as Equipment;
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
			
			cbxModification.ItemsSource = modificationViewSource.View;
			
			cbxEquipment.SelectedIndex = -1;
			cbxModification.SelectedIndex = -1;
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			equipmentBid.Id_modification = (int)cbxModification.SelectedValue;
			
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(equipmentBid.save())
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
			loadingProgress.Visibility = Visibility.Hidden;
			gbxComplectation.Visibility = Visibility.Visible;
			IsEnabled = true;
			if(!isNew)
			{
				Close();
			}
			else
			{
				Title = "Редактирование комплектации в заявке";
				isNew = false;
			}
		}
		
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			System.Windows.MessageBox.Show(equipmentBid.LastError);
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void CbxEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			cbxEquipment.Background = defaultBrush;
			if(modificationViewSource.Source != null)
			{
				modificationViewSource.View.Refresh();
			}
		}
		
		private bool validate()
		{
			if(cbxEquipment.SelectedIndex == -1)
			{
				cbxEquipment.Background = errorBrush;
				return false;
			}
			if(cbxModification.SelectedIndex == -1)
			{
				cbxModification.Background = errorBrush;
				return false;
			}
			return true;
		}
		
		void CbxModification_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			cbxModification.Background = defaultBrush;
		}
		
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			Complectation complectation = new Complectation();
			complectation.Complectation_count = 1;
			complectation.Commentary = "";
			
			
			complectation.IsSaved = false;
			
			equipmentBid.ComplectationCollection.Add(complectation);
			
		}
		
		void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
		{
			Complectation complectation = dgvComplectation.SelectedItem as Complectation;
			if(complectation == null) return;
			if(System.Windows.MessageBox.Show("Удалить комплектацию из оборудования?",
			                                  "Удалить комплектацию из оборудования?",
			                                  MessageBoxButton.YesNo) == MessageBoxResult.No)
				return;
			if(complectation.Id == 0 
			   && equipmentBid.ComplectationCollection.Contains(complectation))
			{
				equipmentBid.ComplectationCollection.Remove(complectation);
				return;
			}
			if(!complectation.remove())
			{
				System.Windows.MessageBox.Show(complectation.LastError);
				return;
			}
			
		}
		
		void BtnSaveRow_Click(object sender, RoutedEventArgs e)
		{
			Button saveButton = sender as Button;
			if(saveButton == null) return;
			
			DataGridRow row = FindItem.FindParentItem<DataGridRow>(saveButton);
			if(row == null) return;
			
			Complectation complectation = (Complectation)row.Item;
			if(complectation == null) return;
			if(equipmentBid == null) return;
			complectation.Id_equipment_bid = equipmentBid.Id;
			
			if(!validate(complectation)) return;
			
			complectationSave(complectation);
		}
		
		void TbxComment_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if(textBox == null) return;
			
			DataGridRow row = FindItem.FindParentItem<DataGridRow>(textBox);
			if(row == null) return;
			
			Complectation complectation = (Complectation)row.Item;
			if(complectation == null) return;
			
			if(complectation.Commentary == textBox.Text) 
			{
				complectation.IsSaved = true;
				return;
			}
			complectation.Commentary = textBox.Text;
			complectation.IsSaved = false;
		}
		
		void DudCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			DecimalUpDown decimalUpDown = sender as DecimalUpDown;
			if(decimalUpDown == null) return;
			
			DataGridRow row = FindItem.FindParentItem<DataGridRow>(decimalUpDown);
			if(row == null) return;
			
			Complectation complectation = (Complectation)row.Item;
			if(complectation == null) return;
			try
			{
				if(complectation.Complectation_count == (int)decimalUpDown.Value)
				{
					complectation.IsSaved = true;
					return;
				}
			}
			catch
			{
				complectation.Complectation_count = 0;
			}
			complectation.IsSaved = false;
			complectation.Complectation_count = (int)decimalUpDown.Value;
		}
		
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			List<Complectation> toRemoveComplectation = 
				equipmentBid.ComplectationCollection.Where(x => x.Id == 0).ToList();
			foreach(var item in toRemoveComplectation)
			{
				equipmentBid.ComplectationCollection.Remove(item);
			}
			List<Complectation> toSetOldValues =
				equipmentBid.ComplectationCollection.Where(x => x.IsSaved == false).ToList();
			foreach(Complectation item in toSetOldValues)
			{
				item.setOldValues();
			}
		}
		
		void BtnSaveAllComplectation_Click(object sender, RoutedEventArgs e)
		{
			prepareComplectationSave();
		}
		
		void prepareComplectationSave ()
		{
			for(int i = 0; i < equipmentBid.ComplectationCollection.Count; i++)
			{
				Complectation complectation = equipmentBid.ComplectationCollection[i];
				if(!validate(complectation)) return;
			}
			
			for(int i = 0; i < equipmentBid.ComplectationCollection.Count; i++)
			{
				Complectation complectation = equipmentBid.ComplectationCollection[i];
				if(complectation.IsSaved == false)
				{
					complectation.Id_equipment_bid = equipmentBid.Id;
					complectationSave(complectation);
				}
			}
		}
		
		void complectationSave (Complectation complectation)
		{
			IsEnabled = false;
			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => {
				if(complectation.save())
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { complectationSuccessSave(complectation); } ));
				}
				else 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { complectationErrorSave(complectation); } ));
				}
			});
		}
		
		void complectationSuccessSave (Complectation complectation)
		{
			complectation.IsSaved = true;
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			
		}
		void complectationErrorSave (Complectation complectation)
		{
			System.Windows.MessageBox.Show(complectation.LastError);
			complectation.IsSaved = true;
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
		}
		
		bool validate (Complectation complectation)
		{
			if(complectation.Commentary == "")
			{
				System.Windows.MessageBox.Show("Поле 'Коментарий' не заполнено у одного или нескольких записей.\nСохранение не возможно!");
				return false;
			}
			if(complectation.Complectation_count < 1)
			{
				System.Windows.MessageBox.Show("Поле 'Количество' меньше 1 у одного или нескольких записей.\nСохранение не возможно!");
				return false;
			}
			return true;
		}
	}
}