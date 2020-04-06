namespace Argent.Xenon.Runtime {
	public interface XnObject {
		KsTypeable Type { get; }
	}

	public interface ImmutObject : XnObject {

	}
}
