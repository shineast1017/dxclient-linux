using System;
using Gtk;
using Mono.Unix;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class addServerPageWidget : Gtk.Bin
    {
		private int _nEditMode = 0;
		private int _nIdx = 0;

		public addServerPageWidget()
        {
            this.Build();

			StyleSheet.SetStyleCancelButton(this.btnCancel);
			StyleSheet.SetStyleButton(this.btnAdd);
		}

		public void Initialization(int nEditMode, int nIdx, string strName, string strIP, string strPort)
		{
			this._nEditMode = nEditMode;
			this._nIdx = nIdx;

			if (nEditMode == 0)
			{
				this.tbxServerName.Text = this.tbxServerIP.Text = this.tbxServerPort.Text = "";
				this.labelTitle.LabelProp = string.Format("<span size='14000'>{0}</span>", Catalog.GetString("Add Server"));
				this.btnAdd.Label = Catalog.GetString("ADD");
				StyleSheet.SetStyleButton(this.btnAdd);
			}
			else
			{
				this.tbxServerName.Text = strName;
				this.tbxServerIP.Text = strIP;
				this.tbxServerPort.Text = strPort;
				this.labelTitle.LabelProp = string.Format("<span size='14000'>{0}</span>", Catalog.GetString("Edit Server"));
				this.btnAdd.Label = Catalog.GetString("EDIT");
				StyleSheet.SetStyleButton(this.btnAdd);
			}
		}

		protected void OnBtnCancelAddServerClicked(object sender, EventArgs e)
		{
			if (MainFunc.callbackShowServerListPage != null)
				MainFunc.callbackShowServerListPage();
		}

		protected void OnBtnOKAddServerClicked(object sender, EventArgs e)
		{
			if (MainFunc.callbackEditServerItem != null)
			{
				if(this._nEditMode == 1)
				{
					// force change
					/*
					ResponseType result = ErrorHandlerManager.QuestionMessage("기존 Auto 설정을 해제하고, 재설정하시겠습니까?");
					MessageDialog md = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
                                                         MessageType.Question, ButtonsType.YesNo, "Modify?");

                    ResponseType result = (ResponseType)md.Run();
                    md.Destroy();

                    if (result != ResponseType.Yes)
                    {
						return;
                    }
                    */
				}
				MainFunc.callbackEditServerItem(this._nEditMode, this._nIdx, this.tbxServerName.Text, this.tbxServerIP.Text, this.tbxServerPort.Text);
			}
		}
	}
}
