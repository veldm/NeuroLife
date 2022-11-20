using System;

namespace NeuralNetwork
{
    public class Sigmoid : IActivationFunction
	{
		public double Output(double x)
		{
			return x < -45.0 ? 0.0 : x > 45.0 ? 1.0 : 1.0 / (1.0 + Math.Exp((float)-x));
		}

		public double Derivative(double x)
		{
			return x * (1 - x);
		}
	}

}
