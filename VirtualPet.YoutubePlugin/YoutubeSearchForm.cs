using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualPet.Core;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.IO;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;

namespace VirtualPet.YoutubePlugin
{
    public partial class YoutubeSearchForm : Form
    {
        IPetInteractor Interactor;
        YoutubePlayerControl youtubePlayerControl;
        bool spawned = false;
        int currentY = 0;
        public YoutubeSearchForm()
        {
            InitializeComponent();
            for (int i = 0; i < 30; ++i)
            {
                YoutubeVideoItem item = new YoutubeVideoItem();
                item.Location = new Point(5, i * (item.Height + 5));
                panel1.Controls.Add(item);
            }
        }

        public YoutubeSearchForm(IPetInteractor interactor)
        {
            InitializeComponent();
            Interactor = interactor;
            youtubePlayerControl = new YoutubePlayerControl(Interactor);
        }
        public void SpawnToy(string video)
        {
            Toy t = new Toy(new Action(() =>
            {
                if (!spawned)
                {
                    Interactor.SpawnDialog(youtubePlayerControl);
                    spawned = true;
                }
                youtubePlayerControl.Visible = true;
                youtubePlayerControl.LoadVideo($"https://www.youtube.com/embed/{video}?autoplay=1");
            }));
            Interactor.SpawnToy(t);
        }

        private async Task SearchAsync(string term)
        {
            UserCredential credential;
            System.Diagnostics.Debug.WriteLine(Environment.CurrentDirectory);
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "client_secrets.json")))
            {
                System.Diagnostics.Debug.WriteLine("No file here");
            }
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] { YouTubeService.Scope.Youtube },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = term; // Replace with your search term.
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = searchListRequest.Execute();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        {
                            videos.Add(String.Format("{0} ({1} {2})", searchResult.Snippet.Title, searchResult.Id.VideoId, searchResult.Snippet.Thumbnails.Default__.Url));
                            AddControl(searchResult.Snippet.Title, searchResult.Id.VideoId);
                        }
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            System.Diagnostics.Debug.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            System.Diagnostics.Debug.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            System.Diagnostics.Debug.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
        }

        private void AddControl(string name, string url)
        {
            Invoke(new Action(() =>
            {
                YoutubeVideoItem item = new YoutubeVideoItem(name);
                item.Click += (sender, e) =>
                {
                    SpawnToy(url);
                    System.Diagnostics.Debug.WriteLine("Doing shit");
                };
                item.Width = panel1.Width - 20;
                item.Location = new Point(5, currentY);
                currentY += item.Height + 5;
                panel1.Controls.Add(item);
                item.Show();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string term = searchTb.Text.Trim();
            if (term != string.Empty)
            {
                currentY = 0;
                panel1.Controls.Clear();
                SearchAsync(term).Wait();
            }
        }
    }
}
