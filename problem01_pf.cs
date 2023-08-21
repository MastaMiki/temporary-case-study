using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Problem01Ori
{
    class Program1
    {
        static byte[] Data_Global = new byte[100000000];
        static long Sum_Global = 0;
        static int G_index = 0;
        static int ChunkSize = Data_Global.Length / Environment.ProcessorCount; // Adjust chunk size as needed
        static object LockObj = new object();

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
        static void ParallelSum(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                lock (LockObj)
                {
                    if (Data_Global[G_index] % 2 == 0)
                    {
                        Sum_Global -= Data_Global[G_index];
                    }
                    else if (Data_Global[G_index] % 3 == 0)
                    {
                        Sum_Global += (Data_Global[G_index] * 2);
                    }
                    else if (Data_Global[G_index] % 5 == 0)
                    {
                        Sum_Global += (Data_Global[G_index] / 2);
                    }
                    else if (Data_Global[G_index] % 7 == 0)
                    {
                        Sum_Global += (Data_Global[G_index] / 3);
                    }
                    Data_Global[G_index] = 0;
                    G_index++;
                }
            }
        }
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\r\n░█████╗░███████╗██████╗░████████╗░░░░░░░█████╗░███████╗\r\n██╔══██╗██╔════╝██╔══██╗╚══██╔══╝░░░░░░██╔═══╝░██╔════╝\r\n██║░░╚═╝█████╗░░██║░░██║░░░██║░░░█████╗██████╗░██████╗░\r\n██║░░██╗██╔══╝░░██║░░██║░░░██║░░░╚════╝██╔══██╗╚════██╗\r\n╚█████╔╝███████╗██████╔╝░░░██║░░░░░░░░░╚█████╔╝██████╔╝\r\n░╚════╝░╚══════╝╚═════╝░░░░╚═╝░░░░░░░░░░╚════╝░╚═════╝░");
            Console.WriteLine("CEDT-65 Group 41: Parallel For Optimize Version");
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
            int totalChunks = (int)Math.Ceiling((double)Data_Global.Length / ChunkSize);
            Parallel.For(0, totalChunks, chunkIndex =>
            {
                int startIndex = chunkIndex * ChunkSize;
                int endIndex = Math.Min(startIndex + ChunkSize, Data_Global.Length);
                ParallelSum(startIndex, endIndex);
            });
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
