using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Drawing;

namespace ePool
{

    class CropBox
    {
        public int TLX { get; set; }
        public int TLY { get; set; }
        public int BRX { get; set; }
        public int BRY { get; set; }

        public CropBox()
        {
            this.TLX = -1;
            this.TLY = -1;
            this.BRX = -1;
            this.BRY = -1;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int BALL_WIDTH = 300;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Crop box object to be used by UI
        /// </summary>
        private CropBox cropBox = new CropBox();

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the depth data received from the camera
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Two-dimenetional representation of the depth data
        /// </summary>
        private short[,] depthArray2D;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private byte[] colorPixels;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(WindowLoaded);
            this.Closing += WindowClosing;
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the depth stream to receive depth frames
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                // Allocate space to put the depth pixels we'll receive
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                // Allocate space to put the color pixels we'll create
                this.colorPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new depth frame data
                this.sensor.DepthFrameReady += this.SensorDepthFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = "No Kinect ready.";
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }


        /// <summary>
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    // Get the min and max reliable depth for the current frame
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    // Convert the depth to RGB and fill 1D array
                    int colorPixelIndex = 0;
                    short[] depthArray1D = new short[this.depthPixels.Length];
                    for (int i = this.depthPixels.Length - 1; i >= 0; --i)
                    {
                        // Get the depth for this pixel
                        short depth = depthPixels[i].Depth;
                        depthArray1D[i] = depth;

                        // To convert to a byte, we're discarding the most-significant
                        // rather than least-significant bits.
                        // We're preserving detail, although the intensity will "wrap."
                        // Values outside the reliable depth range are mapped to 0 (black).

                        // Note: Using conditionals in this loop could degrade performance.
                        // Consider using a lookup table instead when writing production code.
                        // See the KinectDepthViewer class used by the KinectExplorer sample
                        // for a lookup table example.
                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                        // Write out blue byte
                        this.colorPixels[colorPixelIndex++] = intensity;

                        // Write out green byte
                        this.colorPixels[colorPixelIndex++] = intensity;

                        // Write out red byte                        
                        this.colorPixels[colorPixelIndex++] = intensity;

                        // We're outputting BGR, the last byte in the 32 bits is unused so skip it
                        // If we were outputting BGRA, we would write alpha here.
                        ++colorPixelIndex;
                    }

                    // Convert and assign 2D array
                    this.depthArray2D = make2DArray(depthArray1D, this.colorBitmap.PixelHeight, this.colorBitmap.PixelWidth);


                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);


                    // If crop box is valid, draw crop box
                    if (this.cropBox.TLX != -1)
                    {
                        this.colorBitmap.DrawRectangle(this.cropBox.TLX, this.cropBox.TLY, this.cropBox.BRX, this.cropBox.BRY, System.Windows.Media.Color.FromRgb(255, 0, 0));
                    }

                }
            }

        }

        private void saveDepthData()
        {
            if(this.cropBox.TLX == -1)
            {
                this.statusBarText.Text = "You must fisrt specify the bounds of the crop box.";
                return;
            }

            short[,] ballVector = new short[(Int16)BALL_WIDTH,(Int16)BALL_WIDTH];

            //initialize a StreamWriter
            StreamWriter sw = new StreamWriter(@"C:/data.txt");

            Console.WriteLine("Width of image: " + this.colorBitmap.PixelWidth.ToString());
            Console.WriteLine("Height of image: " + this.colorBitmap.PixelHeight.ToString());
            Console.WriteLine("Width of 2d arr: " + this.depthArray2D.GetUpperBound(1).ToString());
            Console.WriteLine("Height of 2d arr: " + this.depthArray2D.GetUpperBound(0).ToString());

            int ballVecI = 0;
            int ballVecJ = 0;
            short minVal = 9999;
            short maxVal = 0;
            for (int i = this.cropBox.TLX; i < this.cropBox.BRX; ++i)
            {
                for (int j = this.cropBox.TLY; j < this.cropBox.BRY; ++j)
                {
                    short val = this.depthArray2D[i, j];
                    ballVector[ballVecI, ballVecJ] = val;
                    ballVecJ++;
                    if (val != 0)
                    {
                        if (val < minVal)
                            minVal = val;
                    }
                }
                ballVecI++;
                ballVecJ = 0;
            }

            // Normalize
            for (int i = 0; i < ballVector.GetUpperBound(1); i++)
            {
                for (int j = 0; j < ballVector.GetUpperBound(0); j++)
                {
                    short val = ballVector[i, j];
                    var newVal = (Int16)(val - minVal);
                    ballVector[i, j] = newVal;
                    if (newVal > maxVal)
                        maxVal = newVal;
                }
            }

            string output = "";
            for (int i = 0; i < ballVector.GetUpperBound(1); i++)
            {
                for (int j = 0; j < ballVector.GetUpperBound(0); j++)
                {
                    output += ballVector[i, j].ToString() + "\t";
                }
                sw.WriteLine(output);
                output = "";
            }

            //dispose of sw
            sw.Close();

            // Create a Bitmap object from a file.
            Bitmap myBitmap = new Bitmap(ballVector.GetUpperBound(0), ballVector.GetUpperBound(1));

            // Set each pixel in myBitmap to black.
            for (int Xcount = 0; Xcount < myBitmap.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height; Ycount++)
                {
                    double c1 = 255 * (double) ballVector[Ycount, Xcount] / (double) maxVal;
                    int color = (int) Math.Round(c1);
                    if (color < 0)
                        color = 0;
                    myBitmap.SetPixel(Xcount, Ycount, System.Drawing.Color.FromArgb(color, color, color));
                }
            }

            myBitmap.Save("C:/img.png");

 

            this.statusBarText.Text = "Depth data saved.";
        }

        private void searchFrame()
        {


        }

        private void button_saveDepthData_Click(object sender, RoutedEventArgs e)
        {
            saveDepthData();
        }

        private static T[,] make2DArray<T>(T[] input, int height, int width)
        {
            T[,] output = new T[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    output[i, j] = input[i * width + j];
                }
            }
            return output;
        }

        private void textBox_TLX_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkDepthDataValues();
        }

        private void textBox_TLY_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkDepthDataValues();
        }

        private void textBox_BRX_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkDepthDataValues();
        }

        private void textBox_BRY_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkDepthDataValues();
        }

        private void checkDepthDataValues()
        {
            try
            {
                // Verify values
                int tlx = int.Parse(textBox_TLX.Text);
                int tly = int.Parse(textBox_TLY.Text);
                int brx = tlx + BALL_WIDTH;
                int bry = tly + BALL_WIDTH;
                textBox_BRX.Text = brx.ToString();
                textBox_BRY.Text = bry.ToString();
                if (tlx < 0 || tlx > this.colorBitmap.PixelWidth)
                    throw new Exception();
                if (tly < 0 || tly > this.colorBitmap.PixelHeight)
                    throw new Exception();
                if (brx < 0 || brx > this.colorBitmap.PixelWidth)
                    throw new Exception();
                if (bry < 0 || bry > this.colorBitmap.PixelHeight)
                    throw new Exception();
                // Update depth tool box object
                this.cropBox.TLX = tlx;
                this.cropBox.TLY = tly;
                this.cropBox.BRX = brx;
                this.cropBox.BRY = bry;
                this.statusBarText.Text = "Depth data tool updated!";
            }
            catch
            {
                this.cropBox = new CropBox();
                this.statusBarText.Text = "Invalid corrdinates given for depth data tool.";
            }

            

        }
    }

}
