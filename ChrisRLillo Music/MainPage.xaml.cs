using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.Web.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Web.Http.Filters;
using System.Collections.ObjectModel;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ChrisRLillo_Music
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _started = false;
        private DispatcherTimer _refreshTimer;
        private bool _isLoading = false;
        private HttpClient httpClient;
        private List<Item> _items;
        private Random _random = new Random();
        private int _currentIndex = -1;
        private DispatcherTimer _imageTimer;
        private int _imageIndex = 1;
        private DispatcherTimer _imageTimer2;
        private int _imageIndex2 = 2;
        public ObservableCollection<VideoItem> FeaturedVideos { get; set; }
    = new ObservableCollection<VideoItem>();
        public ObservableCollection<AlbumItem> Albums { get; set; }
        = new ObservableCollection<AlbumItem>();
        private List<Song> playlist = new List<Song>();
        private int currentIndex = -1;
        private bool isPlaying = false;
        private StorageFolder audioFolder;

        public MainPage()
        {
            this.InitializeComponent();
            var filter = new HttpBaseProtocolFilter();

            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;

            httpClient = new HttpClient(filter);

            FeaturedVideos = new ObservableCollection<VideoItem>
            {   
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/t2k.jpg",
                    Title = "T2K (OFFICIAL MUSIC VIDEO 2026)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/a019_bXDqSI?si=m11CUrPFZeqyWfRN"
                },
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/lnh.jpg",
                    Title = "Love & Hate (Official Music Video)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/LgQMOLyq5PI?si=tvYdw-zqVQMlRatv"
                },
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/whitebalcony.jpg",
                    Title = "White Balcony (Official Video HD/Rawest Form)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/tSjNxyKalzQ?si=nLjjCJutTHkECY3j"
                },
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/amor.jpg",
                    Title = "Amor Fantasmal (Vídeo Oficial)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/-Dsmlrv3OwM?si=LYlkulobo4wmU1u5"
                },
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/ghostly.jpg",
                    Title = "Ghostly Touch (Official Music Video)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/qUcKszsf89E?si=_LBNoQ_WShUlsjNd"
                },
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/poruna.jpg",
                    Title = "Por Una Noche Más (Official Music Video)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/IdmI2n2TRFE?si=1yipQ5JttQOTd4nQ"
                },
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/behind.jpg",
                    Title = "Behind The Line (Official Lyric Video)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/GAFBeJ0cJvI?si=S8WjTPwXq_RpRqaZ"
                },
                new VideoItem
                {
                    ImagePath = "ms-appx:///Assets/wound.jpg",
                    Title = "Wound (Instrumental) (Official Music Video)",
                    Subtitle = "ChrisRLillo",
                    Url = "https://youtu.be/F1GdJkbzk9o?si=IndcKr4gK014AUU4"
                }
            };

            Albums = new ObservableCollection<AlbumItem>
            {
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/lnh2.jpg",
                    Title = "Love & Hate",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/5ykl9a3fjP06d413k5seBI?si=jk0vrAgRSDeUQJrznfJc0Q"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/amor2.jpg",
                    Title = "Amor Fantasmal (Ghostly Touch Spanish Version)",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/1kaicPOLkf0ydgQJw6jImP?si=zW0Rg2lnSlOJetMrdzqs-Q"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/amor2.jpg",
                    Title = "Ghostly Touch (Orchestral Version)",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/2IePDEigdNFZCFo4FyXrFy?si=tGc_RWxkR6KxXXrFwVOggw"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/amor2.jpg",
                    Title = "Ghostly Touch",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/7yzxF3zOPzbgU0FUImsO1d?si=_MhiVQcBTYe1oA5kLnFg7g"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/behind2.jpg",
                    Title = "Illusions (Bonus Tracks)",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/5mVNdcdKZitFerk3o4djL2?si=DsH_jzd8Rw6ik0mlRKu47w"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/illusion.jpg",
                    Title = "Illusions",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/4cO0nQzWHPOSO4JBlcw8GA?si=3cJkI_KGTJSckt4dWpZvfA"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/whitebalcony2.jpg",
                    Title = "White Balcony",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/451tUCJjC8vjAGQEF8FT0M?si=4xbEIzvKQpyUPvogeRvC5A"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/lnh2.jpg",
                    Title = "Don't Be Scared",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/7zVMVcARjLgG1hZiG5souu?si=hDMuP887RTKX23qT4358Tg"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/winds.jpg",
                    Title = "Winds",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/4RTNhzTTXmqIfgAhszyQjd?si=yVtDY_P_R0GPoekqd0s4dA"
                },
                new AlbumItem
                {
                    ImagePath = "ms-appx:///Assets/recuerdo.jpg",
                    Title = "Recuerdo Insano",
                    Subtitle = "Christopher Rios L.",
                    Url = "https://open.spotify.com/album/41AslVwnNKqwAzjWQdgPRL?si=wHTzn2nJSSmmUwb16YZ0xg"
                }
            };

            this.DataContext = this;
            
        }

        public IEnumerable<VideoItem> FeaturedVideosLimited
        {
            get
            {
                return FeaturedVideos.Take(4); // 👈 change 6 to your desired limit
            }
        }

        public IEnumerable<AlbumItem> AlbumsLimited
        {
            get
            {
                return Albums.Take(4); // 👈 change 6 to your desired limit
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_started) return;
            _started = true;

            if (audioFolder == null)
            {
                audioFolder = await ApplicationData.Current.LocalFolder
                    .CreateFolderAsync("audio", CreationCollisionOption.OpenIfExists);
            }

            SlideInStoryboard.Begin();
            await ResetApp();
            CheckStorage();
            await LoadFeedAsync();
            LoadPlaylist();
            StartAutoRefresh();
            StartImageRotation();
            StartImageRotation2();

            // ✅ ADD THIS HERE
            DispatcherTimer storageTimer = new DispatcherTimer();
            storageTimer.Interval = TimeSpan.FromSeconds(15);

            storageTimer.Tick += async (sender, args) =>
            {
                Debug.WriteLine($"App storage: {await GetAppStorageSizeMB():F2} MB");
            };

            storageTimer.Start();
        }

        private async Task<StorageFile> GetOrDownloadAudio(string url, string fileName)
        {
            var file = await audioFolder.TryGetItemAsync(fileName) as StorageFile;

            if (file != null)
                return file;

            file = await audioFolder.CreateFileAsync(fileName, CreationCollisionOption.FailIfExists);

            using (var client = new Windows.Web.Http.HttpClient())
            {
                var buffer = await client.GetBufferAsync(new Uri(url));
                await FileIO.WriteBufferAsync(file, buffer);
            }

            return file;
        }

        private async Task LoadFeedAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;

                var baseUrl =
                    "https://gist.githubusercontent.com/RDCubing/6da60e67b12cfaded23f3f4392f4fb37/raw/chrismusic.json";

                var url = $"{baseUrl}?t={DateTime.UtcNow.Ticks}";

                string json = await httpClient.GetStringAsync(new Uri(url));

                var feed = JsonConvert.DeserializeObject<Feed>(json);

                if (feed?.Items != null && feed.Items.Count > 0)
                {
                    _items = feed.Items;

                    // go sequential instead of random (if you changed earlier)
                    ShowNextFeed();
                }
                else
                {
                    TitleText.Text = "No feed found";
                    ContentText.Text = "";
                    FadeInStoryboard.Begin();
                }
            }
            catch (Exception ex)
            {
                TitleText.Text = "Failed to load feed";
                ContentText.Text = ex.Message;
                FadeInStoryboard.Begin();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async void LoadPlaylist()
        {
            try
            {
                var baseUrl =
    "https://raw.githubusercontent.com/RDCubing/chrisrlillo-music-assets/main/audio/playlist.json";

                var url = $"{baseUrl}?t={DateTime.UtcNow.Ticks}";

                string json = await httpClient.GetStringAsync(new Uri(url));

                // sanity check (VERY IMPORTANT)
                if (!json.TrimStart().StartsWith("["))
                {
                    Debug.WriteLine("Invalid response: " + json.Substring(0, 50));
                    return;
                }

                playlist = JsonConvert.DeserializeObject<List<Song>>(json);

                SongsListView.ItemsSource = playlist;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Playlist error: " + ex.Message);
            }
        }

        private async void PlaySong(int index)
        {
            if (index < 0 || index >= playlist.Count)
                return;

            currentIndex = index;

            var song = playlist[index];

            Player.Stop();
            Player.Source = null;

            await Task.Delay(50);

            MiniPlayerImage.Source = null;

            MiniPlayerTitle.Text = song.Title;
            MiniPlayerArtist.Text = song.Artist;

            var bitmap = new BitmapImage
            {
                CreateOptions = BitmapCreateOptions.IgnoreImageCache,
                UriSource = new Uri(song.ImageUrl + "?v=" + Guid.NewGuid())
            };

            MiniPlayerImage.Source = bitmap;

            var fileName = $"{SanitizeFileName(song.Title)}.mp3";

            var file = await GetOrDownloadAudio(song.AudioUrl, fileName);

            Player.Stop();
            Player.Source = null;

            await Task.Delay(50);

            Player.Source = new Uri(file.Path);
            Player.Play();

            isPlaying = true;
            PlayPauseButton.Content = "Pause";
        }

        private async Task<double> GetAppStorageSizeMB()
        {
            long totalBytes = 0;

            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;

                var files = await localFolder.GetFilesAsync();

                foreach (var file in files)
                {
                    try
                    {
                        var props = await file.GetBasicPropertiesAsync();
                        totalBytes += (long)props.Size;
                    }
                    catch { }
                }

                var folders = await localFolder.GetFoldersAsync();

                foreach (var folder in folders)
                {
                    var folderFiles = await folder.GetFilesAsync();

                    foreach (var file in folderFiles)
                    {
                        try
                        {
                            var props = await file.GetBasicPropertiesAsync();
                            totalBytes += (long)props.Size;
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Size check failed: " + ex.Message);
            }

            return totalBytes / 1024.0 / 1024.0; // MB
        }

        private async void CheckStorage()
        {
            Debug.WriteLine($"App storage: {await GetAppStorageSizeMB():F2} MB");
        }

        private string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return name.Replace(" ", "_");
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Source == null)
                return;

            if (isPlaying)
            {
                Player.Pause();
                PlayPauseButton.Content = "Play";
                isPlaying = false;
            }
            else
            {
                Player.Play();
                PlayPauseButton.Content = "Pause";
                isPlaying = true;
            }
        }

        private async Task ResetApp()
        {
            try
            {
                var local = ApplicationData.Current.LocalFolder;
                var temp = ApplicationData.Current.TemporaryFolder;

                await DeleteAllFiles(temp);

                foreach (var folder in await local.GetFoldersAsync())
                {
                    try
                    {
                        await folder.DeleteAsync();
                    }
                    catch { }
                }

                foreach (var file in await local.GetFilesAsync())
                {
                    try
                    {
                        await file.DeleteAsync();
                    }
                    catch { }
                }

                currentIndex = -1;
                isPlaying = false;
                playlist.Clear();

                Player.Stop();
                Player.Source = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Reset failed: " + ex.Message);
            }
        }

        private async Task DeleteAllFiles(StorageFolder folder)
        {
            foreach (var file in await folder.GetFilesAsync())
            {
                try { await file.DeleteAsync(); } catch { }
            }

            foreach (var sub in await folder.GetFoldersAsync())
            {
                try { await sub.DeleteAsync(); } catch { }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Count == 0) return;

            int next = currentIndex + 1;

            if (next >= playlist.Count)
                next = 0;

            PlaySong(next);
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Count == 0) return;

            int prev = currentIndex - 1;

            if (prev < 0)
                prev = playlist.Count - 1;

            PlaySong(prev);
        }

        private void ShowNextFeed()
        {
            if (_items == null || _items.Count == 0)
                return;

            _currentIndex++;

            if (_currentIndex >= _items.Count)
                _currentIndex = 0;

            var item = _items[_currentIndex];

            TitleText.Text = item.Title;
            ContentText.Text = item.Content;
            FadeInStoryboard.Begin();
        }

        private void StartAutoRefresh()
        {
            _refreshTimer = new DispatcherTimer();

            // refresh every 10 seconds (adjust as you want)
            _refreshTimer.Interval = TimeSpan.FromSeconds(5);

            _refreshTimer.Tick += async (s, e) =>
            {
                if (_isLoading) return;

                await LoadFeedAsync();
            };

            _refreshTimer.Start();
        }

        private void SongsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var song = (Song)e.ClickedItem;
            int index = playlist.IndexOf(song);

            PlaySong(index); // reuse your fixed logic
        }

        private void StartImageRotation()
        {
            _imageTimer = new DispatcherTimer();
            _imageTimer.Interval = TimeSpan.FromSeconds(5);

            _imageTimer.Tick += (s, e) =>
            {
                _imageIndex++;

                if (_imageIndex > 5)
                    _imageIndex = 1;

                RotatingImage.Source =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(
                        new Uri($"ms-appx:///Assets/{_imageIndex}.jpg"));
                ImageFadeStoryboard.Begin();
            };

            _imageTimer.Start();
        }

        private void StartImageRotation2()
        {
            _imageTimer2 = new DispatcherTimer();
            _imageTimer2.Interval = TimeSpan.FromSeconds(5);

            _imageTimer2.Tick += (s, e) =>
            {
                _imageIndex2++;

                if (_imageIndex2 > 5)
                    _imageIndex2 = 1;

                RotatingImage2.Source =
                    new Windows.UI.Xaml.Media.Imaging.BitmapImage(
                        new Uri($"ms-appx:///Assets/{_imageIndex2}.jpg"));
                Image2FadeStoryboard.Begin();
            };

            _imageTimer2.Start();
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoItem;

            if (item != null && !string.IsNullOrEmpty(item.Url))
            {
                await Launcher.LaunchUriAsync(new Uri(item.Url));
            }
        }

        private async void GridView1_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AlbumItem;

            if (item != null && !string.IsNullOrEmpty(item.Url))
            {
                await Launcher.LaunchUriAsync(new Uri(item.Url));
            }
        }

        private void FeaturedVideos_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Videos), FeaturedVideos);
        }

        private void Albums_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Albums), Albums);
        }
    }

    public class Feed
    {
        public string FeedTitle { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class VideoItem
    {
        public string ImagePath { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public string Url { get; set; }
    }

    public class AlbumItem
    {
        public string ImagePath { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public string Url { get; set; }
    }

    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string AudioUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}
