/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 03.05.2017
 * Time: 11:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace StangradCRM.Classes
{
	/// <summary>
	/// Description of IsNumberic.
	/// </summary>
	public static class IsNumberic
	{
		public static bool Valid(string text) {
		    Regex regex = new Regex("[^0-9.-]+");
		    return !regex.IsMatch(text);
		}
	}
}
