﻿using System;
using System.IO;
using System.Windows.Forms;
using Media_PLayer.Structures;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Media_PLayer
{
    public partial class MainForm : Form
    {
        private Media CurrentMedia { get; set; }
        private Player Player { get; set; }

        private Dictionary<string, int> UrlToIndex { get; set; }

        private event EventHandler<CurrentMediaChangedEventArgs> MediaChanged; 

        private Timer Timer { get; }

        public MainForm()
        {
            InitializeComponent();
            UrlToIndex = new Dictionary<string, int>();
            lblVolume.Text = tbVolume.Value.ToString();
            MediaChanged += MainForm_MediaChanged;
            Player = new Player(MediaChanged) {Volume = tbVolume.Value};
            Timer = new Timer() {Interval = 1000};
            Timer.Tick += Timer_Tick;
        }
            
        private void OpenMusicFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter =
                   @"All files (*.flac, *.mp3, *.wav)|*.flac;*.mp3;*.wav|Mp3 files(*.mp3)|*.mp3|Flac files (*.flac)|*.flac|Wav files (*.wav)|*.wav",
                Title = @"Open Music"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            UrlToIndex.Clear();
            ClearGui();
            OpenMusicFile(ofd.FileName);
        }

        private void OpenFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter =
                    @"All files (*.flac, *.mp3, *.wav)|*.flac;*.mp3;*.wav|Mp3 files(*.mp3)|*.mp3|Flac files (*.flac)|*.flac|Wav files (*.wav)|*.wav",
                Title = @"Open Music",
                Multiselect = true
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            UrlToIndex.Clear();
            ClearGui();
            OpenMusicFiles(ofd.FileNames);
        }

        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK) return;
            UrlToIndex.Clear();
            ClearGui();
            var files = Directory.GetFiles(fbd.SelectedPath).ToList();
            OpenMusicFiles(files.Where(IsSong).ToArray());
        }

        private void OpenMusicFiles(string[] files)
        {
            var albumCover = "";
            var awaiter = DisplayAlbumCover(Path.GetDirectoryName(files[0])).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                albumCover = awaiter.GetResult();
                pbAlbumCover.ForeColor = ForeColor;
                pbAlbumCover.SizeMode = PictureBoxSizeMode.StretchImage;
                pbAlbumCover.ImageLocation = albumCover;
            });
            var openedFiles = new List<MusicFile>();
            foreach (var fileName in files)
            {
                MusicFile newMedia = new MusicFile(fileName, (uint) lbOpenedFiles.Items.Count + 1)
                {
                    AlbumCover = albumCover
                };
                lbOpenedFiles.Items.Add(newMedia);
                openedFiles.Add(newMedia);
                UrlToIndex.Add(newMedia.Url, lbOpenedFiles.Items.Count - 1);
            }
            Player.PlayMusicFiles(openedFiles, true);
        }

        private void OpenMusicFile(string fileName)
        {
            MusicFile newMedia = new MusicFile(fileName, (uint) lbOpenedFiles.Items.Count + 1);
            var awaiter = DisplayAlbumCover(Path.GetDirectoryName(fileName)).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                var albumCover = awaiter.GetResult();
                pbAlbumCover.SizeMode = PictureBoxSizeMode.StretchImage;
                pbAlbumCover.ImageLocation = albumCover;
                newMedia.AlbumCover = albumCover;
            });
            CurrentMedia = newMedia;
            lbOpenedFiles.Items.Add(newMedia);
            UrlToIndex.Add(newMedia.Url, 0);
            Player.PlayMusicFile(CurrentMedia);
        }

        private static async Task<string> DisplayAlbumCover(string lookingDirectory)
        {
            return await Task.Run(() =>
            {
                return Directory.GetFiles(lookingDirectory)
                    .FirstOrDefault(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".bmp"));
            });
        }
        
        private static bool IsSong(string file)
        {
            return file.EndsWith(".mp3") || file.EndsWith(".flac") || file.EndsWith(".wav");
        }

        public void ClearGui()
        {
            lbOpenedFiles.Items.Clear();
            pbAlbumCover.ImageLocation = null;
            Player.Clear();
            tbProgress.Value = 0;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (lbOpenedFiles.SelectedIndex != -1)
            {
                Media selectedItem = lbOpenedFiles.SelectedItem as Media;
                Player.PlayMusicFile(selectedItem);
                CurrentMedia = selectedItem;
            }
            else if (lbOpenedFiles.Items.Count > 0)
            {
                Player.PlayMusicFile(lbOpenedFiles.Items[0] as Media);
                CurrentMedia = lbOpenedFiles.Items[0] as Media;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (lbOpenedFiles.Items.Count > 0 && Player.Pause())
                Timer.Stop();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (lbOpenedFiles.Items.Count > 0)
            {
                Player.Stop();
                Timer.Stop();
                tbProgress.Value = 0;
                lbOpenedFiles.SetSelected(lbOpenedFiles.SelectedIndex, false);
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (lbOpenedFiles.Items.Count > 0)
                Player.Previous();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(lbOpenedFiles.Items.Count > 0)
                Player.Next();
        }

        protected virtual void OnMediaChanged(CurrentMediaChangedEventArgs e)
        {
            MediaChanged?.Invoke(this, e);
        }

        private void MainForm_MediaChanged(object sender, CurrentMediaChangedEventArgs e)
        {
            tbProgress.Value = e.CurrentPosition;
            Timer.Start();
            tbProgress.Maximum = e.NewMediaDuration;
            lbOpenedFiles.SetSelected(UrlToIndex[e.Url], true);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (tbProgress.Value + 1 < tbProgress.Maximum)
                tbProgress.Value++;
            else Timer.Stop();
        }

        private void tbVolume_Scroll(object sender, EventArgs e)
        {
            Player.Volume = tbVolume.Value;
            lblVolume.Text = tbVolume.Value.ToString();
            pbVolume.Value = tbVolume.Value;
        }

        private void tbProgress_Scroll(object sender, EventArgs e)
        {
            Player.SetCurrentPosition(tbProgress.Value);
        }

        private void lbOpenedFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lbOpenedFiles.Items.Count > 0 && lbOpenedFiles.SelectedIndex != -1)
            {
                Player.PlayMusicFile(lbOpenedFiles.SelectedItem as Media);
            }
        }
    }
}