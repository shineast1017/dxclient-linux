using System;
using System.Collections.Generic;
using System.Reflection;
using Gtk;
using HCVK.HCVKSLibrary.VO;
using log4net;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class mainPageWidget : Gtk.Bin
    {
		private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private addServerPageWidget _addServerPageWidget = null;
		private serverlistWidget _serverlistWidget = null;

		private logInWidget _logInWidget = null;
		private desktoplistWidget _desktoplistWidget = null;
		private desktopPolicyWidget _desktopPolicyWidget = null;
		private editDesktopPageWidget _editDesktopPageWidget = null;

		public mainPageWidget()
        {
            this.Build();

			this._serverlistWidget = new global::client.serverlistWidget();
			this._serverlistWidget.Events = ((global::Gdk.EventMask)(256));
			this._serverlistWidget.Name = "serverlistwidget";
			this._serverlistWidget.WidthRequest = 256;
			this._serverlistWidget.HeightRequest = 377;
			this.vboxServerlist.Add(this._serverlistWidget);

			this._addServerPageWidget = new global::client.addServerPageWidget();
			this._addServerPageWidget.Events = ((global::Gdk.EventMask)(256));
			this._addServerPageWidget.Name = "addServerPageWidget";
			this.vboxServerlist.Add(this._addServerPageWidget);

			ShowWidgetDefault();

			this._desktoplistWidget = new global::client.desktoplistWidget();
            this._desktoplistWidget.Events = ((global::Gdk.EventMask)(256));
            this._desktoplistWidget.Name = "desktoplistwidget";
            this._desktoplistWidget.WidthRequest = 256;
            this._desktoplistWidget.HeightRequest = 377;
			this.vboxDesktoplist.Add(this._desktoplistWidget);
            
			this._logInWidget = new global::client.logInWidget();
            this._logInWidget.Events = ((global::Gdk.EventMask)(256));
            this._logInWidget.Name = "loginwidget";
            this._logInWidget.WidthRequest = 256;
            this._logInWidget.HeightRequest = 377;
            this.vboxDesktoplist.Add(this._logInWidget);

			this._desktopPolicyWidget = new global::client.desktopPolicyWidget();
            this._desktopPolicyWidget.Events = ((global::Gdk.EventMask)(256));
            this._desktopPolicyWidget.Name = "desktoppolicywidget";
            this._desktopPolicyWidget.WidthRequest = 256;
            this._desktopPolicyWidget.HeightRequest = 377;
			this.vboxDesktoplist.Add(this._desktopPolicyWidget);
            
			_editDesktopPageWidget = null;
			this._editDesktopPageWidget = new global::client.editDesktopPageWidget();
			this._editDesktopPageWidget.Events = ((global::Gdk.EventMask)(256));
			this._editDesktopPageWidget.Name = "editdesktoppagetwidget";
			this._editDesktopPageWidget.WidthRequest = 256;
			this._editDesktopPageWidget.HeightRequest = 377;
			this.vboxDesktoplist.Add(this._editDesktopPageWidget);
            
			MainFunc.callbackShowEditServerPage = this.ShowAddServerPage;
			MainFunc.callbackShowServerListPage = this.ShowWidgetDefault;
			MainFunc.callbackShowLoginPage = this.ShowLogInPage;
			MainFunc.callbackShowDesktopListPage = this.ShowDesktopListPage;
			MainFunc.callbackShowDesktopPolicyPage = this.ShowDesktopPolicyPage;
			MainFunc.callbackShowEditDesktopPage = this.ShowEditDesktopPage;



			ShowDesktopListPage (false);

			this.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(215, 215, 215));
		}




		/// <summary>
		/// Shows serverlist
		/// </summary>
		public void ShowWidgetDefault()
		{
			this._serverlistWidget.Show();
			this._addServerPageWidget.Hide();

			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxServerlist[this._serverlistWidget]));
            w1.Position = 0;
		}

        /// <summary>
        /// Show Add server Page
        /// </summary>
		public void ShowAddServerPage(int nEditMode, int nIdx)
		{
			this._serverlistWidget.Hide();

			VOBrokerServerNew broker = MainFunc.GetBrokerServer(nIdx);


			this._addServerPageWidget.Initialization(nEditMode, nIdx, 
			                                         broker != null ? broker.BrokerServerDesc : "",
			                                         broker != null ? broker.BrokerServerIP : "",
			                                         broker != null ? broker.BrokerServerPort : "");
			this._addServerPageWidget.Show();

			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxServerlist[this._addServerPageWidget]));
            w1.Position = 0;
		}

        // ShowUP  First Login Page frome ServerList 
        public void ShowLoginPageFromServerList(int selidx)
        {
            if (this._serverlistWidget.IsServerLogIn())
            {
                return;
            }

            // show login paget from serverlist item
            this._serverlistWidget.ShowLoginPage(selidx);


            return;
        }

        public void ShowLogInPage(VOBrokerServerNew brokerServer)
		{
			if(this._serverlistWidget.IsServerLogIn())
			{
				return;
			}
			this._logInWidget.Show();
			this._desktoplistWidget.Hide();
			this._desktopPolicyWidget.Hide();
			this._editDesktopPageWidget.Hide();

			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxDesktoplist[this._logInWidget]));
            w1.Position = 0;

			this._logInWidget.BrokerServerInfo = brokerServer;
		}

		public void ShowDesktopListPage(bool bRefresh)
		{
			if(bRefresh) {
				this._desktoplistWidget.InitializeDesktopList ();
				this._desktoplistWidget.UpdateConnectStatus ();
			}

			this._logInWidget.Hide();
			this._desktoplistWidget.Show();
            this._desktopPolicyWidget.Hide();
            this._editDesktopPageWidget.Hide();

			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxDesktoplist[this._desktoplistWidget]));
            w1.Position = 0;
            
		}

		public void ShowDesktopPolicyPage(List<VODesktopPolicies> listPolicies)
		{
			this._logInWidget.Hide();
            this._desktoplistWidget.Hide();
            this._desktopPolicyWidget.Show();
            this._editDesktopPageWidget.Hide();

			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxDesktoplist[this._desktopPolicyWidget]));
            w1.Position = 0;

			this._desktopPolicyWidget.SetDesktopProperties(listPolicies);
		}

		public void ShowEditDesktopPage(VODesktopPoolEx vODesktopPoolEx)
		{
			this._logInWidget.Hide();
            this._desktoplistWidget.Hide();
			this._desktopPolicyWidget.Hide();
			this._editDesktopPageWidget.Show();

			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxDesktoplist[this._editDesktopPageWidget]));
            w1.Position = 0;

			_editDesktopPageWidget.SetDesktopInfo(vODesktopPoolEx);
		}

	}
}
