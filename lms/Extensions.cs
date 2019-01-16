using System;
using System.Text;

namespace Lumen.Anatomy {
	public static class Extensions {
		public static void Print(String message, ConsoleColor color = ConsoleColor.Gray, Int32 ident = 0) {
			Console.ForegroundColor = color;
			Console.WriteLine("\t".Repeat(ident) + message);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static String Repeat(this String str, Int32 count) {
			StringBuilder builder = new StringBuilder();

			for(Int32 i = 0; i < count; i++) {
				builder.Append(str);
			}

			return builder.ToString();
		}
	}
}
