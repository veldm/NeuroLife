using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace NeuroLife
{
    class Program
    {
        static readonly List<Creature> world = new List<Creature>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
#if DEBUG
            StreamWriter writer = new StreamWriter("..\\..\\Log.txt", false);
            writer.Write("");
            writer.Close();
#endif
            //Console.WriteLine(Creature.identSymbols);
            //Console.ReadKey();
            bool stop = false;
            Random rnd = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                //stop = true;
                Creature newLife = new Creature(world, rnd.NextDouble(), rnd.NextDouble(),
                Creature.identSymbols[rnd.Next(0, Creature.identSymbols.Length)]);
                world.Add(newLife);
            };

            for (int i = 0; i < 35; i++)
            {
                Creature creature = new Creature(world, rnd.NextDouble(), rnd.NextDouble(),
                    Creature.identSymbols[rnd.Next(0, Creature.identSymbols.Length)]);
                world.Add(creature);
            }
            do
            {
                try
                {
                    Parallel.ForEach(world.Cast<Creature>(),
                    creature =>
                    {
                        creature.DoStep();
                    });
                    Console.Clear();
                    foreach (Creature creature in world)
                    {

                        int xx = (int)Math.Ceiling(creature.x * 50.0);
                        int yy = (int)Math.Ceiling(creature.y * 50.0);

                        Console.SetCursorPosition(xx, yy);
                        Console.ForegroundColor = creature.powerIndex switch
                        {
                            0 => ConsoleColor.White,
                            1 => ConsoleColor.Green,
                            2 => ConsoleColor.Cyan,
                            3 => ConsoleColor.Blue,
                            4 => ConsoleColor.Yellow,
                            5 => ConsoleColor.Magenta,
                            _ => ConsoleColor.Red
                        };
                        Console.Write(creature.ident);
#if DEBUG
                        writer = new StreamWriter("..\\..\\Log.txt", true);
                        writer.Write($"\r\nid:{creature.id}," +
                            $" HL_size:{creature.network.HiddenLayers[0].Count}, " +
                            $"Last_error:{creature.network.LastError}");
                        writer.Close();
#endif
                    }
                    if (rnd.NextDouble() < 0.0333)
                    {
                        Creature newLife = new Creature(world, rnd.NextDouble(), rnd.NextDouble(),
                            Creature.identSymbols[rnd.Next(0, Creature.identSymbols.Length)]);
                        world.Add(newLife);
                    }
                }
                catch (Exception esx)
                {
                    //Console.SetCursorPosition(0, 6);
                    //Console.Write(esx.Message + esx.Source + esx.StackTrace);
                }
            }
            while (!stop);
        }
    }
}
