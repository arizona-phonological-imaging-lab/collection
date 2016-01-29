using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace VideoPostProcessor
{
    
    public partial class Form1 : Form
    {
        private string dtopfolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private new List<string> files = new List<string>();
        private new List<string> fullPaths = new List<string>();
        private new List<string> fileNames = new List<string>();
        private new List<string> paths = new List<string>();
        private new List<string> frameFolders = new List<string>();
        private new List<string> folderNames = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            label1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = dtopfolder;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                //string file[];
                //string fullPath[];
                //string fileName[];
                //string path[];
                //string folderName[];
                string file = openFileDialog1.FileName;
                string fullPath = openFileDialog1.FileName;
                Console.WriteLine(fullPath);
                string fileName = openFileDialog1.SafeFileName;
                string path = fullPath.Replace(fileName, "");
                string folderName = Path.GetFileName(Path.GetDirectoryName(path));
                //files.Add(file);
                fullPaths.Add(fullPath);
                fileNames.Add(fileName);
                paths.Add(path);
                folderNames.Add(folderName);
                
                Console.WriteLine(Directory.GetCurrentDirectory());
                //MessageBox.Show("File: " + file + "\nFullPath: " + fullPath + "\nFolder: " + folderName + "\nFilename: " + fileName + "\nPath: " + path + "\nDesktop Folder: " + dtopfolder);
                button2.Enabled = true;
                label1.Text = label1.Text + @"\" + folderName + @"\" + fileName + '\n';
                label1.Enabled = true;
                //label1.Text = label1.Text + (fullPath + "\n");
                label2.Visible = true;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            postProcess();
        }

        private void postProcess()
        {
            // FFMPEG video post processing starts here
            for (int i = 0; i < folderNames.Count; i++)
            {
                //string file = files[i];
                string fullPath = fullPaths[i];
                string fileName = fileNames[i];
                string path = paths[i];
                string folderName = folderNames[i];
                string frameFolder;
                label2.Text = "Processing Video file at: " + @"\" + folderName + @"\" + fileName + '\n';
                //int idx = fileName.Length - 4;
                //Console.WriteLine(fileName.Substring(fileName.Length-4));
                //Console.WriteLine(fileName.Length.ToString());
                //intermeshVidKinect(fullPath, @"\" + folderName + @"\coords.txt", folderName + @"\ref_coords.txt", folderName + @"\alignedCoords.txt", folderName + @"\frames\");

                // if the file format is MTS (as it is for many camcorders), convert to .avi first
                if (".MTS" == fileName.Substring(fileName.Length - 4))
                {
                    Process conv_process = new System.Diagnostics.Process();
                    ProcessStartInfo conv_startInfo = new ProcessStartInfo();

                    conv_startInfo.FileName = @"" + Directory.GetCurrentDirectory() + @"\..\..\..\..\ffmpeg\bin\ffmpeg.exe";
                    string conv_args = "ffmpeg -i " + fileName + "-deinterlace -qscale 0 " + fileName.Substring(fileName.Length - 4) + ".avi";
                    conv_startInfo.Arguments = conv_args;

                    conv_process.StartInfo = conv_startInfo;
                    conv_process.Start();

                    conv_process.WaitForExit();

                    conv_process.Close();
                    conv_process.Dispose();
                }

                Process process = new System.Diagnostics.Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();

                // keeps FFMPEG window hidden unless the menu item is checked
                if (showFFMPEGToolStripMenuItem.Checked == false)
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }




                //startInfo.FileName = @"C:\Users\apiladmin\Documents\Github\data-collection\VideoPostProcessor\VideoPostProcessor\..\..\ffmpeg\bin\ffmpeg.exe";
                startInfo.FileName = @"" + Directory.GetCurrentDirectory() + @"\..\..\..\..\ffmpeg\bin\ffmpeg.exe";
                Console.WriteLine(Directory.GetCurrentDirectory());
                // start arguments string
                string args = "-i " + fullPath;

                if (jPEGToolStripMenuItem.Checked == true)
                {
                    // create output folder for JPG frames
                    if (fileName == "video.avi")
                    {
                        frameFolder = path + "frames";
                    }
                    else
                    {
                        frameFolder = path + fileName + "_frames";
                    }
                    if (!(System.IO.Directory.Exists(frameFolder)))
                    {
                        System.IO.Directory.CreateDirectory(frameFolder);
                        label2.Text = @"Exporting JPG frames to " + frameFolder + ".";
                    }
                    // if folder exists, check for frames
                    else if (Directory.GetFiles(frameFolder).Length != 0)
                    {
                        label2.Text = @"Error! JPG frames already exist in " + frameFolder + " folder.";
                        return;
                    }
                    //use FFMPEG JPEG command here
                    args += " -r 30000/1001 -q:v 0 -f image2 " + frameFolder + "/" + "frame-%07d.jpg";
                }
                else if (pNGToolStripMenuItem.Checked == true)
                {
                    // create output folder for PNG frames
                    // create output folder for JPG frames
                    if (fileName == "video.avi")
                    {
                        frameFolder = path + "frames";
                    }
                    else
                    {
                        frameFolder = path + fileName + "_frames";
                    }
                    if (!(System.IO.Directory.Exists(frameFolder)))
                    {
                        System.IO.Directory.CreateDirectory(frameFolder);
                        label2.Text = @"Exporting PNG frames to " + frameFolder + " folder.";
                    }
                    // if folder exists, check for frames
                    else if (Directory.GetFiles(frameFolder).Length != 0)
                    {
                        label2.Text = @"Error! PNG frames already exist in " + frameFolder + " folder.";
                        return;
                    }
                    // use FFMPEG PNG command
                    args += " -r 30000/1001 -q:v 0 -f image2 " + frameFolder + "/" + "frame-%07d.png";
                }
                if (wAVToolStripMenuItem.Checked == true)
                {
                    string audioFile;
                    // check for audio file
                    if (fileName == "video.avi")
                    {
                        audioFile = path + folderName + ".wav";
                    }
                    else
                    {
                        audioFile = path + fileName + ".wav";
                    }

                    Console.WriteLine(audioFile);
                    if (System.IO.File.Exists(audioFile))
                    {
                        label2.Text = "Error! Audio file already exists.";
                        return;
                    }

                    // use FFMPEG WAV command
                    // added '-map_channel 0.1.0 -map_channel -1' on 9/15/2013
                    // outputs left audio channel only
                    args += " -acodec pcm_s16le -map_channel 0.1.0 -map_channel -1 -ac 1 " + audioFile;
                    label2.Text += "\nExporting WAV file to " + @"\" + folderName + @"\ folder.";
                }

                startInfo.Arguments = args;

                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();

                // Let user know it is finished
                label2.Text = "Processing Complete!";

                process.Close();
                process.Dispose();
            }
        }

        //private void intermeshVidKinect(string vidFileName, string kinectFileName, string kinectRefCoords, string outputFileName, string framesFolder)
        //{

        //    string[] vidText = System.IO.File.ReadAllLines(@vidFileName);
        //    string[] kinectText = System.IO.File.ReadAllLines(@kinectFileName);
        //    string[] kinectRefText = System.IO.File.ReadAllLines(@kinectRefCoords);
        //    //System.IO.StreamReader kinectRefText = new System.IO.StreamReader(kinectRefCoords);
        //    int inputFrames = System.IO.Directory.GetFiles(framesFolder, "frame*.*").Length;

        //    string[] temp;

        //    int vidTextLength = vidText.Length;
        //    int kinectTextLength = kinectText.Length;

        //    double[] vidTimes = new double[vidTextLength];
        //    double[] kinectTimes = new double[kinectTextLength];
        //    int kinectRefPitch;
        //    int kinectRefYaw;
        //    int kinectRefRoll;
        //    string[] kinectPitch = new string[kinectTextLength];
        //    string[] kinectYaw = new string[kinectTextLength];
        //    string[] kinectRoll = new string[kinectTextLength];
        //    string[] kinectDistanceX = new string[kinectTextLength];
        //    string[] kinectDistanceY = new string[kinectTextLength];
        //    string[] kinectDistanceZ = new string[kinectTextLength];

        //    string output = "";
        //    output += "Frame Number\tFrameTime\tKinectTime\tPitch\tYaw\tRoll\tTranslation on X\tTranslation on Y\tTranslation on Z\r\n";

        //    //====================================================================
        //    // Extract the start and end times of the video from the vidTimes file
        //    //====================================================================

        //    for (int i = 0; i < vidText.Length; i++)
        //    {
        //        temp = vidText[i].Split(':');
        //        temp[1] = temp[1].TrimStart();
        //        vidTimes[i] = Convert.ToDouble(temp[1]);
        //    }

        //    //====================================================================
        //    // Extract the reference pitch, yaw, and roll information from ref_coords
        //    //====================================================================

        //    var refs = kinectRefText[0].Split('\t');
        //    //kinectRefPitch = refs[0];
        //    //kinectRefYaw = refs[1];
        //    //kinectRefRoll = refs[2];
        //    int.TryParse(refs[0],out kinectRefPitch);
        //    int.TryParse(refs[1], out kinectRefYaw);
        //    int.TryParse(refs[2], out kinectRefRoll);

        //    //====================================================================
        //    // Extract the pitch, yaw and roll information from the Kinect file
        //    //====================================================================

        //    char[] kinectDividers = new char[] { ':', ' ', '°' };

        //    for (int i = 0; i < kinectText.Length / 2; i++)
        //    {

        //        temp = kinectText[(2 * i)].Split(kinectDividers);
        //        temp[2] = temp[2].TrimStart();

        //        kinectTimes[i] = Convert.ToDouble(temp[2]);
        //        int tmpPitch;
        //        int tmpYaw;
        //        int tmpRoll;
        //        kinectRoll[i] = temp[18];
        //        kinectRoll[i] = kinectRoll[i].Substring(0, kinectRoll[i].Length - 1); // Remove final parenthesis
        //        int.TryParse(temp[10], out tmpPitch);
        //        int.TryParse(temp[14], out tmpYaw);
        //        int.TryParse(kinectRoll[i], out tmpRoll);
        //        kinectPitch[i] = (tmpPitch - kinectRefPitch).ToString();
        //        kinectYaw[i] = (tmpYaw - kinectRefYaw).ToString();
        //        kinectRoll[i] = (tmpRoll - kinectRefRoll).ToString();
        //        //kinectRoll[i] = kinectRoll[i].Substring(0, kinectRoll[i].Length - 1); // Remove final parenthesis
                

        //    }

        //    //========================================================================
        //    // Extract the distance from zero-point information from the Kinect file
        //    //========================================================================

        //    for (int i = 0; i < kinectText.Length / 2; i++)
        //    {
        //        temp = kinectText[(2 * i) + 1].Split(kinectDividers);
        //        kinectDistanceX[i] = temp[10];
        //        kinectDistanceY[i] = temp[16];
        //        kinectDistanceZ[i] = temp[22];
        //    }

        //    //=============================================================================
        //    // Determine the total duration of the video and calculate number of frames
        //    //=============================================================================

        //    //double framesPerSecond = 30 * 2.9175;

        //    double videoStartTime = vidTimes[1];
        //    double videoStopTime = vidTimes[2];
        //    double totalVideoTime = videoStopTime - videoStartTime;

        //    //int totalFrames = Convert.ToInt32(totalVideoTime/(framesPerSecond));

        //    //double[] frameTimes = new double[totalFrames];
        //    //double[] frames = new double[totalFrames];
        //    double[] frameTimes = new double[inputFrames];
        //    double[] frames = new double[inputFrames];

        //    //Console.WriteLine(totalVideoTime.ToString());
        //    //Console.WriteLine(totalFrames.ToString());

        //    //=============================================================================
        //    // Create a vector with the timestamp for each frame
        //    //=============================================================================

        //    double durationOfEachFrame = totalVideoTime / inputFrames;
        //    Console.WriteLine("totalVideoTime:" + totalVideoTime.ToString() + ", inputFrames:" + inputFrames.ToString() + ", durationOfEachFrame: " + durationOfEachFrame.ToString());

        //    for (int i = 0; i < inputFrames; i++)
        //    {
        //        frameTimes[i] = videoStartTime + Convert.ToDouble(durationOfEachFrame * i);
        //        frames[i] = i + 1;
        //    }

        //    //=============================================================================
        //    // Search for the Kinect coordinate closest in time to the timestamp of
        //    // each frame.
        //    //=============================================================================

        //    double tempTemporalDistance = 0;
        //    double bestTemporalDistance = 1000000000;
        //    int bestTemporalDistanceIndex = 0;

        //    for (int i = 0; i < frameTimes.Length; i++)
        //    {
        //        for (int j = 0; j < kinectTimes.Length; j++)
        //        {
        //            tempTemporalDistance = frameTimes[i] - kinectTimes[j];
        //            if (Math.Abs(tempTemporalDistance) < bestTemporalDistance)
        //            {
        //                bestTemporalDistance = tempTemporalDistance;
        //                bestTemporalDistanceIndex = j;
        //            }
        //        }

        //        output += frames[i] + "\t" + frameTimes[i] + "\t" + kinectTimes[bestTemporalDistanceIndex].ToString() + "\t" + kinectPitch[bestTemporalDistanceIndex] + "\t" + kinectYaw[bestTemporalDistanceIndex] + "\t" + kinectRoll[bestTemporalDistanceIndex] + "\t" + kinectDistanceX[bestTemporalDistanceIndex] + "\t" + kinectDistanceY[bestTemporalDistanceIndex] + "\t" + kinectDistanceZ[bestTemporalDistanceIndex] + "\r\n";
        //        tempTemporalDistance = 0;
        //        bestTemporalDistance = 1000000000;
        //    }


        //    /*for (int i = 0; i < 2; i++) {

        //        temp = kinectText[i].Split(kinectDividers);

        //        for (int j = 0; j < temp.Length; j++) {
        //            Console.WriteLine("j: " + j.ToString() + ", " + temp[j]);
        //        }

        //    }*/

        //    //Console.Write(output);

        //    System.IO.File.WriteAllText(@outputFileName, output);
        //    MessageBox.Show("File ready\nThe folder had " + inputFrames + " frames.");

        //}
        //private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    MessageBox.Show("One PostProcess has finished!");
        //}

        //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    System.ComponentModel.BackgroundWorker worker;
        //    worker = (System.ComponentModel.BackgroundWorker)sender;
        //}

        //private void StartThread()
        //{
            
        //    backgroundWorker1.RunWorkerAsync(postProcess);
        //}

        private void jPEGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pNGToolStripMenuItem.Checked = true;
        }

        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jPEGToolStripMenuItem.Checked = false;
        }
    }
}
