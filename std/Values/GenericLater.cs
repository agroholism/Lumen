namespace Lumen.Lang {
	public class GenericLater : Type {
		public static Value Instance { get; private set; } = new GenericLater();

		public GenericLater() : base("<generic-later>") {

		}
	}
}
