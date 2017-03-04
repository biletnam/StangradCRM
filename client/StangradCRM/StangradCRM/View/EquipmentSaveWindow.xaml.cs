/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 15.08.2016
 * Время: 11:02
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

using System.Windows;

using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Threading;
using StangradCRM.Model;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for EquipmentSaveWindow.xaml
	/// </summary>
	public partial class EquipmentSaveWindow : Window
	{
		private Equipment equipment = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public EquipmentSaveWindow(Equipment equipment = null)
		{
			InitializeComponent();
			
			defaultBrush = tbxName.Background;

			if(equipment != null)
			{
				Title = "Редактирование оборудования (" + equipment.Name + ")";
				tbxName.Text = equipment.Name;
				tbxSerialNumber.Text = equipment.Serial_number.ToString();
				
				this.equipment = equipment;
			}
			else 
			{
				this.equipment = new Equipment();
			}
			
			tbxName.TextChanged += delegate { tbxName.Background = defaultBrush; };
			tbxSerialNumber.TextChanged += delegate { tbxSerialNumber.Background = defaultBrush; };
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			equipment.Name = tbxName.Text;
			if(tbxSerialNumber.Text == "")
			{
				equipment.Serial_number = null;
			}
			else
			{
				equipment.Serial_number = Int32.Parse(tbxSerialNumber.Text);
			}
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
				if(equipment.save())
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(); } ));
				}
				else 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave(); } ));
				}
			});

		}
		
		private bool validate ()
		{
			if(tbxName.Text == "")
			{
				tbxName.Background = errorBrush;
				return false;
			}
			try
			{
				if(tbxSerialNumber.Text == "")
				{
					return true;
				}
				int serialNumber = Int32.Parse(tbxSerialNumber.Text);
				return true;
			}
			catch
			{
				tbxSerialNumber.Background = errorBrush;
				return false;
			}
		}
		
		private void successSave()
		{
			Close();
		}
		
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			MessageBox.Show(equipment.LastError);
		}
		
	}
}