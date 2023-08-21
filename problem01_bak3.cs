using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Problem01
{
    class Program
    {
        static byte[] Data_Global = new byte[100000000];
        static long Sum_Global = 0;
        static int G_index = 0;
        static int loopSize = 10;

        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new FileStream("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            try 
            {
                Data_Global = (byte[]) bf.Deserialize(fs);
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Read Failed:" + se.Message);
                returnData = 1;
            }
            finally
            {
                fs.Close();
            }

            return returnData;
        }
        static void sum(int index)
        {
            if (Data_Global[index] % 2 == 0)
            {
                Sum_Global -= Data_Global[index];
            }
            else if (Data_Global[index] % 3 == 0)
            {
                Sum_Global += (Data_Global[index] *2);
            }
            else if (Data_Global[index] % 5 == 0)
            {
                Sum_Global += (Data_Global[index] / 2);
            }
            else if (Data_Global[index] %7 == 0)
            {
                Sum_Global += (Data_Global[index] / 3);
            }
            Data_Global[index] = 0;
            G_index++;
        }

        static void ProcessDataChunk(int startIndex, int endIndex, ConcurrentQueue<int> resultQueue)
        {
            // Process the data chunk and generate a result
            int result = ProcessChunk(startIndex, endIndex);

            // Add the result to the queue
            resultQueue.Enqueue(result);
        }

        static int ProcessChunk(int start_index, int end_index)
        {
            for (int i = start_index; i < end_index; i++)
            {
                sum(i);
            }
            return 1;
        }



        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            int i, y;

            /* Read data from file */
            Console.Write("Data read...");
            y = ReadData();
            if (y == 0)
            {
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Read Failed!");
            }

            int numberOfThreads = Environment.ProcessorCount - 1; // Number of threads/tasks to use
            /*Task[] tasks = new Task[numberOfThreads];*/
            int chunkSize = loopSize / numberOfThreads;
            ConcurrentQueue<int> resultQueue = new ConcurrentQueue<int>();

            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();
            for (int j = 0; j < numberOfThreads; j++)
            {
                int startIndex = j * chunkSize;
                int endIndex = (j == numberOfThreads - 1) ? loopSize : startIndex + chunkSize;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    ProcessDataChunk(startIndex, endIndex, resultQueue);
                });
            }
            while (resultQueue.Count < numberOfThreads)
            {
                Thread.Sleep(1); // Wait for a short time before checking again
            }

            sw.Stop();
            Console.WriteLine("Done.");

            /* Result */
            Console.WriteLine("Summation result: {0}", Sum_Global);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds.ToString() + "ms");

            Console.WriteLine("Please Enter to exit the process...");
            Console.ReadKey();
        }
    }
}
