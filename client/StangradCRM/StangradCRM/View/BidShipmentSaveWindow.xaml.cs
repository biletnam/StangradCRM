/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 31.08.2016
 * Время: 13:00
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using StangradCRM.Classes;
using StangradCRM.Core;
using StangradCRM.Model;
using StangradCRM.ViewModel;

namespace StangradCRM.View
{
	/// <summary>
	/// Interaction logic for BidShipmentSaveWindow.xaml
	/// </summary>
	public partial class BidShipmentSaveWindow : Window
	{
		private Bid bid = null;
		private Action callback = null;
		private Brush defaultBrush;
		private Brush errorBrush = new SolidColorBrush(Color.FromRgb(250, 200, 200));		
		
		private int current_archive = 0;
		
		CollectionViewSource bidFilesViewSource = new CollectionViewSource();
		
		public BidShipmentSaveWindow(Bid bid, Action callback = null)
		{
			InitializeComponent();
			defaultBrush = cbxTransportCompany.Background;
			
			Title += bid.Id.ToString();
			current_archive = bid.Is_archive;
			tbxComment.Text = bid.Comment;
			this.bid = bid;
			
			this.callback = callback;
			
			CollectionViewSource viewSource = new CollectionViewSource();
			viewSource.Source = TransportCompanyViewModel.instance().Collection;
			cbxTransportCompany.ItemsSource = viewSource.View;
			viewSource.SortDescriptions.Add(new SortDescription("Row_order", ListSortDirection.Descending));
			cbxTransportCompany.SelectedIndex = -1;
			
			if(bid.Id_transport_company != null)
				cbxTransportCompany.SelectedValue = bid.Id_transport_company;
			
			tbxWaybill.Text = bid.Waybill;
			
			bidFilesViewSource.Source = bid.BidFilesCollection;
			bidFilesViewSource.Filter += delegate(object sender, FilterEventArgs e) 
			{
				BidFiles bidFiles = e.Item as BidFiles;
				if(bidFiles == null)
				{
					e.Accepted = false;
					return;
				}
				e.Accepted = bidFiles.IsCurrent;
			};
			
			DataContext= new 
			{
				BidFilesCollection = bidFilesViewSource.View,
			};
			
			
		}
		
		void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!validate()) return;
			
