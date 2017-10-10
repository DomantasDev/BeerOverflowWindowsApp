using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace BeerOverflowWindowsApp.BarRaters
{
    [Description("A manual beer meter")]
    public partial class ManualBeerMeter : Control
    {
        private Rectangle _marker;
        public string Rating { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MarkerHeight
        {
            set
            {
                _marker.Height = value;
                UpdateBeerMeter();
            }
            get { return _marker.Height; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MarkerWidth
        {
            set
            {
                _marker.Width = value;
                UpdateBeerMeter();
            }
            get { return _marker.Width; }
        }

        Rectangle cr;
        public ManualBeerMeter()
        {
            InitializeComponent();
            DoubleBuffered = true;
            MarkerHeight = 10;
            MarkerWidth = 30;
            Refresh();
        }

        private int topY;
        private int botY;
        protected override void OnResize(EventArgs e)
        {
            cr = ClientRectangle;
            cr.Inflate(-1, -1);
            UpdateBeerMeter();
        }

        public void UpdateBeerMeter()
        {
            _marker = new Rectangle((cr.Width - _marker.Width) / 2, cr.Bottom - _marker.Height, _marker.Width, _marker.Height);
            topY = cr.Top + _marker.Height / 2;
            botY = cr.Bottom - _marker.Height / 2;
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            g.FillRectangle(new SolidBrush(Parent.BackColor), cr);
            g.DrawLine(Pens.Black, cr.X + cr.Width / 2, topY, cr.X + cr.Width / 2, botY);
            g.DrawEllipse(Pens.Black, _marker);
            g.DrawString(text, new Font("Arial", 16), new SolidBrush(Color.Black), cr.X, cr.Top + 45);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        int difference;
        private Boolean mousePressed;
        private void ManualBeerMeter_MouseDown(object sender, MouseEventArgs e)
        {
            if (_marker.Contains(e.X, e.Y) && e.Button == MouseButtons.Left)
            {
                difference = e.Y - _marker.Y;
                mousePressed = true;
            }
        }

        private String text = "0 %";
        private const string textToDisplay = "BEEROVERFLOW!!!";
        private void ManualBeerMeter_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePressed)
            {
                _marker.Y = e.Y - difference;
                if (_marker.Top < cr.Top)
                    _marker.Y = cr.Top;
                if (_marker.Bottom > cr.Bottom)
                    _marker.Y = cr.Bottom - _marker.Height;
                Rating = (int)((botY - _marker.Y - _marker.Height / 2) / ((botY - (float)topY) / 100f)) + "";
                text = Rating == "100" ? textToDisplay : Rating + " %";
                Refresh();
                Thread.Sleep(5);
            }
        }

        private void ManualBeerMeter_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mousePressed = false;
        }
    }
}
