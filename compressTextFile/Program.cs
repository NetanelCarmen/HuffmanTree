using HuffmanTest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace compressTextFile
{
    class Program
    {
        static void Main(string[] args)
        {
            int action;
            string inputFilePath;
            string inputFileName;
            string outputFileName;
            if (args.Length < 4)
            {
                action = 0;
                Console.WriteLine("Please choose option:");
                Console.WriteLine("1 - encode");
                Console.WriteLine("2 - decode");
                string option = Console.ReadLine();
                action = int.Parse(option);

                while (action != 1 && action != 2)
                {
                    Console.WriteLine("there is no such option");
                    Console.WriteLine("1 - encode");
                    Console.WriteLine("2 - decode");
                    option = Console.ReadLine();
                    action = int.Parse(option);
                }

                Console.WriteLine("Please enter the full path of the input file");
                inputFilePath = Console.ReadLine();
                Console.WriteLine("Please enter the name of the input file ()");
                inputFileName = Console.ReadLine();
                Console.WriteLine("Please enter the name of the output file");
                outputFileName = Console.ReadLine();
            } 
            else
            {
                action = int.Parse(args[0]);
                inputFilePath = args[1];
                inputFileName = args[2];
                outputFileName = args[3];
            }

            string inputFileFullName = inputFilePath + "\\" + inputFileName;
            string outputFileFullName = inputFilePath + "\\" + outputFileName;


            if (action == 1)
            {
                HuffmanTree.Encode(inputFileFullName, outputFileFullName);
            }
            else if(action == 2)
            {
                HuffmanTree.Decode(inputFileFullName, outputFileFullName);
            }
        }
    }
}
