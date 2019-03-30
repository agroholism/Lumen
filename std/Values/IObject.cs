using System;

namespace Lumen.Lang {
    /// <summary> Интерфейс, реализуемый всеми объектами. </summary>
    public interface IObject : Value {
        IObject Parent { get; set; }

        /// <summary> Устанавливает поле объекта. </summary> 
        /// <param name="name"> Имя поля. </param> 
        /// <param name="value"> Значение. </param>
        /// <param name="scope"> Область видимости, из которой осуществляется доступ. </param>
        void SetField(String name, Value value, Scope scope);

        Value GetField(String name, Scope scope);

        Boolean TryGetField(String name, out Value result);

        Boolean IsParentOf(Value value);
    }
}
