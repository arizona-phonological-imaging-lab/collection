// ------------------------------------------------------------------
// CaptureTest.cs
// Sample application to show the DirectX.Capture class library.
//
// History:
//	2003-Jan-25		BL		- created
//
// Copyright (c) 2003 Brian Low
// ------------------------------------------------------------------

using System;
using System.Windows.Media;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using DirectX.Capture;
using Microsoft.VisualBasic;
using ScreenShotDemo;
using System.Drawing.Imaging;
using System.Threading;
//using IronPython.Hosting;
//using IronPython;
//using Microsoft.Kinect;
//using Microsoft.Kinect.Face;
using Microsoft.Samples.Kinect.HDFaceBasics;
//using System.Windows.Media;
//using System.Windows.Media.Media3D;
//using Microsoft.Samples.Kinect.HDFaceBasics;

namespace CaptureTest
{
    
    public class CaptureTest : System.Windows.Forms.Form
    {
        // 2013-02-11 __ZSC__
        private string new_path;
        private string video_name;
        private string date_today = DateTime.Now.ToString("yyyy-MM-dd");
        private string launch_time = DateTime.Now.ToString("HH:mm:ss");
        private string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //public MainWindow kinect = new MainWindow();
        public string coordinates;
        
        private string subjectID;
        
        private Capture capture = null;
        private Filters filters = new Filters();
        //private System.Object blah = new Object();
        //private System.Windows.Media.CaptureDevice blah = new System.Windows.Media.CaptureDevice();

        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem mnuExit;
        private System.Windows.Forms.MenuItem mnuDevices;
        private System.Windows.Forms.MenuItem mnuVideoDevices;
        private System.Windows.Forms.MenuItem mnuAudioDevices;
        private System.Windows.Forms.MenuItem mnuVideoCompressors;
        private System.Windows.Forms.MenuItem mnuAudioCompressors;
        private System.Windows.Forms.MenuItem mnuVideoSources;
        private System.Windows.Forms.MenuItem mnuAudioSources;
        private System.Windows.Forms.Panel panelVideo;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem mnuAudioChannels;
        private System.Windows.Forms.MenuItem mnuAudioSamplingRate;
        private System.Windows.Forms.MenuItem mnuAudioSampleSizes;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem mnuFrameSizes;
        private System.Windows.Forms.MenuItem mnuFrameRates;
        private System.Windows.Forms.Button btnCue;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem mnuPreview;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem mnuPropertyPages;
        private System.Windows.Forms.MenuItem mnuVideoCaps;
        private System.Windows.Forms.MenuItem mnuAudioCaps;
        private System.Windows.Forms.MenuItem mnuChannel;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem mnuInputType;
        private IContainer components;
        //Kinect Related objects
        //private System.Windows.Forms.Button btnRefCoordsLabel;
        //private System.Windows.Forms.Button btnStartFaceCapture;
        //private System.Windows.Forms.Button btnWriteCoords;
        //private System.Windows.Forms.TextBox trackingStatus;
        //private System.Windows.Forms.Label pitchStatus;

        
        //public static ReadOnlyCollection<VideoCaptureDevice> GetAvailableVideoCaptureDevices();

        private DateTime date1 = new DateTime(0);
        private MenuItem menuPostProcess;
        private MenuItem menuRunPostProcess;
        private Label label2;
        private TextBox txt_recordingID;
        private TextBox trackingStatus;
        private TextBox pitchStatus;
        private TextBox yawStatus;
        private TextBox rollStatus;
        private Label kinectRegionLabel;
        private Label label4;
        private TextBox captureStatus;
        private TextBox yStatus;
        private TextBox xStatus;
        private TextBox zStatus;
        private Button btnStartCapture;
        private Button btnGetRefCoords;
        private string startEndtimes;
        //private string refTime;
        //Process myProc;

