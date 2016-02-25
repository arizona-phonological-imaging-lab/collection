using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserControl
{
    public class VuMeter : System.Windows.Forms.UserControl
    {
        ///// <summary>
        ///// UserControl offers a VuMeter bar which can be used as user control
        ///// in a form.
        ///// </summary>
        //public class VuMeterLed : System.Windows.Forms.UserControl
        //{
        int ledVal;			// VU meter value - range 1 to 15
        int peakVal;			// Peak value
        const int ledCount = 15;		// Number of LEDs

        // Array of LED colours			Unlit 	Lit centre
        Color[] ledColours = new Color[2 * ledCount]{
										Color.Green,	Color.LightGreen,
										Color.Green,	Color.LightGreen,
										Color.Green,	Color.LightGreen,
										Color.Green,	Color.LightGreen,
										Color.Green,	Color.LightGreen,
										Color.Green,	Color.LightGreen,
										Color.Green,	Color.LightGreen,
										Color.Green,	Color.LightGreen,
										Color.Orange,	Color.Yellow,
										Color.Orange,	Color.Yellow,
										Color.Orange,	Color.Yellow,
										Color.Orange,	Color.Yellow,
										Color.Red,		Color.LightSalmon,
										Color.Red,		Color.LightSalmon,
										Color.Red,		Color.LightSalmon,
	};

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public void VuMeterLed()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
            this.Name = "VuMeterLED";
            this.Size = new System.Drawing.Size(100, 20);		// Default size for control
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawLeds(g);
        }

        public int Peak
        {
            get { return peakVal; }
            set
            {
                if (value < 0)
                {
                    peakVal = 0;
                }
                else
                    if (value >= ledCount)
                    {
                        peakVal = ledCount - 1;
                    }
                    else
                    {
                        peakVal = value;
                    }
            }
        }

        public int Volume
        // Determine how many LEDs to light - valid range 0 - 14
        {
            get { return ledVal; }
            set
            {
                // Check if maximum number of leds is reached, if so adjust value
                if (value >= ledCount)
                {
                    ledVal = ledCount - 1;
                }
                else
                {
                    ledVal = value;
                }
                this.Invalidate();					// Re-draw the control
            }
        }

        private void DrawLeds(Graphics g)
        {
            // Rectangle values for each individual LED - fit them nicely inside the border
            int ledLeft = this.ClientRectangle.Left;
            int ledTop = this.ClientRectangle.Top;
            int ledWidth = this.ClientRectangle.Width / ledCount;
            int ledHeight = this.ClientRectangle.Height;

            // Create the LED rectangle
            Rectangle ledRect;
            SolidBrush sob;

            for (int i = 0; i < ledCount; i++)
            {
                // Light the LED if it's under current value, or if it's the peak value.
                if (i <= ledVal)
                {
                    sob = new SolidBrush(ledColours[i * 2 + 1]);
                }
                else
                    if (i == peakVal)
                    {
                        sob = new SolidBrush(Color.WhiteSmoke);
                    }
                    else
                    {
                        sob = new SolidBrush(ledColours[i * 2]);
                    }

                ledRect = new Rectangle(ledLeft, ledTop, ledWidth, ledHeight);
                g.FillRectangle(sob, ledRect);
                ledLeft += ledWidth;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            // 
            // VuMeterLed
            // 
            this.Name = "VuMeterLed";
            this.Size = new System.Drawing.Size(100, 20);
        }
        #endregion
    }
    //}
}
