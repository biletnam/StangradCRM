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
		public static void Start (Dispatcher dispatcher, 
		                          int updateTime = 60000, 
		                          Action beforeCallback = null, 
		                          Action afterCallback = null)
		{
			Task.Factory.StartNew( () => 
            {
				while(true) 
				{
					if(beforeCallback != null)
					{
						dispatcher.BeginInvoke(DispatcherPriority.Background, beforeCallback);
					}
					System.Threading.Thread.Sleep(updateTime);
					if(afterCallback != null)
					{
						dispatcher.BeginInvoke(DispatcherPriority.Background, afterCallback);
					}
					dispatcher.BeginInvoke(DispatcherPriority.Background, 
					                       new Action(() => BidViewModel.instance().RemoteLoad()));
				}
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
		}
	}
}
