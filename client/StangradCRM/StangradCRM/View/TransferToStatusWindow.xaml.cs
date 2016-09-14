/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 13.09.2016
 * Время: 18:21
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

using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for TransferToStatusWindow.xaml
	/// </summary>
	public partial class TransferToStatusWindow : Window
	{
		Bid bid;
		Action callback;
		public TransferToStatusWindow(Bid bid, Action callback = null)
		{
			InitializeComponent();
			Title = "Передача заявки №" + bid.Id.ToString() + " в другой статус.";
			BidStatus current = BidStatusViewModel.instance().getById(bid.Id_bid_status);
			List<BidStatus> bid_status = BidStatusViewModel.instance().Collection.ToList();
			if(current != null && bid_status.Contains(current))
			{
				bid_status.Remove(current);
			}
			
			DataContext = new
			{
				StatusCollection = bid_status
			};
			
			this.bid = bid;
			this.callback = callback;
		}
		
		void BtnTransfer_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			if(sender == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			BidStatus status = row.Item as BidStatus;
			if(status == null) return;
			
			if(MessageBox.Show("Передать заявку в статус '" + status.Name + "'?", 
			                   "Передать заявку в другой статус?", 
			                   MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
			bid.Id_bid_status = status.Id;
			if(bid.Id_bid_status == (int)Classes.BidStatus.InWork)
			{
				PlannedShipmentDateSetWindow window 
					= new PlannedShipmentDateSetWindow(bid, new Action<DateTime>( (planned_shipment_date) => { bid.Planned_shipment_date = planned_shipment_date; }));
				window.ShowDialog();
			}
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}
			Close();
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}