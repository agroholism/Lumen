namespace Lumen.Lang {
	public class GenericLater : Module {
		public static IValue Instance { get; private set; } = new GenericLater();
	}
}
