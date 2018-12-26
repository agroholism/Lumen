namespace StandartLibrary {
	internal class GotoE : System.Exception {
		public Value[] result;

		public GotoE(Value[] result) {
			this.result = result;
		}
	}
}