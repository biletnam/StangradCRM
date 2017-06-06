/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 13.09.2016
 * Время: 19:55
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for PlannedShipmentDateSetWindow.xaml
	/// </summary>
	public partial class PlannedShipmentDateSetWindow : Window
	{
		private Bid bid = null;
		private Action<DateTime> okCallback = null;
		private Action cancelCallback = null;
		
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));
		
		public PlannedShipmentDateSetWindow(Bid bid,
		                                    Action<DateTime> okCallback = null,
		                                    Action cancelCallback = null)
		{
			InitializeComponent();
			Title = "Установка предполагаемой даты отгрузки для заявки №" + bid.Id.ToString();
			defaultBrush = dpPlannedShipmentDate.Background;
			
			CRMSetting crmSetting = CRMSettingViewModel.instance().getByMashineName("planned_shipment_day_count");
			int dayCount = 0;
			if(crmSetting != null)
			{
				try
				{
					dayCount = int.Parse(crmSetting.Setting_value);
				} catch {}
			}
			if(bid.Planned_shipment_date == null)
			{
				dpPlannedShipmentDate.SelectedDate = DateTime.Now.AddDays(dayCount);			
			}
			else
			{
				dpPlannedShipmentDate.SelectedDate = bid.Planned_shipment_date;
			}
			dpPlannedShipmentDate.SelectedDateChanged += delegate { dpPlannedShipmentDate.Background = defaultBrush; };
			
			tbxComment.Text = bid.Comment;
			
			this.bid = bid;
			this.okCallback = okCallback;
			this.cancelCallback = cancelCallback;
			
			
			CollectionViewSource viewSource = new CollectionViewSource();
			viewSource.Source = TransportCompanyViewModel.instance().Collection;
			cbxPlannedTransportCompany.ItemsSource = viewSource.View;
			viewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			cbxPlannedTransportCompany.SelectedIndex = -1;
			
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			bid.Comment = tbxComment.Text;
			if(cbxPlannedTransportCompany.SelectedIndex != -1)
				bid.Id_transport_company = (int)cbxPlannedTransportCompany.SelectedValue;
			
			if(okCallback != null)
			{
				okCallback((DateTime)dpPlannedShipmentDate.SelectedDate);
			}
			Close();
		}
		
		private bool validate ()
		{
			if(dpPlannedShipmentDate.SelectedDate == null)
			{
				dpPlannedShipmentDate.Background = errorBrush;
				return false;
			}
			return true;
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			if(cancelCallback != null)
			{
				cancelCallback();
			}
			Close();
		}
		void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(cancelCallback != null)
			{
				cancelCallback();
			}
		}
	}
}