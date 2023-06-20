using System;
using System.Drawing;
using Gtk;

namespace client
{
	public class StyleSheet
	{
		// serverinfo background
		public static Gdk.Color ServerBGHover;
		public static Gdk.Color ServerBGEnable;
		public static Gdk.Color ServerBGDisable;
		public static Gdk.Color ServerBGSelected;

        // serverinfo name
		public static Gdk.Color SNameFontHover;
        public static Gdk.Color SNameFontEnable;
        public static Gdk.Color SNameFontDisable;
        public static Gdk.Color SNameFontSelected;

        // serverinfo IP
		public static Gdk.Color SIPFontHover;
        public static Gdk.Color SIPFontEnable;
        public static Gdk.Color SIPFontDisable;
        public static Gdk.Color SIPFontSelected;

        // serverinfo Port
		public static Gdk.Color SPortBGHover;
        public static Gdk.Color SPortBGEnable;
        public static Gdk.Color SPortBGDisable;
        public static Gdk.Color SPortBGSelected;

		public static Gdk.Color SPortFontHover;
        public static Gdk.Color SPortFontEnable;
        public static Gdk.Color SPortFontDisable;
        public static Gdk.Color SPortFontSelected;

		public static Gdk.Color ServerLableColor;
		public static Gdk.Color ValueColor;

		public static Gdk.Pixbuf ImageServerNoraml;
		public static Gdk.Pixbuf ImageServerDisable;
		public static Gdk.Pixbuf ImageServerHover;
		public static Gdk.Pixbuf ImageServerSelected;
		//public static Gdk.Pixbuf ImageServerPushed;

		public static Gdk.Pixbuf ImageArrowNoraml;
		public static Gdk.Pixbuf ImageArrowDisable;
		public static Gdk.Pixbuf ImageArrowHover;
		public static Gdk.Pixbuf ImageArrowSelected;
        //public static Gdk.Pixbuf ImageServerPushed;

		public static Gdk.Color LeftItemFGNormal;
		public static Gdk.Color LeftItemFGDisable;
		public static Gdk.Color LeftItemFGHover;
		public static Gdk.Color LeftItemFGSelected;

		public static Gdk.Color LeftItemBGNormal;
        public static Gdk.Color LeftItemBGDisable;
        public static Gdk.Color LeftItemBGHover;
        public static Gdk.Color LeftItemBGSelected;

		// desktopinfo background
        public static Gdk.Color DesktopBGHover;
        public static Gdk.Color DesktopBGEnable;
        public static Gdk.Color DesktopBGDisable;
        public static Gdk.Color DesktopBGSelected;

        // desktopinfo name
        public static Gdk.Color DNameFontHover;
        public static Gdk.Color DNameFontEnable;
        public static Gdk.Color DNameFontDisable;
        public static Gdk.Color DNameFontSelected;

        // desktopinfo IP
        public static Gdk.Color DIPFontHover;
        public static Gdk.Color DIPFontEnable;
        public static Gdk.Color DIPFontDisable;
        public static Gdk.Color DIPFontSelected;

		public static Gdk.Color DesktopLableColor;

		public static Gdk.Pixbuf ImageSharedNoraml;
        public static Gdk.Pixbuf ImageSharedDisable;
        public static Gdk.Pixbuf ImageSharedHover;
        public static Gdk.Pixbuf ImageSharedSelected;
        //public static Gdk.Pixbuf ImageSharedPushed;

        public static Gdk.Pixbuf ImageAutoStartNoraml;
        public static Gdk.Pixbuf ImageAutoStartDisable;
        public static Gdk.Pixbuf ImageAutoStartHover;
        public static Gdk.Pixbuf ImageAutoStartSelected;
        //public static Gdk.Pixbuf ImageAutoStartPushed;

