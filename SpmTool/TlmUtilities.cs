namespace SpmTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class TlmUtilities
    {
        public static void DecodeFile(string path)
        {
            string txtPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".txt");

            if (File.Exists(txtPath)) File.Delete(txtPath); // HACK:!!!

            using (var outputStream = File.OpenWrite(txtPath))
            using (var writer = new StreamWriter(outputStream))
            using (var stream = File.OpenRead(path))
            {
                Console.WriteLine(txtPath);

                var reader = new BinaryReader(stream);

                for (int count = 0; ; count++)
                {
                    int head = reader.ReadInt32();
                    writer.WriteLine("Head: " + head);

                    int model = reader.ReadByte();
                    writer.WriteLine("Model: " + model);

                    writer.WriteLine("Zero: " + reader.ReadByte());
                    int rxType = reader.ReadByte();
                    writer.WriteLine("RxType: " + rxType);

                    for (int count1 = 0; count1 < 5; count1++)
                    {
                        int b = reader.ReadByte();
                        writer.WriteLine(count1 + "> " + b);
                    }

                    string name = "";
                    for (int count2 = 0; count2 < 24; count2++)
                    {
                        int b = reader.ReadByte();
                        if (b == 0) continue;
                        name += (char)b;
                    }
                    writer.WriteLine("Name: " + name);

                    while (true)
                    {
                        if (stream.Length == stream.Position) return;

                        head = reader.ReadInt32();
                        if (head != -1)
                        {
                            reader.BaseStream.Seek(-4, SeekOrigin.Current);
                            break;
                        }
                        writer.WriteLine("Head: " + head);

                        int section = reader.ReadByte();
                        int section2 = reader.ReadByte();
                        int sectiona = reader.ReadByte();
                        int sectionb = reader.ReadByte();
                        writer.WriteLine("Section: " + section + ", " + sectiona + ", " + sectionb + ", " + (section == section2));
                        for (int count3 = 0; count3 < 28; count3++)
                        {
                            int b = reader.ReadByte();
                            writer.WriteLine(count3 + "> " + b);
                        }
                    }

                    string csvPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "(" + count + ").csv");
                    Console.WriteLine(csvPath);

                    using (var csvStream = File.OpenWrite(csvPath))
                    using (var csvWriter = new StreamWriter(csvStream))
                    {
                        int[] servos;
                        switch (rxType)
                        {
                            case 1:
                            case 4:
                                csvWriter.WriteLine("Time,THR,AIL,ELE,RUD,GER,AX1,AX2");
                                servos = new int[7];
                                break;
                            case 2:
                            case 3:
                                csvWriter.WriteLine("Time,THR,AIL,ELE,RUD,GER,AX1,AX2,AX3,AX4,AX5");
                                servos = new int[10];
                                break;
                            default:
                                throw new Exception("Unknown rxType: " + rxType);
                        }


                        while (true)
                        {
                            if (stream.Length == stream.Position) return;

                            int time = reader.ReadInt32();
                            if (time == -1)
                            {
                                reader.BaseStream.Seek(-4, SeekOrigin.Current);
                                break;
                            }

                            int b1 = reader.ReadByte();
                            int b2 = reader.ReadByte();

                            if (b2 != 28 && b2 != 9 && b2 != 58)    // are these different bit lengths?
                            {
                                reader.BaseStream.Seek(14, SeekOrigin.Current);
                                continue;
                            }

                            for (int slot = 0; slot < 7; slot++)
                            {
                                int h = reader.ReadByte();
                                int l = reader.ReadByte();

                                int position;
                                int servo;
                                int more;
                                if (rxType > 1)
                                {
                                    position = (h & 0x7) * 256 + l;
                                    servo = (h >> 3) & 0xf;
                                    more = h >> 7;
                                }
                                else
                                {
                                    position = (h & 0x3) * 256 + l;
                                    servo = (h >> 2) & 0xf;
                                    more = h >> 6;
                                }

                                servos[servo] = position;
                            }

                            csvWriter.Write(time);
                            for (int servo = 0; servo < servos.Length; servo++)
                            {
                                csvWriter.Write("," + servos[servo]);
                            }
                            csvWriter.WriteLine();
                        }
                    }
                }
            }
        }
    }
}
