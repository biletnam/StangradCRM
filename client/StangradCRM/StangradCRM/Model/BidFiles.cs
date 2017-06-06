/*
 * Created by SharpDevelop.
 * User: Дмитрий Строкин
 * Date: 24.04.2017
 * Time: 11:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using HTTPManager;
using StangradCRM.ViewModel;
using StangradCRMLibs;

namespace StangradCRM.Model
{
	/// <summary>
	/// Description of BidFiles.
	/// </summary>
	/// 
	
	public class ResponsePathData
	{
		public string Path { get; set; }
	}
	
	
	public class BidFiles : Core.Model
	{
		
		private readonly int chuckSize = 500000;
		private List<string> chucks = new List<string>();
		private int chucksCount = 0;
		
		public string Name { get; set; }
		public int Id_bid { get; set; }
		public string Path { get; set; }
		public byte [] FileBody { get; set; }
		public string Ext { get; set; }
		
		public bool IsCurrent { get; set; }
		
		public int PartCount 
		{
			get {
				return chucksCount;
			}
		}
		
		public int Part
		{
			get {
				return chucksCount - chucks.Count;
			}
		}
		
		public BidFiles() {}
		
		public override void replace(object o)
		{
			BidFiles bidFiles = o as BidFiles;
			if(bidFiles == null) return;
			
			Name = bidFiles.Name;
			
			raiseAllProperties();
		}
		
		public override void raiseAllProperties()
		{
			RaisePropertyChanged("Name", Name);
			RaisePropertyChanged("Id_bid", Id_bid);
			RaisePropertyChanged("Path", Path);
		}
		
		protected override void prepareSaveData(HTTPManager.HTTPRequest http)
		{
			string base64string = Convert.ToBase64String(FileBody);
			chucksCount = (int)Math.Ceiling((double)(base64string.Length / chuckSize));
			if(chucksCount < 2) {
				http.addParameter("file", base64string);
			}
			else {
				for(int i = 0; i < chucksCount; i++) {
					int pos = (i * chuckSize);
	
					if(i == chucksCount-1)
					{
						chucks.Add(base64string.Substring(pos));
					}
					else
					{
						chucks.Add(base64string.Substring(pos, chuckSize));
					}
				}
				http.addParameter("file", chucks[0]);
			}
			
			http.addParameter("name", Name);
			http.addParameter("id_bid", Id_bid);
			http.addParameter("ext", Ext);
			if(Id != 0)
			{
				http.addParameter("id", Id);
			}
			if(chucks.Count > 0)
				chucks.RemoveAt(0);
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
				return "BidFiles";
			}
		}
		
		protected override StangradCRM.Core.IViewModel CurrentViewModel {
			get {
				return BidFilesViewModel.instance();
			}
		}
		
		public override void beforeSave()
		{
			Bid bid = BidViewModel.instance().getById(Id_bid);
			if(bid != null && !bid.BidFilesCollection.Contains(this))
			{
				bid.BidFilesCollection.Add(this);
			}
		}
		
		protected override bool afterSave(StangradCRMLibs.ResponseParser parser)
		{
			bool result = base.afterSave(parser);
			Bid bid = BidViewModel.instance().getById(Id_bid);
			if(result)
			{
				
				if(chucks.Count > 0) {
					
					RaisePropertyChanged("PartCount", null);
					RaisePropertyChanged("Part", null);
					
					if(!saveChucks()) {
						remove();
						return false;
					}
				}
				
				ResponsePathData parserResult = parser.ToObject<ResponsePathData>();
				Path = parserResult.Path;
				
				raiseAllProperties();
				
				if(bid != null && !bid.BidFilesCollection.Contains(this))
				{
					bid.BidFilesCollection.Add(this);
				}
				
				IsSaved = true;
			}
			else {
				CurrentViewModel.remove(this);
				if(bid != null && bid.BidFilesCollection.Contains(this))
				{
					bid.BidFilesCollection.Remove(this);
				}
			}
			return result;
		}
		
		protected override bool afterRemove(ResponseParser parser, bool soft = false)
		{
			bool result = base.afterRemove(parser, soft);
			if(result)
			{
				Bid bid = BidViewModel.instance().getById(Id_bid);
				if(bid != null && bid.BidFilesCollection.Contains(this))
				{
					bid.BidFilesCollection.Remove(this);
				}
			}
			return result;
		}
		
		private bool saveChucks ()
		{
			while(chucks.Count > 0) {
				
				if(!saveChuck(chucks[0]))
					return false;
				
				chucks.RemoveAt(0);
				RaisePropertyChanged("Part", null);
			}
			return true;
		}
		
		private bool saveChuck (string chuck) {
			
			HTTPRequest http = Core.Model.Request();
			http.addParameter("entity", Entity + "/savechuck");
			http.addParameter("id", Id);
			http.addParameter("chuck", chuck);
			if(!Core.Model.exec(http, this))
			{
				return false;
			}

			ResponseParser parser = ResponseParser.Parse(http.ResponseData);
			
			if(!parser.NoError)
			{
				LastError = parser.LastError;
				return false;
			}
			if(parser.ServerErrorFlag != 0)
			{
				LastError = parser.ToObject<string>();
				return false;
			}
			return true;
		}
 		
	}
}
