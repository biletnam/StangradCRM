/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 12.08.2016
 * Время: 13:29
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StangradCRM.Core
{
	/// <summary>
	/// Description of BaseViewModel.
	/// </summary>
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
		
	    protected void RaisePropertyChanged(string name, object v) {
	        if (PropertyChanged != null) {
	            PropertyChanged(this, new PropertyChangedEventArgs(name));
	        }
	    }
		public event PropertyChangedEventHandler PropertyChanged;
		
	}
}
