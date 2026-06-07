namespace CDS.ImageDisplay.ImageBrowsing
{
    partial class ImageListPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _debounceTimer.Stop();
                _placeholder?.Dispose();
                components?.Dispose(); // disposes _imageList and _debounceTimer
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _listView = new ListView();
            _imageList = new ImageList(components);
            _debounceTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // _listView
            // 
            _listView.Dock = DockStyle.Fill;
            _listView.FullRowSelect = true;
            _listView.HeaderStyle = ColumnHeaderStyle.None;
            _listView.Location = new Point(0, 0);
            _listView.MultiSelect = false;
            _listView.Name = "_listView";
            _listView.Size = new Size(300, 400);
            _listView.SmallImageList = _imageList;
            _listView.TabIndex = 0;
            _listView.UseCompatibleStateImageBehavior = false;
            _listView.View = View.Details;
            _listView.VirtualMode = true;
            _listView.CacheVirtualItems += OnListViewCacheVirtualItems;
            _listView.ItemActivate += OnListViewItemActivate;
            _listView.RetrieveVirtualItem += OnListViewRetrieveVirtualItem;
            _listView.SelectedIndexChanged += OnListViewSelectedIndexChanged;
            _listView.SizeChanged += OnListViewSizeChanged;
            // 
            // _imageList
            // 
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.ImageSize = new Size(16, 16);
            _imageList.TransparentColor = Color.Transparent;
            // 
            // _debounceTimer
            // 
            _debounceTimer.Tick += OnDebounceTimerTick;
            // 
            // ImageListPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_listView);
            Name = "ImageListPanel";
            Size = new Size(300, 400);
            ResumeLayout(false);
        }

        #endregion

        private ListView _listView = null!;
        private ImageList _imageList = null!;
        private System.Windows.Forms.Timer _debounceTimer = null!;
    }
}