		// window7
        public static Gdk.Pixbuf ImageWin7Noraml;
        public static Gdk.Pixbuf ImageWin7Disable;
        public static Gdk.Pixbuf ImageWin7Hover;
        public static Gdk.Pixbuf ImageWin7Selected;
        //public static Gdk.Pixbuf ImageWin7Pushed;

        //window10
        public static Gdk.Pixbuf ImageWin10Noraml;
        public static Gdk.Pixbuf ImageWin10Disable;
        public static Gdk.Pixbuf ImageWin10Hover;
        public static Gdk.Pixbuf ImageWin10Selected;
        //public static Gdk.Pixbuf ImageWin10Pushed;

        // centos
        public static Gdk.Pixbuf ImageCentOSNoraml;
        public static Gdk.Pixbuf ImageCentOSDisable;
        public static Gdk.Pixbuf ImageCentOSHover;
        public static Gdk.Pixbuf ImageCentOSSelected;
        //public static Gdk.Pixbuf ImageCentOSPushed;

        //ubuntu
        public static Gdk.Pixbuf ImageUbuntuNoraml;
        public static Gdk.Pixbuf ImageUbuntuDisable;
        public static Gdk.Pixbuf ImageUbuntuHover;
        public static Gdk.Pixbuf ImageUbuntuSelected;
        //public static Gdk.Pixbuf ImageUbuntuPushed;

		public static Gdk.Pixbuf ImageBookmarkArrowNoraml;
		public static Gdk.Pixbuf ImageBookmarkArrowDisable;
		public static Gdk.Pixbuf ImageBookmarkArrowHover;
		public static Gdk.Pixbuf ImageBookmarkArrowSelected;


        // tmaxos
        public static Gdk.Pixbuf ImageTmaxNoraml;
        public static Gdk.Pixbuf ImageTmaxDisable;
        public static Gdk.Pixbuf ImageTmaxHover;
        public static Gdk.Pixbuf ImageTmaxSelected;


        // hancom
        public static Gdk.Pixbuf ImageHancomNoraml;
        public static Gdk.Pixbuf ImageHancomDisable;
        public static Gdk.Pixbuf ImageHancomHover;
        public static Gdk.Pixbuf ImageHancomSelected;

        // hamonikr
        public static Gdk.Pixbuf ImageHamonikrNoraml;
        public static Gdk.Pixbuf ImageHamonikrDisable;
        public static Gdk.Pixbuf ImageHamonikrHover;
        public static Gdk.Pixbuf ImageHamonikrSelected;



        public StyleSheet()
		{
		}

