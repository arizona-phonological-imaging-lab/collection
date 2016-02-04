// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.HDFaceBasics
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Forms;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Face;

    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //int i = 0;
        System.Collections.Generic.Queue<Tuple<double, double, double, float, float, float, string>> q = new System.Collections.Generic.Queue<Tuple<double, double, double, float, float, float, string>>();
        /// <summary>
        /// Currently used KinectSensor
        /// </summary>
        private KinectSensor sensor = null;
        
        /// <summary>
        /// Body frame source to get a BodyFrameReader
        /// </summary>
        private BodyFrameSource bodySource = null;

        /// <summary>
        /// Body frame reader to get body frames
        /// </summary>
        private BodyFrameReader bodyReader = null;
        
        public System.Text.StringBuilder coordinates = new System.Text.StringBuilder();

        //Establishes the tuple to hold the reference coordinates
        public Tuple<double, double, double, float, float, float, string> refCoords;

        private DateTime date1 = new DateTime(0);

        //private Quaternion quat;

        /// <summary>
        /// HighDefinitionFaceFrameSource to get a reader and a builder from.
        /// Also to set the currently tracked user id to get High Definition Face Frames of
        /// </summary>
        private HighDefinitionFaceFrameSource highDefinitionFaceFrameSource = null;

        /// <summary>
        /// HighDefinitionFaceFrameReader to read HighDefinitionFaceFrame to get FaceAlignment
        /// </summary>
        private HighDefinitionFaceFrameReader highDefinitionFaceFrameReader = null;

        /// <summary>
        /// FaceAlignment is the result of tracking a face, it has face animations location and orientation
        /// </summary>
        private FaceAlignment currentFaceAlignment = null;

        /// <summary>
        /// FaceModel is a result of capturing a face
        /// </summary>
        private FaceModel currentFaceModel = null;

        /// <summary>
        /// FaceModelBuilder is used to produce a FaceModel
        /// </summary>
        private FaceModelBuilder faceModelBuilder = null;

        /// <summary>
        /// The currently tracked body
        /// </summary>
        private Body currentTrackedBody = null;

        /// <summary>
        /// The currently tracked body
        /// </summary>
        private ulong currentTrackingId = 0;

        /// <summary>
        /// The ID for the last incoming frame
        /// </summary>
        private ulong pastTrackingId = 0;

        /// <summary>
        /// Gets or sets the current tracked user id
        /// </summary>
        private string currentBuilderStatus = string.Empty;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        private string statusText = "Ready To Start Capture";

        /// <summary>
        /// Finds the current date
        /// </summary>
        private string date_today = DateTime.Now.ToString("yyyy-MM-dd");

        // Multi-threading related vars
        public delegate void SetTextCallback(TextBox txtbox, string text);

        public delegate string CheckTextCallback(TextBox txtBox);

        public delegate void SetTextColorCallback(Button btn, string color);

        public delegate void SetColorCallback(TextBox txtbox, string color);

        public delegate void SetEnableCallback(Button btn, string status);

        public delegate bool CheckEnableCallback(Button btn);

        System.Windows.Forms.Control control = new System.Windows.Forms.Control();

        // vars from Capture Test
        public TextBox trackingStatus;
        public TextBox captureStatus;
        public TextBox pitchStatus;
        public TextBox yawStatus;
        public TextBox rollStatus;
        public TextBox xStatus;
        public TextBox yStatus;
        public TextBox zStatus;
        public TextBox subjID;
        public Button btnGetRefCoords;
        public Button btnStartCapture;
        public Button btnStart;
        public Button btnStop;
        

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public void Entrance(TextBox trackingStatus, TextBox pitchStatus, TextBox yawStatus, TextBox rollStatus,
                                TextBox xStatus, TextBox yStatus, TextBox zStatus, TextBox captureStatus,
                                Button btnGetRefCoords, Button btnStartCapture, Button btnStart, Button btnStop,
                                TextBox subjID, Control control)
        {
            //this.InitializeComponent();
            //this.DataContext = this;
            this.trackingStatus = trackingStatus;
            this.pitchStatus = pitchStatus;
            this.yawStatus = yawStatus;
            this.rollStatus = rollStatus;
            this.xStatus = xStatus;
            this.yStatus = yStatus;
            this.zStatus = zStatus;
            this.captureStatus = captureStatus;
            this.btnGetRefCoords = btnGetRefCoords;
            this.btnStartCapture = btnStartCapture;
            this.btnStart = btnStart;
            this.btnStop = btnStop;
            this.subjID = subjID;
            this.control = control;
            this.InitializeHDFace();
            
        }

        private void ChangeText(TextBox txtbox, string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txtbox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeText);
                
                control.Invoke(d, new object[] { txtbox, text });

            }
            else
            {
                txtbox.Text = text;
            }
        }

        private string CheckText(TextBox txtBox)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txtBox.InvokeRequired)
            {
                CheckTextCallback d = new CheckTextCallback(CheckText);

                return (string)control.Invoke(d, new object[] { txtBox });


            }
            else
            {
                return txtBox.Text;
            }
        }

        private void ChangeTextColor(Button btn, string color)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (btn.InvokeRequired)
            {
                SetTextColorCallback d = new SetTextColorCallback(ChangeTextColor);

                control.Invoke(d, new object[] { btn, color });

            }
            else
            {
                if (color == "grey" || color == "gray")
                {
                    btn.ForeColor = System.Drawing.SystemColors.GrayText;
                }
                else
                {
                    btn.ForeColor = System.Drawing.SystemColors.ControlText;
                }
            }
        }

        private void ChangeColor(TextBox txtbox, string color)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txtbox.InvokeRequired)
            {
                SetColorCallback d = new SetColorCallback(ChangeColor);
                
                control.Invoke(d, new object[] { txtbox, color });

            }
            else
            {
                if (color == "red")
                {
                    txtbox.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    txtbox.BackColor = System.Drawing.Color.Green;
                }
            }
        }
        private void SetEnable(Button btn, string status)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (btn.InvokeRequired)
            {
                SetEnableCallback d = new SetEnableCallback(SetEnable);

                control.Invoke(d, new object[] { btn, status });

            }
            else
            {

                if (status == "enable")
                {
                    btn.Enabled = true;
                }
                else
                {
                    btn.Enabled = false;
                }
            }
        }

        private bool CheckEnable(Button btn)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (btn.InvokeRequired)
            {
                CheckEnableCallback d = new CheckEnableCallback(CheckEnable);

                return (bool)control.Invoke(d, new object[] { btn });


            }
            else
            {
                return btn.Enabled;
            }
        }



        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the current tracked user id
        /// </summary>
        private ulong CurrentTrackingId
        {
            get
            {
                return this.currentTrackingId;
            }

            set
            {
                this.currentTrackingId = value;

                this.StatusText = this.MakeStatusText();
            }
        }

        private ulong PastTrackingId
        {
            get
            {
                return this.pastTrackingId;
            }

            set
            {
                this.pastTrackingId = value;

                this.StatusText = this.MakeStatusText();
            }
        }

        /// <summary>
        /// Gets or sets the current Face Builder instructions to user
        /// </summary>
        private string CurrentBuilderStatus
        {
            get
            {
                return this.currentBuilderStatus;
            }

            set
            {
                this.currentBuilderStatus = value;

                this.StatusText = this.MakeStatusText();
            }
        }

        /// <summary>
        /// Returns the length of a vector from origin
        /// </summary>
        /// <param name="point">Point in space to find it's distance from origin</param>
        /// <returns>Distance from origin</returns>
        private static double VectorLength(CameraSpacePoint point)
        {
            var result = Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2);

            result = Math.Sqrt(result);

            return result;
        }

        /// <summary>
        /// Finds the closest body from the sensor if any
        /// </summary>
        /// <param name="bodyFrame">A body frame</param>
        /// <returns>Closest body, null of none</returns>
        private static Body FindClosestBody(BodyFrame bodyFrame)
        {
            Body result = null;
            double closestBodyDistance = double.MaxValue;

            Body[] bodies = new Body[bodyFrame.BodyCount];
            bodyFrame.GetAndRefreshBodyData(bodies);

            foreach (var body in bodies)
            {
                if (body.IsTracked)
                {
                    var currentLocation = body.Joints[JointType.SpineBase].Position;

                    var currentDistance = VectorLength(currentLocation);

                    if (result == null || currentDistance < closestBodyDistance)
                    {
                        result = body;
                        closestBodyDistance = currentDistance;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Find if there is a body tracked with the given trackingId
        /// </summary>
        /// <param name="bodyFrame">A body frame</param>
        /// <param name="trackingId">The tracking Id</param>
        /// <returns>The body object, null of none</returns>
        private static Body FindBodyWithTrackingId(BodyFrame bodyFrame, ulong trackingId)
        {
            Body result = null;

            Body[] bodies = new Body[bodyFrame.BodyCount];
            bodyFrame.GetAndRefreshBodyData(bodies);

            foreach (var body in bodies)
            {
                if (body.IsTracked)
                {
                    if (body.TrackingId == trackingId)
                    {
                        result = body;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the current collection status
        /// </summary>
        /// <param name="status">Status value</param>
        /// <returns>Status value as text</returns>
        private string GetCollectionStatusText(FaceModelBuilderCollectionStatus status)
        {
            string res = string.Empty;

            if ((status & FaceModelBuilderCollectionStatus.FrontViewFramesNeeded) != 0)
            {
                this.ChangeText(this.captureStatus, "Look FORWARD");
                //this.forwardNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "FrontViewFramesNeeded";
                return res;
            }
            //else
            //{
            //    this.forwardNeeded.Opacity = 50;
            //    this.forwardNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            //}

            if ((status & FaceModelBuilderCollectionStatus.LeftViewsNeeded) != 0)
            {
                this.ChangeText(this.captureStatus, "Look LEFT");
                //this.leftNeeded.Opacity = 100;
                //this.leftNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "LeftViewsNeeded";
                return res;
            }
            //else
            //{
            //    this.leftNeeded.Opacity = 50;
            //    this.leftNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            //}

            if ((status & FaceModelBuilderCollectionStatus.RightViewsNeeded) != 0)
            {

                this.ChangeText(this.captureStatus, "Look RIGHT");
                //this.rightNeeded.Opacity = 100;
                //this.rightNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "RightViewsNeeded";
                return res;
            }
            //else
            //{
            //    this.rightNeeded.Opacity = 50;
            //    this.rightNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            //}
            Console.WriteLine("status " + status.ToString());
            Console.WriteLine("Collection Status " + FaceModelBuilderCollectionStatus.RightViewsNeeded.ToString());
            if ((status & FaceModelBuilderCollectionStatus.TiltedUpViewsNeeded) != 0)
            {
                this.ChangeText(this.captureStatus, "Look UP");
                //this.upNeeded.Opacity = 100;
                //this.upNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "TiltedUpViewsNeeded";
                Console.WriteLine(res);
                return res;
            }
            Console.WriteLine("Passed Tilted UP");
            //else
            //{
            //    this.upNeeded.Opacity = 50;
            //    this.upNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            //}

            if ((status & FaceModelBuilderCollectionStatus.MoreFramesNeeded) != 0)
            {
                Console.WriteLine("MoreFramesNeeded");
                res = "TiltedUpViewsNeeded";
                return res;
            }
            Console.WriteLine("Right before Capture Completion Status");
            if ((status & FaceModelBuilderCollectionStatus.Complete) != 0)
            {
                this.ChangeText(this.captureStatus, "CAPTURED");
                this.ChangeColor(this.captureStatus, "green");
                //this.forwardNeeded.Opacity = 0;
                //this.leftNeeded.Opacity = 0;
                //this.rightNeeded.Opacity = 0;
                //this.upNeeded.Opacity = 0;
                //this.captured.Opacity = 100;
                res = "Complete";
                return res;
            }

            return res;
        }

        /// <summary>
        /// Helper function to format a status message
        /// </summary>
        /// <returns>Status text</returns>
        private string MakeStatusText()
        {
            string status = string.Format(System.Globalization.CultureInfo.CurrentCulture, "APIL. Builder Status: {0}, Current Tracking ID: {1}", this.CurrentBuilderStatus, this.CurrentTrackingId);

            return status;
        }

        /// <summary>
        /// Fires when Window is Loaded
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitializeHDFace();
        }

        /// <summary>
        /// Initialize Kinect object
        /// </summary>
        public void InitializeHDFace()
        {
            this.CurrentBuilderStatus = "Ready To Start Capture";

            this.sensor = KinectSensor.GetDefault();
            this.bodySource = this.sensor.BodyFrameSource;
            this.bodyReader = this.bodySource.OpenReader();
            this.bodyReader.FrameArrived += this.BodyReader_FrameArrived;
            this.highDefinitionFaceFrameSource = new HighDefinitionFaceFrameSource(this.sensor);
            this.highDefinitionFaceFrameSource.TrackingIdLost += this.HdFaceSource_TrackingIdLost;

            this.highDefinitionFaceFrameReader = this.highDefinitionFaceFrameSource.OpenReader();
            this.highDefinitionFaceFrameReader.FrameArrived += this.HdFaceReader_FrameArrived;

            this.currentFaceModel = new FaceModel();
            this.currentFaceAlignment = new FaceAlignment();

            this.InitializeMesh();
            this.UpdateMesh();

            this.sensor.Open();
        }

        /// <summary>
        /// Initializes a 3D mesh to deform every frame
        /// </summary>
        private void InitializeMesh()
        {
            var vertices = this.currentFaceModel.CalculateVerticesForAlignment(this.currentFaceAlignment);

            var triangleIndices = this.currentFaceModel.TriangleIndices;

            var indices = new Int32Collection(triangleIndices.Count);

            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                uint index01 = triangleIndices[i];
                uint index02 = triangleIndices[i + 1];
                uint index03 = triangleIndices[i + 2];

                indices.Add((int)index03);
                indices.Add((int)index02);
                indices.Add((int)index01);
            }

            //====================================================================================
            // SJohnston, RCoto 20150217
            // Commented out all the lines that refer to the object theGeometry. This is to keep
            // the program from drawing the face onto the screen, thereby reducing processor
            // load by 10-12%.
            // We added comments to the lines below AND to line 389 in function UpdateMesh()!
            //=====================================================================================

            //this.theGeometry.TriangleIndices = indices;
            //this.theGeometry.Normals = null;
            //this.theGeometry.Positions = new Point3DCollection();
            //this.theGeometry.TextureCoordinates = new PointCollection();

            //foreach (var vert in vertices)
            //{
            //    this.theGeometry.Positions.Add(new Point3D(vert.X, vert.Y, -vert.Z));
            //    this.theGeometry.TextureCoordinates.Add(new Point());
            //}

        }


        /// <summary>
        /// Sends the new deformed mesh to be drawn
        /// </summary>
        private void UpdateMesh()
        {
            var vertices = this.currentFaceModel.CalculateVerticesForAlignment(this.currentFaceAlignment);


            for (int i = 0; i < vertices.Count; i++)
            {
                var vert = vertices[i];
                //this.theGeometry.Positions[i] = new Point3D(vert.X, vert.Y, -vert.Z);
            }



            //double w = this.currentFaceAlignment.FaceOrientation.W;
            //double x = this.currentFaceAlignment.FaceOrientation.X;
            //double y = this.currentFaceAlignment.FaceOrientation.Y;
            //double z = this.currentFaceAlignment.FaceOrientation.Z;

            //double w2 = Math.Pow(w, 2);
            //double x2 = Math.Pow(x, 2);
            //double y2 = Math.Pow(y, 2);
            //double z2 = Math.Pow(z, 2);

            ////CHRobot (rolando cited)
            ////double roll = Math.Atan2(2*(w*x + y*z), (w2-x2-y2+z2));
            ////double pitch = -1 * (Math.Asin(2*(x*z - w*y)));
            ////double yaw = Math.Atan2(2*(w*z + x*y), (w2+x2-y2-z2));

            ////euclideanspace.com quat to euler
            //double yaw = Math.Atan2(2 * (y * w) - 2 * (x * z), 1 - (2 * (y2)) - (2 * (z2)));
            //double roll = Math.Asin(2 * (x * y) + 2 * (z * w));
            //double pitch = Math.Atan2(2 * (x * w) - 2 * (y * z), 1 - (2 * (x2)) - (2 * (z2)));

            //yaw = Math.Round((yaw * (180 / Math.PI)) * -1, 1);
            //pitch = Math.Round(pitch * (180 / Math.PI), 1);
            //roll = Math.Round((roll * (180 / Math.PI)) * -1, 1);

            //Attempt to back-calculate the "angle" variable which 'looks' like it could be used to calc p/y/r
            //double angle = Math.Acos(w) * 2;
            //double n_x = x / Math.Sin(0.5 * angle);
            //double n_z = z / Math.Sin(0.5 * angle);
            //double n_y = y / Math.Sin(0.5 * angle);

            //var coords = this.GetCoords();

            //double pitch = coords.Item1;
            //double yaw = coords.Item2;
            //double roll = coords.Item3;

            //date1 = DateTime.Now;

            //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Rotation:    (Pitch: " + pitch.ToString();
            //coordinates += "°, Yaw: " + yaw.ToString();
            //coordinates += "°, Roll: " + roll.ToString();
            //coordinates += ")\r\n";
            //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Translation: (Zero point in X-Axis: " + this.currentFaceAlignment.HeadPivotPoint.X.ToString() + " mm, Zero-point in Y-Axis: " + this.currentFaceAlignment.HeadPivotPoint.Y.ToString() + " mm, Distance from Kinect: " + this.currentFaceAlignment.HeadPivotPoint.Z.ToString() + " mm)" + "\r\n";

            //check to see if the reference coordinates have been obtained yet

            //if (this.refCoordButton.IsEnabled == false && this.writeCoordsButton.IsEnabled == true
            
            if (this.btnGetRefCoords.Enabled == false && this.CheckEnable(btnStop) == true )
            {

                //Obtain Coordinates
                var coords = this.GetCoords();

                double pitch = coords.Item1;
                double yaw = coords.Item2;
                double roll = coords.Item3;
                float x = coords.Item4;
                float y = coords.Item5;
                float z = coords.Item6;


                //date1 = DateTime.Now;

                //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Rotation:    (Pitch: " + pitch.ToString();
                //coordinates += "°, Yaw: " + yaw.ToString();
                //coordinates += "°, Roll: " + roll.ToString();
                //coordinates += ")\r\n";
                //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Translation: (Zero point in X-Axis: " + this.currentFaceAlignment.HeadPivotPoint.X.ToString() + " mm, Zero-point in Y-Axis: " + this.currentFaceAlignment.HeadPivotPoint.Y.ToString() + " mm, Distance from Kinect: " + this.currentFaceAlignment.HeadPivotPoint.Z.ToString() + " mm)" + "\r\n";

                if (this.CheckEnable(this.btnStart) == false && this.CheckEnable(this.btnStop) == true )
                {

                    q.Enqueue(coords);
                }
                
                //if (i == 2000){ 
                //    //q.Dequeue();
                //    string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //    System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);
                //    coordinates = "";
                //    i = 0;
                //}
                
                //alert user if pitch is out of alignment
                if ((pitch - refCoords.Item1) > 3 || (pitch - refCoords.Item1) < -3)
                {
                    this.ChangeColor(pitchStatus, "red");
                    //this.pitchBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");
                }
                else
                {
                    this.ChangeColor(pitchStatus, "green");
                    //this.pitchBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
                }

                //alert user if yaw is out of alignment
                if ((yaw - refCoords.Item2) > 3 || (yaw - refCoords.Item2) < -3)
                {
                    this.ChangeColor(yawStatus, "red");
                    //this.yawBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");
                }
                else
                {
                    this.ChangeColor(yawStatus, "green");
                    //this.yawBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
                }

                //alert user if roll is out of alignment
                if ((roll - refCoords.Item3) > 3 || (roll - refCoords.Item3) < -3)
                {
                    this.ChangeColor(rollStatus, "red");
                    //this.rollBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");
                }
                else
                {
                    this.ChangeColor(rollStatus, "green");
                    //this.rollBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
                }
                //alert user if X is out of alignment
                if ((x - refCoords.Item4) > 0.004 || (x - refCoords.Item4) < -0.004)
                {
                    this.ChangeColor(xStatus, "red");
                }
                else
                {
                    this.ChangeColor(xStatus, "green");
                }
                //alert user if Y is out of alignment
                if ((y - refCoords.Item5) > 0.004 || (y - refCoords.Item5) < -0.004)
                {
                    this.ChangeColor(yStatus, "red");
                }
                else
                {
                    this.ChangeColor(yStatus, "green");
                }
                //alert user if Z is out of alignment
                if ((z - refCoords.Item6) > 0.004 || (z - refCoords.Item6) < -0.004)
                {
                    this.ChangeColor(zStatus, "red");
                }
                else
                {
                    this.ChangeColor(zStatus, "green");
                }
            }

            if (this.CurrentTrackingId != 0 || this.PastTrackingId != 0)
            {
                //this.PastTrackingId = this.CurrentTrackingId;
                //Console.WriteLine("Frame has arrived!");
                this.ChangeText(trackingStatus, "TRACKING");
                this.ChangeColor(trackingStatus, "green");
                //this.trackLabel.Text = "TRACKING";
                //this.trackLabel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
            }
            //else if (this.CurrentTrackingId == 0 && this.PastTrackingId == 0)
            //{
            //    this.PastTrackingId = this.CurrentTrackingId;
            //    this.trackLabel.Text = "NOT TRACKING";
            //    this.trackLabel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707"); //Red
            //}

            //string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);
        }

        /// <summary>
        /// Get a set of Coordinate values
        /// </summary>
        private Tuple<double,double,double,float,float,float,string> GetCoords() 
        {
            double w = this.currentFaceAlignment.FaceOrientation.W;
            double x = this.currentFaceAlignment.FaceOrientation.X;
            double y = this.currentFaceAlignment.FaceOrientation.Y;
            double z = this.currentFaceAlignment.FaceOrientation.Z;
            float transX = this.currentFaceAlignment.HeadPivotPoint.X;
            float transY = this.currentFaceAlignment.HeadPivotPoint.Y; 
            float transZ = this.currentFaceAlignment.HeadPivotPoint.Z;
            

            double w2 = Math.Pow(w, 2);
            double x2 = Math.Pow(x, 2);
            double y2 = Math.Pow(y, 2);
            double z2 = Math.Pow(z, 2);

            //euclideanspace.com quat to euler
            double yaw = Math.Atan2(2 * (y * w) - 2 * (x * z), 1 - (2 * (y2)) - (2 * (z2)));
            double roll = Math.Asin(2 * (x * y) + 2 * (z * w));
            double pitch = Math.Atan2(2 * (x * w) - 2 * (y * z), 1 - (2 * (x2)) - (2 * (z2)));

            yaw = Math.Round((yaw * (180 / Math.PI)) * -1, 1);
            pitch = Math.Round(pitch * (180 / Math.PI), 1);
            roll = Math.Round((roll * (180 / Math.PI)) * -1, 1);
            date1 = DateTime.Now;
            return Tuple.Create(pitch, yaw, roll, transX, transY, transZ, date1.ToString("yyyyyyyyMMddHHmmssfff"));
        }

        /// <summary>
        /// Start a face capture on clicking the button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void StartCapture_Button_Click(object sender, RoutedEventArgs e)
        {
            this.StartCapture();
        }

        /// <summary>
        /// Capture neutral coordinates
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void RefCoordCapture_Button_Click(object sender, RoutedEventArgs e)
        {
            this.RefCoordCapture();
            this.refCoordButton.IsEnabled = false;
        }

        /// <summary>
        /// Write the coordinates to a file
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WriteCoords_Button_Click(object sender, RoutedEventArgs e)
        {
            this.WriteCoords();
            //writeCoordsButton.IsEnabled = false;
            //this.refCoordButton.IsEnabled = false;
        }

        /// <summary>
        /// Disposes this instance and clears the native resources allocated
        /// </summary>
        public void Dispose()
        {
            //string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);

            //this.InternalDispose();
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// This event fires when a BodyFrame is ready for consumption
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            this.CheckOnBuilderStatus();

            var frameReference = e.FrameReference;
            using (var frame = frameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    // We might miss the chance to acquire the frame, it will be null if it's missed
                    return;
                }

                if (this.currentTrackedBody != null)
                {
                    this.currentTrackedBody = FindBodyWithTrackingId(frame, this.CurrentTrackingId);

                    if (this.currentTrackedBody != null)
                    {
                        return;
                    }
                }

                Body selectedBody = FindClosestBody(frame);

                if (selectedBody == null)
                {
                    return;
                }

                this.currentTrackedBody = selectedBody;
                this.CurrentTrackingId = selectedBody.TrackingId;

                this.highDefinitionFaceFrameSource.TrackingId = this.CurrentTrackingId;
            }
        }

        /// <summary>
        /// This event is fired when a tracking is lost for a body tracked by HDFace Tracker
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void HdFaceSource_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            var lostTrackingID = e.TrackingId;

            if (this.CurrentTrackingId == lostTrackingID)
            {
                this.CurrentTrackingId = 0;
                this.currentTrackedBody = null;
                if (this.faceModelBuilder != null)
                {
                    this.faceModelBuilder.Dispose();
                    this.faceModelBuilder = null;
                }

                this.highDefinitionFaceFrameSource.TrackingId = 0;
            }
        }


        /// <summary>
        /// This event is fired when a new HDFace frame is ready for consumption
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void HdFaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                // We might miss the chance to acquire the frame; it will be null if it's missed.
                // Also ignore this frame if face tracking failed.
                if (frame == null || !frame.IsFaceTracked)
                {

                    this.ChangeText(this.trackingStatus, "NOT TRACKING");
                    this.ChangeColor(this.trackingStatus, "red");

                    //this.trackLabel.Text = "NOT TRACKING";
                    //this.trackLabel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707"); //Red

                    //date1 = DateTime.Now;
                    //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Rotation:    (Pitch: NaN";
                    //coordinates += ", Yaw: NaN";
                    //coordinates += ", Roll: NaN";
                    //coordinates += ")\r\n";
                    //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Translation: (Zero point in X-Axis: NaN, Zero-point in Y-Axis: NaN, Distance from Kinect: NaN)\r\n";
                    //string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    //System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);

                    return;
                }

                frame.GetAndRefreshFaceAlignmentResult(this.currentFaceAlignment);

                this.UpdateMesh();
            }
        }

        /// <summary>
        /// Obtain neutral coordinates
        /// </summary>
        public void RefCoordCapture()
        {
            //date1 = DateTime.Now;
            refCoords = GetCoords();

            double pitch = refCoords.Item1;
            double yaw = refCoords.Item2;
            double roll = refCoords.Item3;
            float x = refCoords.Item4;
            float y = refCoords.Item5;
            float z = refCoords.Item6;

            string ref_coords = pitch.ToString() + "\t" + yaw.ToString() + "\t" + roll.ToString() + "\t" + x.ToString() + "\t" + y.ToString() + "\t" + z.ToString();

            string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            System.IO.File.WriteAllText(dtopfolder + @"\ref_coords.txt", ref_coords);

            this.ChangeTextColor(btnGetRefCoords, "gray");
            this.SetEnable(btnGetRefCoords, "disable");
        }

        /// <summary>
        /// Start a face capture operation
        /// </summary>
        public void StartCapture()
        {
            this.ChangeTextColor(btnStartCapture, "gray");
            this.SetEnable(btnStartCapture, "disable");

            this.StopFaceCapture();

            this.faceModelBuilder = null;

            this.faceModelBuilder = this.highDefinitionFaceFrameSource.OpenModelBuilder(FaceModelBuilderAttributes.None);

            this.faceModelBuilder.BeginFaceDataCollection();

            this.faceModelBuilder.CollectionCompleted += this.HdFaceBuilder_CollectionCompleted;
            //this.forwardNeeded.Opacity = 25;
            //this.upNeeded.Opacity = 25;
            //this.leftNeeded.Opacity = 25;
            //this.rightNeeded.Opacity = 25;
        }

        /// <summary>
        /// Write the coordinates collected to a file
        /// </summary>
        public void WriteCoords()
        {
            System.Windows.Forms.MessageBox.Show("Recording Stopped\nWill begin writing Coords file\nPlease click 'OK' and wait for notification.");
            //if (writeCoordsButton.IsEnabled == true)
            //{
            string coordstring;
            Console.WriteLine("Q's Count" + q.Count.ToString());
            while (q.Count > 0)
            {
                var coords = q.Dequeue();
                //double pitch = coords.Item1;
                //double yaw = coords.Item2;
                //double roll = coords.Item3;
                //string transX = coords.Item4;
                //string transY = coords.Item5;
                //string transZ = coords.Item6;
                //string date = coords.Item7;
                coordinates.Append("Time: " + coords.Item7 + " Rotation:    (Pitch: " + coords.Item1.ToString() +
                                "°, Yaw: " + coords.Item2.ToString() + "°, Roll: " + coords.Item3.ToString() + ")\r\n" +
                                "Time: " + coords.Item7 + " Translation: (Zero point in X-Axis: " + coords.Item4.ToString() +
                                " mm, Zero-point in Y-Axis: " + coords.Item5.ToString() + " mm, Distance from Kinect: " +
                                coords.Item6.ToString() + " mm)" + "\r\n");
            }
            //}
            coordstring = coordinates.ToString();
            string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordstring );
            System.Windows.Forms.MessageBox.Show("Completed writing coords file, carry on.");
            // Move KinectCoordinate file and ReferenceCoordinateFile
            string destinationDir = CheckText(subjID);
            string coords_file = "coords.txt";
            string ref_coords_file = "ref_coords.txt";

            string pattern = "[\\~#%&*{}/:<>?|\"-]";
            string replacement = "_";
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern);
            destinationDir = System.Text.RegularExpressions.Regex.Replace(regEx.Replace(destinationDir, replacement), @"\s+", "_");
            destinationDir = dtopfolder + @"\" + date_today + "_" + destinationDir;

            string coordSourceFile = System.IO.Path.Combine(dtopfolder, coords_file);
            string coordDestFile = System.IO.Path.Combine(destinationDir, coords_file);
            System.IO.File.Move(coordSourceFile,
                coordDestFile);

            string refSourceFile = System.IO.Path.Combine(dtopfolder, ref_coords_file);
            string refDestFile = System.IO.Path.Combine(destinationDir, ref_coords_file);
           System.IO.File.Move(refSourceFile,
                refDestFile);

           System.Windows.Forms.MessageBox.Show("Coords files written!");
        }

        /// <summary>
        /// Cancel the current face capture operation
        /// </summary>
        private void StopFaceCapture()
        {
            if (this.faceModelBuilder != null)
            {
                this.faceModelBuilder.Dispose();
                this.faceModelBuilder = null;
            }
        }

        /// <summary>
        /// This event fires when the face capture operation is completed
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void HdFaceBuilder_CollectionCompleted(object sender, FaceModelBuilderCollectionCompletedEventArgs e)
        {
            var modelData = e.ModelData;

            this.currentFaceModel = modelData.ProduceFaceModel();

            this.faceModelBuilder.Dispose();
            this.faceModelBuilder = null;

            //string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);

            //this.InternalDispose();
            //this.forwardNeeded.Opacity = 0;
            //this.leftNeeded.Opacity = 0;
            //this.rightNeeded.Opacity = 0;
            //this.upNeeded.Opacity = 0;
            //this.captured.Opacity = 100;
            GC.SuppressFinalize(this);

            this.CurrentBuilderStatus = "Capture Complete";
        }

        /// <summary>
        /// Check the face model builder status
        /// </summary>
        private void CheckOnBuilderStatus()
        {
            if (this.faceModelBuilder == null)
            {
                return;
            }

            string newStatus = string.Empty;

            var captureStatus = this.faceModelBuilder.CaptureStatus;
            newStatus += captureStatus.ToString();

            var collectionStatus = this.faceModelBuilder.CollectionStatus;

            newStatus += ", " + GetCollectionStatusText(collectionStatus);

            this.CurrentBuilderStatus = newStatus;
        }
    }
}