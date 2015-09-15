// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CaptureTest
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Face;

    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int i = 0;
        System.Collections.Generic.Queue<Tuple<double, double, double, string, string, string, string>> q = new System.Collections.Generic.Queue<Tuple<double, double, double, string, string, string, string>>();
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

        public string coordinates;

        //Establishes the tuple to hold the reference coordinates
        public Tuple<double, double, double, string, string, string, string> refCoords;

        private DateTime date1 = new DateTime(0);

        private Quaternion quat;

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
        /// Gets or sets the current tracked user id
        /// </summary>
        private string currentBuilderStatus = string.Empty;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        private string statusText = "Ready To Start Capture";

        //var converter = new System.Windows.Media.BrushConverter();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
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
                this.forwardNeeded.Opacity = 100;
                this.forwardNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "FrontViewFramesNeeded";
                return res;
            }
            else
            {
                this.forwardNeeded.Opacity = 50;
                this.forwardNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            }

            if ((status & FaceModelBuilderCollectionStatus.LeftViewsNeeded) != 0)
            {
                this.leftNeeded.Opacity = 100;
                this.leftNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "LeftViewsNeeded";
                return res;
            }
            else
            {
                this.leftNeeded.Opacity = 50;
                this.leftNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            }

            if ((status & FaceModelBuilderCollectionStatus.RightViewsNeeded) != 0)
            {
                
                this.rightNeeded.Opacity = 100;
                this.rightNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "RightViewsNeeded";
                return res;
            }
            else
            {
                this.rightNeeded.Opacity = 50;
                this.rightNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            }

            if ((status & FaceModelBuilderCollectionStatus.TiltedUpViewsNeeded) != 0)
            {
                
                this.upNeeded.Opacity = 100;
                this.upNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");//red
                res = "TiltedUpViewsNeeded";
                return res;
            }
            else
            {
                this.upNeeded.Opacity = 50;
                this.upNeeded.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB0B0B0");//gray
            }

            if ((status & FaceModelBuilderCollectionStatus.MoreFramesNeeded) != 0)
            {
                res = "TiltedUpViewsNeeded";
                return res;
            }

            if ((status & FaceModelBuilderCollectionStatus.Complete) != 0)
            {
                this.forwardNeeded.Opacity = 0;
                this.leftNeeded.Opacity = 0;
                this.rightNeeded.Opacity = 0;
                this.upNeeded.Opacity = 0;
                this.captured.Opacity = 100;
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
        private void InitializeHDFace()
        {
            this.CurrentBuilderStatus = "Ready To Start Capture";

            this.sensor = KinectSensor.GetDefault();
            //this.quat = 
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
            if (this.refCoordButton.IsEnabled == false)
            {
                //Obtain Coordinates
                var coords = this.GetCoords();

                double pitch = coords.Item1;
                double yaw = coords.Item2;
                double roll = coords.Item3;

                //date1 = DateTime.Now;

                //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Rotation:    (Pitch: " + pitch.ToString();
                //coordinates += "°, Yaw: " + yaw.ToString();
                //coordinates += "°, Roll: " + roll.ToString();
                //coordinates += ")\r\n";
                //coordinates += "Time: " + date1.ToString("yyyyyyyyMMddHHmmssfff") + " Translation: (Zero point in X-Axis: " + this.currentFaceAlignment.HeadPivotPoint.X.ToString() + " mm, Zero-point in Y-Axis: " + this.currentFaceAlignment.HeadPivotPoint.Y.ToString() + " mm, Distance from Kinect: " + this.currentFaceAlignment.HeadPivotPoint.Z.ToString() + " mm)" + "\r\n";

                //i += 1;
                q.Enqueue(coords);
                //if (i == 2000){ 
                //    //q.Dequeue();
                //    string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //    System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);
                //    coordinates = "";
                //    i = 0;
                //}
                
                //alert user if pitch is out of alignment
                if ((pitch - refCoords.Item1) > 10 || (pitch - refCoords.Item1) < -10)
                {
                    this.pitchBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");
                }
                else
                {
                    this.pitchBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
                }

                //alert user if yaw is out of alignment
                if ((yaw - refCoords.Item2) > 10 || (yaw - refCoords.Item2) < -10)
                {
                    this.yawBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");
                }
                else
                {
                    this.yawBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
                }

                //alert user if roll is out of alignment
                if ((roll - refCoords.Item3) > 10 || (roll - refCoords.Item3) < -10)
                {
                    this.rollBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707");
                }
                else
                {
                    this.rollBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
                }
            }

            if (this.CurrentTrackingId != 0)
            {
                this.trackLabel.Text = "TRACKING";
                this.trackLabel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF0AA60A");
            }

            //string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);
        }

        /// <summary>
        /// Get a set of Coordinate values
        /// </summary>
        private Tuple<double,double,double,string,string,string,string> GetCoords() 
        {
            double w = this.currentFaceAlignment.FaceOrientation.W;
            double x = this.currentFaceAlignment.FaceOrientation.X;
            double y = this.currentFaceAlignment.FaceOrientation.Y;
            double z = this.currentFaceAlignment.FaceOrientation.Z;
            string transX = this.currentFaceAlignment.HeadPivotPoint.X.ToString();
            string transY = this.currentFaceAlignment.HeadPivotPoint.Y.ToString(); 
            string transZ = this.currentFaceAlignment.HeadPivotPoint.Z.ToString();
            

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
        private void StartCapture_Button_Click(object sender, RoutedEventArgs e)
        {
            this.StartCapture();
        }

        /// <summary>
        /// Capture neutral coordinates
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void RefCoordCapture_Button_Click(object sender, RoutedEventArgs e)
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
            //this.refCoordButton.IsEnabled = false;
        }

        /// <summary>
        /// Disposes this instance and clears the native resources allocated
        /// </summary>
        public void Dispose()
        {
            string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);

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
                    this.trackLabel.Text = "NOT TRACKING";
                    this.trackLabel.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFF90707"); //Red

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
        private void RefCoordCapture()
        {
            //date1 = DateTime.Now;
            refCoords = GetCoords();

            double pitch = refCoords.Item1;
            double yaw = refCoords.Item2;
            double roll = refCoords.Item3;

            string ref_coords = pitch.ToString() + "\t" + yaw.ToString() + "\t" + roll.ToString();

            string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            System.IO.File.WriteAllText(dtopfolder + @"\ref_coords.txt", ref_coords);
        }

        /// <summary>
        /// Start a face capture operation
        /// </summary>
        private void StartCapture()
        {
            this.StopFaceCapture();

            this.faceModelBuilder = null;

            this.faceModelBuilder = this.highDefinitionFaceFrameSource.OpenModelBuilder(FaceModelBuilderAttributes.None);

            this.faceModelBuilder.BeginFaceDataCollection();

            this.faceModelBuilder.CollectionCompleted += this.HdFaceBuilder_CollectionCompleted;
            this.forwardNeeded.Opacity = 25;
            this.upNeeded.Opacity = 25;
            this.leftNeeded.Opacity = 25;
            this.rightNeeded.Opacity = 25;
        }

        /// <summary>
        /// Write the coordinates collected to a file
        /// </summary>
        private void WriteCoords()
        {
            //System.Linq.Enumerable.Range(0,q.Count);
            while (q.Count > 0){
                var coords = q.Dequeue();
                //double pitch = coords.Item1;
                //double yaw = coords.Item2;
                //double roll = coords.Item3;
                //string transX = coords.Item4;
                //string transY = coords.Item5;
                //string transZ = coords.Item6;
                //string date = coords.Item7;
                coordinates += "Time: " + coords.Item7 + " Rotation:    (Pitch: " + coords.Item1.ToString() +
                                "°, Yaw: " + coords.Item2.ToString() + "°, Roll: " + coords.Item3.ToString() + ")\r\n" +
                                "Time: " + coords.Item7 + " Translation: (Zero point in X-Axis: " + coords.Item4 +
                                " mm, Zero-point in Y-Axis: " + coords.Item5 + " mm, Distance from Kinect: " +
                                coords.Item6 + " mm)" + "\r\n";
            }
            string dtopfolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            System.IO.File.WriteAllText(dtopfolder + @"\coords.txt", coordinates);
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
            this.forwardNeeded.Opacity = 0;
            this.leftNeeded.Opacity = 0;
            this.rightNeeded.Opacity = 0;
            this.upNeeded.Opacity = 0;
            this.captured.Opacity = 100;
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