		public static void Initialize()
		{
			Color color;

			ServerBGHover = new Gdk.Color(193, 229, 255);
			ServerBGEnable = new Gdk.Color(255, 255, 255);
			ServerBGDisable = new Gdk.Color(235, 235, 235);
			ServerBGSelected = new Gdk.Color(84, 185, 255);

            // serverinfo name
			color = ColorTranslator.FromHtml("#0f77c0");
			SNameFontHover = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#111111");
			SNameFontEnable = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#b1b1b1");
			SNameFontDisable = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#ffffff");
			SNameFontSelected = new Gdk.Color(color.R, color.G, color.B);

            // serverinfo IP
			color = ColorTranslator.FromHtml("#53a3db");
			SIPFontHover = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#a6a6a6");
			SIPFontEnable = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#c6c6c6");
			SIPFontDisable = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#ffffff");
			SIPFontSelected = new Gdk.Color(color.R, color.G, color.B);

			// serverinfo port
			SPortBGHover = new Gdk.Color(133, 197, 241);
			SPortBGEnable = new Gdk.Color(200, 191, 191);
			SPortBGDisable = new Gdk.Color(233, 233, 233);
			SPortBGSelected = new Gdk.Color(35, 166, 254);

			SPortFontHover = new Gdk.Color(255, 255, 255);
			SPortFontEnable = new Gdk.Color(255, 255, 255);
			SPortFontDisable = new Gdk.Color(255, 255, 255);
			SPortFontSelected = new Gdk.Color(255, 253, 254);

			color = ColorTranslator.FromHtml("#2072aa");
			ServerLableColor = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#2b5b93");
            DesktopLableColor = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#fefefe");
			ValueColor = new Gdk.Color(color.R, color.G, color.B);

			ImageServerNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_broker_enable.png");
			ImageServerDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_broker_disable.png");
			ImageServerHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_broker_over.png");
			ImageServerSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_broker_selected.png");

			ImageArrowNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.server_arrow_enable.png");
			ImageArrowDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.server_arrow_disable.png");
			ImageArrowHover = Gdk.Pixbuf.LoadFromResource("client.Resources.server_arrow_over.png");
			ImageArrowSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.server_arrow_selected.png");
		
			color = ColorTranslator.FromHtml("#878991");
			LeftItemFGNormal = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#18191c");
			LeftItemFGDisable = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#bec0c7");
			LeftItemFGHover = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#e4e4e4");
			LeftItemFGSelected = new Gdk.Color(color.R, color.G, color.B);

			color = ColorTranslator.FromHtml("#393a40");
            LeftItemBGNormal = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#393a40");
            LeftItemBGDisable = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#48494e");
            LeftItemBGHover = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#48494e");
            LeftItemBGSelected = new Gdk.Color(color.R, color.G, color.B);

            // desktop
			color = ColorTranslator.FromHtml("#b5d8ff");
			DesktopBGHover = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#ffffff");
			DesktopBGEnable = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#ebebeb");
			DesktopBGDisable = new Gdk.Color(color.R, color.G, color.B);
			color = ColorTranslator.FromHtml("#54a4ff");
			DesktopBGSelected = new Gdk.Color(color.R, color.G, color.B);

            // desktopinfo name
            color = ColorTranslator.FromHtml("#0f77c0");
            DNameFontHover = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#111111");
            DNameFontEnable = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#b1b1b1");
            DNameFontDisable = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#ffffff");
            DNameFontSelected = new Gdk.Color(color.R, color.G, color.B);

			// desktopinfo IP
            color = ColorTranslator.FromHtml("#53a3db");
            DIPFontHover = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#a6a6a6");
            DIPFontEnable = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#c6c6c6");
            DIPFontDisable = new Gdk.Color(color.R, color.G, color.B);
            color = ColorTranslator.FromHtml("#ffffff");
            DIPFontSelected = new Gdk.Color(color.R, color.G, color.B);

			ImageSharedNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_share_enable.png");
			ImageSharedDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_share_disable.png");
			ImageSharedHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_share_over.png");
			ImageSharedSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_share_select.png");
            //ImageSharedPushed;

			ImageAutoStartNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_autostart_enable.png");
			ImageAutoStartDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_autostart_disable.png");
			ImageAutoStartHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_autostart_over.png");
			ImageAutoStartSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_autostart_select.png");
            //ImageAutoStartPushed;

            // window7
			ImageWin7Noraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win7_enable.png");
			ImageWin7Disable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win7_disable.png");
			ImageWin7Hover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win7_over.png");
			ImageWin7Selected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win7_select.png");
            //ImageWin7Pushed;

            //window10
			ImageWin10Noraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win10_enable.png");
			ImageWin10Disable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win10_disable.png");
			ImageWin10Hover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win10_over.png");
			ImageWin10Selected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_win10_select.png");
            //ImageWin10Pushed;

            // centos
			ImageCentOSNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_centos_enable.png");
			ImageCentOSDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_centos_disable.png");
			ImageCentOSHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_centos_over.png");
			ImageCentOSSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_centos_select.png");
            //ImageCentOSPushed;

            //ubuntu
			ImageUbuntuNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_ubuntu_enable.png");
			ImageUbuntuDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_ubuntu_disable.png");
			ImageUbuntuHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_ubuntu_over.png");
			ImageUbuntuSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_ubuntu_select.png");
			//ImageUbuntuPushed;

			ImageBookmarkArrowNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.bookmark_arrow_enable.png");
			//ImageBookmarkArrowDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.server_arrow_disable.png");
			ImageBookmarkArrowHover = Gdk.Pixbuf.LoadFromResource("client.Resources.bookmark_arrow_over.png");
            //ImageBookmarkArrowSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.server_arrow_selected.png");


            // tmax
            ImageTmaxNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_tmax_enable.png");
            ImageTmaxDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_tmax_disable.png");
            ImageTmaxHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_tmax_over.png");
            ImageTmaxSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_tmax_select.png");

            // hancom
            ImageHancomNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hancom_enable.png");
            ImageHancomDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hancom_disable.png");
            ImageHancomHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hancom_over.png");
            ImageHancomSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hancom_select.png");

            // hamonikr
            ImageHamonikrNoraml = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hamonikr_enable.png");
            ImageHamonikrDisable = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hamonikr_disable.png");
            ImageHamonikrHover = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hamonikr_over.png");
            ImageHamonikrSelected = Gdk.Pixbuf.LoadFromResource("client.Resources.icon_dt_hamonikr_select.png");


        }

