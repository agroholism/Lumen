using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StandartLibrary {
	public static class Converter {
		public static List<Value> ToList(this Value value) {
			return ToList(value, null);
		}

		public static Boolean ToBoolean(this Value value) {
			if(value is Bool b) {
				return b.value;
			}

			if(value is Null) {
				return false;
			}

			return true;
		}

		public static Fun ToFunction(this Value value, Scope scope) {
			if(value is Fun fun) {
				return fun;
			}
			throw new Exception("невозможно преобразовать значение типа " + value.Type.meta.Name + " в значение типа Kernel.Function", stack: scope);
		}

		public static BigFloat ToBigFloat(this Value value, Scope scope) {
			if(value is Num n) {
				return n.value;
			}
			throw new Exception("невозможно преобразовать значение типа " + value.Type.meta.Name + " в значение типа Kernel.Fix", stack: scope);
		}

		public static Dictionary<Value, Value> ToMap(this Value value, Scope e) {
			if (value is Map klist) {
				return klist.value;
			}

			KType type = value.Type;

			if (type.AttributeExists("map") && type.GetAttribute("map", e) is Fun fun) {
				Scope s = new Scope(e);
				s.This = value;

				return ToMap(fun.Run(s), e);
			}

			throw new Exception($"невозможно преобразовать объект типа {type.meta.Name} к объекту типа Kernel.List", stack: e);
		}

		public static List<Value> ToList(this Value value, Scope e) {
			if (value is Vec klist) {
				return klist.value;
			}

			KType type = value.Type;

			if (type.AttributeExists("vec") && type.GetAttribute("vec", e) is Fun fun) {
				Scope s = new Scope(e);
				s.This = value;
				return ToList(fun.Run(s), e);
			}
			else if (type.AttributeExists("seq") && type.GetAttribute("seq", e) is Fun fun1) {
				Scope s = new Scope(e);
				s.This = value;
				return ToIterator(fun1.Run(s), e).ToList();
			}

			throw new Exception($"невозможно преобразовать объект типа {type.meta.Name} к объекту типа Kernel.List", stack: e);
		}

		public static IEnumerable<Value> ToIterator(this Value val, Int32 count, Scope e) {
			if (val is Enumerator seq) {
				foreach (Value i in seq.innerValue) {
					yield return i;
				}
			}
			else {
				if (val.Type.AttributeExists("seq") && val.Type.GetAttribute("seq", e) is Fun fun) {
					Scope s = new Scope(e);
					s.This = val;
					foreach (var i in ToIterator(fun.Run(s, new Num(count)), count, e)) {
						yield return i;
					}
				}
			}
		}

		public static IEnumerable<Value> ToIterator(this Value val, Scope e) {
			if (val is Enumerator seq) {
				foreach (Value i in seq.innerValue) {
					yield return i;
				}
			}
			else {
				if (val.Type.AttributeExists("seq") && val.Type.GetAttribute("seq", e) is Fun fun) {
					Scope s = new Scope(e);
					s.This = val;
					foreach (Value i in ToIterator(fun.Run(s), e)) {
						yield return i;
					}
				}
			}
		}

		static string bukv = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		#region BaseConverter

		#endregion

		/// <summary>
		/// Переводит из деятичной системы счисления в систему счисления с основанием N
		/// </summary>
		/// <param name="number">Число, которое переводим </param>
		/// <param name="sys">Система счисления, в которую переводим</param>
		/// <returns>Возвращает переведенное число в строковом формате</returns>
		public static string ToN(string number, string sys) {
			string newNum = "";
			int num = Convert.ToInt32(number);
			int chast = Convert.ToInt32(number);
			ArrayList numTemp = new ArrayList();
			while (chast > 0) {
				chast = chast / Convert.ToInt32(sys);
				numTemp.Add(num - chast * Convert.ToInt32(sys));
				num = chast;
			}
			int j;
			for (j = numTemp.Count - 1; j >= 0; j--)
				newNum += newCh(numTemp[j].ToString(), "to");
			return newNum;
		}
		/// <summary>
		/// Функция, заменяет буквы на числа и наоборот
		/// </summary>
		/// <param name="sym">Число, над которым нужно работать</param>
		/// <param name="otk">В какую сторону осуществляется действие относительно десятичной системы счисления</param>
		/// <returns>Возвращает букву, если числу соответствует буква и наоборот, иначе число</returns>
		public static string newCh(string sym, string otk) {
			string s = "";
			if (otk == "to") {
				if (Convert.ToInt32(sym) > 10)
					s += bukv.Substring(Convert.ToInt32(sym) - 10, 1);
				else
					s += sym;
			}
			else if (otk == "from") {
				if (bukv.IndexOf(sym) == -1)
					s += sym;
				else
					s += (bukv.IndexOf(sym) + 10).ToString();
			}
			return s;
		}


		/// <summary>
		/// Переводит системы счисления с основанием N в деятичную систему счисления 
		/// </summary>
		/// <param name="number">Число, которое переводим </param>
		/// <param name="sys">Система счисления, из которой переводим</param>
		/// <returns>Возвращает переведенное число в строковом формате</returns>
		public static string FromN(string number, string sys) {
			int newNum = 0;
			string temp = "";
			int t;
			int i;
			for (i = 0; i < number.Length; i++) {
				temp = "";
				temp += newCh(number.Substring(i, 1), "from");
				t = (int)System.Math.Pow(Convert.ToDouble(sys), Convert.ToDouble(number.Length - (i + 1)));
				newNum += Convert.ToInt32(temp) * t;
			}
			return newNum.ToString();
		}

		public static string FromTo(string number, string sysN, string sysK) {
			string temp = "";
			temp = FromN(number, sysN);
			temp = ToN(temp, sysK);
			return temp;
		}
	}
}
