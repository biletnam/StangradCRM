/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 31.08.2016
 * Время: 13:00
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for BidShipmentSaveWindow.xaml
	/// </summary>
	public partial class BidShipmentSaveWindow : Window
	{
		private Bid bid = null;
		private Action callback = null;
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));		
		
		private int current_archive = 0;
		
		public BidShipmentSaveWindow(Bid bid, Action callback = null)
		{
			InitializeComponent();
			defaultBrush = cbxTransportCompany.Background;
			
			Title += bid.Id.ToString();
			current_archive = bid.Is_archive;
			tbxComment.Text = bid.Comment;
			this.bid = bid;
			
			this.callback = callback;
			
			CollectionViewSource viewSource = new CollectionViewSource();
			viewSource.Source = TransportCompanyViewModel.instance().Collection;
			cbxTransportCompany.ItemsSource = viewSource.View;
			viewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			cbxTransportCompany.SelectedIndex = -1;
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			bid.Id_transport_company = (int)cbxTransportCompany.SelectedValue;
			bid.Is_shipped = 1;
			bid.Shipment_date = dpShipmentDate.SelectedDate;
			bid.Comment = tbxComment.Text;
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
              	if(bid.Debt == 0)
              	{
              		bid.Is_archive = 1;
              	}
              	if(bid.save())
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
			if(callback != null) callback();
			if(bid.Is_archive != 0 && current_archive != bid.Is_archive)
			{
				MessageBox.Show("Заявка передана в архив!");
			}
			Close();
		}
		
		private void errorSave()
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			MessageBox.Show("Не удалось изменить статус заявки.\n" + bid.LastError);
		}		
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		bool validate ()
		{
			if(dpShipmentDate.SelectedDate == null)
			{
				dpShipmentDate.Background = errorBrush;
				return false;
			}
			if(cbxTransportCompany.SelectedIndex == -1)
			{
				cbxTransportCompany.Background = errorBrush;
				return false;
			}
			return true;
		}
	}
}