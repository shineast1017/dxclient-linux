using System;
using System.Collections.Generic;
using HCVK.HCVKSLibrary.VO;
using System.Linq;
using Gtk;

namespace client
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class desktopPolicyWidget : Gtk.Bin
    {
        Gdk.Pixbuf _imageOn = Gdk.Pixbuf.LoadFromResource("client.Resources.switch_2_on.png");
        Gdk.Pixbuf _imageOff = Gdk.Pixbuf.LoadFromResource("client.Resources.switch_2_off.png");

        public desktopPolicyWidget()
        {
            this.Build();

            StyleSheet.SetStyleButton(this.btnOK);
        }

        protected void OnBtnOKClicked(object sender, EventArgs e)
        {
            if (MainFunc.callbackShowDesktopListPage != null)
                MainFunc.callbackShowDesktopListPage(false);
        }
        public void SetDesktopProperties(List<VODesktopPolicies> listPolicies)
        {
            if (listPolicies.Count > 0)
            {
                this.scrolledwindow1.Show();

                int nLength = this.vboxlist.Children == null ? 0 : this.vboxlist.Children.Length;
                int nNewItem = nLength - listPolicies.Count;
                int nCnt;

                for (nCnt = nNewItem; nCnt < 0; nCnt++)
                {
                    // Container child vboxlist.Gtk.Box+BoxChild
                    global::Gtk.HBox hbox = new global::Gtk.HBox();
                    hbox.Name = "hbox1";
                    hbox.Spacing = 6;
                    // Container child hbox1.Gtk.Box+BoxChild
                    global::Gtk.Label labeln = new global::Gtk.Label();
                    labeln.WidthRequest = 180;
                    labeln.HeightRequest = 25;
                    labeln.Name = "label1";
                    labeln.Xalign = 0F;
                    labeln.Yalign = 0F;
                    labeln.LabelProp = "";
                    hbox.Add(labeln);
                    global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(hbox[labeln]));
                    w4.Position = 0;
                    w4.Expand = false;
                    w4.Fill = false;
                    // Container child hbox1.Gtk.Box+BoxChild
                    global::Gtk.Image imagen = new global::Gtk.Image();
                    imagen.Name = "image8";
                    imagen.Yalign = 0F;
                    imagen.Pixbuf = null;
                    hbox.Add(imagen);
                    global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(hbox[imagen]));
                    w5.Position = 1;
                    w5.Expand = false;
                    w5.Fill = false;
                    this.vboxlist.Add(hbox);
                    global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxlist[hbox]));
                    w7.Position = this.vboxlist.Children.Length - 1;

                    hbox.ShowAll();
                }
                this.vboxlist.ShowAll();
                for (nCnt = nNewItem; nCnt > 0; nCnt--)
                {
                    this.vboxlist.Children[nLength - nCnt].Hide();
                }
                nCnt = 0;
                listPolicies.ToArray().OrderBy(s => s.PolicyName).ToList().ForEach(item =>
                {
                    HBox widget = (HBox)this.vboxlist.Children[nCnt];
                    ((Label)widget.Children[0]).Text = item.PolicyName;
                    ((Image)widget.Children[1]).Pixbuf = item.Apply == true ? _imageOn : _imageOff;
                    nCnt++;
                });
            }
            else
            {
                this.scrolledwindow1.Hide();
            }
        }
    }
}
