using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public static class Converter {
		public static Boolean ToBoolean(this Value value) {
			if (value is Bool b) {
				return b.value;
			}

			if (value == Prelude.None) {
				return false;
			}

			return true;
		}

		public static BigFloat ToBigFloat(this Value value, Scope scope) {
			if (value is BigNumber big) {
				return big.value;
			}

			if (value is Number num) {
				return num.value;
			}

			return ToDouble(ToNum(value, scope), scope);
		}


		public static Double ToDouble(this Value value, Scope scope) {
			if (value is Number num) {
				return num.value;
			}

			return ToDouble(ToNum(value, scope), scope);
		}

		public static Int32 ToInt(this Value value, Scope scope) {
			if (value is Number num) {
				return (Int32)num.value;
			}

			return ToInt(ToNum(value, scope), scope);
		}

		public static Number ToNum(this Value value, Scope scope) {
			if (value is Number num) {
				return num;
			}

			IType type = value.Type;

			if (type.TryGetMember("toNumber", out Value converterPrototype)
					&& converterPrototype is Fun converter) {
				return ToNum(converter.Run(new Scope(scope), value), scope);
			}

			throw new LumenException(Exceptions.CONVERT_ERROR.F(value.Type, Prelude.Number));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="scope"></param>
		/// <returns></returns>
		/// <exception cref="LumenException">Can not convert value of type X to function</exception>
		public static Fun ToFunction(this Value value, Scope scope) {
			if (value is Fun fun) {
				return fun;
			}

			if (value is Module module && module.TryGetMember("<init>", out Value init)) {
				return init.ToFunction(scope);
			}

			throw new LumenException(Exceptions.CONVERT_ERROR.F(value.Type, Prelude.Function));
		}

		public static Dictionary<Value, Value> ToMap(this Value value, Scope e) {
			if (value is Map map) {
				return map.value;
			}

			IType type = value.Type;

			if (type.TryGetMember("toMap", out Value converterPrototype) && converterPrototype is Fun converter) {
				return ToMap(converter.Run(new Scope(e), value), e);
			}

			throw new LumenException(Exceptions.CONVERT_ERROR.F(value.Type, Prelude.Map));
		}

		public static Array ToArray(this Value value, Scope e) {
			if (value is Array klist) {
				return klist;
			}

			if (value is IType iobj) {
				if (iobj.TryGetMember("vec", out Value f)) {
					Scope s = new Scope(e) {
						["this"] = value
					};
					return ToArray((f as Fun).Run(s), e);
				}

				throw new LumenException($"невозможно преобразовать объект к объекту типа Kernel.List");
			}

			IType type = value.Type;

			if (type.TryGetMember("vec", out Value prf) && prf is Fun fun) {
				Scope s = new Scope(e) {
					["this"] = value
				};
				return ToArray(fun.Run(s), e);
			}

			throw new LumenException(Exceptions.CONVERT_ERROR.F(value.Type, Prelude.Array));
		}

		internal static LinkedList ToLinkedList(this Value value, Scope e) {
			if (value is Lazy lazy) {
				return lazy.Force().ToLinkedList(e);
			}

			if (value is List list) {
				return list.value;
			}

			if (value is Stream en) {
				return LinkedList.Create(en);
			}

			if (value is Array a) {
				return LinkedList.Create(a.InternalValue);
			}

			if ((value.Type as Constructor).Parent.HasMixin(Prelude.Collection)) {
				Fun f = (value.Type as Constructor).Parent.GetMember("toList", e) as Fun;
				return ToLinkedList(f.Run(new Scope(e), value), e);
			}

			throw new LumenException($"невозможно преобразовать объект типа {value.Type} к объекту типа Kernel.List");
		}

		public static List<Value> ToList(this Value value, Scope e) {
			if (value is Array klist) {
				return klist.InternalValue;
			}

			IType type = value.Type;

			if (type.TryGetMember("vec", out Value prf) && prf is Fun fun) {
				Scope s = new Scope(e) {
					["this"] = value
				};
				return ToList(fun.Run(s), e);
			}
			else if (type.TryGetMember("seq", out Value prf1) && prf1 is Fun fun1) {
				Scope s = new Scope(e) {
					["this"] = value
				};
				return ToStream(fun1.Run(s), e).ToList();
			}

			throw new LumenException($"невозможно преобразовать объект типа {type} к объекту типа Kernel.List");
		}

		public static IEnumerable<Value> ToStream(this Value val, Scope e) {
			if (val is Stream seq) {
				foreach (Value i in seq.innerValue) {
					yield return i;
				}
			}
			else if (val is IEnumerable<Value> iev) {
				foreach (Value i in iev) {
					yield return i;
				}
			}
			else if (val is List l) {
				foreach (Value i in l.value) {
					yield return i;
				}
			}
			else if (val is Array a) {
				foreach (Value i in a.InternalValue) {
					yield return i;
				}
			}
			else if (val is Text t) {
				foreach (Char i in t.value) {
					yield return new Text(i.ToString());
				}
			}
			else if (val is Lazy lazy) {
				foreach (var i in lazy.Force().ToStream(e)) {
					yield return i;
				}
			}
			else {
				if (val is IType iobj) {
					if (iobj.TryGetMember("toStream", out Value f)) {
						foreach (Value i in ToStream((f as Fun).Run(e, val), e)) {
							yield return i;
						}

						yield break;
					}
				}

				if (val.Type.TryGetMember("toStream", out Value prf) && prf is Fun fun) {
					foreach (Value i in ToStream(fun.Run(new Scope(e), val), e)) {
						yield return i;
					}
				}
			}
		}

		public static Number ToValue(Int32 value) {
			return new Number(value);
		}

		public static Number ToValue(Double value) {
			return new Number(value);
		}

		public static Text ToValue(String value) {
			return new Text(value);
		}

		static String bukv = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		#region BaseConverter

		#endregion

		/// <summary>
		/// Переводит из деятичной системы счисления в систему счисления с основанием N
		/// </summary>
		/// <param name="number">Число, которое переводим </param>
		/// <param name="sys">Система счисления, в которую переводим</param>
		/// <returns>Возвращает переведенное число в строковом формате</returns>
		public static String ToN(String number, String sys) {
			String newNum = "";
			Int32 num = Convert.ToInt32(number);
			Int32 chast = Convert.ToInt32(number);
			List<Int32> numTemp = new List<Int32>();
			while (chast > 0) {
				chast = chast / Convert.ToInt32(sys);
				numTemp.Add(num - chast * Convert.ToInt32(sys));
				num = chast;
			}
			Int32 j;
			for (j = numTemp.Count - 1; j >= 0; j--) {
				newNum += newCh(numTemp[j].ToString(), "to");
			}

			return newNum;
		}
		/// <summary>
		/// Функция, заменяет буквы на числа и наоборот
		/// </summary>
		/// <param name="sym">Число, над которым нужно работать</param>
		/// <param name="otk">В какую сторону осуществляется действие относительно десятичной системы счисления</param>
		/// <returns>Возвращает букву, если числу соответствует буква и наоборот, иначе число</returns>
		public static String newCh(String sym, String otk) {
			String s = "";
			if (otk == "to") {
				if (Convert.ToInt32(sym) > 10) {
					s += bukv.Substring(Convert.ToInt32(sym) - 10, 1);
				}
				else {
					s += sym;
				}
			}
			else if (otk == "from") {
				if (bukv.IndexOf(sym) == -1) {
					s += sym;
				}
				else {
					s += (bukv.IndexOf(sym) + 10).ToString();
				}
			}
			return s;
		}

		/// <summary>
		/// Переводит системы счисления с основанием N в деятичную систему счисления 
		/// </summary>
		/// <param name="number">Число, которое переводим </param>
		/// <param name="sys">Система счисления, из которой переводим</param>
		/// <returns>Возвращает переведенное число в строковом формате</returns>
		public static String FromN(String number, String sys) {
			Int32 newNum = 0;
			String temp = "";
			Int32 t;
			Int32 i;
			for (i = 0; i < number.Length; i++) {
				temp = "";
				temp += newCh(number.Substring(i, 1), "from");
				t = (Int32)System.Math.Pow(Convert.ToDouble(sys), Convert.ToDouble(number.Length - (i + 1)));
				newNum += Convert.ToInt32(temp) * t;
			}
			return newNum.ToString();
		}

		public static String FromTo(String number, String sysN, String sysK) {
			String temp = "";
			temp = FromN(number, sysN);
			temp = ToN(temp, sysK);
			return temp;
		}
	}
}
