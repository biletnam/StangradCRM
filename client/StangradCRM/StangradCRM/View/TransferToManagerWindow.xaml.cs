/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 13.09.2016
 * Время: 17:49
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using StangradCRM.Model;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for TransferToManagerWindow.xaml
	/// </summary>
	public partial class TransferToManagerWindow : Window
	{
		Bid bid;
		Action callback;
		public TransferToManagerWindow(Bid bid, Action callback = null)
		{
			InitializeComponent();
			Title = "Передача заявки №" + bid.Id.ToString() + " другому менеджеру.";
			List<Manager> manager = ManagerViewModel.instance().Collection.ToList();
			Manager currentManager = ManagerViewModel.instance().getById(Auth.getInstance().Id);
			
			if(manager.Contains(currentManager))
			{
				manager.Remove(currentManager);
			}
		
			//Удаление всех пользователей, роль которых != менеджер или директор
			manager.RemoveAll(x => (x.Id_role != (int)Classes.Role.Manager) && (x.Id_role != (int)Classes.Role.Director));
			
			tbxComment.Text = bid.Comment;
			
			DataContext = new
			{
				ManagerCollection = manager
			};
			this.bid = bid;
			this.callback = callback;
		}
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void BtnTransfer_Click(object sender, RoutedEventArgs e)
		{			
			Button button = sender as Button;
			if(sender == null) return;
			
			DataGridRow row = Classes.FindItem.FindParentItem<DataGridRow>(button);
			if(row == null) return;
			
			Manager manager = row.Item as Manager;
			if(manager == null) return;
			
			if(MessageBox.Show("Передать заявку менеджеру " + manager.Name + "?", 
			                   "Передать заявку другому менеджеру?", 
			                   MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
			bid.Id_manager = manager.Id;
			bid.Comment = tbxComment.Text;
			if(!bid.save())
			{
				MessageBox.Show(bid.LastError);
				return;
			}
			bid.remove(true);
			Close();
		}
	}
}