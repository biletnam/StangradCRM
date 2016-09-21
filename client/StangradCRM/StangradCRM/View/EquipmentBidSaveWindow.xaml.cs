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
		
		//Конструктор при добавлении новой комплектации в заявку
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
		
		//Конструктор при редактировании комплектации в заявке
		public EquipmentBidSaveWindow(EquipmentBid equipmentBid)
		{
			InitializeComponent();
			Title = "Редактирование комплектации в заявке №" + equipmentBid.Id_bid.ToString();
			
			setViewSources();
			
			Equipment equipment = EquipmentViewModel.instance().getById(equipmentBid.Id_equipment);
			if(equipment != null) 
			{
				cbxEquipment.SelectedItem = equipment;
			}
			
			if(equipmentBid.Id_modification != null)
			{
				Modification modification = ModificationViewModel.instance().getById((int)equipmentBid.Id_modification);
				if(modification != null) 
				{
					cbxModification.SelectedItem = modification;
				}
			}
			
			DataContext = new 
			{
				ComplectationCollection = equipmentBid.ComplectationCollection
			};
			
			this.equipmentBid = equipmentBid;
			cbxEquipment.IsEnabled = false;
		}
		
		//Установка коллекций
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
		
		
		//----> Оборудование/Модификации
		
		//Клик на кнопку сброса выбранной модификации
		void BtnResetModifications_Click(object sender, RoutedEventArgs e)
		{
			cbxModification.SelectedIndex = -1;
		}
		
		//Клик на кнопку сохранения оборудования/модификации
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			equipmentBid.Id_equipment = (int)cbxEquipment.SelectedValue;
			if(cbxModification.SelectedIndex != -1)
			{
				equipmentBid.Id_modification = (int)cbxModification.SelectedValue;
			}
			else
			{
				equipmentBid.Id_modification = null;
			}
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
		
		//Ф-я валидации перед сохранением оборудования/модификации
		private bool validate()
		{
			if(cbxEquipment.SelectedIndex == -1)
			{
				cbxEquipment.Background = errorBrush;
				return false;
			}
			return true;
		}
		
		//Ф-я обработки успешного сохранения оборудования/модификации
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
				cbxEquipment.IsEnabled = false;
			}
		}
		
		//Ф-я обработки ошибки при сохранении оборудования/модификации
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			System.Windows.MessageBox.Show(equipmentBid.LastError);
		}
		
		//Ф-я закрытия окна
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		//Выбор оборудования из списка оборудования, обновляет список модификаций
		void CbxEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			cbxEquipment.Background = defaultBrush;
			if(modificationViewSource.Source != null)
			{
				modificationViewSource.View.Refresh();
			}
		}
		
		//Обработка события закрытия окна
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
		
		
		//----> Комплектации 
		
		//Нажатие на кнопку добавления комплектации, создает и добавляет комплектацию в коллекцию
		void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Complectation complectation = new Complectation();
				complectation.Complectation_count = 1;
				complectation.Id_complectation_item = 0;
				complectation.IsSaved = false;
				equipmentBid.ComplectationCollection.Add(complectation);
			}
			catch(Exception ex)
			{
				System.Windows.MessageBox.Show(ex.ToString());
			}
		}
		
		//Обработка изменения количества комплектаций (в каждой строке)
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
		
		//Клик по кнопке удаления комплектации, удаляет комплектацию
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
		
		//Изменение наименования комплектации, устанавливает статус "Не сохранено"
		void DlcComplectationItem_TextChanged(object sender, TextChangedEventArgs e)
		{
			StangradCRM.Controls.DownListControl downListControl 
				= sender as StangradCRM.Controls.DownListControl;
			
			if(downListControl == null) return;
			
			DataGridRow row = FindItem.FindParentItem<DataGridRow>(downListControl);
			if(row == null) return;
			
			Complectation complectation = (Complectation)row.Item;
			if(complectation == null) return;
			
			//complectation.Id_complectation_item = 0;
			complectation.NewComplectationItemName = downListControl.Text;
			
			complectation.IsSaved = false;
		}
		
		//Выбор существующего наименования из выпадающего списка
		void DlcComplectationItem_OnSelect(object sender, StangradCRM.Controls.SelectionChanged e)
		{
			StangradCRM.Controls.DownListControl downList 
				= sender as StangradCRM.Controls.DownListControl;
			if(downList == null) return;
			
			DataGridRow row = FindItem.FindParentItem<DataGridRow>(downList);
			if(row == null) return;
			
			Complectation complectation = (Complectation)row.Item;
			if(complectation == null) return;
			
			if(e.Value != null)
			{
				int id_complectation_item = (int)e.Value;
				if(complectation.Id_complectation_item == id_complectation_item)
				{
					complectation.IsSaved = true;
					return;
				}
				ComplectationItem item = ComplectationItemViewModel.instance().getById(id_complectation_item);
				complectation.Id_complectation_item = id_complectation_item;
				complectation.IsSaved = false;
			}
		}
		
		//Нажатие на кнпку сохранения комплектаций
		void BtnSaveAllComplectation_Click(object sender, RoutedEventArgs e)
		{
			prepareComplectationSave();
		}
		
		//Ф-я подготовки комплектаций к сохранению
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
		
		//Ф-я валидации комплектации перед сохранением
		bool validate (Complectation complectation)
		{
			if(complectation.Id_complectation_item == 0 
			   && complectation.NewComplectationItemName == "")
			{
				System.Windows.MessageBox.Show("Поле 'Наименование' не заполнено у одного или нескольких записей.\nСохранение не возможно!");
				return false;
			}
			if(complectation.Complectation_count < 1)
			{
				System.Windows.MessageBox.Show("Поле 'Количество' меньше 1 у одного или нескольких записей.\nСохранение не возможно!");
				return false;
			}
			return true;
		}
		
		//Ф-я сохранения комплектаций
		void complectationSave (Complectation complectation)
		{
			IsEnabled = false;
			loadingProgress.Visibility = Visibility.Visible;
			Task.Factory.StartNew(() => {
              	if(complectation.Id_complectation_item == 0)
              	{
              		ComplectationItem item = new ComplectationItem();
              		item.Name = complectation.NewComplectationItemName;
              		if(!item.save())
              		{
              			Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { complectationItemErrorSave(item); } ));
              			return;
              		}
          			complectation.Id_complectation_item = item.Id;
              	}
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
		
		//Ф-я обработки успешного сохранения комплектации
		void complectationSuccessSave (Complectation complectation)
		{
			complectation.IsSaved = true;
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			
		}
		
		//Ф-я обработки ошибочного сохранения комплектации
		void complectationErrorSave (Complectation complectation)
		{
			System.Windows.MessageBox.Show(complectation.LastError);
			complectation.IsSaved = true;
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
		}
		
		//Ф-я обработки ошибочного сохранения элемента комплектации
		void complectationItemErrorSave (ComplectationItem item)
		{
			System.Windows.MessageBox.Show(item.LastError);
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
		}
	}
}