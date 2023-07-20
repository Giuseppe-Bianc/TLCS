using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Text;

namespace TLCS {
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[RankColumn]
	[StopOnFirstError]
	[SimpleJob(warmupCount: 15, iterationCount: 50)]
	public class Bencmarks {
		[Params(1, 100)]
		public int COUNT { get; set; }
		private const string input = "3.133  + 3.133E-1 + 3.133e-1 + 3.133E+2 + 3.133e-2 + 4 * (2 - 1)^2";
		private Lexer lexer;

		public static void ConstructImput(int n, StringBuilder inp) {
			for (int i = 0; i < n; ++i) {
				inp.AppendLine(input);
			}
		}

		[GlobalSetup]
		public void Setup() {
			StringBuilder inp = new();
			switch (COUNT) {
				case 1:
					ConstructImput(1, inp);
					break;
				case 100:
					ConstructImput(100, inp);
					break;
			}

			lexer = new Lexer(inp.ToString());
		}

		[Benchmark(Baseline = true)]
		public void LexerPerf() {
			lexer.GetTokens();
		}

		[Benchmark]
		public void LexerModPerf() {
			lexer.GetTokensMod();
		}

	}
}