        public CaptureTest()
        {
            // 2013-02-20 __ZC__
            //Process myProc;
            // 4-22-2013 removing process for Face Tracking?? problem with Laptop running the secondary process
            //myProc = Process.Start(dtopfolder + @"\dev\GitHub\Ultraspeech\UltraCapture\FaceTrackingBasics-WPF\bin\x64\Release\FaceTrackingBasics.exe");

            //myProc = Process.Start(@"C:\Users\apiladmin\Documents\GitHub\APIL\UltraCapture\FaceTrackingBasics-WPF\bin\x64\Release\FaceTrackingBasics.exe");
            //coordinates = kinect.statusText;
            //Console.WriteLine(coordinates.ToString());
            //myProc = Process.Start(@"C:\Users\apiladmin\Desktop\HDFaceBasics-WPF\bin\x64\Release\HDFaceBasics-WPF.exe");
            //
            //myProc = Process.Start(@"C:\Users\apiladmin\Documents\Github\data-collection\UltraCapture\CaptureProgram\HDFaceBasics-WPF\bin\x64\Debug\HDFaceBasics-WPF.exe");
            //Process process = new System.Diagnostics.Process();
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.FileName = "python.exe";
            //startInfo.Arguments = @"C:\\Users\\apiladmin\\Desktop\\stimulus.py";
            //process.StartInfo = startInfo;
            //process.Start();

            //var kinect = new MainWindow();
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            MainWindow kinect_init = new MainWindow();
            //System.Threading.Thread kinectThread = new System.Threading.Thread(kinect.InitializeComponent());
            ThreadStart kinect = new ThreadStart( () => kinect_init.Entrance(trackingStatus, pitchStatus, yawStatus, rollStatus,
                                                                                xStatus, yStatus, zStatus, captureStatus,
                                                                                btnGetRefCoords, btnStartCapture) );
            Thread kinectThread = new Thread(kinect);
            kinectThread.Start();
            //kinect.InitializeComponent();
            //kinect.InitializeHDFace();
            //Console.WriteLine("blah");
            
            Console.WriteLine("Hello");
            //InitializeKinect();
            //System.Collections.ObjectModel.ReadOnlyCollection<VideoCapabilities> blah = new System.Collections.ObjectModel.ReadOnlyCollection<VideoCaptureDevice> GetAvailableVideoCaptureDevices();
            //System.Windows.DependencyObject
            // start 2013-02-19 __ZC__
       
            //var kinect = new MainWindow();
            // need to specify how to get an actual subject ID number
            //subjectID = "000123";
            /*subjectID = txt_recordingID.Text;

            // Creates desktop folder for all files

            new_path = dtopfolder + @"\" + date_today + "_" + subjectID;

            if (!(System.IO.Directory.Exists(new_path)))
            {
                System.IO.Directory.CreateDirectory(new_path);
                //System.Windows.Forms.MessageBox.Show("created new directory at:\n\n" + new_path);
            }
            else
            {
                new_path += "_1";
                System.IO.Directory.CreateDirectory(new_path);
            }

            // specify video filename
            video_name = new_path + @"\" + "video.avi";*/

            // end 2013-02-19


            // Start with the first video/audio devices
            // Don't do this in the Release build in case the
            // first devices cause problems.

            // ======================================================
            // Commented 4-4-2013 -- We want first device initialized
            //#if DEBUG
            // ======================================================
            capture = new Capture(filters.VideoInputDevices[0], filters.AudioInputDevices[0]);
            capture.CaptureComplete += new EventHandler(OnCaptureComplete);
            // ======================================================
            ////#endif
            // ======================================================

            
            // Update the main menu
            // Much of the interesting work of this sample occurs here

            //var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);


            try { updateMenu(); }
            catch { }
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


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnuExit = new System.Windows.Forms.MenuItem();
            this.mnuDevices = new System.Windows.Forms.MenuItem();
            this.mnuVideoDevices = new System.Windows.Forms.MenuItem();
            this.mnuAudioDevices = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.mnuVideoCompressors = new System.Windows.Forms.MenuItem();
            this.mnuAudioCompressors = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.mnuVideoSources = new System.Windows.Forms.MenuItem();
            this.mnuFrameSizes = new System.Windows.Forms.MenuItem();
            this.mnuFrameRates = new System.Windows.Forms.MenuItem();
            this.mnuVideoCaps = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.mnuAudioSources = new System.Windows.Forms.MenuItem();
            this.mnuAudioChannels = new System.Windows.Forms.MenuItem();
            this.mnuAudioSamplingRate = new System.Windows.Forms.MenuItem();
            this.mnuAudioSampleSizes = new System.Windows.Forms.MenuItem();
            this.mnuAudioCaps = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mnuChannel = new System.Windows.Forms.MenuItem();
            this.mnuInputType = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.mnuPropertyPages = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.mnuPreview = new System.Windows.Forms.MenuItem();
            this.menuPostProcess = new System.Windows.Forms.MenuItem();
            this.menuRunPostProcess = new System.Windows.Forms.MenuItem();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.btnCue = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_recordingID = new System.Windows.Forms.TextBox();
            this.trackingStatus = new System.Windows.Forms.TextBox();
            this.pitchStatus = new System.Windows.Forms.TextBox();
            this.yawStatus = new System.Windows.Forms.TextBox();
            this.rollStatus = new System.Windows.Forms.TextBox();
            this.kinectRegionLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.captureStatus = new System.Windows.Forms.TextBox();
            this.yStatus = new System.Windows.Forms.TextBox();
            this.xStatus = new System.Windows.Forms.TextBox();
            this.zStatus = new System.Windows.Forms.TextBox();
            this.btnStartCapture = new System.Windows.Forms.Button();
            this.btnGetRefCoords = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Location = new System.Drawing.Point(391, 390);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(124, 20);
            this.txtFilename.TabIndex = 0;
            this.txtFilename.Text = "video.avi";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(321, 393);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Filename:";
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(482, 422);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(60, 24);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Record";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(572, 422);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(40, 24);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(628, 422);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(120, 24);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.mnuDevices,
            this.menuItem7,
            this.menuPostProcess});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuExit});
            this.menuItem1.Text = "File";
            // 
            // mnuExit
            // 
            this.mnuExit.Index = 0;
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuDevices
            // 
            this.mnuDevices.Index = 1;
            this.mnuDevices.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuVideoDevices,
            this.mnuAudioDevices,
            this.menuItem4,
            this.mnuVideoCompressors,
            this.mnuAudioCompressors});
            this.mnuDevices.Text = "Devices";
            // 
            // mnuVideoDevices
            // 
            this.mnuVideoDevices.Index = 0;
            this.mnuVideoDevices.Text = "Video Devices";
            // 
            // mnuAudioDevices
            // 
            this.mnuAudioDevices.Index = 1;
            this.mnuAudioDevices.Text = "Audio Devices";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // mnuVideoCompressors
            // 
            this.mnuVideoCompressors.Index = 3;
            this.mnuVideoCompressors.Text = "Video Compressors";
            // 
            // mnuAudioCompressors
            // 
            this.mnuAudioCompressors.Index = 4;
            this.mnuAudioCompressors.Text = "Audio Compressors";
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 2;
            this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuVideoSources,
            this.mnuFrameSizes,
            this.mnuFrameRates,
            this.mnuVideoCaps,
            this.menuItem5,
            this.mnuAudioSources,
            this.mnuAudioChannels,
            this.mnuAudioSamplingRate,
            this.mnuAudioSampleSizes,
            this.mnuAudioCaps,
            this.menuItem3,
            this.mnuChannel,
            this.mnuInputType,
            this.menuItem6,
            this.mnuPropertyPages,
            this.menuItem8,
            this.mnuPreview});
            this.menuItem7.Text = "Options";
            // 
            // mnuVideoSources
            // 
            this.mnuVideoSources.Index = 0;
            this.mnuVideoSources.Text = "Video Sources";
            // 
            // mnuFrameSizes
            // 
            this.mnuFrameSizes.Index = 1;
            this.mnuFrameSizes.Text = "Video Frame Size";
            // 
            // mnuFrameRates
            // 
            this.mnuFrameRates.Index = 2;
            this.mnuFrameRates.Text = "Video Frame Rate";
            this.mnuFrameRates.Click += new System.EventHandler(this.mnuFrameRates_Click);
            // 
            // mnuVideoCaps
            // 
            this.mnuVideoCaps.Index = 3;
            this.mnuVideoCaps.Text = "Video Capabilities...";
            this.mnuVideoCaps.Click += new System.EventHandler(this.mnuVideoCaps_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "-";
            // 
            // mnuAudioSources
            // 
            this.mnuAudioSources.Index = 5;
            this.mnuAudioSources.Text = "Audio Sources";
            // 
            // mnuAudioChannels
            // 
            this.mnuAudioChannels.Index = 6;
            this.mnuAudioChannels.Text = "Audio Channels";
            // 
            // mnuAudioSamplingRate
            // 
            this.mnuAudioSamplingRate.Index = 7;
            this.mnuAudioSamplingRate.Text = "Audio Sampling Rate";
            // 
            // mnuAudioSampleSizes
            // 
            this.mnuAudioSampleSizes.Index = 8;
            this.mnuAudioSampleSizes.Text = "Audio Sample Size";
            // 
            // mnuAudioCaps
            // 
            this.mnuAudioCaps.Index = 9;
            this.mnuAudioCaps.Text = "Audio Capabilities...";
            this.mnuAudioCaps.Click += new System.EventHandler(this.mnuAudioCaps_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 10;
            this.menuItem3.Text = "-";
            // 
            // mnuChannel
            // 
            this.mnuChannel.Index = 11;
            this.mnuChannel.Text = "TV Tuner Channel";
            // 
            // mnuInputType
            // 
            this.mnuInputType.Index = 12;
            this.mnuInputType.Text = "TV Tuner Input Type";
            this.mnuInputType.Click += new System.EventHandler(this.mnuInputType_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 13;
            this.menuItem6.Text = "-";
            // 
            // mnuPropertyPages
            // 
            this.mnuPropertyPages.Index = 14;
            this.mnuPropertyPages.Text = "PropertyPages";
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 15;
            this.menuItem8.Text = "-";
            // 
            // mnuPreview
            // 
            this.mnuPreview.Index = 16;
            this.mnuPreview.Text = "Preview";
            this.mnuPreview.Click += new System.EventHandler(this.mnuPreview_Click);
            // 
            // menuPostProcess
            // 
            this.menuPostProcess.Index = 3;
            this.menuPostProcess.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuRunPostProcess});
            this.menuPostProcess.Text = "Post Processing";
            // 
            // menuRunPostProcess
            // 
            this.menuRunPostProcess.Index = 0;
            this.menuRunPostProcess.Text = "Run Post Process";
            this.menuRunPostProcess.Click += new System.EventHandler(this.menuRunPostProcess_Click);
            // 
            // panelVideo
            // 
            this.panelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelVideo.Location = new System.Drawing.Point(324, 8);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(420, 366);
            this.panelVideo.TabIndex = 6;
            // 
            // btnCue
            // 
            this.btnCue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCue.Location = new System.Drawing.Point(324, 419);
            this.btnCue.Name = "btnCue";
            this.btnCue.Size = new System.Drawing.Size(80, 24);
            this.btnCue.TabIndex = 8;
            this.btnCue.Text = "Preview";
            this.btnCue.Click += new System.EventHandler(this.btnCue_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(565, 390);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Recording ID:";
            // 
            // txt_recordingID
            // 
            this.txt_recordingID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_recordingID.Location = new System.Drawing.Point(645, 387);
            this.txt_recordingID.Name = "txt_recordingID";
            this.txt_recordingID.Size = new System.Drawing.Size(103, 20);
            this.txt_recordingID.TabIndex = 10;
            this.txt_recordingID.Text = "000123";
            // 
            // trackingStatus
            // 
            this.trackingStatus.BackColor = System.Drawing.Color.Red;
            this.trackingStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackingStatus.Location = new System.Drawing.Point(9, 66);
            this.trackingStatus.Name = "trackingStatus";
            this.trackingStatus.Size = new System.Drawing.Size(306, 47);
            this.trackingStatus.TabIndex = 12;
            this.trackingStatus.Text = "NOT TRACKING";
            this.trackingStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.trackingStatus.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // pitchStatus
            // 
            this.pitchStatus.BackColor = System.Drawing.Color.Red;
            this.pitchStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pitchStatus.Location = new System.Drawing.Point(9, 174);
            this.pitchStatus.Name = "pitchStatus";
            this.pitchStatus.Size = new System.Drawing.Size(100, 35);
            this.pitchStatus.TabIndex = 13;
            this.pitchStatus.Text = "PITCH";
            this.pitchStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // yawStatus
            // 
            this.yawStatus.BackColor = System.Drawing.Color.Red;
            this.yawStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yawStatus.Location = new System.Drawing.Point(125, 174);
            this.yawStatus.Name = "yawStatus";
            this.yawStatus.Size = new System.Drawing.Size(83, 35);
            this.yawStatus.TabIndex = 14;
            this.yawStatus.Text = "YAW";
            this.yawStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rollStatus
            // 
            this.rollStatus.BackColor = System.Drawing.Color.Red;
            this.rollStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rollStatus.Location = new System.Drawing.Point(223, 174);
            this.rollStatus.Name = "rollStatus";
            this.rollStatus.Size = new System.Drawing.Size(92, 35);
            this.rollStatus.TabIndex = 15;
            this.rollStatus.Text = "ROLL";
            this.rollStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // kinectRegionLabel
            // 
            this.kinectRegionLabel.AutoSize = true;
            this.kinectRegionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kinectRegionLabel.Location = new System.Drawing.Point(43, 34);
            this.kinectRegionLabel.Name = "kinectRegionLabel";
            this.kinectRegionLabel.Size = new System.Drawing.Size(218, 29);
            this.kinectRegionLabel.TabIndex = 19;
            this.kinectRegionLabel.Text = "Kinect Notifications";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(65, 331);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(192, 25);
            this.label4.TabIndex = 21;
            this.label4.Text = "Capture Notifications";
            // 
            // captureStatus
            // 
            this.captureStatus.BackColor = System.Drawing.Color.Red;
            this.captureStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.captureStatus.Location = new System.Drawing.Point(58, 408);
            this.captureStatus.Name = "captureStatus";
            this.captureStatus.Size = new System.Drawing.Size(214, 35);
            this.captureStatus.TabIndex = 25;
            this.captureStatus.Text = "NOT CAPTURING";
            this.captureStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // yStatus
            // 
            this.yStatus.BackColor = System.Drawing.Color.Red;
            this.yStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yStatus.Location = new System.Drawing.Point(9, 232);
            this.yStatus.Name = "yStatus";
            this.yStatus.Size = new System.Drawing.Size(122, 35);
            this.yStatus.TabIndex = 26;
            this.yStatus.Text = "Y-ALIGN";
            this.yStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // xStatus
            // 
            this.xStatus.BackColor = System.Drawing.Color.Red;
            this.xStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xStatus.Location = new System.Drawing.Point(193, 232);
            this.xStatus.Name = "xStatus";
            this.xStatus.Size = new System.Drawing.Size(122, 35);
            this.xStatus.TabIndex = 27;
            this.xStatus.Text = "X-ALIGN";
            this.xStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // zStatus
            // 
            this.zStatus.BackColor = System.Drawing.Color.Red;
            this.zStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zStatus.Location = new System.Drawing.Point(104, 282);
            this.zStatus.Name = "zStatus";
            this.zStatus.Size = new System.Drawing.Size(122, 35);
            this.zStatus.TabIndex = 28;
            this.zStatus.Text = "Z-ALIGN";
            this.zStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnStartCapture
            // 
            this.btnStartCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartCapture.Location = new System.Drawing.Point(105, 364);
            this.btnStartCapture.Name = "btnStartCapture";
            this.btnStartCapture.Size = new System.Drawing.Size(119, 33);
            this.btnStartCapture.TabIndex = 29;
            this.btnStartCapture.Text = "Start Capture";
            this.btnStartCapture.UseVisualStyleBackColor = true;
            // 
            // btnGetRefCoords
            // 
            this.btnGetRefCoords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetRefCoords.Location = new System.Drawing.Point(48, 128);
            this.btnGetRefCoords.Name = "btnGetRefCoords";
            this.btnGetRefCoords.Size = new System.Drawing.Size(238, 33);
            this.btnGetRefCoords.TabIndex = 30;
            this.btnGetRefCoords.Text = "Take Reference Coordinates";
            this.btnGetRefCoords.UseVisualStyleBackColor = true;
            // 
            // CaptureTest
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(756, 455);
            this.Controls.Add(this.btnGetRefCoords);
            this.Controls.Add(this.btnStartCapture);
            this.Controls.Add(this.zStatus);
            this.Controls.Add(this.xStatus);
            this.Controls.Add(this.yStatus);
            this.Controls.Add(this.captureStatus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.kinectRegionLabel);
            this.Controls.Add(this.rollStatus);
            this.Controls.Add(this.yawStatus);
            this.Controls.Add(this.pitchStatus);
            this.Controls.Add(this.trackingStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_recordingID);
            this.Controls.Add(this.btnCue);
            this.Controls.Add(this.panelVideo);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFilename);
            this.Menu = this.mainMenu;
            this.Name = "CaptureTest";
            this.Text = "Ultrasound Capture";
            this.Load += new System.EventHandler(this.CaptureTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
		{
            //System.Windows.Forms.Application.Run(new MainWindow());
            AppDomain currentDomain = AppDomain.CurrentDomain;

            //MainWindow kinect = new MainWindow();
            //var kinect = new MainWindow();
            //System.Threading.Thread kinectThread = new System.Threading.Thread(kinect.InitializeComponent);
            //kinectThread.Start();
            //kinect.InitializeComponent();
            //kinect.InitializeHDFace();
            //Console.WriteLine("blah");
            //InitializeComponent();
            Application.Run(new CaptureTest());

            
		}

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            if (capture != null)
                capture.Stop();
            //kinect.WriteCoords();
            System.Windows.Forms.Application.Exit();
        }

        private void btnCue_Click(object sender, System.EventArgs e)
        {
            // 2013-02-19 __ZC__
            // added try/exception from Preview dropdown menu
            // allows Prevew button to show or hide preview
            try
            {
                if (capture.PreviewWindow == null)
                {
                    capture.PreviewWindow = panelVideo;
                    mnuPreview.Checked = true;
                }
                else
                {
                    capture.PreviewWindow = null;
                    mnuPreview.Checked = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to enable/disable preview. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }


        private void btnStart_Click(object sender, System.EventArgs e)
        {
            try
            {
                btnStart.BackColor = System.Drawing.Color.Red;
                // start 2013-02-19 __ZC__

                // need to specify how to get an actual subject ID number
                //subjectID = "000123";
                string input_subject = txt_recordingID.Text;

                // getting rid of bad characters otherwise ffmpeg post-processing will break
                string pattern = "[\\~#%&*{}/:<>?|\"-]";
                string replacement = "_";

                System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern);
                subjectID = System.Text.RegularExpressions.Regex.Replace(regEx.Replace(input_subject, replacement), @"\s+", "_");

                // Creates desktop folder for all files
                
                new_path = dtopfolder + @"\" + date_today + "_" + subjectID;

                if (!(System.IO.Directory.Exists(new_path)))
                {
                    System.IO.Directory.CreateDirectory(new_path);
                    //System.Windows.Forms.MessageBox.Show("created new directory at:\n\n" + new_path);
                }
                else
                {
                    new_path += "_1";
                    System.IO.Directory.CreateDirectory(new_path);
                }

                // specify video filename
                video_name = new_path + @"\" + "video.avi";

                // end 2013-02-19

                if (capture == null)
                    throw new ApplicationException("Please select a video and/or audio device.");
                if (!capture.Cued)
                    capture.Filename = new_path + @"\" + txtFilename.Text;

                date1 = DateTime.Now;
                startEndtimes += "Before start command: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + "\r\n";

                capture.Start();

                date1 = DateTime.Now;
                startEndtimes += "After start command:  " + date1.ToString("yyyyyyyyMMddHHmmssfff") + "\r\n";


                btnCue.Enabled = false;
                btnStart.Enabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (capture == null)
                    throw new ApplicationException("Please select a video and/or audio device.");
                if (btnStart.BackColor == System.Drawing.Color.Red)
                {
                    // 4-22-2013 removing process for Face Tracking?? doesn't work on Laptop
                    //kinect.WriteCoords();
                    //kinectThread.Abort();
                    //myProc.CloseMainWindow();
                    // Changed from Kill() to CloseMainWindow() so that coords.txt is generated
                    //myProc.Kill();

                    btnStart.BackColor = System.Drawing.Color.Gray;
                    date1 = DateTime.Now;
                    startEndtimes += "Before stop time:     " + date1.ToString("yyyyyyyyMMddHHmmssfff") + "\r\n";
                    capture.Stop();
                    date1 = DateTime.Now;
                    startEndtimes += "Stop time:            " + date1.ToString("yyyyyyyyMMddHHmmssfff") + "\r\n";

                    System.IO.File.WriteAllText((new_path + @"\" + "vidTimes.txt"), startEndtimes);

                    // moves the coords.txt from desktop to subjectID folder
                    string coords_file = "coords.txt";
                    string ref_coords_file = "ref_coords.txt";

                    string coordSourceFile = System.IO.Path.Combine(dtopfolder, coords_file);
                    string coordDestFile = System.IO.Path.Combine(new_path, coords_file);
                    File.Move(coordSourceFile,
                        coordDestFile);

                    string refSourceFile = System.IO.Path.Combine(dtopfolder, ref_coords_file);
                    string refDestFile = System.IO.Path.Combine(new_path, ref_coords_file);
                    File.Move(refSourceFile,
                        refDestFile);

                    btnCue.Enabled = true;
                    // for tracking if video is running
                    btnStart.Enabled = false;

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("No recording started");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void updateMenu()
        {
            MenuItem m;
            Filter f;
            Source s;
            Source current;
            PropertyPage p;
            Control oldPreviewWindow = null;

            // Disable preview to avoid additional flashes (optional)
            if (capture != null)
            {
                oldPreviewWindow = capture.PreviewWindow;
                capture.PreviewWindow = null;
            }

            // Load video devices
            Filter videoDevice = null;
            if (capture != null)
                //Console.WriteLine("HELLO");
                videoDevice = capture.VideoDevice;
            //if (capture == null)
            //    Console.WriteLine("GOODBYE");
            //if (true)
            //    Console.WriteLine("EXIST");
            //Console.WriteLine("1: " + videoDevice.ToString());
            mnuVideoDevices.MenuItems.Clear();
            m = new MenuItem("(None)", new EventHandler(mnuVideoDevices_Click));
            m.Checked = (videoDevice == null);
            mnuVideoDevices.MenuItems.Add(m);
            for (int c = 0; c < filters.VideoInputDevices.Count; c++)
            {
                f = filters.VideoInputDevices[c];
                //Console.WriteLine("2: " + f.Name.ToString());
                m = new MenuItem(f.Name, new EventHandler(mnuVideoDevices_Click));
                //Console.WriteLine("3: "+ m.ToString() + ", " + m.Name.ToString());
                m.Checked = (videoDevice == f);
                mnuVideoDevices.MenuItems.Add(m);
            }
            mnuVideoDevices.Enabled = (filters.VideoInputDevices.Count > 0);

            // Load audio devices
            Filter audioDevice = null;
            if (capture != null)
                audioDevice = capture.AudioDevice;
            mnuAudioDevices.MenuItems.Clear();
            m = new MenuItem("(None)", new EventHandler(mnuAudioDevices_Click));
            m.Checked = (audioDevice == null);
            mnuAudioDevices.MenuItems.Add(m);
            for (int c = 0; c < filters.AudioInputDevices.Count; c++)
            {
                f = filters.AudioInputDevices[c];
                m = new MenuItem(f.Name, new EventHandler(mnuAudioDevices_Click));
                m.Checked = (audioDevice == f);
                mnuAudioDevices.MenuItems.Add(m);
            }
            mnuAudioDevices.Enabled = (filters.AudioInputDevices.Count > 0);


            // Load video compressors
            try
            {
                mnuVideoCompressors.MenuItems.Clear();
                m = new MenuItem("(None)", new EventHandler(mnuVideoCompressors_Click));
                m.Checked = (capture.VideoCompressor == null);
                mnuVideoCompressors.MenuItems.Add(m);
                for (int c = 0; c < filters.VideoCompressors.Count; c++)
                {
                    f = filters.VideoCompressors[c];
                    m = new MenuItem(f.Name, new EventHandler(mnuVideoCompressors_Click));
                    m.Checked = (capture.VideoCompressor == f);
                    mnuVideoCompressors.MenuItems.Add(m);
                }
                mnuVideoCompressors.Enabled = ((capture.VideoDevice != null) && (filters.VideoCompressors.Count > 0));
            }
            catch { mnuVideoCompressors.Enabled = false; }

            // Load audio compressors
            try
            {
                mnuAudioCompressors.MenuItems.Clear();
                m = new MenuItem("(None)", new EventHandler(mnuAudioCompressors_Click));
                m.Checked = (capture.AudioCompressor == null);
                mnuAudioCompressors.MenuItems.Add(m);
                for (int c = 0; c < filters.AudioCompressors.Count; c++)
                {
                    f = filters.AudioCompressors[c];
                    m = new MenuItem(f.Name, new EventHandler(mnuAudioCompressors_Click));
                    m.Checked = (capture.AudioCompressor == f);
                    mnuAudioCompressors.MenuItems.Add(m);
                }
                mnuAudioCompressors.Enabled = ((capture.AudioDevice != null) && (filters.AudioCompressors.Count > 0));
            }
            catch { mnuAudioCompressors.Enabled = false; }

            // Load video sources
            try
            {
                mnuVideoSources.MenuItems.Clear();
                current = capture.VideoSource;
                //Console.WriteLine("1: " + capture.VideoSource.ToString());
                //Console.WriteLine("2: " + capture.VideoSources[0].ToString());
                for (int c = 0; c < capture.VideoSources.Count; c++)
                {
                    s = capture.VideoSources[c];
                    m = new MenuItem(s.Name, new EventHandler(mnuVideoSources_Click));
                    m.Checked = (current == s);
                    mnuVideoSources.MenuItems.Add(m);
                }
                mnuVideoSources.Enabled = (capture.VideoSources.Count > 0);
            }
            catch { mnuVideoSources.Enabled = false; }

            // Load audio sources
            try
            {
                mnuAudioSources.MenuItems.Clear();
                current = capture.AudioSource;
                for (int c = 0; c < capture.AudioSources.Count; c++)
                {
                    s = capture.AudioSources[c];
                    m = new MenuItem(s.Name, new EventHandler(mnuAudioSources_Click));
                    m.Checked = (current == s);
                    mnuAudioSources.MenuItems.Add(m);
                }
                mnuAudioSources.Enabled = (capture.AudioSources.Count > 0);
            }
            catch { mnuAudioSources.Enabled = false; }

            // Load frame rates
            try
            {
                mnuFrameRates.MenuItems.Clear();
                int frameRate = (int)(capture.FrameRate * 1000);
                m = new MenuItem("15 fps", new EventHandler(mnuFrameRates_Click));
                m.Checked = (frameRate == 15000);
                mnuFrameRates.MenuItems.Add(m);
                m = new MenuItem("24 fps (Film)", new EventHandler(mnuFrameRates_Click));
                m.Checked = (frameRate == 24000);
                mnuFrameRates.MenuItems.Add(m);
                m = new MenuItem("25 fps (PAL)", new EventHandler(mnuFrameRates_Click));
                m.Checked = (frameRate == 25000);
                mnuFrameRates.MenuItems.Add(m);
                m = new MenuItem("29.997 fps (NTSC)", new EventHandler(mnuFrameRates_Click));
                m.Checked = (frameRate == 29997);
                mnuFrameRates.MenuItems.Add(m);
                m = new MenuItem("30 fps (~NTSC)", new EventHandler(mnuFrameRates_Click));
                m.Checked = (frameRate == 30000);
                mnuFrameRates.MenuItems.Add(m);
                m = new MenuItem("59.994 fps (2xNTSC)", new EventHandler(mnuFrameRates_Click));
                m.Checked = (frameRate == 59994);
                mnuFrameRates.MenuItems.Add(m);
                mnuFrameRates.Enabled = true;
            }
            catch { mnuFrameRates.Enabled = false; }

            // Load frame sizes
            try
            {
                mnuFrameSizes.MenuItems.Clear();
                System.Drawing.Size frameSize = capture.FrameSize;
                m = new MenuItem("160 x 120", new EventHandler(mnuFrameSizes_Click));
                m.Checked = (frameSize == new System.Drawing.Size(160, 120));
                mnuFrameSizes.MenuItems.Add(m);
                m = new MenuItem("320 x 240", new EventHandler(mnuFrameSizes_Click));
                m.Checked = (frameSize == new System.Drawing.Size(320, 240));
                mnuFrameSizes.MenuItems.Add(m);
                m = new MenuItem("640 x 480", new EventHandler(mnuFrameSizes_Click));
                m.Checked = (frameSize == new System.Drawing.Size(640, 480));
                mnuFrameSizes.MenuItems.Add(m);
                m = new MenuItem("1024 x 768", new EventHandler(mnuFrameSizes_Click));
                m.Checked = (frameSize == new System.Drawing.Size(1024, 768));
                mnuFrameSizes.MenuItems.Add(m);
                mnuFrameSizes.Enabled = true;
            }
            catch { mnuFrameSizes.Enabled = false; }

            // Load audio channels
            try
            {
                mnuAudioChannels.MenuItems.Clear();
                short audioChannels = capture.AudioChannels;
                m = new MenuItem("Mono", new EventHandler(mnuAudioChannels_Click));
                m.Checked = (audioChannels == 1);
                mnuAudioChannels.MenuItems.Add(m);
                m = new MenuItem("Stereo", new EventHandler(mnuAudioChannels_Click));
                m.Checked = (audioChannels == 2);
                mnuAudioChannels.MenuItems.Add(m);
                mnuAudioChannels.Enabled = true;
            }
            catch { mnuAudioChannels.Enabled = false; }

            // Load audio sampling rate
            try
            {
                mnuAudioSamplingRate.MenuItems.Clear();
                int samplingRate = capture.AudioSamplingRate;
                m = new MenuItem("8 kHz", new EventHandler(mnuAudioSamplingRate_Click));
                m.Checked = (samplingRate == 8000);
                mnuAudioSamplingRate.MenuItems.Add(m);
                m = new MenuItem("11.025 kHz", new EventHandler(mnuAudioSamplingRate_Click));
                m.Checked = (capture.AudioSamplingRate == 11025);
                mnuAudioSamplingRate.MenuItems.Add(m);
                m = new MenuItem("22.05 kHz", new EventHandler(mnuAudioSamplingRate_Click));
                m.Checked = (capture.AudioSamplingRate == 22050);
                mnuAudioSamplingRate.MenuItems.Add(m);
                m = new MenuItem("44.1 kHz", new EventHandler(mnuAudioSamplingRate_Click));
                m.Checked = (capture.AudioSamplingRate == 44100);
                mnuAudioSamplingRate.MenuItems.Add(m);
                mnuAudioSamplingRate.Enabled = true;
            }
            catch { mnuAudioSamplingRate.Enabled = false; }

            // Load audio sample sizes
            try
            {
                mnuAudioSampleSizes.MenuItems.Clear();
                short sampleSize = capture.AudioSampleSize;
                m = new MenuItem("8 bit", new EventHandler(mnuAudioSampleSizes_Click));
                m.Checked = (sampleSize == 8);
                mnuAudioSampleSizes.MenuItems.Add(m);
                m = new MenuItem("16 bit", new EventHandler(mnuAudioSampleSizes_Click));
                m.Checked = (sampleSize == 16);
                mnuAudioSampleSizes.MenuItems.Add(m);
                mnuAudioSampleSizes.Enabled = true;
            }
            catch { mnuAudioSampleSizes.Enabled = false; }

            // Load property pages
            try
            {
                mnuPropertyPages.MenuItems.Clear();
                for (int c = 0; c < capture.PropertyPages.Count; c++)
                {
                    p = capture.PropertyPages[c];
                    m = new MenuItem(p.Name + "...", new EventHandler(mnuPropertyPages_Click));
                    mnuPropertyPages.MenuItems.Add(m);
                }
                mnuPropertyPages.Enabled = (capture.PropertyPages.Count > 0);
            }
            catch { mnuPropertyPages.Enabled = false; }

            // Load TV Tuner channels
            try
            {
                mnuChannel.MenuItems.Clear();
                int channel = capture.Tuner.Channel;
                for (int c = 1; c <= 25; c++)
                {
                    m = new MenuItem(c.ToString(), new EventHandler(mnuChannel_Click));
                    m.Checked = (channel == c);
                    mnuChannel.MenuItems.Add(m);
                }
                mnuChannel.Enabled = true;
            }
            catch { mnuChannel.Enabled = false; }

            // Load TV Tuner input types
            try
            {
                mnuInputType.MenuItems.Clear();
                m = new MenuItem(TunerInputType.Cable.ToString(), new EventHandler(mnuInputType_Click));
                m.Checked = (capture.Tuner.InputType == TunerInputType.Cable);
                mnuInputType.MenuItems.Add(m);
                m = new MenuItem(TunerInputType.Antenna.ToString(), new EventHandler(mnuInputType_Click));
                m.Checked = (capture.Tuner.InputType == TunerInputType.Antenna);
                mnuInputType.MenuItems.Add(m);
                mnuInputType.Enabled = true;
            }
            catch { mnuInputType.Enabled = false; }

            // Enable/disable caps
            mnuVideoCaps.Enabled = ((capture != null) && (capture.VideoCaps != null));
            mnuAudioCaps.Enabled = ((capture != null) && (capture.AudioCaps != null));

            // Check Preview menu option
            mnuPreview.Checked = (oldPreviewWindow != null);
            mnuPreview.Enabled = (capture != null);

            // Reenable preview if it was enabled before
            if (capture != null)
                capture.PreviewWindow = oldPreviewWindow;
        }

        private void mnuVideoDevices_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Get current devices and dispose of capture object
                // because the video and audio device can only be changed
                // by creating a new Capture object.
                Filter videoDevice = null;
                Filter audioDevice = null;
                if (capture != null)
                {
                    videoDevice = capture.VideoDevice;
                    audioDevice = capture.AudioDevice;
                    capture.Dispose();
                    capture = null;
                }

                // Get new video device
                MenuItem m = sender as MenuItem;
                videoDevice = (m.Index > 0 ? filters.VideoInputDevices[m.Index - 1] : null);

                // Create capture object
                if ((videoDevice != null) || (audioDevice != null))
                {
                    capture = new Capture(videoDevice, audioDevice);
                    capture.CaptureComplete += new EventHandler(OnCaptureComplete);
                }

                // Update the menu
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Video device not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuAudioDevices_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Get current devices and dispose of capture object
                // because the video and audio device can only be changed
                // by creating a new Capture object.
                Filter videoDevice = null;
                Filter audioDevice = null;
                if (capture != null)
                {
                    videoDevice = capture.VideoDevice;
                    audioDevice = capture.AudioDevice;
                    capture.Dispose();
                    capture = null;
                }

                // Get new audio device
                MenuItem m = sender as MenuItem;
                audioDevice = (m.Index > 0 ? filters.AudioInputDevices[m.Index - 1] : null);

                // Create capture object
                if ((videoDevice != null) || (audioDevice != null))
                {
                    capture = new Capture(videoDevice, audioDevice);
                    capture.CaptureComplete += new EventHandler(OnCaptureComplete);
                }

                // Update the menu
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Audio device not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuVideoCompressors_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Change the video compressor
                // We subtract 1 from m.Index beacuse the first item is (None)
                MenuItem m = sender as MenuItem;
                capture.VideoCompressor = (m.Index > 0 ? filters.VideoCompressors[m.Index - 1] : null);
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Video compressor not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }

        }

        private void mnuAudioCompressors_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Change the audio compressor
                // We subtract 1 from m.Index beacuse the first item is (None)
                MenuItem m = sender as MenuItem;
                capture.AudioCompressor = (m.Index > 0 ? filters.AudioCompressors[m.Index - 1] : null);
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Audio compressor not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuVideoSources_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Choose the video source
                // If the device only has one source, this menu item will be disabled
                MenuItem m = sender as MenuItem;
                capture.VideoSource = capture.VideoSources[m.Index];
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to set video source. Please submit bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuAudioSources_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Choose the audio source
                // If the device only has one source, this menu item will be disabled
                MenuItem m = sender as MenuItem;
                capture.AudioSource = capture.AudioSources[m.Index];
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to set audio source. Please submit bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }


        private void mnuExit_Click(object sender, System.EventArgs e)
        {
            if (capture != null)
                capture.Stop();
            System.Windows.Forms.Application.Exit();
        }

        private void mnuFrameSizes_Click(object sender, System.EventArgs e)
        {
            try
            {
                // Disable preview to avoid additional flashes (optional)
                bool preview = (capture.PreviewWindow != null);
                capture.PreviewWindow = null;

                // Update the frame size
                MenuItem m = sender as MenuItem;
                string[] s = m.Text.Split('x');
                System.Drawing.Size size = new System.Drawing.Size(int.Parse(s[0]), int.Parse(s[1]));
                capture.FrameSize = size;

                // Update the menu
                updateMenu();

                // Restore previous preview setting
                capture.PreviewWindow = (preview ? panelVideo : null);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Frame size not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuFrameRates_Click(object sender, System.EventArgs e)
        {
            try
            {
                MenuItem m = sender as MenuItem;
                string[] s = m.Text.Split(' ');
                capture.FrameRate = double.Parse(s[0]);
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Frame rate not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }


        private void mnuAudioChannels_Click(object sender, System.EventArgs e)
        {
            try
            {
                MenuItem m = sender as MenuItem;
                capture.AudioChannels = (short)Math.Pow(2, m.Index);
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Number of audio channels not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuAudioSamplingRate_Click(object sender, System.EventArgs e)
        {
            try
            {
                MenuItem m = sender as MenuItem;
                string[] s = m.Text.Split(' ');
                int samplingRate = (int)(double.Parse(s[0]) * 1000);
                capture.AudioSamplingRate = samplingRate;
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Audio sampling rate not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuAudioSampleSizes_Click(object sender, System.EventArgs e)
        {
            try
            {
                MenuItem m = sender as MenuItem;
                string[] s = m.Text.Split(' ');
                short sampleSize = short.Parse(s[0]);
                capture.AudioSampleSize = sampleSize;
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Audio sample size not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuPreview_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (capture.PreviewWindow == null)
                {
                    capture.PreviewWindow = panelVideo;
                    mnuPreview.Checked = true;
                }
                else
                {
                    capture.PreviewWindow = null;
                    mnuPreview.Checked = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to enable/disable preview. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuPropertyPages_Click(object sender, System.EventArgs e)
        {
            try
            {
                MenuItem m = sender as MenuItem;
                capture.PropertyPages[m.Index].Show(this);
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable display property page. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuChannel_Click(object sender, System.EventArgs e)
        {
            try
            {
                MenuItem m = sender as MenuItem;
                capture.Tuner.Channel = m.Index + 1;
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable change channel. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuInputType_Click(object sender, System.EventArgs e)
        {
            try
            {
                MenuItem m = sender as MenuItem;
                capture.Tuner.InputType = (TunerInputType)m.Index;
                updateMenu();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable change tuner input type. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuVideoCaps_Click(object sender, System.EventArgs e)
        {
            try
            {
                string s;
                s = String.Format(
                    "Video Device Capabilities\n" +
                    "--------------------------------\n\n" +
                    "Input Size:\t\t{0} x {1}\n" +
                    "\n" +
                    "Min Frame Size:\t\t{2} x {3}\n" +
                    "Max Frame Size:\t\t{4} x {5}\n" +
                    "Frame Size Granularity X:\t{6}\n" +
                    "Frame Size Granularity Y:\t{7}\n" +
                    "\n" +
                    "Min Frame Rate:\t\t{8:0.000} fps\n" +
                    "Max Frame Rate:\t\t{9:0.000} fps\n",
                    capture.VideoCaps.InputSize.Width, capture.VideoCaps.InputSize.Height,
                    capture.VideoCaps.MinFrameSize.Width, capture.VideoCaps.MinFrameSize.Height,
                    capture.VideoCaps.MaxFrameSize.Width, capture.VideoCaps.MaxFrameSize.Height,
                    capture.VideoCaps.FrameSizeGranularityX,
                    capture.VideoCaps.FrameSizeGranularityY,
                    capture.VideoCaps.MinFrameRate,
                    capture.VideoCaps.MaxFrameRate);
                System.Windows.Forms.MessageBox.Show(s);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable display video capabilities. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void mnuAudioCaps_Click(object sender, System.EventArgs e)
        {
            try
            {
                string s;
                s = String.Format(
                    "Audio Device Capabilities\n" +
                    "--------------------------------\n\n" +
                    "Min Channels:\t\t{0}\n" +
                    "Max Channels:\t\t{1}\n" +
                    "Channels Granularity:\t{2}\n" +
                    "\n" +
                    "Min Sample Size:\t\t{3}\n" +
                    "Max Sample Size:\t\t{4}\n" +
                    "Sample Size Granularity:\t{5}\n" +
                    "\n" +
                    "Min Sampling Rate:\t\t{6}\n" +
                    "Max Sampling Rate:\t\t{7}\n" +
                    "Sampling Rate Granularity:\t{8}\n",
                    capture.AudioCaps.MinimumChannels,
                    capture.AudioCaps.MaximumChannels,
                    capture.AudioCaps.ChannelsGranularity,
                    capture.AudioCaps.MinimumSampleSize,
                    capture.AudioCaps.MaximumSampleSize,
                    capture.AudioCaps.SampleSizeGranularity,
                    capture.AudioCaps.MinimumSamplingRate,
                    capture.AudioCaps.MaximumSamplingRate,
                    capture.AudioCaps.SamplingRateGranularity);
                System.Windows.Forms.MessageBox.Show(s);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable display audio capabilities. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void OnCaptureComplete(object sender, EventArgs e)
        {
            // Demonstrate the Capture.CaptureComplete event.
            Debug.WriteLine("Capture complete.");
        }

        // 2013-02-05 __ZSC__
        // added palate button and screen capture functionality
        //private void palate_Click(object sender, EventArgs e)
        //{
        //    // date and desktop folder path for saving stuff
        //    //String date_today = DateTime.Now.ToString("yyyy-MM-dd");
        //    //String dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //    ScreenCapture sc = new ScreenCapture();
        //    // capture this window, and save it
        //    //System.Windows.Forms.MessageBox.Show(new_path);
        //    sc.CaptureWindowToFile(this.Handle, new_path + "\\palate_" + date_today + ".jpg", ImageFormat.Jpeg);
        //}

        private void CaptureTest_Load(object sender, EventArgs e)
        {
            // work in progress
            // trying to get window to display to get Subject ID info on window load

            //ExperimentInfo form = new ExperimentInfo();
            //form.Show();
        }

        private void menuRunPostProcess_Click(object sender, EventArgs e)
        {
            // check that capture is not still recording
            if (!btnStart.Enabled)
            {
                //System.Windows.Forms.MessageBox.Show("I have started running the post processing function");

                // catches if people are trying to post-process without recording
                if (capture == null)
                    throw new ApplicationException("Please record something before Post-Processing.");
                // create output folder for frames
                string frame_folder = new_path + @"\" + "frames";
                //System.Windows.Forms.MessageBox.Show("frame_folder: " + frame_folder);
                if (!(System.IO.Directory.Exists(frame_folder)))
                {
                    System.IO.Directory.CreateDirectory(frame_folder);
                    //System.Windows.Forms.MessageBox.Show("created new directory at:\n\n" + frame_folder);
                }

                // run ffmpeg command line script
                Process process = new System.Diagnostics.Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.FileName = "cmd.exe";
                // moves the coords.txt from desktop to subjectID folder
                string coords_file = "coords.txt";
                string ref_coords_file = "ref_coords.txt";

                string coordSourceFile = System.IO.Path.Combine(dtopfolder, coords_file);
                string coordDestFile = System.IO.Path.Combine(new_path, coords_file);

                string refSourceFile = System.IO.Path.Combine(dtopfolder, ref_coords_file);
                string refDestFile = System.IO.Path.Combine(new_path, ref_coords_file);

                //System.Windows.Forms.MessageBox.Show("sourcefile: " + sourceFile);
                //System.Windows.Forms.MessageBox.Show("destFile: "+destFile);


                if ((System.IO.File.Exists(coordSourceFile)) && (System.IO.File.Exists(refSourceFile)))
                {
                   // Console.WriteLine("I'll try to move the files");
                    System.IO.File.Move(coordSourceFile, coordDestFile);
                    System.IO.File.Move(refSourceFile, refDestFile);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Unable to move coords.txt files.\nFiles Not Found!");
                }

                // moves the stimulus file from stim folder to subjectID folder
                string stim_file = "stimulus_response.csv";

                //string stimSource = System.IO.Path.Combine(dtopfolder, "Stimulus Display", stim_file);
                string stimSource = System.IO.Path.Combine(dtopfolder, stim_file);
                string stimDest = System.IO.Path.Combine(new_path, stim_file);

                /*if (System.IO.File.Exists(stimSource))
                {
                    System.IO.File.Move(stimSource, stimDest);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Unable to move stimulus file.\nFile Not Found!");
                }*/

                // extracts video frames and audio file
                startInfo.Arguments = @"/C/ffmpeg/bin/ffmpeg -i " + video_name + " -r 30000/1001 -qscale 0 -f image2 " + frame_folder + @"\frame-%07d.png -acodec copy " + new_path + @"\" + subjectID + ".wav";
                //System.Windows.Forms.MessageBox.Show(startInfo.Arguments.ToString());
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();
                
                //var py = Python.CreateEngine();
                //var scope = py.CreateScope();
                //scope.SetVariable("vidpath",new_path + @"\" + "vidtimes.txt");
                //scope.SetVariable("coordspath", coordDestFile);
                //scope.SetVariable("framespath", frame_folder);
                //scope.SetVariable("wavpath", new_path + @"\" + subjectID + ".wav");
                //scope.SetVariable("kinect_ref_file", refDestFile);
                //scope.SetVariable("dirpath", new_path);

                //var paths = py.GetSearchPaths();
                //string path1 = "C:\\Program Files (x86)\\IronPython 2.7\\Lib";
                //string path2 = "C:\\Program Files (x86)\\IronPython 2.7\\DLLs";
                //string path3 = "C:\\Program Files (x86)\\IronPython 2.7";
                //string path4 = "C:\\Program Files (x86)\\IronPython 2.7\\Lib\\site-packages";
                //paths.Add(path1);
                //paths.Add(path2);
                //paths.Add(path3);
                //paths.Add(path4);
                //py.SetSearchPaths(paths);
                
                //try
                //{
                //    py.ExecuteFile("kinect_rotation.py");
                //}
                //catch (Exception exc)
                //{
                //    Console.WriteLine("python script - kinect_rotation.py - could not execute. Exception {0}", exc);
                //}
                // makes any necessary pitch rotation to the extracted ultrasound video frames
                startInfo.FileName = "python.exe";
                //startInfo.RedirectStandardError = true;
                //startInfo.UseShellExecute = false;  
                startInfo.Arguments = @"C:\\Users\\apiladmin\\Desktop\\kinect_rotation.py " + new_path + @"\\" + "vidTimes.txt" + " " + coordDestFile + " " + frame_folder + " " + new_path + @"\\" + subjectID + ".wav" + " " + refDestFile + " " + new_path;
                //System.Windows.Forms.MessageBox.Show(startInfo.Arguments.ToString());
                process.StartInfo = startInfo;
                process.Start();
                //StreamReader se = process.StandardError;
                ////StreamReader so = process.StandardOutput;
                //String err = se.ReadToEnd();
                ////String outp = so.ReadToEnd();
                //string[] r = err.Split(new char[] { ' ' }); // get the parameter
                //Console.WriteLine("Standard Error: {0}\n", err);

                process.WaitForExit();

                // some way to show when process is done
                System.Windows.Forms.MessageBox.Show("Post-processing has completed");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please STOP recording before Post-Processing");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }        
    }
}
