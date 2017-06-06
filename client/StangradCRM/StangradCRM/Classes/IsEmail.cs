/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 03.05.2017
 * Time: 11:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of IsEmail.
	/// </summary>
	public static class IsEmail
	{
		public static bool Valid(string email) {
			try {
		        var addr = new System.Net.Mail.MailAddress(email);
		        return addr.Address == email;
			}
			catch {
				return false;
			}
		}
	}
}
