namespace Lumen.Lang {
	public class GenericLater : Module {
		public static Value Instance { get; private set; } = new GenericLater();
	}
}