        public static void SetStyleButton(Gtk.Button button)
		{
			Color color;

			color = ColorTranslator.FromHtml("#2ba1f1");
			button.ModifyBg(StateType.Normal, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#43aff9");
			//button.ModifyBg(StateType.Insensitive, new Gdk.Color(color.R, color.G, color.B));
			button.ModifyBg(StateType.Insensitive, new Gdk.Color(255,0,0));
			button.ModifyBg(StateType.Selected, new Gdk.Color(255, 255, 255));
			color = ColorTranslator.FromHtml("#91d3ff");
			button.ModifyBg(StateType.Active, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#1194ed");
			button.ModifyBg(StateType.Prelight, new Gdk.Color(color.R, color.G, color.B));

			color = ColorTranslator.FromHtml("#a9dcff");
			button.Child.ModifyFg(StateType.Normal, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#7ccaff");
			button.Child.ModifyFg(StateType.Insensitive, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#a9dcff");
			button.Child.ModifyFg(StateType.Selected, new Gdk.Color(color.R, color.G, color.B));
			button.Child.ModifyFg(StateType.Active, new Gdk.Color(255, 255, 255));
			button.Child.ModifyFg(StateType.Prelight, new Gdk.Color(255, 255, 255));
		}

		public static void SetStyleCancelButton(Gtk.Button button)
        {
            Color color;

			color = ColorTranslator.FromHtml("#5c5d63");
            button.ModifyBg(StateType.Normal, new Gdk.Color(color.R, color.G, color.B));
            color = ColorTranslator.FromHtml("#43aff9");
            //button.ModifyBg(StateType.Insensitive, new Gdk.Color(color.R, color.G, color.B));
            button.ModifyBg(StateType.Insensitive, new Gdk.Color(255, 0, 0));
            button.ModifyBg(StateType.Selected, new Gdk.Color(255, 255, 255));
			color = ColorTranslator.FromHtml("#9a9a9a");
            button.ModifyBg(StateType.Active, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#434449");
            button.ModifyBg(StateType.Prelight, new Gdk.Color(color.R, color.G, color.B));

			color = ColorTranslator.FromHtml("#dcdcdc");
            button.Child.ModifyFg(StateType.Normal, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#e5e5e5");
            button.Child.ModifyFg(StateType.Insensitive, new Gdk.Color(color.R, color.G, color.B));
			color = ColorTranslator.FromHtml("#ffffff");
            button.Child.ModifyFg(StateType.Selected, new Gdk.Color(color.R, color.G, color.B));
            button.Child.ModifyFg(StateType.Active, new Gdk.Color(255, 255, 255));
            button.Child.ModifyFg(StateType.Prelight, new Gdk.Color(255, 255, 255));
        }
	}

	public class BGEventbox : Gtk.EventBox
	{
		public BGEventbox()
		{
		}
	}
}
