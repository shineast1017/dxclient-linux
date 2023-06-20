using System;
using System.Linq;
using Gtk;
using HCVK.HCVKSLibrary.VO;
using Mono.Unix;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class editDesktopPageWidget : Gtk.Bin
    {
		private VODesktopPoolEx _desktopPoolEx = null;

		public editDesktopPageWidget()
        {
            this.Build();

			StyleSheet.SetStyleCancelButton(this.btnCancel);
			StyleSheet.SetStyleButton(this.btnOK);
        }

        /// <summary>
        /// Ons the button cancel clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
		protected void OnBtnCancelClicked(object sender, EventArgs e)
		{
			MainFunc.callbackEditDesktopItem = null;

			if (MainFunc.callbackShowDesktopListPage != null)
                MainFunc.callbackShowDesktopListPage(false);
		}

        /// <summary>
        /// Ons the button OKC licked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
		protected void OnBtnOKClicked(object sender, EventArgs e)
		{
			//MessageDialog md = new MessageDialog(MainWindow.mainWindow, DialogFlags.DestroyWithParent,
			//                                     MessageType.Question, ButtonsType.YesNo, "Modify?");

			//ResponseType result = (ResponseType)md.Run();
			//md.Destroy();
			ResponseType result = ErrorHandlerManager.QuestionMessage(Catalog.GetString("변경된 내용을 저장하시겠습니까?"));

            if (result == ResponseType.Yes)
            {
				Save();


            }
		}

		public void SetDesktopInfo(VODesktopPoolEx vODesktopPoolEx)
		{
            this._desktopPoolEx = vODesktopPoolEx;

            this.tbxName.Text = _desktopPoolEx.DesktopPool.Desktop.DesktopName;
            this.tbxIP.Text = _desktopPoolEx.DesktopPool.Desktop.DesktopIP;
            this.tbxType.Text = _desktopPoolEx.DesktopPool.AccessDiv;

            // protocol 
            this.cbxProtocol.Clear();

            CellRendererText cell = new CellRendererText();
            this.cbxProtocol.PackStart(cell, false);
            this.cbxProtocol.AddAttribute(cell, "text", 0);

            ListStore store = new ListStore(typeof(string));
            this.cbxProtocol.Model = store;

            string[] strAvailableProtocols = _desktopPoolEx.DesktopPool.SupportProtocols.Split(VOProtocol.VOProtocolType.PROTOCOL_SEPARATOR);
            if (strAvailableProtocols.Length > 0)
            {
                //strAvailableProtocols.ToList().ForEach(s => this.cbxProtocol.AppendText(s));
                for (int nIndex = 0; nIndex < strAvailableProtocols.Length; nIndex++)
                {
                    this.cbxProtocol.AppendText(strAvailableProtocols[nIndex]);
                    if (_desktopPoolEx.Protocol.Equals(strAvailableProtocols[nIndex], StringComparison.CurrentCultureIgnoreCase))
                    {
                        this.cbxProtocol.Active = nIndex;
                    }
                }

                if (this.cbxProtocol.Active == -1)
                {
                    this.cbxProtocol.Active = 0;
                }
            }

            this.cbxResolution.Active = MainWindow.mainWindow.environment.vODisplayProperties.Resolution;

            if (MainWindow.mainWindow.environment.vOPerformanceProperties.IsAudio)
                this.radioAudioOn.Active = true;
            else
                this.radioAudioOff.Active = true;

            this.cbxSpeed.Active = MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoS;
        }
        
		public void Save()
		{
			this._desktopPoolEx.DesktopPool.Desktop.DesktopName = this.tbxName.Text;
			this._desktopPoolEx.Protocol = this.cbxProtocol.ActiveText;
			this._desktopPoolEx.ResolutionIndex = this.cbxResolution.Active;
			this._desktopPoolEx.Audio = this.radioAudioOn.Active;
			this._desktopPoolEx.SpeedIndex = this.cbxSpeed.Active;

            MainWindow.mainWindow.environment.vOGeneralsProperties.Protocol = this.cbxProtocol.ActiveText;
            MainWindow.mainWindow.environment.vODisplayProperties.Resolution = this.cbxResolution.Active;
            MainWindow.mainWindow.environment.vOPerformanceProperties.IsAudio = this.radioAudioOn.Active;
            MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoS = this.cbxSpeed.Active;
            MainWindow.mainWindow.environment.vOPerformanceProperties.BandWidthQoSName = this.cbxSpeed.ActiveText;
            MainWindow.mainWindow.SaveConfiguration();

            if (MainFunc.callbackEditDesktopItem != null)
				MainFunc.callbackEditDesktopItem(this._desktopPoolEx);
			
			if (MainFunc.callbackShowDesktopListPage != null)
                MainFunc.callbackShowDesktopListPage(false);
		}
	}
}
