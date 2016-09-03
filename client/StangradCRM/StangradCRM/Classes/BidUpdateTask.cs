/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 02.09.2016
 * Время: 17:28
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

using StangradCRM.ViewModel;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of BidUpdateTask.
	/// </summary>
	public static class BidUpdateTask
	{
		public static void Start (Dispatcher dispatcher)
		{
			Task.Factory.StartNew( () => 
            {
				while(true) 
				{
					System.Threading.Thread.Sleep(40000);
					dispatcher.BeginInvoke(DispatcherPriority.Background, 
					                       new Action(() => BidViewModel.instance().RemoteLoad()));
				}
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
		}
	}
}