			if(cbIsMailSend.IsChecked == false)
			{
				saveBid();
			}
			else 
			{
				sendMail();
			}
			
		}
		
		void saveBid () 
		{
			bid.Id_transport_company = (int)cbxTransportCompany.SelectedValue;
			bid.Is_shipped = 1;
			bid.Shipment_date = dpShipmentDate.SelectedDate;
			bid.Waybill = tbxWaybill.Text;
			bid.Comment = tbxComment.Text;
			loadingProgress.Visibility = Visibility.Visible;
			IsEnabled = false;
			
			Task.Factory.StartNew(() => {
              	if(bid.Debt == 0)
              	{
              		bid.Is_archive = 1;
              	}
              	if(bid.save())
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { successSave(); } ));
				}
				else 
				{
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { errorSave("Не удалось изменить статус заявки.\n" + bid.LastError); } ));
				}
			});
		}
		
		private void successSave()
		{
			if(callback != null) callback();
			if(bid.Is_archive != 0 && current_archive != bid.Is_archive)
			{
				MessageBox.Show("Заявка передана в архив!");
			}
			Close();
		}
		
		private void errorSave(string message)
		{
			loadingProgress.Visibility = Visibility.Hidden;
			IsEnabled = true;
			MessageBox.Show(message);
		}		
		
		void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		bool validate ()
		{
			if(dpShipmentDate.SelectedDate == null)
			{
				dpShipmentDate.Background = errorBrush;
				return false;
			}
			if(cbxTransportCompany.SelectedIndex == -1)
			{
				cbxTransportCompany.Background = errorBrush;
				return false;
			}
			return true;
		}
		
		void Btn_add_file_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Multiselect = true;
			if(dialog.ShowDialog() != true) return;
			
			for(int i = 0; i < dialog.FileNames.Length; i++) {
				saveFile(dialog.FileNames[i], dialog.SafeFileNames[i]);
			}
		}
		
		void BtnDeleteFile_Click(object sender, RoutedEventArgs e)
		{
			BidFiles bidFile = dgvBidFiles.SelectedItem as BidFiles;
			if(bidFile == null) return;
			if(MessageBox.Show("Удалить файл?", "Удалить файл?", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
			if(!bidFile.remove()) {
				MessageBox.Show(bidFile.LastError);
			}
		}
		
		void saveFile (string fileName, string safeFileName) {
			
			if(new FileInfo(fileName).Length > 6500000)
			{
				MessageBox.Show("Файл " + safeFileName + " не может быть сохранен.\nРазмер больше 6.5 мегабайт.");
				return;
			}
			
			BidFiles bidFile = new BidFiles();
			bidFile.Name = safeFileName;
			bidFile.Id_bid = bid.Id;
			bidFile.FileBody = File.ReadAllBytes(fileName);
			bidFile.Ext = Path.GetExtension(fileName);
			bidFile.IsCurrent = true;
			
			//start task
			Task.Factory.StartNew(() => {
				//save file
				if(!bidFile.save()) {
					MessageBox.Show(bidFile.LastError);
					return;
				}
				else {
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => {  } ));
				}
			});
		}
		
		void sendMail () {
			
			Buyer buyer = bid.BidBuyer;
			
			CRMSettingViewModel crmSetting = CRMSettingViewModel.instance();
			
			MailMessage message = new MailMessage();
			
			message.To.Add(buyer.Email);
			message.From = new MailAddress(crmSetting.getByMashineName("email").Setting_value);
			message.Subject = getTheme();
			message.Body = getMessageBody();
			message.IsBodyHtml = true;
			
			TSObservableCollection<BidFiles> bidFiles = 
				bid.BidFilesCollection;
			
			List<MemoryStream> streamsList = new List<MemoryStream>();
			
			for(int i = 0; i < bidFiles.Count; i++) {
				if(bidFiles[i].IsCurrent == false) continue;
				
				MemoryStream stream = new MemoryStream(bidFiles[i].FileBody);
				Attachment attachment = new Attachment(stream, bidFiles[i].Name);
				message.Attachments.Add(attachment);
				streamsList.Add(stream);
			}
			
			SmtpClient smtpClient = new SmtpClient(crmSetting.getByMashineName("smtp_server").Setting_value, int.Parse(crmSetting.getByMashineName("smtp_port").Setting_value));
			smtpClient.Credentials = new NetworkCredential(crmSetting.getByMashineName("mail_user").Setting_value, crmSetting.getByMashineName("mail_password").Setting_value);
			int useSSL = int.Parse(crmSetting.getByMashineName("mail_use_ssl").Setting_value);
			if(useSSL == 1)
				smtpClient.EnableSsl = true;
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			
			loadingProgress.Visibility = Visibility.Visible;
			
			Task.Factory.StartNew(() => {
			
				try {
					smtpClient.Send(message);
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { closeStreams(streamsList); saveBid(); } ));
				}
				catch(Exception ex) {
					Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () => { closeStreams(streamsList); errorSave("Не отправить письмо.\n" + ex.ToString()); } ));
				}
			                      	
			});
		}
		
		
		void closeStreams (List<MemoryStream> streams) 
		{
			for(int i = 0; i < streams.Count; i++)
			{
				streams[i].Close();
				streams[i].Dispose();
			}
		}
		
		void CbIsMailSend_Click(object sender, RoutedEventArgs e)
		{
			if((bool)cbIsMailSend.IsChecked)
			{
				gbxBidFiles.Visibility = Visibility.Visible;
			}
			else 
			{
				gbxBidFiles.Visibility = Visibility.Collapsed;
			}
		}
		
		string getTheme ()
		{
			int transportCompanyId = (int)cbxTransportCompany.SelectedValue;
			TransportCompany transportCompany = TransportCompanyViewModel.instance().getById(transportCompanyId);
			
			if(transportCompany == null) return "";
			
			MessageTemplates messageTemplate = MessageTemplatesViewModel.instance().getById(transportCompany.Id_message_template);
			
			if(messageTemplate == null) return "";
			
			return messageTemplate.Theme;
		}
		
		string getMessageBody () 
		{
			
			int transportCompanyId = (int)cbxTransportCompany.SelectedValue;
			TransportCompany transportCompany = TransportCompanyViewModel.instance().getById(transportCompanyId);
			
			if(transportCompany == null) return "";
			
			MessageTemplates messageTemplate = MessageTemplatesViewModel.instance().getById(transportCompany.Id_message_template);
			
			if(messageTemplate == null) return "";
			
			string msg = messageTemplate.Template;
			
			msg = msg.Replace("{SHIPMENT_DATE}", dpShipmentDate.SelectedDate.Value.ToString("dd.MM.yyyy"));
			msg = msg.Replace("{TRANSPORT_COMPANY_NAME}", transportCompany.Name);
			msg = msg.Replace("{WAYBILL}", tbxWaybill.Text);
			msg = msg.Replace("{TRANSPORT_COMPANY_SITE}", transportCompany.Site);
			
			return msg;
		}
		
		
		//Изменение выбранной транспортной компании
		void CbxTransportCompany_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			
			if(bid.BidBuyer.Email != "" && IsEmail.Valid(bid.BidBuyer.Email))
			{
				cbIsMailSend.IsChecked = true;
				gbxBidFiles.Visibility = Visibility.Visible;
			}
			else 
			{
				cbIsMailSend.IsChecked = false;
				cbIsMailSend.IsEnabled = false;
				gbxBidFiles.Visibility = Visibility.Collapsed;
				return;
			}
			
			if(e.AddedItems.Count == 0) return;
			
			TransportCompany tc = e.AddedItems[0] as TransportCompany;
			if(tc == null) 
			{
				cbIsMailSend.IsChecked = false;
				cbIsMailSend.IsEnabled = false;
				gbxBidFiles.Visibility = Visibility.Collapsed;
				return;
			}
			
			MessageTemplates messageTemplate = MessageTemplatesViewModel.instance().getById(tc.Id_message_template);
			//Если шаблон null
			if(messageTemplate == null)
			{
				cbIsMailSend.IsChecked = false;
				cbIsMailSend.IsEnabled = false;
				gbxBidFiles.Visibility = Visibility.Collapsed;
				return;
			}

			// Если шаблон пустой
			if(messageTemplate.Template == "")
			{
				cbIsMailSend.IsChecked = false;
				cbIsMailSend.IsEnabled = false;
				gbxBidFiles.Visibility = Visibility.Collapsed;
			}
			else 
			{
				cbIsMailSend.IsEnabled = true;
				cbIsMailSend.IsChecked = true;
				gbxBidFiles.Visibility = Visibility.Visible;
			}
		}
		
	}
}