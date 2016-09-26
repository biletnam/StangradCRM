/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 21.09.2016
 * Время: 16:40
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of OpenDirectoryDialog.
	/// </summary>
	public static class OpenDirectoryDialog
	{
        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow (System.IntPtr handle)
            {
                _handle = handle;
            }
            #region
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
}
	}
}
