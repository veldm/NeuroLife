using NeuralNetwork;
using System;
using System.Collections.Generic;

namespace NeuroLife
{
    /// <summary>
    /// Представляет простейшее существо
    /// </summary>
    public class Creature
    {
        #region Поля и свойства
        public static string identSymbols = "▼▲☼♣#$%+@XO☆©µ¶þ§±ň";

        /// <summary>
        /// Опыт (кол-во встреч)
        /// </summary>
        public int experienceIndex = 0;

        /// <summary>
        /// Сила (кол-во съеденых сородичей)
        /// </summary>
        public int powerIndex = 0;

        double sensorLeft, sensorBottom, sensorRight, sensorTop; //Данные сенсоров
        double moverLeft, moverBottom, moverRight, moverTop; // Инструкции для движителей

        List<double[]> Memory = new List<double[]>();
        List<double[]> MemorySense = new List<double[]>();

        int step = 0;
        int stepmax = 0;

        // Координаты
        public double x { get; set; }
        public double y { get; set; }

        /// <summary>
        /// Ближайший сосед
        /// </summary>
        Creature nearestNeighbour;

        // Идентификаторы
        public string id = Guid.NewGuid().ToString();
        public char ident;

        /// <summary>
        /// Собственно нейросеть<para/>
        /// "Мозг" существа
        /// </summary>
        public NeuralNet network;

        readonly Random rnd = new Random((int)(DateTime.Now.Ticks % int.MaxValue));

        /// <summary>
        /// Окружающий существо мир
        /// </summary>
        List<Creature> World;
        #endregion

