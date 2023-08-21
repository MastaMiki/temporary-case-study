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
        static int loopSize = 1000000000;
        static object sumLock = new object();

        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new FileStream("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                Data_Global = (byte[])bf.Deserialize(fs);
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

        static int sum(int index)
        {
            int sum_data = 0;
            if (Data_Global[index] % 2 == 0)
            {
                sum_data -= Data_Global[index];
            }
            else if (Data_Global[index] % 3 == 0)
            {
                sum_data += (Data_Global[index] * 2);
            }
            else if (Data_Global[index] % 5 == 0)
            {
                sum_data += (Data_Global[index] / 2);
            }
            else if (Data_Global[index] % 7 == 0)
            {
                sum_data += (Data_Global[index] / 3);
            }
            Data_Global[index] = 0;
            G_index++;
            return sum_data;
        }


        static void ProcessChunk(int start_index, int end_index)
        {
            int sum_data = 0;
            for (int i = start_index; i < end_index; i++)
            {
                sum_data += sum(i);
            }
            lock (sumLock)
            {
                Sum_Global += sum_data;
            }
        }



        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\r\n░█████╗░███████╗██████╗░████████╗░░░░░░░█████╗░███████╗\r\n██╔══██╗██╔════╝██╔══██╗╚══██╔══╝░░░░░░██╔═══╝░██╔════╝\r\n██║░░╚═╝█████╗░░██║░░██║░░░██║░░░█████╗██████╗░██████╗░\r\n██║░░██╗██╔══╝░░██║░░██║░░░██║░░░╚════╝██╔══██╗╚════██╗\r\n╚█████╔╝███████╗██████╔╝░░░██║░░░░░░░░░╚█████╔╝██████╔╝\r\n░╚════╝░╚══════╝╚═════╝░░░░╚═╝░░░░░░░░░░╚════╝░╚═════╝░");
            Console.WriteLine("CEDT-65 Group 41: MultiThreading For Optimize Version");
            int numberOfThreads = Environment.ProcessorCount; // Number of threads/tasks to use
            int chunkSize = loopSize / numberOfThreads;
            ConcurrentQueue<int> resultQueue = new ConcurrentQueue<int>();
            Task[] tasks = new Task[numberOfThreads];

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

            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();
            for (int j = 0; j < numberOfThreads; j++)
            {
                int startIndex = j * chunkSize;
                int endIndex = (j == numberOfThreads - 1) ? loopSize : startIndex + chunkSize;

                tasks[j] = Task.Factory.StartNew(() => ProcessChunk(startIndex, endIndex));
            }

            // Wait for all tasks to complete
            Task.WaitAll(tasks);

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
