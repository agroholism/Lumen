using System;

namespace Lumen.Lang.Std {
	/// <summary> Экземпляр типа Kernel.String. </summary>
	public class KString : Value, IFormattable {
		internal String innerValue;

		/// <summary> Empty string. </summary>
		public static KString Empty => String.Empty;

		public override String ToString() {
			return this.ToString(null);
		}
		#region HEnumerable
		/*
		public Value this[Int32 first, Int32 second, Boolean flatten] {
			get {
				int i = first;
				int j = second;

				if (first < 0)
					first = value.Length + first;

				if (second < 0)
					second = value.Length + second;

				if (second != value.Length)
					second++;

				if (first > value.Length || second > value.Length || first < 0 || second < 0)
					throw new HException("выход за пределы строки при срезе вида [i:j]: границы требуемого диапазонa [" + i + ":" + j + "] превышают длину строки [" + value.Length + "]", "IndexException");

				if (first >= second)
					return new KString("");

				return new KString(value.Substring(first, second - first + 1));
			}
			set {
				int i = first;
				int j = second;

				if (first < 0)
					first = this.value.Length + first;

				if (second < 0)
					second = this.value.Length + second;

				if (i == 0)
					if (second != this.value.Length)
						second++;

				if (first > this.value.Length || second > this.value.Length)
					throw new HException("выход за пределы списка при срезе вида [i:j]: границы требуемого диапазонa [" + i + ":" + j + "] превышают длину списка [" + this.value.Length + "]", "IndexException");

				if (second + 1 > this.value.Length)
					this.value = this.value.Substring(0, first) + value.ToString();
				else
					this.value = this.value.Substring(0, first) + value.ToString() +
						this.value.Substring(second + 1, this.value.Length - second - 1);
			}
		}

		public Value this[Int32 index, Boolean flatten] {
			get {
				int i = index;
				index = index < 0 ? value.Length + index : index;

				if (index >= value.Length || index < 0)
					throw new HException("выход за пределы строки при срезе вида [i]. Требуемый индекс [" + i + "] превышает длину строки [" + value.Length + "]", "IndexException");

				return new KString(value[index].ToString());
			}
			set {
				int i = index;
				index = index < 0 ? this.value.Length + index : index;

				if (index == this.value.Length)
					this.value = this.value + value.ToString();
				else if (index == -1)
					this.value = value.ToString() + this.value;
				else if (index < this.value.Length && index > 0)
					this.value = this.value.Substring(0, index) + value.ToString() + this.value.Substring(index + 1, this.value.Length - index - 1);
				else
					throw new HException("выход за пределы строки при установке вида [i]. Требуемый индекс [" + i + "] превышает длину строки [" + this.value.Length + "]", "IndexException");

			}
		}

		/// <summary> Изменяет символ на позиции index подстрокой. </summary> <param name="index"> Позиция символа. </param> <returns> Значение типа String. </returns>
		public KString this[Int32 index] {
			get {
				index = index < 0 ? value.Length + index : index;

				if (index >= value.Length || index < 0)
					throw new Exception("выход за пределы списка при срезе вида [i]: требуемый индекс [" + (index < 0 ? index + value.Length : index) + "] превышает длину списка [" + value.Length + "]");

				return new KString(value[index].ToString());
			}

			set {
				index = index < 0 ? this.value.Length + index : index;

				if (index > this.value.Length)
					throw new Exception("выход за пределы списка при срезе вида [i]: требуемый индекс [" + index + "] превышает длину списка [" + this.value.Length + "]");

				if (index == this.value.Length) {
					this.value = this.value + value;
					return;
				}

				StringBuilder a = new StringBuilder();

				a.Append(this.value.Substring(0, index));
				a.Append(value);
				a.Append(this.value.Substring(index + 1, this.value.Length - index - 1));

				this.value = a.ToString();
			}
		}

		public Value this[String what] {
			get => new Fix(value.IndexOf(what));

			set {
				if (what == "")
					this.value = string.Join("", this.value.Select(x => x.ToString() + value));
				else
					this.value = this.value.Replace(what, value.ToString());
			}
		}
		*/
		#endregion

		public KString(String value) {
			this.innerValue = value;
		}

		public static implicit operator KString(String value) {
			return new KString(value);
		}

		public String ToString(Scope e) {
			return this.innerValue;
		}

		public KType Type => StandartModule.String;

		#region Service

		public override Boolean Equals(Object obj) {
			if (obj is KString str) {
				return this.innerValue == str.innerValue;
			}
			return false;
		}

		public override Int32 GetHashCode() {
			return this.innerValue.GetHashCode();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.innerValue.ToString(formatProvider);
		}

		public Int32 GetLength() {
			return innerValue.Length;
		}

		public Int32 CompareTo(Object obj) {
			if (obj is Value) {
				Scope s = new Scope(null);
				s.This = this;
				//return (int)Converter.ToDouble(((Fun)Type.Get("<=>", null)).Run(s, (Value)obj), null);
			}

			throw new Exception("невозможно сравнить значения заданных типов");
		}

		public Value Clone() {
			return (Value)this.MemberwiseClone();
		}

		#endregion
	}
}