        /// <summary>
        /// Конструктор<para/>
        /// Создаёт новый экземпляр класса <see cref="Creature"/> с заданными параметрами
        /// </summary>
        /// <param name="world"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="_ident"></param>
        public Creature(List<Creature> world, double X, double Y, char _ident)
        {
            x = X;
            y = Y;
            ident = _ident;
            moverLeft = moverRight = moverBottom = moverTop = 0.0;
            sensorLeft = sensorRight = sensorBottom = sensorTop = 0.0;
            network = new NeuralNet(4, rnd.Next(10), 4, 5);
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer(null, true, 4));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), true, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), false, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), true, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), false, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), true, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), false, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), true, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), false, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), true, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), false, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), true, rnd.Next(100) + 10));
            //network.AddLayer(new Encog.Neural.Networks.Layers.BasicLayer
            //    (new Encog.Engine.Network.Activation.ActivationSigmoid(), false, 4));
            //network.Structure.FinalizeStructure();
            //network.Reset();
            World = world;
        }

        public void DoStep()
        {
            WatchOut(); // Оглядеться
            Move(); // Сделать шаг
            step++;
            if (step > stepmax)
            {
                int maxstep = 5 - (int)(sensorLeft + sensorRight + sensorBottom + sensorTop);
                stepmax = rnd.Next(40 / (maxstep));
                step = 0;
                Memorization(); // Запоминть свои действия и состояние окружающего мира
                ExperienceComprehanding(); // Осмыслить свой опыт и скорректировать стратегию поведения
            }

            // Принять решение о направлении следующего шага исходя из данных об окружении
            double[] Data = { sensorLeft, sensorRight, sensorBottom, sensorTop };
            double[] output = network.Compute(Data);
            moverLeft = output[0];
            moverRight = output[1];
            moverBottom = output[2];
            moverTop = output[3];
            //double[] Input = new double[] { 0, 0, 0, 0 };
            //DataSet trainingSet;
            //if (nearestNeighbour.powerIndex > powerIndex)
            //{
            //    double[] SenseData = new double[] { 0, 0, 0, 0 };
            //    trainingSet = new DataSet(Input, SenseData);
            //}
            //else
            //{
            //    double[] SenseData = new double[] { 1, 1, 1, 1 };
            //    trainingSet = new DataSet(Input, SenseData);
            //}


        }

        /// <summary>
        /// Получение актуальной информации об окружающем мире
        /// </summary>
        void WatchOut()
        {
            while (true)
            {
                double mind = double.MaxValue;

                try
                {
                    foreach (Creature life in World)
                    {
                        if (life.id != this.id)
                        {
                            var d = GetDist(life);
                            if (d < mind)
                            {
                                mind = d;
                                nearestNeighbour = life;
                                if (d < 0.1)
                                {
                                    if (rnd.NextDouble() > 0.5)
                                    {
                                        powerIndex++;
                                        if (powerIndex > 10)
                                        {
                                            powerIndex = 10;
                                        }
                                        nearestNeighbour.powerIndex++;
                                        if (nearestNeighbour.powerIndex > 10)
                                        {
                                            nearestNeighbour.powerIndex = 10;
                                        }

                                        lock (World)
                                        {
                                            if (!World.Contains(this))
                                            {
                                                continue;
                                            }
                                            if (nearestNeighbour.powerIndex < powerIndex)
                                            {
                                                World.Remove(nearestNeighbour);
                                            }
                                            else if (powerIndex < nearestNeighbour.powerIndex)
                                            {
                                                World.Remove(this);
                                            }
                                            else
                                            {
                                                if (rnd.NextDouble() > 0.5)
                                                    World.Remove(this);
                                                else World.Remove(nearestNeighbour);
                                            }
                                        }
                                        continue;
                                    }
                                    experienceIndex++;
                                    if (experienceIndex > 5)
                                    {
                                        experienceIndex = 5;
                                    }
                                    nearestNeighbour.experienceIndex++;
                                    if (nearestNeighbour.experienceIndex > 5)
                                    {
                                        nearestNeighbour.experienceIndex = 5;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
                break;
            }

            if (nearestNeighbour != null)
            {
                sensorLeft = GetDistFromCoord(x - 0.05, nearestNeighbour.x, y, nearestNeighbour.y);
                sensorRight = GetDistFromCoord(x + 0.05, nearestNeighbour.x, y, nearestNeighbour.y);
                sensorBottom = GetDistFromCoord(x, nearestNeighbour.x, y - 0.05, nearestNeighbour.y);
                sensorTop = GetDistFromCoord(x, nearestNeighbour.x, y + 0.05, nearestNeighbour.y);
            }

            double GetDist(Creature l1)
            {
                return GetDistFromCoord(l1.x, x, l1.y, y);
            }

            static double GetDistFromCoord(double x1, double x2, double y1, double y2)
            {
                return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }
        }

        /// <summary>
        /// Перемещение исходя из принятого решения
        /// </summary>
        void Move()
        {
            if (moverLeft > 0.01)
            {
                moverLeft = 0.01;
            }
            else if (moverLeft < 0)
            {
                moverLeft = 0;
            }

            if (moverRight > 0.01)
            {
                moverRight = 0.01;
            }
            else if (moverRight < 0)
            {
                moverRight = 0;
            }

            if (moverBottom > 0.01)
            {
                moverBottom = 0.01;
            }
            else if (moverBottom < 0)
            {
                moverBottom = 0;
            }

            if (moverTop > 0.01)
            {
                moverTop = 0.01;
            }
            else if (moverTop < 0)
            {
                moverTop = 0;
            }

            x += moverLeft - moverRight;
            y += moverBottom - moverTop;

            if (x < 0)
            {
                //x = 1 - x;
                x = 0;
            }

            if (y < 0)
            {
                //y = 1 - y;
                y = 0;
            }

            if (x > 1)
            {
                //x = x - 1;
                x = 1;
            }

            if (y > 1)
            {
                //y = y - 1;
                y = 1;
            }
        }

        /// <summary>
        /// Запоминание — накопление жизненного опыта
        /// </summary>
        void Memorization()
        {
            double[] Data = { sensorLeft, sensorRight, sensorBottom, sensorTop };
            double[] SenseData = { moverLeft, moverRight, moverBottom, moverTop };
            Memory.Add(Data);
            MemorySense.Add(SenseData);

            // Удаляем старые данные (для борьбы с переобучением)
            if (Memory.Count > 100/*0*/)
            {
                // Если весь опыт существа — это просто блуждание в простанстве без встреч
                // с другими существами, то его можно положить бесполезным и обнулить
                if (experienceIndex == 0)
                {
                    World.Remove(this);
                    Creature newlife = new Creature(World, x, y, ident);
                    World.Add(newlife);
                }
                Memory.RemoveAt(0);
                MemorySense.RemoveAt(0);
            }
        }

        /// <summary>
        /// Осмысление опыта (обучение НС)
        /// </summary>
        void ExperienceComprehanding()
        {
            if (Memory.Count > 0)
            {
                //network.Reset();
                //double[][] InputData = new double[Memory.Count][];
                //double[][] SenseData = new double[Memory.Count][];
                //for (int i = 0; i < Memory.Count; i++)
                //{
                //    InputData[i] = Memory[i];
                //    SenseData[i] = MemorySense[i];
                //}

                // Создаём тренировочные наборы
                List<DataSet> trainingSets = new List<DataSet>();
                for (int i = 0; i < Memory.Count; i++)
                {
                    trainingSets.Add(new DataSet(Memory[i], MemorySense[i]));
                }
                //IMLDataSet trainingSet =
                //    new Encog.ML.Data.Basic.BasicMLDataSet(InputData, SenseData);
                //IMLTrain train = new Encog.Neural.Networks.Training.
                //    Propagation.Resilient.ResilientPropagation(network, trainingSet);

                //int epoch = 1;

                // Прогоняем НС по всем созданым наборам
                network.Train(trainingSets, 0.01);
                //double old = 9999, delta;
                //do
                //{
                //    train.Iteration();
                //    epoch++;
                //    delta = Math.Abs(old - train.Error);
                //    old = train.Error;
                //}
                //while (train.Error > 0.0001 && epoch < 3000 && delta > 0.00001);

                //train.FinishTraining();

                //double sumd = 0;
                //foreach (IMLDataPair pair in trainingSet)
                //{
                //    IMLData output = network.Compute(pair.Input);
                //    sumd += Math.Abs(pair.Ideal[0] - output[0]);
                //    sumd /= trainingSet.InputSize;
                //}
            }
        }
    }
}
