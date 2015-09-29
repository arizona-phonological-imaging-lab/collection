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
            for (int i = 0; i <= folderNames.Count; i++)
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
                
                // if the file format is MTS (as it is for many camcorders), convert to .avi first
                if (".MTS" == fileName.Substring(fileName.Length-4))
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
                        frameFolder = path + fileName+"_frames";
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
