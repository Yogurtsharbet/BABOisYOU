using System;
using System.Collections.Generic;
using System.IO;

namespace BABOisYOU {
    class Map {
        public void SaveMap(List<SpriteBlock> blocks, int level) {
            string path = "./map/MAP" + level;
            using (StreamWriter writer = new StreamWriter(path))
                foreach (var block in blocks)
                    writer.WriteLine($"{block.type}|{block.posX}|{block.posY}");
        }
        public List<SpriteBlock> LoadMap(int level) {
            List<SpriteBlock> blocks = new List<SpriteBlock>();
            string path = "./map/MAP" + level;
            using (StreamReader reader = new StreamReader(path)) {
                string eachLine;
                while ((eachLine = reader.ReadLine()) != null) {
                    string[] parsed = eachLine.Split('|');
                    SpriteBlock temp = new SpriteBlock((S_TYPE)Enum.Parse(typeof(S_TYPE), parsed[0]));
                    temp.setXY(int.Parse(parsed[1]), int.Parse(parsed[2]));
                    if (parsed.Length > 3)
                        temp.setDirection(int.Parse(parsed[3]));
                    temp.Print();
                    blocks.Add(temp);
                }
            }
            return blocks;
        }
        public void PrintLogo() {
            string path = "./map/LOGO";
            using (StreamReader reader = new StreamReader(path)) {
                string eachLine;
                int posY = 0;
                while ((eachLine = reader.ReadLine()) != null) {
                    Console.SetCursorPosition(30, 15 + posY++);
                    int posX = 0;

                    foreach (var each in eachLine) {
                        posX++;
                        if (each == 'B' || each == '%') Console.ForegroundColor = ConsoleColor.Black;
                        else if (posX > 250 && posX < 350) {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else if(posY > 90) {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else if (each == ' ') Console.BackgroundColor = ConsoleColor.White;
                        else {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.BackgroundColor = ConsoleColor.Magenta;
                        }
                        Console.Write(each);
                        Console.ResetColor();
                    }
                }
            }

        }
    }
}
