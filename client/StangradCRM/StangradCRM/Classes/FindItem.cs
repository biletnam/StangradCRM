/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 19.08.2016
 * Время: 11:10
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of FindParent.
	/// </summary>
	public static class FindItem
	{
		public static Parent FindParentItem<Parent>(DependencyObject child)
		            where Parent : DependencyObject
		{
		   DependencyObject parentObject = child;
		
		   //We are not dealing with Visual, so either we need to fnd parent or
		   //get Visual to get parent from Parent Heirarchy.
		   while (!((parentObject is System.Windows.Media.Visual)
		           || (parentObject is System.Windows.Media.Media3D.Visual3D)))
		   {
		       if (parentObject is Parent || parentObject == null)
		       {
		           return parentObject as Parent;
		       }
		       else
		       {
		          parentObject = (parentObject as FrameworkContentElement).Parent;
		       }
		    }
		
		    //We have not found parent yet , and we have now visual to work with.
		    parentObject = VisualTreeHelper.GetParent(parentObject);
		
		    //check if the parent matches the type we're looking for
		    if (parentObject is Parent || parentObject == null)
		    {
		       return parentObject as Parent;
		    }
		    else
		    {
		        //use recursion to proceed with next level
		        return FindParentItem<Parent>(parentObject);
		    }
		}
		
		public static UIElement FindUid(this DependencyObject parent, string uid)
		{
		    var count = VisualTreeHelper.GetChildrenCount(parent);
		    if (count == 0) return null;
		
		    for (int i = 0; i < count; i++)
		    {
		        var el = VisualTreeHelper.GetChild(parent, i) as UIElement;
		        if (el == null) continue;
		
		        if (el.Uid == uid) return el;
		
		        el = el.FindUid(uid);
		        if (el != null) return el;
		    }
		    return null;
		}
		
	    public static T GetVisualChild<T>(this Visual parent) where T : Visual
	    {
	        T child = default(T);
	
	        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
	        {
	            Visual visualChild = (Visual)VisualTreeHelper.GetChild(parent, index);
	            child = visualChild as T;
	
	            if (child == null)
	                child = GetVisualChild<T>(visualChild);//Find Recursively
	
	            if (child != null)
	                break;
	        }
	        return child;
	    }
		
	}
}
