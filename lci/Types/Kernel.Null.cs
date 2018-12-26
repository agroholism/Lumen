using System;

namespace StandartLibrary {
	[Serializable]
	internal sealed class NullClass : KType {
		internal NullClass() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
			Name = "Kernel.Null",
				//BaseType = StandartModule.Object
			};
		}
	}
}
