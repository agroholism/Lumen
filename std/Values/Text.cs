using System;

namespace Lumen.Lang {
    /// <summary> Экземпляр типа Kernel.String. </summary>
    public class Text : Value {
        internal String value;

        public IType Type => Prelude.Text;

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

				return new KString(value.SubString(first, second - first + 1));
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
					this.value = this.value.SubString(0, first) + value.ToString();
				else
					this.value = this.value.SubString(0, first) + value.ToString() +
						this.value.SubString(second + 1, this.value.Length - second - 1);
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
					this.value = this.value.SubString(0, index) + value.ToString() + this.value.SubString(index + 1, this.value.Length - index - 1);
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

				a.Append(this.value.SubString(0, index));
				a.Append(value);
				a.Append(this.value.SubString(index + 1, this.value.Length - index - 1));

				this.value = a.ToString();
			}
		}

		public Value this[String what] {
			get => new Fix(value.IndexOf(what));

			set {
				if (what == "")
					this.value = String.Join("", this.value.Select(x => x.ToString() + value));
				else
					this.value = this.value.Replace(what, value.ToString());
			}
		}
		*/
        #endregion

        public Text(String value) {
            this.value = value;
        }

        public static implicit operator String(Text value) {
            return new Text(value);
        }

        public override String ToString() {
            return this.value;
        }

        #region Service

        public override Boolean Equals(Object obj) {
            if (obj is Text String) {
                return this.value == String.value;
            }
            return false;
        }

        public override Int32 GetHashCode() {
            return this.value.GetHashCode();
        }

        public String ToString(String format, IFormatProvider formatProvider) {
            return this.value.ToString(formatProvider);
        }

        public Int32 GetLength() {
            return this.value.Length;
        }

        public Int32 CompareTo(Object obj) {
            if(obj is Text text) {
                return this.value.CompareTo(text.value);
            }

            throw new LumenException("невозможно сравнить значения заданных типов");
        }

        public Value Clone() {
            return (Value)this.MemberwiseClone();
        }

        #endregion
    }
}
