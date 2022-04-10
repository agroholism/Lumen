namespace Lumen.Lang {
	public interface IConstructor : IType {
		Module Parent { get; }

		IValue MakeInstance(params IValue[] values);
	}
}
