/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 08/23/2016
 * Время: 14:07
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace StangradCRM.Core
{
	/// <summary>
	/// Потокобезопасный класс ObservableCollection
	/// </summary>
	public class TSObservableCollection<T> : ObservableCollection<T>
	{
    	public override event NotifyCollectionChangedEventHandler CollectionChanged;
    	private bool suspendCollectionChangeNotification = false;
		
    	public TSObservableCollection () : base() {}
    	public TSObservableCollection (List<T> collection) : base(collection) {}
    	public TSObservableCollection (IEnumerable<T> collection) : base(collection) {}
    	
    	
	  	protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
	  	{
	        if (!this.suspendCollectionChangeNotification)
	        {
		        NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
		        if (eventHandler != null)
		        {
		            Delegate[] delegates = eventHandler.GetInvocationList();
		            bool isEventInvoked = false;
		            foreach (NotifyCollectionChangedEventHandler handler in delegates)
		            {
		                isEventInvoked = false;
		                if (handler.Target is DispatcherObject)
		                {
		                    DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
		                    if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
		                    {
		                        dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
		                        isEventInvoked = true;
		                    }
		                }
		
		                if (!isEventInvoked)
		                {
		                    handler(this, e);
		                }
	                }
	            }
	        }
		}
	}
}