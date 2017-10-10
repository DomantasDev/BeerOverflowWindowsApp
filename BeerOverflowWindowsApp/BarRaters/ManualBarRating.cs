using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using BeerOverflowWindowsApp.Properties;

namespace BeerOverflowWindowsApp.BarRaters
{
    public partial class ManualBarRating : Control
    {
        List<Rectangle> beerGlassList;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        private int imageSize;
        public int ImageSize
        {
            get { return imageSize; }
            set
            {
                imageSize = value;
                SetImages();
                MinimumSize = new Size(imageSize * 5, imageSize);
                MaximumSize = new Size(imageSize * 5, imageSize);
                for (int i = 0; i < 5; i++)
                {
                    beerGlassList[i] = (new Rectangle(i * imageSize, 0, imageSize, imageSize));
                }
                Refresh();
            }
        }

        public string Rating
        {
            get { return numberOfGlasses + 1 + ""; }
            private set { }
        }
        ToolTip toolTip;
        public ManualBarRating()
        {
            DoubleBuffered = true;
            InitializeComponent();
            imageSize = 100;
            MinimumSize = new Size(imageSize * 5, imageSize);
            MaximumSize = new Size(imageSize * 5, imageSize);
            beerGlassList = new List<Rectangle>(5);
            for(int i = 0; i < 5; i++)
            {
                beerGlassList.Add(new Rectangle(i * imageSize, 0, imageSize, imageSize));
            }
            SetImages();
            toolTip = new ToolTip();
            toolTip.ShowAlways = true;
            Refresh();
        }

        Image selectedImage;
        Image unSelectedImage;
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            for (int i = 0; i <= numberOfGlasses; i++)
            {
                g.DrawImageUnscaledAndClipped(selectedImage, beerGlassList[i]);
            }
            for(int i = numberOfGlasses + 1; i < 5; i++)
            {
                g.DrawImageUnscaledAndClipped(unSelectedImage, beerGlassList[i]);
            }
        }

        Boolean mouseEntered;
        private int numberOfGlasses;
        private void ManualBarRating_MouseEnter(object sender, EventArgs e)
        {
            mouseEntered = true;
        }

        private const string toolTipMessage1 = "Got in a fight with a bartender";
        private const string toolTipMessage2 = "Half empty";
        private const string toolTipMessage3 = "Meh";
        private const string toolTipMessage4 = "Good enough";
        private const string toolTipMessage5 = "Beeroverflow!";
        private string[] toolTipMessages = new string[5] {toolTipMessage1, toolTipMessage2, toolTipMessage3, toolTipMessage4, toolTipMessage5 };
        int lastLocation = -1;
        private void ManualBarRating_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseEntered && (e.X / imageSize) != lastLocation)
            {
                SelectRating(e);
                lastLocation = e.X / imageSize;
                toolTip.Show(toolTipMessages[lastLocation], this, e.X + 10, e.Y + 10, 3000);
            }
        }

        private void ManualBarRating_MouseLeave(object sender, EventArgs e)
        {
            lastLocation = -1;
            toolTip.Hide(this);
            mouseEntered = false;
            numberOfGlasses = 0;
            Refresh();
        }

        private void SelectRating(MouseEventArgs e)
        {
            for (int i = 4; i >= 0; i--)
            {
                if (beerGlassList[i].Contains(e.Location))
                {
                    numberOfGlasses = i;
                    Refresh();
                    break;
                }
            }
        }

        private void SetImages()
        {
            selectedImage = ResizeImage(Resources.LightBeerGlass, imageSize, imageSize);
            unSelectedImage = ResizeImage(Resources.DarkBeerGlass, imageSize, imageSize);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}
