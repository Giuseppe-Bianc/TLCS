using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Text;

namespace TLCS {
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[RankColumn]
	[StopOnFirstError]
	public class Bencmarks {
		[Params(4, 5)]
		public int COUNT { get; set; }
		private const string input = "3.133  + 3.133E-1 + 3.133e-1 + 3.133E+2 + 3.133e-2 + 4 * (2 - 1)^2";
		private Lexer lexer;
		[GlobalSetup]
		public void Setup() {
			StringBuilder inp = new();
			switch (COUNT) {
				case 4:
					for (int i = 0; i < 4; ++i) {
						inp.AppendLine(input);
					}
					break;
				case 5:
					for (int i = 0; i < 5; ++i) {
						inp.AppendLine(input);
					}
					break;
			}

			lexer = new Lexer(inp.ToString());
		}

		[Benchmark(Baseline=true)]
		public void LexerPerf() {
			lexer.GetTokens();
		}

		/*[Benchmark]
		public void LexerModPerf() {
			lexer.GetTokensMod();
		}*/

	}
}
