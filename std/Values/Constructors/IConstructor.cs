namespace Lumen.Lang {
	public interface IConstructor : IType {
		Type Parent { get; }

		Value MakeInstance(params Value[] values);
	}
}
