using System;

namespace QFramework
{
	/// <summary>
	/// Summary description for Program.
	/// </summary>
	public class Program
	{
		public Program()
		{
		}

		public static void Main()
		{

			for (int i = 2; i<= 170; i++)
			{
				BinomialTree tree = new BinomialTree(100,95,0.5,.3,.08,EPutCall.Put,i);
				decimal presentValue = Convert.ToDecimal(tree.OptionValue());
				System.Diagnostics.Debug.WriteLine(string.Format("{0},{1}",i.ToString(),presentValue.ToString()));
			}
         }
	}
}
