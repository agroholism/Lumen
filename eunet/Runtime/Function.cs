namespace Argent.Xenon.Runtime {
	public interface Function : ImmutObject {
		XnObject Run(Scope scope, params XnObject[] arguments);
	}
}
