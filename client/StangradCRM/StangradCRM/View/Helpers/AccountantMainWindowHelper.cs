/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 02.09.2016
 * Время: 17:08
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Windows.Controls;
using System.Windows.Data;

using StangradCRM.Model;
using StangradCRM.View.Controls.AccountantControls;
using StangradCRM.ViewModel;

namespace StangradCRM.View.Helpers
{
	/// <summary>
	/// Description of AccountantMainWindowHelper.
	/// </summary>
	public static class AccountantMainWindowHelper
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
				e.Accepted = bid.IsVisible;
			};
			switch (bidStatusId)
			{
					case 1:		
						control = new MainControlID1(viewSource);
						break;
					case 2:
						control = new MainControlID2(viewSource);
						break;
					default:
						control = new StangradCRM.View.Controls.DefaultMainControl();
						break;
			}
			return control;
		}
		
	}
}
