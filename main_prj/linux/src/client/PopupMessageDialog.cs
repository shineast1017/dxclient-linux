using System;
using Newtonsoft.Json.Linq;
namespace client
{
	public class SendMsg
	{
		public string url_ { set; get; }
		public object Body_ { set; get; }
	}


	public partial class PopupMessageDialog : Gtk.Dialog {

		const string TYPE_CHECK_ACK = "CHECK_ACK";


		JArray _JArrData = null;

		public PopupMessageDialog ()
		{
			this.Build();
		}

		public PopupMessageDialog (JArray inArrData)
		{
			if (inArrData != null) {
				_JArrData = (JArray)inArrData.DeepClone();

			}
			this.Build();
		}


		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			SendMsg _sendMsg = null;
			// if _addData have data,

			if (_JArrData != null && _JArrData.Count > 0) 
			{
				foreach (JObject sub_msg in _JArrData) 
				{
					// find send message
					_sendMsg = GetSendMsgToBroker (TYPE_CHECK_ACK);
					if(_sendMsg != null)
						MainWindow.mainWindow.SendConfirmMessageToBroker (_sendMsg.Body_, _sendMsg.url_);
				}
			}

			this.Destroy ();
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			this.Destroy ();
		}

		private SendMsg GetSendMsgToBroker(string sType)
		{
			SendMsg retMsg = null;

			foreach (JObject sub_msg in _JArrData)
			{
				// find
				if (sub_msg ["type"].ToString () == sType) {

					if(retMsg == null) retMsg = new SendMsg ();

					retMsg.url_ = sub_msg ["url"].ToString ();
					retMsg.Body_ = sub_msg ["body"];
				}

			}

			return retMsg;
		}


		public void SetMessage (string content, string time)
		{
			tbxContent.Buffer.Text = content;
			lb_time.Text = time;
		}

	}
}
