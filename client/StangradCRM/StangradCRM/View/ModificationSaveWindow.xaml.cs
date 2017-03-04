/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/15/2016
 * Время: 14:00
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for ModificationSaveWindow.xaml
	/// </summary>
	public partial class ModificationSaveWindow : Window
	{
		private Equipment equipment = null;
		private Modification modification = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public ModificationSaveWindow(Equipment equipment = null, Modification modification = null)
		{
			InitializeComponent();
			
			defaultBrush = tbxName.Background;
			cbxEquipment.ItemsSource = EquipmentViewModel.instance().Collection;
			if(equipment != null)
			{
				this.equipment = equipment;
				cbxEquipment.SelectedItem = equipment;
			}
			
			if(modification != null)
			{
				Title = "Редактирование модификации (" + modification.Name + ")";
				tbxName.Text = modification.Name;
				cbxEquipment.SelectedItem = EquipmentViewModel.instance().getById(modification.Id_equipment);
				this.modification = modification;
			}
			else 
			{
				this.modification = new Modification();
			}
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
			cbxEquipment.SelectionChanged += delegate { cbxEquipment.Background = defaultBrush; };
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			modification.Name = tbxName.Text;
			modification.Id_equipment = (int)cbxEquipment.SelectedValue;

			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(modification.save())
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
			MessageBox.Show(modification.LastError);
		}
		
		private bool validate ()
		{
			if(tbxName.Text == "")
			{
				tbxName.Background = errorBrush;
				return false;
			}
			if(cbxEquipment.SelectedIndex == -1)
			{
				cbxEquipment.Background = errorBrush;
				return false;
			}
			return true;
		}
		
	}
}