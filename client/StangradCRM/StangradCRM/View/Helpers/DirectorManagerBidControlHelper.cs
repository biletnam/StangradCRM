/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 02.09.2016
 * Время: 16:24
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows.Controls;
using System.Windows.Data;

using StangradCRM.Model;
using StangradCRM.View.Controls.DirectorControls;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View.Helpers
{
	/// <summary>
	/// Description of DirectorManagerBidControlHelper.
	/// </summary>
	public static class DirectorManagerBidControlHelper
	{
		
		public static UserControl GetControl (int bidStatusId)
		{
			UserControl control;
			CollectionViewSource viewSource = new CollectionViewSource();
			viewSource.Source = BidViewModel.instance().getCollectionByStatus(bidStatusId);
			
			viewSource.Filter += delegate(object sender, FilterEventArgs e)
			{
				Bid bid = e.Item as Bid;
				if(bid == null) return;
				if(bid.Id_manager == Auth.getInstance().Id)
				{
					e.Accepted = false;
					return;
				}
				e.Accepted = bid.IsVisible;
			};
			switch (bidStatusId)
			{
					case 1:		
						control = new ManagerBidControlID1(viewSource);
						break;
					case 2:
						control = new ManagerBidControlID2(viewSource);
						break;
					default:
						control = new StangradCRM.View.Controls.DefaultMainControl();
						break;
			}
			return control;
		}
	}
}
