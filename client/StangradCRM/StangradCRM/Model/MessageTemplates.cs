/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 15.05.2017
 * Time: 11:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using StangradCRM.ViewModel;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of MessageTemplates.
	/// </summary>
	public class MessageTemplates : Core.Model
	{
		public string Name { get; set; }
		public string Theme { get; set; }
		public string Template { get; set; }
		
		public MessageTemplates() {}
		
		public override void replace(object o)
		{
			MessageTemplates messageTemplate = o as MessageTemplates;
			if(messageTemplate == null) return;
			
			Name = messageTemplate.Name;
			Template = messageTemplate.Template;
			
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
			RaisePropertyChanged("Template", Template);
			RaisePropertyChanged("Theme", Theme);
		}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			http.addParameter("name", Name);
			http.addParameter("template", Template);
			http.addParameter("theme", Theme);
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
		}
		
		protected override void prepareRemoveData(HTTPManager.HTTPRequest http)
		{
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
		}
		
		protected override string Entity {
			get {
				return "MessageTemplates";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return MessageTemplatesViewModel.instance();
			}
		}
	}
}
