using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualPet.Core;

namespace VirtualPet.YoutubePlugin
{
    public partial class YoutubePlayerControl : UserControl
    {
        IPetInteractor Interactor;
        public YoutubePlayerControl()
        {
            InitializeComponent();
            InitializeAsync();
        }
        public YoutubePlayerControl(IPetInteractor interactor)
        {
            InitializeComponent();
            Interactor = interactor;
            InitializeAsync();
        }
        async void InitializeAsync()
        {
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder:"temp");
            await webView21.EnsureCoreWebView2Async(env);
        }
        public void LoadVideo(string url)
        {
            webView21.Source = new Uri(url);
        }

        private void mainTmr_Tick(object sender, EventArgs e)
        {
            this.Location = new Point(Interactor.GetPetLocation().X - this.Width, Interactor.GetPetLocation().Y - this.Height);
        }
    }

}
