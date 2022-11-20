using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
    public class Neuron
	{
		public List<Synapse> InputSynapses { get; set; }
		public List<Synapse> OutputSynapses { get; set; }
		public double Bias { get; set; }
		public double BiasDelta { get; set; }
		public double Gradient { get; set; }
		public double Value { get; set; }
        public IActivationFunction ActivationFunction { get; }

        public Neuron()
		{
			InputSynapses = new List<Synapse>();
			OutputSynapses = new List<Synapse>();
			Bias = NeuralNet.GetRandom();
		}

		public Neuron(IEnumerable<Neuron> inputNeurons,
			IActivationFunction activationFunction) : this()
		{
			foreach (var inputNeuron in inputNeurons)
			{
				var synapse = new Synapse(inputNeuron, this);
				inputNeuron.OutputSynapses.Add(synapse);
				InputSynapses.Add(synapse);
			}
            ActivationFunction = activationFunction;
        }

		public virtual double CalculateValue()
		{
			return Value = ActivationFunction.Output(InputSynapses.Sum(a =>
				a.Weight * a.InputNeuron.Value) + Bias);
		}

		public double CalculateError(double target)
		{
			return target - Value;
		}

		public double CalculateGradient(double? target = null)
		{
			if (target == null)
				return Gradient = OutputSynapses.Sum(a => a.OutputNeuron.Gradient * a.Weight)
					* ActivationFunction.Derivative(Value);

			return Gradient = CalculateError(target.Value) *
				ActivationFunction.Derivative(Value);
		}

		public void UpdateWeights(double learnRate, double momentum)
		{
			var prevDelta = BiasDelta;
			BiasDelta = learnRate * Gradient;
			Bias += BiasDelta + momentum * prevDelta;

			foreach (var synapse in InputSynapses)
			{
				prevDelta = synapse.WeightDelta;
				synapse.WeightDelta = learnRate * Gradient * synapse.InputNeuron.Value;
				synapse.Weight += synapse.WeightDelta + momentum * prevDelta;
			}
		}

	}

}
