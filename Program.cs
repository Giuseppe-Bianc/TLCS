using BenchmarkDotNet.Running;
using System.Text;

namespace TLCS {
	public static class Program {
		static void Main(string[] args) {
			string input = "3.133  + 3.133E-1 + 3.133e-1 + 3.133E+2 + 3.133e-2 + 4 * (2 - 1)^2";
			//string input = "#a222 + #1a.22a";
			Lexer lexer = new(input);
			List<Token> tokens = lexer.GetTokens();

			// Use string.Join to optimize printing
			string tokensString = string.Join(Environment.NewLine, tokens);

			// Print all tokens at once
			Console.WriteLine(tokensString);


			Console.WriteLine("Premi Invio per continuare...");
			if (Console.ReadLine().Equals("e")) {
				Environment.Exit(0);
			} else {
				Console.Clear();
			}
			var summary = BenchmarkRunner.Run<Bencmarks>();
		}
	}
}