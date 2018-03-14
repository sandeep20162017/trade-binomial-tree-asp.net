using System;

namespace QFramework
{
	/// <summary>
	/// Represents a Cox-Ross-Rubenstein binomial tree option pricing calculator.  May be used for pricing European or American options
	/// </summary>
	public class BinomialTree
	{
		#region "Private Members"

		private double assetPrice = 0.0;
		private double strike = 0.0;
		private double timeStep = 0.0;
		private double volatility = 0.0;
		private EPutCall putCall = EPutCall.Call;
		
		private double riskFreeRate = 0.0;
		private int steps = 0;
		
		#endregion

		#region "Properties'
		public double AssetPrice
		{
			get { return assetPrice;}
			set { assetPrice = value; }
		}

		public double Strike
		{
			get { return strike; }
			set { strike = value; }
		}

		public double TimeStep 
		{
			get { return timeStep; }
			set { timeStep = value; }
		}

		public double Volatility
		{
			get { return volatility; }
			set { volatility = value; }
		}

		public EPutCall PutCall
		{
			get { return putCall; }
			set{ putCall = value; }
		}

		public double RiskFreeRate
		{
			get { return riskFreeRate; }
			set { riskFreeRate = value; }
		}

		public int Steps
		{
			get { return steps; }
			set  { steps = value; }
		}

		#endregion

		#region "Constructors"
		/// <summary>
		/// Empty constructor.  All properties may be set.
		/// </summary>
		public BinomialTree()
		{
		}

		/// <summary>
		/// Constructor that takes all parameters used for calculatin option value using binomial tree
		/// </summary>
		/// <param name="assetPriceParam"></param>
		/// <param name="strikeParam"></param>
		/// <param name="timeStepParam"></param>
		/// <param name="volatilityParam"></param>
		/// <param name="riskFreeRateParam"></param>
		/// <param name="putCallParam"></param>
		/// <param name="optionStyleParam"></param>
		/// <param name="stepsParam"></param>
		public BinomialTree(
			double assetPriceParam, 
			double strikeParam, 
			double timeStepParam, 
			double volatilityParam, 
			double riskFreeRateParam, 
			EPutCall putCallParam, 
			int stepsParam)
		{
			assetPrice = assetPriceParam;
			strike = strikeParam;
			volatility = volatilityParam;
			timeStep = timeStepParam;
			riskFreeRate = riskFreeRateParam;
			putCall = putCallParam;
			steps = stepsParam;
		}
		#endregion

		#region "Binomial Tree"
		/// <summary>
		/// Part of the binomial node value equation, represents the binomial coefficient
		/// </summary>
		/// <param name="m"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		private double BinomialCoefficient(int m, int n)
		{
			return Factorial(n) / (Factorial(m) * Factorial(n - m));
		}

		/// <summary>
		/// Calculates the value of an individual node in the binomial tree
		/// </summary>
		/// <param name="m"></param>
		/// <param name="n"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		private double BinomialNodeValue(int m, int n, double p)
		{
			return BinomialCoefficient(m, n) * Math.Pow(p, (double)m) * Math.Pow(1.0 - p, (double)(n - m));
		}

		/// <summary>
		/// Returns the present value of the option
		/// </summary>
		/// <returns></returns>
		public double OptionValue()
		{
			double totalValue = 0.0;
			double u = OptionUp(timeStep, volatility, steps);
			double d = OptionDown(timeStep, volatility, steps);
			double p = Probability(timeStep, volatility, steps, riskFreeRate);
			double nodeValue = 0.0;
			double payoffValue= 0.0;
			for (int j = 0; j <= steps; j++)
			{
				payoffValue = Payoff(AssetPrice * Math.Pow(u, (double)j) * Math.Pow(d, (double)(steps - j)), strike, putCall);
				nodeValue = BinomialNodeValue(j, steps, p);
				totalValue += nodeValue * payoffValue;
			}
			return PresentValue(totalValue, riskFreeRate, timeStep);
		}
		#endregion

		#region "Probabilities"
		private double OptionUp(double t, double s, int n)
		{
			return Math.Exp(s * Math.Sqrt(t / n));
		}

		private double OptionDown(double t, double s, int n)
		{
			return Math.Exp(-s * Math.Sqrt(t / n));
		}

		private double Probability(double t, double s, int n, double r)
		{
			double d1 = FutureValue(1.0, r, t / n);
			double d2 = OptionUp(t, s, n);
			double d3 = OptionDown(t, s, n);
			return (d1 - d3) / (d2 - d3);
		}
		#endregion
		
		#region "Payoffs"

		private double Payoff(double S, double X, EPutCall PutCall)
		{
			switch (PutCall)
			{
				case EPutCall.Call:
					return Call(S, X);

				case EPutCall.Put:
					return Put(S, X);

				default:
					return 0.0;
			}
		}

		private double Call(double S, double X)
		{
			return Math.Max(0.0, S - X);
		}

		private double Put(double S, double X)
		{
			return Math.Max(0.0, X - S);
		}
		#endregion

		#region "Financial Math Utility Functions"
		private double Factorial(int n)
		{
			double d = 1.0;
			for (int j = 1; j <= n; j++)
			{
				d *= j;
			}
			return d;
		}

		private double FutureValue(double P, double r, double n)
		{
			return P * Math.Pow(1.0 + r, n);
		}

		private double PresentValue(double F, double r, double n)
		{
			return F / Math.Exp(r * n);
		}
		#endregion
		
	}
} 
