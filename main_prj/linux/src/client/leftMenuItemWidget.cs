using System;
namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class leftMenuItemWidget : Gtk.Bin
    {
		public int PageIdx { get; set; }
		private Gtk.StateType _stateType = Gtk.StateType.Normal ;

		private Gdk.Pixbuf _imageNoraml;
        private Gdk.Pixbuf _imageDisable;
        private Gdk.Pixbuf _imageHover;
        private Gdk.Pixbuf _imageSelected;
        //public  Gdk.Pixbuf _imagePushed;

        private Gdk.Pixbuf _imageArrowNoraml;
        private Gdk.Pixbuf _imageArrowDisable;
        private Gdk.Pixbuf _imageArrowHover;
        private Gdk.Pixbuf _imageArrowSelected;
      
		public Gtk.StateType StateType
		{
			get { return _stateType; }
			set
			{
				if(_stateType != value)
				{
					_stateType = value;
					SetStyle();
				}
			}
		}

        public leftMenuItemWidget()
        {
            this.Build();

        }

        public void Initialize(int nIdx)
		{
			PageIdx = nIdx;
			string strName = string.Empty;
            if(PageIdx == 0)
			{
				this.labelTitle.LabelProp = string.Format("<span size='14000'>HOME</span>");
				strName = "home";
			}
			else if (PageIdx == 1)
            {
                this.labelTitle.LabelProp = string.Format("<span size='14000'>BOOKMARK</span>");
				strName = "bookmark";
            }
			else if (PageIdx == 2)
            {
                this.labelTitle.LabelProp = string.Format("<span size='14000'>SETTING</span>");
				strName = "setting";
            }
            else if (PageIdx == 3)
            {
                this.labelTitle.LabelProp = string.Format("<span size='14000'>POWER OFF</span>");
                strName = "power off";
            }

            _imageNoraml = Gdk.Pixbuf.LoadFromResource(string.Format("client.Resources.icon_{0}_enable.png", strName));
			_imageDisable = Gdk.Pixbuf.LoadFromResource(string.Format("client.Resources.icon_{0}_disable.png", strName));
			_imageHover = Gdk.Pixbuf.LoadFromResource(string.Format("client.Resources.icon_{0}_over.png", strName));
			_imageSelected = Gdk.Pixbuf.LoadFromResource(string.Format("client.Resources.icon_{0}_selected.png", strName));

			_imageArrowNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_arrow_enable.png");
			_imageArrowDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_arrow_disable.png");
			_imageArrowHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_arrow_over.png");
			_imageArrowSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_arrow_selected.png");

			SetStyle();
		}

		protected void OnEventboxButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			if (_stateType == Gtk.StateType.Insensitive || _stateType == Gtk.StateType.Selected)
				return;
			
			if (args.Event.Button == 1)
			{
				StateType = Gtk.StateType.Selected;
				MainWindow.mainWindow.ChangePage(PageIdx);
			}
		}

		protected void OnEventboxEnterNotifyEvent(object o, Gtk.EnterNotifyEventArgs args)
		{
			if (_stateType == Gtk.StateType.Insensitive || _stateType == Gtk.StateType.Selected)
                return;

			StateType = Gtk.StateType.Prelight;
		}

		protected void OnEventboxLeaveNotifyEvent(object o, Gtk.LeaveNotifyEventArgs args)
		{
			if (_stateType == Gtk.StateType.Insensitive || _stateType == Gtk.StateType.Selected)
                return;

			StateType = Gtk.StateType.Normal;
		}

		private void SetStyle()
		{
			if (_imageNoraml == null)
				return;
			switch(this.StateType)
			{
				case Gtk.StateType.Normal:
					this.imageIcon.Pixbuf = _imageNoraml;
					this.imageArrow.Pixbuf = _imageArrowNoraml;
					this.labelTitle.ModifyFg(Gtk.StateType.Normal, StyleSheet.LeftItemFGNormal);
					this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.LeftItemBGNormal);
					break;
				case Gtk.StateType.Insensitive:
					this.imageIcon.Pixbuf = _imageDisable;
					this.imageArrow.Pixbuf = _imageArrowDisable;
					this.labelTitle.ModifyFg(Gtk.StateType.Normal, StyleSheet.LeftItemFGDisable);
					this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.LeftItemBGDisable);
                    break;
				case Gtk.StateType.Selected:
					this.imageIcon.Pixbuf = _imageSelected;
					this.imageArrow.Pixbuf = _imageArrowSelected;
					this.labelTitle.ModifyFg(Gtk.StateType.Normal, StyleSheet.LeftItemFGSelected);
					this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.LeftItemBGSelected);
                    break;
				case Gtk.StateType.Prelight:
					this.imageIcon.Pixbuf = _imageHover;
					this.imageArrow.Pixbuf = _imageArrowHover;
					this.labelTitle.ModifyFg(Gtk.StateType.Normal, StyleSheet.LeftItemFGHover);
					this.eventbox.ModifyBg(Gtk.StateType.Normal, StyleSheet.LeftItemBGHover);
                    break;
			}
			//this.image
		}

	}
}
