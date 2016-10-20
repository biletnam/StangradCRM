/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 02.09.2016
 * Время: 17:28
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using StangradCRM.ViewModel;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of UpdateTask.
	/// </summary>
	public static class UpdateTask
	{
		public static void Start (Dispatcher dispatcher,
		                          Action action,
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
					if(action != null)
						action();
				}
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
		}
		
		public static void StartSingle (Dispatcher dispatcher, 
	                              Action action,
		                          Action beforeCallback = null, 
		                          Action afterCallback = null)
		{
			Task.Factory.StartNew( () => 
            {
				if(beforeCallback != null)
				{
					dispatcher.BeginInvoke(DispatcherPriority.Background, beforeCallback);
				}
				if(action != null)
					action();
				if(afterCallback != null)
				{
					dispatcher.BeginInvoke(DispatcherPriority.Background, afterCallback);
				}
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
		}
		
	}
}
