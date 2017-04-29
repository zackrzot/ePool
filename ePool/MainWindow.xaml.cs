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
        private int BALL_WIDTH = 25;

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
        private WriteableBitmap depthBitmap;

        /// <summary>
        /// Intermediate storage for the depth data received from the camera
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Two-dimenetional representation of the depth data
        /// </summary>
        private short[,] depthArray2D;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private byte[] renderedDepthPixels;

        private SpeechRecognizer mySpeechRecognizer;

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
            Logger.Init(this.richTextBox_console);

            InitializeSensor();
            InitializeDMX();

            populateDropDowns();

            LightThread.StartLights();

            // Start speech recognizer after KinectSensor started successfully.
            this.mySpeechRecognizer = SpeechRecognizer.Create();

            if (null != this.mySpeechRecognizer)
            {
                this.mySpeechRecognizer.Start(sensor.AudioSource);
                Logger.Log("VR started.");
            }
        }

        private void InitializeSensor()
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
                // Depth sensor
                // Turn on the depth stream to receive depth frames
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                // Allocate space to put the depth pixels we'll receive
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                // Allocate space to put the color pixels we'll create
                this.renderedDepthPixels = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                // This is the bitmap we'll display on-screen
                this.depthBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image.Source = this.depthBitmap;
                // Add an event handler to be called whenever there is new depth frame data
                this.sensor.DepthFrameReady += this.SensorDepthFrameReady;

                // Color sensor
                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                // Set the image we display to point to the bitmap where we'll put the image data
                this.ImageColor.Source = this.colorBitmap;
                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                // Start the sensor!
                try { this.sensor.Start(); }
                catch (IOException) { this.sensor = null; }
                Logger.Log("Sensor started.");
            }

            if (null == this.sensor)
            {
                Logger.Log("No Kinect ready.");
            }
        }

        private void InitializeDMX()
        {
            try
            {
                OpenDMX.start();                                            //find and connect to devive (first found if multiple)
                if (OpenDMX.status == FT_STATUS.FT_DEVICE_NOT_FOUND)       //update status
                    Logger.Log("No Enttec USB Device Found");
                else if (OpenDMX.status == FT_STATUS.FT_OK)
                    Logger.Log("Found DMX on USB");
                else
                    Logger.Log("Error Opening DMX Device");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
                Logger.Log("Error Connecting to Enttec USB Device");
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LightThread.StopLights();

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

                    // Load depth values to 1D array
                    short[] depthArray1D = new short[this.depthPixels.Length];
                    for (int i = 0; i < this.depthPixels.Length; ++i)
                        depthArray1D[i] = depthPixels[i].Depth;

                    // Convert the 1D data to the desired 2D array
                    this.depthArray2D = make2DArray(depthArray1D, this.depthBitmap.PixelHeight, this.depthBitmap.PixelWidth);

                    // Convert the depth to RGB
                    int colorPixelIndex = 0;
                    for (int i = 0; i <= this.depthArray2D.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j <= this.depthArray2D.GetUpperBound(1); j++)
                        {
                            short depth = this.depthArray2D[i, j];
                            byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                            this.renderedDepthPixels[colorPixelIndex++] = intensity;
                            this.renderedDepthPixels[colorPixelIndex++] = intensity;
                            this.renderedDepthPixels[colorPixelIndex++] = intensity;
                            ++colorPixelIndex;
                        }
                    }

                    // Write the pixel data into our bitmap
                    this.depthBitmap.WritePixels(
                        new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                        this.renderedDepthPixels,
                        this.depthBitmap.PixelWidth * sizeof(int),
                        0);

                    // If crop box is valid, draw crop box
                    if (this.cropBox.TLX != -1)
                    {
                        this.depthBitmap.DrawRectangle(this.cropBox.TLX, this.cropBox.TLY, this.cropBox.BRX, this.cropBox.BRY, System.Windows.Media.Color.FromRgb(255, 0, 0));
                    }

                }
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        private void saveDepthData()
        {
            if(this.cropBox.TLX == -1)
            {
                this.richTextBox_console.AppendText("You must fisrt specify the bounds of the crop box.");
                return;
            }

            Console.WriteLine("Width of 2d: " + this.depthArray2D.GetUpperBound(0).ToString());
            Console.WriteLine("Height of 2d: " + this.depthArray2D.GetUpperBound(1).ToString());

            //initialize a StreamWriter
            StreamWriter sw = new StreamWriter(@"C:/data.txt");

            // Crop the sub-vector, identify the min depth in the region
            short[,] ballVector = new short[(Int16)BALL_WIDTH, (Int16)BALL_WIDTH];
            int ballVecI = 0;
            int ballVecJ = 0;
            short minVal = 9999;
            for (int i = this.cropBox.TLY; i < this.cropBox.BRY; ++i)
            {
                for (int j = this.cropBox.TLX; j < this.cropBox.BRX; ++j)
                {

                    short val = 0;
                    try
                    {
                        val = this.depthArray2D[(i - 1), (j - 1)];
                    }
                    catch {
                        Console.WriteLine("Width of 2d: " + this.depthArray2D.GetUpperBound(0).ToString());
                        Console.WriteLine("Height of 2d: " + this.depthArray2D.GetUpperBound(1).ToString());
                        Console.WriteLine("i-1: " + (i - 1).ToString());
                        Console.WriteLine("j-1: " + (j - 1).ToString());
                    }
                    ballVector[ballVecI, ballVecJ] = val;
                    ballVecJ++;
                    if (val != 0)
                        if (val < minVal)
                            minVal = val;
                }
                ballVecI++;
                ballVecJ = 0;
            }

            // Normalize and identify max depth of region
            short maxVal = 0;
            for (int i = 0; i < ballVector.GetUpperBound(0); i++)
            {
                for (int j = 0; j < ballVector.GetUpperBound(1); j++)
                {
                    short val = ballVector[i, j];
                    var newVal = (Int16)(val - minVal);
                    ballVector[i, j] = newVal;
                    if (newVal > maxVal)
                        maxVal = newVal;
                }
            }

            // Generate and write text output
            string output = "";
            for (int i = 0; i < ballVector.GetUpperBound(0); i++)
            {
                for (int j = 0; j < ballVector.GetUpperBound(1); j++)
                    output += ballVector[i, j].ToString() + "\t";
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

            // Write file to disk
            myBitmap.Save("C:/img.png");

            Logger.Log("Depth data saved.");
        }

        private void button_saveDepthData_Click(object sender, RoutedEventArgs e)
        {
            checkDepthDataValues();
            saveDepthData();
        }

        private static T[,] make2DArray<T>(T[] input, int height, int width)
        {
            T[,] output = new T[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    output[(i), j] = input[i * width + j];
                }
            }
            return output;
        }

        private void checkDepthDataValues()
        {
            try
            {
                // Verify values
                int tlx = int.Parse(textBox_TLX.Text)-1;
                int tly = int.Parse(textBox_TLY.Text)-1;
                int brx = tlx + BALL_WIDTH;
                int bry = tly + BALL_WIDTH;
                textBox_BRX.Text = brx.ToString();
                textBox_BRY.Text = bry.ToString();
                if (tlx < 0 || tlx > this.depthBitmap.PixelWidth-1)
                    throw new Exception();
                if (tly < 0 || tly > this.depthBitmap.PixelHeight-1)
                    throw new Exception();
                if (brx < 0 || brx > this.depthBitmap.PixelWidth-1)
                    throw new Exception();
                if (bry < 0 || bry > this.depthBitmap.PixelHeight-1)
                    throw new Exception();
                // Update depth tool box object
                this.cropBox.TLX = tlx;
                this.cropBox.TLY = tly;
                this.cropBox.BRX = brx;
                this.cropBox.BRY = bry;
                Logger.Log("Depth data tool updated!");
            }
            catch
            {
                this.cropBox = new CropBox();
                Logger.Log("Invalid corrdinates given for depth data tool.");
            }

            

        }


        private void populateDropDowns()
        {
            this.comboBox_light.ItemsSource = Enum.GetValues(typeof(LightCtrl.Light));
            this.comboBox_color.ItemsSource = Enum.GetValues(typeof(LightCtrl.Color));
            this.comboBox_gobo.ItemsSource = Enum.GetValues(typeof(LightCtrl.Gobo));
            this.comboBox_strobe.ItemsSource = Enum.GetValues(typeof(LightCtrl.Strobe));
        }

        private void slider_pan_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.label_pan.Content = Math.Floor(this.slider_pan.Value).ToString();
        }

        private void slider_pan_fine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.label_pan_fine.Content = Math.Floor(this.slider_pan_fine.Value).ToString();
        }

        private void slider_tilt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.label_tilt.Content = Math.Floor(this.slider_tilt.Value).ToString();
        }

        private void slider_tilt_fine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.label_tile_fine.Content = Math.Floor(this.slider_tilt_fine.Value).ToString();
        }

        private void button_setLight_Click(object sender, RoutedEventArgs e)
        {
            LightCtrl.SetAll(
                (LightCtrl.Light)Enum.Parse(typeof(LightCtrl.Light), comboBox_light.SelectedValue.ToString()),
                (LightCtrl.Color)Enum.Parse(typeof(LightCtrl.Color), comboBox_color.SelectedValue.ToString()),
                (LightCtrl.Gobo)Enum.Parse(typeof(LightCtrl.Gobo), comboBox_gobo.SelectedValue.ToString()),
                (LightCtrl.Strobe)Enum.Parse(typeof(LightCtrl.Strobe), comboBox_strobe.SelectedValue.ToString()),
                (Int32)Math.Floor(slider_pan.Value),
                (Int32)Math.Floor(slider_pan_fine.Value),
                (Int32)Math.Floor(slider_tilt.Value),
                (Int32)Math.Floor(slider_tilt_fine.Value));
        }

        private void button_lightOff_Click(object sender, RoutedEventArgs e)
        {
            Presets.SetOff();
        }
    }
    
}
