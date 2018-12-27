using System;

namespace Lumen.Lang.Std {
	/// <summary> Интерфейс, реализуемый всеми объектами. </summary>
	public interface IObject : Value {
		/// <summary> Возвращает поле объекта </summary> 
		/// <param name="name"> Имя поля </param> 
		/// <param name="mode"> Модификатор доступа </param>
		/// <param name="e"> Область видимости, из которой осуществляется доступ. </param>
		Value Get(String name, AccessModifiers mode, Scope e);

		/// <summary> Устанавливает поле объекта. </summary> 
		/// <param name="name"> Имя поля. </param> 
		/// <param name="value"> Значение. </param>
		/// <param name="mode"> Модификатор доступа. </param>
		/// <param name="e"> Область видимости, из которой осуществляется доступ. </param>
		void Set(String name, Value value, AccessModifiers mode, Scope e);
	}
}
