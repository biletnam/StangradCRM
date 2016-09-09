/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 26.08.2016
 * Время: 17:17
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows.Controls;
using System.Windows.Data;

using StangradCRM.Model;
using StangradCRM.View.Controls.ManagerControls;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Helpers
{
	/// <summary>
	/// Description of ManagerMainWindowHelper.
	/// </summary>
	public static class ManagerMainWindowHelper
	{		
		public static UserControl GetControl (int bidStatusId)
		{
			UserControl control;
			CollectionViewSource bidViewSource = new CollectionViewSource();
			bidViewSource.Source = BidViewModel.instance().getCollectionByStatus(bidStatusId);		
			bidViewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				Bid bid = e.Item as Bid;
				if(bid == null) return;
				e.Accepted = bid.IsVisible;
			};

			switch (bidStatusId)
			{
					case 1:		
						control = new MainControlID1(bidViewSource);
						break;
					case 2:
						control = new MainControlID2(bidViewSource);
						break;
					default:
						control = new StangradCRM.View.Controls.DefaultMainControl();
						break;
			}
			return control;
		}
	}
}
