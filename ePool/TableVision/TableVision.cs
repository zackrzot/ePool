using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;

namespace ePool
{
    public class BallBox
    {
        public int TLX { get; set; }
        public int TLY { get; set; }
        public int BRX { get; set; }
        public int BRY { get; set; }

        public BallBox(int tlx, int tly, int brx, int bry)
        {
            this.TLX = tlx;
            this.TLY = tly;
            this.BRX = brx;
            this.BRY = bry;
        }
    }


    public class TableVision
    {
        public static short[,] DepthArray2D;

        private static Int16 BALL_DIM = 25;

        private static List<short[,]> ballVectors = new List<short[,]>();

        public static List<BallBox> DetectedBalls = new List<BallBox>();

        public static void LoadBallSignatures()
        {
            String dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data\\BallSignatures\\";

            Logger.Log("Loading ball signatures to memory.");
            Logger.Log(dir);

            DirectoryInfo d = new DirectoryInfo(dir);

            int count = 0;
            int failed = 0;
            foreach (var file in d.GetFiles("*.txt"))
            {
                String text = File.ReadAllText(file.FullName);

                try { readTextDataToBallVector(text); count++; }
                catch(Exception ex) { Logger.Log("Unable to properly parse ball vector file: " + ex.ToString()); failed++; }
                count++;
            }

            Logger.Log(count.ToString() +  " ball signatures loaded to memory.");

            if(failed>0)
                Logger.Log("Error: Failed to load " + failed.ToString() + " ball signatures loaded to memory.");
        }

        private static void readTextDataToBallVector(String text)
        {
            short[,] ballVector = new short[BALL_DIM, BALL_DIM];

            int row = 0;

            foreach (var line in text.Split('\n'))
            {
                int col = 0;
                foreach (var dat in line.Split('\t'))
                {
                    String numToParse = dat.Trim();
                    if (numToParse != "")
                    {
                        ballVector[row, col] = (short)int.Parse(numToParse);
                        col++;
                    }
                }
                row++;
            }
            ballVectors.Add(ballVector);
        }

        public static void DetectBalls()
        {
            Thread t = new Thread(new ThreadStart(findBallWork));
            t.Start();
        }

        private static void findBallWork()
        {
            Console.WriteLine("findBallWork started.");

            int high = 0;
            int sum = 0;
            int count = 0;
            int low = 9999999;
            int score = 0;

            // Get instance of depth vector
            short[,] depthVector = DepthArray2D;

            depthVector = getVectorCrop(depthVector, 200, 150, 200);

            // Slide top to bottom
            for (int y = 0; y < depthVector.GetLength(1) - BALL_DIM - 1; y++)
            {
                // Slide left to right
                for (int x = 0; x < depthVector.GetLength(0) - BALL_DIM - 1; x++)
                {
                    // Get sub frame with x y being the origin
                    short[,] roi = getVectorCrop(depthVector, x, y, BALL_DIM);

                    // Normalize the region of interest
                    roi = NormalizeVector(roi);

                    // Compare against data
                    foreach(var sig in ballVectors)
                    {
                        score = getDifScore(sig, roi);
                        if (score > high)
                            high = score;
                        if (score < low)
                            low = score;
                        sum += score;
                        count++;
                    }
                }
            }
            Console.WriteLine("Comparisons done: " + count.ToString());
            Console.WriteLine("High: " + high.ToString());
            Console.WriteLine("Low: " + low.ToString());
            Console.WriteLine("Sum: " + sum.ToString());
            Console.WriteLine("Average: " + Math.Floor((double)(sum/count)).ToString());


            Console.WriteLine("findBallWork started.");
        }

        private static int getDifScore(short[,] a, short[,] b)
        {
            int score = 0;
            for (int i = 0; i < a.GetUpperBound(0); i++)
            {
                for (int j = 0; j < a.GetUpperBound(1); j++)
                {
                    score = score + Math.Abs(a[i, j] - b[i, j]);
                }
            }
            return score;
        }

        private static short[,] getVectorCrop(short[,] source, int xOrigin, int yOrigin, Int16 dim)
        {
            short[,] result = new short[dim, dim];

            for(int row = 0; row < dim; row++)
            {
                for(int col = 0; col < dim; col++)
                {
                    result[row, col] = source[yOrigin + row, xOrigin + col];
                }

            }

            return result;
        }

        public static short[,] NormalizeVector(short[,] vec)
        {
            short minVal = 9999;
            for (int i = 0; i < vec.GetUpperBound(0); i++)
            {
                for (int j = 0; j < vec.GetUpperBound(1); j++)
                {
                    short val = vec[i, j];
                    if (val != 0)
                        if (val < minVal)
                            minVal = val;
                }
            }
            short maxVal = 0;
            for (int i = 0; i < vec.GetUpperBound(0); i++)
            {
                for (int j = 0; j < vec.GetUpperBound(1); j++)
                {
                    short val = vec[i, j];
                    var newVal = (Int16)(val - minVal);
                    vec[i, j] = newVal;
                    if (newVal > maxVal)
                        maxVal = newVal;
                }
            }
            return vec;
        }




    }
}
