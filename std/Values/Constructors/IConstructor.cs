namespace Lumen.Lang {
	public interface IConstructor : IType {
		Module Parent { get; }

		Value MakeInstance(params Value[] values);
	}
}
