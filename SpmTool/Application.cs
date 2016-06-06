using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpmTool
{
    class Application
    {
        static void Main(string[] args)
        {
            var path = args[0];

            if (File.Exists(path))
            {
                string dir = Path.GetDirectoryName(path);
                string targetDir = Path.Combine(dir, "DX9");
                Directory.CreateDirectory(targetDir);

                convertFile(path, targetDir);
                return;
            }

            if (Directory.Exists(path))
            {
                string targetDir = Path.Combine(path, "DX9");
                Directory.CreateDirectory(targetDir);

                foreach (string file in Directory.GetFiles(path))
                {
                    if(!file.EndsWith(".spm", StringComparison.InvariantCultureIgnoreCase)) continue;

                    try
                    {
                        convertFile(file, targetDir);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error converting: " + file);
                        Console.WriteLine(e);
                        Console.WriteLine();
                    }
                }
                return;
            }
        }

        static void convertFile(string file, string targetDir)
        {
            var dx8Spm = File.ReadAllText(file);

            bool isAirplaneOrHelicopter = SpmUtilities.IsAirplane(dx8Spm) || SpmUtilities.IsHelicopter(dx8Spm);
            if (!isAirplaneOrHelicopter)
            {
                Console.WriteLine(file + " is not an Airplane/Helicopter");
                return;
            }

            string outFile = Path.Combine(targetDir, Path.GetFileName(file));
            Console.WriteLine(file + " -> " + outFile);

            string dx9Spm;
            int modelNumber = SpmUtilities.GetModelNumberFromFilename(file);
            if (modelNumber != -1)
            {
                string modelName = modelNumber + ": " + SpmUtilities.GetModelName(dx8Spm);
                dx9Spm = SpmConvert.DX8To(dx8Spm, modelName: modelName);
            }
            else
            {
                dx9Spm = SpmConvert.DX8To(dx8Spm);
            }

            File.WriteAllText(outFile, dx9Spm);
        }


    }
}
