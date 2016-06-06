using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace SpmTool.Tests
{
    class AdHoc
    {
        void roundtripAll_DX9()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string dx8Spm = File.ReadAllText(file);

                if (SpmUtilities.IsCorrupt(dx8Spm))
                {
                    Console.WriteLine("Corrupted model: " + file);
                    continue;
                }

                //if (!SpmUtilities.IsAirplane(dx8Spm) && !SpmUtilities.IsHelicopter(dx8Spm)) continue;

                string xml = SpmToXml.Convert(dx8Spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);
                var nav = doc.CreateNavigator();

                var name = nav.SelectSingleNode(@"SPM/Spektrum/Name/text()").Value;
                if (name.Length > 10)
                {
                    Console.WriteLine("Name too long: " + file);
                    continue;
                }

                Console.WriteLine(file);
                string dx9Spm = SpmConvert.DX8To(dx8Spm);
                string dx8Spm2 = SpmConvert.DX9To(dx9Spm);
                string dx9Spm2 = SpmConvert.DX8To(dx8Spm2);

                try
                {
                    Assert.That(dx9Spm, Is.EqualTo(dx9Spm2));
                }
                catch
                {
                    File.WriteAllText("dx8Spm.spm", dx8Spm);
                    File.WriteAllText("dx8Spm2.spm", dx8Spm2);
                    File.WriteAllText("dx9Spm.spm", dx9Spm);
                    File.WriteAllText("dx9Spm2.spm", dx9Spm2);
                    throw;
                }
            }
        }

        void roundtripAll_FilterDX8()
        {
            int count = 0;
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string dx8Spm = File.ReadAllText(file);

                if (!SpmUtilities.IsAirplane(dx8Spm) && !SpmUtilities.IsHelicopter(dx8Spm)) continue;

                dx8Spm = SpmConvert.FilterDX8(dx8Spm);

                string xml = SpmToXml.Convert(dx8Spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);
                var nav = doc.CreateNavigator();

                var name = nav.SelectSingleNode(@"SPM/Spektrum/Name/text()").Value;
                if (name.Length > 10)
                {
                    Console.WriteLine("Name too long: " + file);
                    continue;
                }

                string dx9Spm = SpmConvert.DX8To(dx8Spm);
                string dx8Spm2 = SpmConvert.DX9To(dx9Spm);
                string dx9Spm2 = SpmConvert.DX8To(dx8Spm2);

                try
                {
                    count++;
                    Console.WriteLine(count + ": " + file);
                    Assert.That(dx8Spm, Is.EqualTo(dx8Spm2));
                    Assert.That(dx9Spm, Is.EqualTo(dx9Spm2));
                }
                catch
                {
                    File.WriteAllText("dx8Spm.spm", dx8Spm);
                    File.WriteAllText("dx8Spm2.spm", dx8Spm2);
                    File.WriteAllText("dx9Spm.spm", dx9Spm);
                    File.WriteAllText("dx9Spm2.spm", dx9Spm2);
                    throw;
                }
            }
        }

        void roundtripAll_DX7S()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string dx8Spm = File.ReadAllText(file);

                if (SpmUtilities.IsCorrupt(dx8Spm)) continue;

                string xml = SpmToXml.Convert(dx8Spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);
                var nav = doc.CreateNavigator();

                var generator = nav.SelectSingleNode(@"SPM/Spektrum/Generator/text()").Value;
                if (generator != "DX7S") continue;

                Console.WriteLine(file);
                string dx9Spm = SpmConvert.DX8To(dx8Spm);
                string dx8Spm2 = SpmConvert.DX9To(dx9Spm);
                string dx9Spm2 = SpmConvert.DX8To(dx8Spm2);

                try
                {
                    Assert.That(dx9Spm, Is.EqualTo(dx9Spm2));
                }
                catch
                {
                    File.WriteAllText("dx8Spm.spm", dx8Spm);
                    File.WriteAllText("dx8Spm2.spm", dx8Spm2);
                    File.WriteAllText("dx9Spm.spm", dx9Spm);
                    File.WriteAllText("dx9Spm2.spm", dx9Spm2);
                    throw;
                }
            }
        }

        void roundtrip()
        {
            string file = @"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples\larry@pflumber.com\01ALPHA_.SPM";
            string dx8Spm = File.ReadAllText(file);

            string xml = SpmToXml.Convert(dx8Spm);
            var reader = new StringReader(xml);
            XPathDocument doc = new XPathDocument(reader);
            var nav = doc.CreateNavigator();

            Console.WriteLine(file);

            string dx9Spm = SpmConvert.DX8To(dx8Spm);
            string dx8Spm2 = SpmConvert.DX9To(dx9Spm);
            string dx9Spm2 = SpmConvert.DX8To(dx8Spm2);

            File.WriteAllText("dx8Spm.spm", dx8Spm);
            File.WriteAllText("dx8Spm2.spm", dx8Spm2);
            File.WriteAllText("dx9Spm.spm", dx9Spm);
            File.WriteAllText("dx9Spm2.spm", dx9Spm2);
        }

        void go()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                //var tailNode = nav.SelectSingleNode(@"SPM/Acro/Tail/text()");
                //if (tailNode == null) continue;

                //string tail = tailNode.Value;
                //Console.WriteLine(tail);

                //if (tail != "Dual_Ele") continue;

                var trimIDNodes = nav.Select(@"SPM/FlapSystem/trimID/text()");
                foreach (XPathNavigator trimIDNode in trimIDNodes)
                {
                    int trimID = int.Parse(trimIDNode.Value);
                    dictionary[trimID] = Path.GetFileName(file);
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void sail()
        {
            string dir = "Sail";
            Directory.CreateDirectory(dir);

            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                if (SpmUtilities.IsCorrupt(spm)) continue;

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var type = nav.SelectSingleNode(@"SPM/Spektrum/Type/text()").Value;
                if (type != "Sail") continue;

                File.Copy(file, Path.Combine(dir, Path.GetFileName(file)), true);

                var motor = nav.SelectSingleNode(@"SPM/Sail/Motor/text()");
                Console.WriteLine(file + ", " + motor);
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void dx18()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\DX18_Setups", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var ele = nav.Select(@"SPM/DR_Expo/drHigh2/Element/text()");
                foreach (XPathNavigator eleNode in ele)
                {
                    int eleInt = int.Parse(eleNode.Value);
                    dictionary[eleInt] = Path.GetFileName(file);

                    if (eleInt == 198)
                    {
                        Console.WriteLine(file);
                    }
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }


        void RAE_Mix()
        {
            var dictionary = new SortedList<string, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var key = "" + nav.SelectSingleNode(@"SPM/RAE-Mix/conditionID");

                dictionary[key] = file;
            }

            foreach (var entry in dictionary)
            {
                Console.WriteLine(entry.Key + ", " + entry.Value);
            }
        }

        void FMode_data()
        {
            var dictionary = new SortedList<string, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var fmode = nav.SelectSingleNode(@"SPM/FMode/data").ToString();
                var type = nav.SelectSingleNode("/SPM/Spektrum/Type").Value;
                var generator = nav.SelectSingleNode("/SPM/Spektrum/Generator").Value;

                dictionary[fmode] = type + "," + generator;
            }

            foreach (var entry in dictionary)
            {
                Console.WriteLine(entry.Key + ", " + entry.Value);
            }
        }


        void TLM()
        {
            //string path = @"H:\XTRA.TLM";
            //string path = @"H:\crackyak.TLM";
            string path = @"H:\XTRAx.TLM";
            //string path = @"H:\sl.TLM";
            //string path = @"H:\006~6 Ac.TLM";

            TlmUtilities.DecodeFile(path);
        }

        void types()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string type = nav.SelectSingleNode(@"SPM/Spektrum/Type/text()").Value;
                Console.WriteLine(type + ": " + Path.GetFileName(file));
            }
        }

        void problem()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Problem", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);

                string dx9 = SpmConvert.DX8To(spm);

                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string type = nav.SelectSingleNode(@"SPM/Spektrum/Type/text()").ToString();
                Console.WriteLine(type);
            }
        }

        void trim()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string type = "" + nav.SelectSingleNode(@"SPM/Spektrum/Type/text()");
                string trimType = "" + nav.SelectSingleNode(@"SPM/Config/TrimType/text()");
                string trimMode = "" + nav.SelectSingleNode(@"SPM/Config/trimMode/text()");

                if (trimType == "Common") continue;
                Console.WriteLine(type + ", " + trimType + ", " + trimMode + ": " + Path.GetFileNameWithoutExtension(file) + ", " + email);
            }
        }

        void warning()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string fltMode = nav.SelectSingleNode(@"SPM/Warning/FltMode/text()").Value;
                string type = nav.SelectSingleNode(@"SPM/Spektrum/Type/text()").Value;

                Console.WriteLine(type + ", " + fltMode + ": " + Path.GetFileNameWithoutExtension(file) + ", " + email);
            }
        }

        void warning_fltMode()
        {
            var dictionary = new SortedList<string, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string fltMode = nav.SelectSingleNode(@"SPM/Warning/FltMode/text()").Value;
                string type = nav.SelectSingleNode(@"SPM/Spektrum/Type/text()").Value;
                dictionary[type + ":" + fltMode] = Path.GetFileName(file);
            }

            foreach (var entry in dictionary)
            {
                Console.WriteLine(entry.Key);
            }
        }

        void flaps()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string type = nav.SelectSingleNode(@"SPM/Spektrum/Type/text()").Value;
                if (type == "Acro")
                {
                    string condition = nav.SelectSingleNode(@"SPM/FlapSystem/conditionID/text()").Value;
                    if(condition == "0") continue;

                    string flaps = nav.SelectSingleNode(@"SPM/Warning/Flaps/text()").Value;
                    if (flaps == "%0000") continue;

                    Console.WriteLine(type + ", " + condition + ", " + flaps + ": " + Path.GetFileNameWithoutExtension(file) + ", " + email);
                }
            }
        }

        void ar_mix()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string generator = nav.SelectSingleNode(@"SPM/Spektrum/Generator/text()").Value;
                string vcode = nav.SelectSingleNode(@"SPM/Spektrum/VCode/text()").Value;

                string type = nav.SelectSingleNode(@"SPM/Spektrum/Type/text()").Value;
                if (type == "Acro")
                {
                    string condition = nav.SelectSingleNode(@"SPM/AR-Mix/conditionID/text()").Value;
                    if (condition == "0") continue;

                    string y = nav.SelectSingleNode(@"SPM/AR-Mix/Curvedata/Y/Element/text()").Value;

                    Console.WriteLine(generator + vcode + ": " + type + ", " + condition + ", " + y + ": " + Path.GetFileNameWithoutExtension(file) + ", " + email);
                }
            }
        }

        void servo_speed()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var servos = nav.Select(@"SPM/Servo");
                foreach (XPathNavigator servo in servos)
                {
                    string speed = servo.SelectSingleNode(@"speed/text()").Value;
                    if (speed == "32736") continue;

                    double seconds = (3641 / int.Parse(speed)) * .1;
                    if (seconds == 0) continue;

                    string name = servo.SelectSingleNode(@"name/text()").Value;

                    Console.WriteLine(name + ", " + seconds + "s : " + Path.GetFileNameWithoutExtension(file) + ", " + email);
                }
            }
        }

        void servo_sourceID()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var servos = nav.Select(@"SPM/Servo");
                foreach (XPathNavigator servo in servos)
                {
                    int sourceID = int.Parse(servo.SelectSingleNode(@"sourceID/text()").Value);
                    string name = servo.SelectSingleNode(@"name/text()").Value;
                    dictionary[sourceID] = name;
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void DR_Expo_analogID()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var servos = nav.Select(@"SPM/DR_Expo");
                foreach (XPathNavigator servo in servos)
                {
                    int analogID = int.Parse(servo.SelectSingleNode(@"analogID/text()").Value);
                    dictionary[analogID] = "";
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void DR_Expo_conditionID()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var servos = nav.Select(@"SPM/DR_Expo");
                foreach (XPathNavigator servo in servos)
                {
                    int analogID = int.Parse(servo.SelectSingleNode(@"conditionID/text()").Value);
                    dictionary[analogID] = "";
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void PMix_activePositions()
        {
            //var dictionary = new SortedList<string, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                if (SpmUtilities.IsCorrupt(spm)) continue;
                if (!SpmUtilities.IsHelicopter(spm)) continue;

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var mixes = nav.Select(@"SPM/P-Mix");

                foreach (XPathNavigator mix in mixes)
                {
                    var activePositions = mix.SelectSingleNode(@"activePositions/text()").Value;
                    var conditionID = mix.SelectSingleNode(@"conditionID/text()").Value;
                    var analogID = mix.SelectSingleNode(@"analogID/text()").Value;
                    var outChan = mix.SelectSingleNode(@"outChan/text()").Value;
                    if (activePositions == "%0000") continue;

                    Console.WriteLine(file);
                    Console.WriteLine("activePositions: " + activePositions);
                    Console.WriteLine("conditionID: " + conditionID);
                    Console.WriteLine("analogID: " + analogID);
                    Console.WriteLine("outChan: " + outChan);
                    Console.WriteLine();
                }
            }

            //foreach (var entry in dictionary)
            //{
            //    Console.WriteLine(entry.Key + ": " + entry.Value);
            //}
        }

        void PMix_analogID()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var nodes = nav.Select(@"SPM/P-Mix/analogID/text()");
                foreach (XPathNavigator node in nodes)
                {
                    int id = int.Parse(node.Value);
                    dictionary[id] = "";
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void PMix_outChan()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var nodes = nav.Select(@"SPM/P-Mix/outChan/text()");
                foreach (XPathNavigator node in nodes)
                {
                    int id = int.Parse(node.Value);
                    dictionary[id] = "";
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void PMix_conditionID()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var nodes = nav.Select(@"SPM/P-Mix/conditionID/text()");
                foreach (XPathNavigator node in nodes)
                {
                    int id = int.Parse(node.Value);
                    dictionary[id] = "";
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void sourceID()
        {
            var dictionary = new SortedList<int, string>();
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                var sourceIDs = nav.Select(@"SPM/*/sourceID/text()");
                foreach (XPathNavigator sourceID in sourceIDs)
                {
                    dictionary[int.Parse(sourceID.Value)] = sourceID.Value;
                }
            }

            foreach (var entry in dictionary)
            {
                Console.Write(entry.Key + ", ");
            }
        }

        void Pre205_Subtrim()
        {
            string previousOwner = null;
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string generator = nav.SelectSingleNode(@"SPM/Spektrum/Generator/text()").Value;
                string vcode = nav.SelectSingleNode(@"SPM/Spektrum/VCode/text()").Value;
                double version;
                try
                {
                    version = double.Parse(vcode.Substring(1));
                }
                catch
                {
                    Console.WriteLine("VCode=" + vcode);
                    continue;
                }

                if (version >= 2.05) continue;

                string owner = email + ", " + generator + ":" + vcode; 

                var servos = nav.Select(@"SPM/Servo");
                foreach (XPathNavigator servo in servos)
                {
                    string direction = servo.SelectSingleNode(@"direction/text()").Value;
                    if (direction != "Reverse") continue;

                    string name = servo.SelectSingleNode(@"name/text()").Value;

                    string subTrim = servo.SelectSingleNode(@"subTrim/text()").Value;
                    if (subTrim != "0")
                    {
                        if (owner != previousOwner)
                        {
                            Console.WriteLine();
                            Console.WriteLine(owner);
                            previousOwner = owner;
                        }

                        Console.WriteLine(Path.GetFileName(file) + ": " + name + "> Sub Trim needs to be reversed");
                    }

                    string travelLow = servo.SelectSingleNode(@"travelLow/text()").Value;
                    string travelHigh = servo.SelectSingleNode(@"travelHigh/text()").Value;
                    if (double.Parse(travelLow) != -double.Parse(travelHigh))
                    {
                        if (owner != previousOwner)
                        {
                            Console.WriteLine();
                            Console.WriteLine(owner);
                            previousOwner = owner;
                        }

                        Console.WriteLine(Path.GetFileName(file) + ": " + name + "> Travel Low/High needs to be swapped");
                    }
                }
            }
        }

        void version()
        {
            foreach (string file in Directory.GetFiles(@"C:\Source\MutantDesign\SpmTool\SpmTool.Tests\Samples", "*.spm", SearchOption.AllDirectories))
            {
                string email = Path.GetFileName(Path.GetDirectoryName(file));
                string spm = File.ReadAllText(file);

                string xml = SpmToXml.Convert(spm);
                var reader = new StringReader(xml);
                XPathDocument doc = new XPathDocument(reader);

                var nav = doc.CreateNavigator();

                string generator = nav.SelectSingleNode(@"SPM/Spektrum/Generator/text()").Value;
                string vcode = nav.SelectSingleNode(@"SPM/Spektrum/VCode/text()").Value;
                Console.WriteLine(generator + " " + vcode + ": " + Path.GetFileName(file) + ", " + email);
            }


        }


        static string pad(string str, int length)
        {
            string s = str;
            while (s.Length < 3)
            {
                s = s + " ";
            }
            return s;
        }

    }
}
