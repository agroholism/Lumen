using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lumen.Lang.Std {
	internal sealed class StringClass : Record {
		internal StringClass() {
			this.meta = new TypeMetadata {
				Name = "Kernel.String"
			};

			#region operators

			SetAttribute(Op.PLUS, new LambdaFun((e, args) => {
				return new KString(e.Get("this").ToString(e) + args[0].ToString(e));
			}));
			SetAttribute("<<", new LambdaFun((e, args) => {
				KString obj = e.Get("this") as KString;
				obj.innerValue = obj.innerValue + args[0].ToString(e);
				return obj;
			}));
		/*	SetAttribute("@~", new LambdaFun((e, args) => {
				return new Regex(new System.Text.RegularExpressions.Regex(e.Get("this").ToString(e)));
			}));*/
			SetAttribute("*", new LambdaFun((e, args) => {
				String str = e.Get("this").ToString(e);
				Int32 count = (Int32)Converter.ToDouble(args[0], e);
				StringBuilder buffer = new StringBuilder();

				for (Int32 i = 0; i < count; i++) {
					buffer.Append(str);
				}

				return new KString(buffer.ToString());
			}));
			SetAttribute("@*", new LambdaFun((e, args) => {
				return new Num(e.Get("this").ToString(e).Length);
			}));
			SetAttribute("++", new LambdaFun((e, args) => {
				String value = e.Get("this").ToString();
				String chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
				Int32 last_char = value.Length - 1;
				while (true) {
					Int32 index = chars.IndexOf(value[last_char]);
					if (index == -1) {
						last_char--;
						if (last_char == -1) {
							return new KString(value.Substring(0, value.Length - 1) + (Char)(1 + value[value.Length - 1]));
						}
						continue;
					}
					while (true) {
						index = chars.IndexOf(value[last_char]);
						if (chars[index] == 'Z') {
							if (last_char == 0) {
								value = "AA" + value.Substring(1);
								break;
							}
							value = value.Substring(0, last_char) + "A" + value.Substring(last_char + 1);
							last_char--;
						}
						else if (chars[index] == 'z') {
							if (last_char == 0) {
								value = "aa" + value.Substring(1);
								break;
							}
							value = value.Substring(0, last_char) + "a" + value.Substring(last_char + 1);
							last_char--;
						}
						else if (chars[index] == '9') {
							if (last_char == 0) {
								value = "10" + value.Substring(1);
								break;
							}
							value = value.Substring(0, last_char) + "0" + value.Substring(last_char + 1);
							last_char--;
						}
						else {
							value = value.Substring(0, last_char) + (Char)(1 + chars[index]) + value.Substring(last_char + 1);
							break;
						}
					}
					break;
				}
				return new KString(value);
			}));
			SetAttribute("[]", new LambdaFun((e, args) => {
				String value = e.Get("this").ToString();
				if (args.Length == 1) {
					if (args[0] is Num) {
						Int32 index = (Int32)Converter.ToDouble(args[0], e);
						Int32 i = index;

						index = index < 0 ? value.Length + index : index;

						if (index >= value.Length || index < 0) {
							throw new Exception("выход за пределы строки при срезе вида [i]. Требуемый индекс [" + i + "] превышает длину строки [" + value.Length + "]", stack: e);
						}
						return new KString(GlobalizationEach(value).Skip(index).First().ToString());
						return new KString(value[index].ToString());
					}
					else if (args[0] is KString) {
						Int32 position = value.IndexOf(args[0].ToString());
						if (position == -1) {
							return Const.FALSE;
						}
						else {
							return new Num(position);
						}
					}
				/*	else if (args[0].Type.includedModules.IndexOf(StandartModule.Enumerable) != -1) {
						IEnumerable<Value> enumerator = Converter.ToIterator(args[0], 1, e);
						StringBuilder sb = new StringBuilder();
						foreach (Value i in enumerator) {
							if (i is Number fix) {
								sb.Append(value[(Int32)fix]);
							}
						}

						return new KString(sb.ToString());
					}*/
				}
				else if (args.Length == 2) {

				}
				return new Num(e.Get("this").ToString().Length);
			}));
			SetAttribute("@!", new LambdaFun((e, args) => {
				Char[] a = e.Get("this").ToString().ToCharArray();
				Array.Reverse(a);
				return new KString(new String(a));
			}));
			SetAttribute("=~", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();

				return new Bool(args[0].ToString() == e.Get("this").ToString());
			}));
			SetAttribute("==", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				if (!(args[0] is KString))
					return new Bool(false);

				return new Bool(args[0].ToString() == e.Get("this").ToString());
			}));
			SetAttribute("!=", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				if (!(args[0] is KString))
					return new Bool(true);

				return new Bool(args[0].ToString() != e.Get("this").ToString());
			}));
			SetAttribute("<", new LambdaFun((e, args) => {
				return new Bool(e.Get("this").ToString().CompareTo(args[0].ToString()) < 0);
			}));
			SetAttribute("<=", new LambdaFun((e, args) => {
				return new Bool(e.Get("this").ToString().CompareTo(args[0].ToString()) <= 0);
			}));
			SetAttribute(">", new LambdaFun((e, args) => {

				return new Bool(e.Get("this").ToString().CompareTo(args[0].ToString()) > 0);
			}));
			SetAttribute(">=", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				if (!(args[0] is KString))
					throw new Exception("правый операнд должен иметь тип Kernel.String", stack: e);

				return new Bool(e.Get("this").ToString().CompareTo(args[0].ToString()) >= 0);
			}));
			SetAttribute("<=>", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				if (!(args[0] is KString))
					throw new Exception("правый операнд должен иметь тип Kernel.String", stack: e);

				return new Num(e.Get("this").ToString(e).CompareTo(args[0].ToString(e)));
			}));
			/*SetAttribute("..", new LambdaFun((e, args) => {
				return Range.RANGE_CREATOR.Run(new Scope(), e.Get("this"), ((Fun)GetAttribute("++", e)).Run(new Scope { ["this"] = args[0] }));
			}));
			SetAttribute("...", new LambdaFun((e, args) => {
				return Range.RANGE_CREATOR.Run(new Scope(e), e.Get("this"), args[0]);
			}));*/
			SetAttribute("%", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				/*if (args[0].Type.includedModules.Contains(StandartModule.Enumerable)) {
					return new KString(String.Format(e.Get("this").ToString(), Converter.ToList(args[0], e).ToArray()));
				}
				else {*/
					return new KString(String.Format(e.Get("this").ToString(), args));
				//}
			}));
			SetAttribute("/", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				return new Vec(
					e.Get("this").ToString().Split(
						args[0].ToString().ToCharArray(),
						StringSplitOptions.RemoveEmptyEntries
					).Select(x => (Value)new KString(x)).ToList());
			}));

			#endregion

			SetAttribute("concat", new LambdaFun((e, args) => {
				String[] z = new String[args.Length + 1];

				z[0] = e.Get("this").ToString(e);
				Int32 index = 1;
				foreach (Value i in args) {
					z[index] = i.ToString(e);
					index++;
				}

				return new KString(String.Concat(z));
			}));
			SetAttribute("to_i", new LambdaFun((e, args) => {
				String v = e.Get("this").ToString();

				if (args.Length > 0) {
					Double wait = Converter.ToDouble(args[0], e);
					Int32 index = 0;
					if (wait == 2) {
						return new Enumerator(GlobalizationEach(v).Select(i => new Vec(new List<Value> { i, new Num(index++) })));
					}
					else if (wait > 2) {
						return new Enumerator(GlobalizationEach(v).Select(i => new Vec(new List<Value> { i, new Num(index++), new Num((Int32)i.ToString()[0]) })));
					}
				}

				return new Enumerator(GlobalizationEach(v));
			}));
			SetAttribute("to_s", new LambdaFun((e, args) => {
				return e.Get("this");
			}));
			SetAttribute("get_ascii?", new LambdaFun((e, args) => {
				return (Bool)e.Get("this").ToString(e).All(i => (Int32)i < 255);
			}));
			SetAttribute("get_upper", new LambdaFun((e, args) => {
				return new KString(e.Get("this").ToString().ToUpper());
			}));
			SetAttribute("get_lower", new LambdaFun((e, args) => {
				return new KString(e.Get("this").ToString().ToLower());
			}));
			SetAttribute("encode", new LambdaFun((e, args) => {

				return new Vec(Encoding.GetEncoding(args[0].ToString()).GetBytes(e.Get("this").ToString(e)).Select(i => (Value)(Num)i).ToList());
			}));
			/*Set("decode", new LambdaFun((e, args) => {
				return (KString)String.Join("", Encoding.GetEncoding(args[0].ToString()).GetChars(Converter.ToList(args[1], e).Select(i => (Byte)(Int32)(Num)i).ToArray()));
			}));*/
			SetAttribute("contains?", new LambdaFun((e, args) => {
				return new Bool(e.Get("this").ToString().Contains(args[0].ToString()));
			}));
			SetAttribute("index", new LambdaFun((e, args) => {
				return new Num(e.Get("this").ToString().IndexOfAny(args[0].ToString().ToCharArray()));
			}));
			SetAttribute("sub", new LambdaFun((e, args) => {
				return new KString(e.Get("this").ToString().Replace(args[0].ToString(), args[1].ToString()));
			}));
			SetAttribute("start_with?", new LambdaFun((e, args) => {
				return new Bool(e.Get("this").ToString().StartsWith(args[0].ToString()));
			}));
			SetAttribute("end_with?", new LambdaFun((e, args) => {
				return new Bool(e.Get("this").ToString().EndsWith(args[0].ToString()));
			}));
			SetAttribute("control?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsControl(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsControl(x))).ToList());
			}));
			SetAttribute("digit?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsDigit(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsDigit(x))).ToList());
			}));
			SetAttribute("letter?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsLetter(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsLetter(x))).ToList());
			}));
			SetAttribute("letter_or_digit?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsLetterOrDigit(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsLetterOrDigit(x))).ToList());
			}));
			SetAttribute("lower?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsLower(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsLower(x))).ToList());
			}));
			SetAttribute("get_chars", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				return new Vec(s.Select<Char, Value>(x => new Num(x)).ToList());
			}));

			SetAttribute("number?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsNumber(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsNumber(x))).ToList());
			}));
			SetAttribute("punctuation?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsPunctuation(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsPunctuation(x))).ToList());
			}));
			SetAttribute("separator?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsSeparator(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsSeparator(x))).ToList());
			}));
			SetAttribute("symbol?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsSymbol(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsSymbol(x))).ToList());
			}));
			SetAttribute("upper?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsUpper(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsUpper(x))).ToList());
			}));
			SetAttribute("get_say", new LambdaFun((e, args) => {
				Value s = e.Get("this");
				Console.Write(s);
				return s;
			}));
			SetAttribute("get_empty?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				return new Bool(String.Empty == s);
			}));
			SetAttribute("get_size", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				return new Num(s.Length);
			}));

			SetAttribute("tr", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();

				String source = args[0].ToString();
				String to = args[1].ToString();

				StringBuilder buff = new StringBuilder();

				for (int i = 0; i < source.Length; i++) {
					if (i + 1 < source.Length && source[i + 1] == '-') {
						if (Char.IsLetterOrDigit(source[i]) && Char.IsLetterOrDigit(source[i + 2])) {
							for (int j = (int)source[i]; j <= (int)source[i + 2]; j++) {
								buff.Append((Char)j);
							}
							i += 2;
						}
					}
					else {
						buff.Append(source[i]);
					}
				}

				source = buff.ToString();
				Console.WriteLine(source);
				buff.Clear();

				for (int i = 0; i < to.Length; i++) {
					if (i + 1 < to.Length && to[i + 1] == '-') {
						if (Char.IsLetterOrDigit(to[i]) && Char.IsLetterOrDigit(to[i + 2])) {
							for (int j = (int)to[i]; j <= (int)to[i + 2]; j++) {
								buff.Append((Char)j);
							}
							i += 2;
						}
					}
					else {
						buff.Append(to[i]);
					}
				}

				to = buff.ToString();
				Console.WriteLine(to);
				buff.Clear();
				for (int i = 0; i < s.Length; i++) {
					Int32 position = source.IndexOf(s[i]);
					if (position != -1) {
						buff.Append(position >= to.Length ? to[to.Length - 1] : to[position]);
					}
					else {
						buff.Append(s[i]);
					}
				}
				return new KString(buff.ToString());
			}));
			SetAttribute("white_space?", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (s.Length == 1)
					return new Bool(Char.IsWhiteSpace(s[0]));
				return new Vec(s.Select<Char, Value>(x => new Bool(Char.IsWhiteSpace(x))).ToList());
			}));
			SetAttribute("capitalize", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString().ToLower();
				return new KString(Char.ToUpper(s[0]) + s.Substring(1));
			}));
			SetAttribute("title_case", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				TextInfo t = CultureInfo.CurrentCulture.TextInfo;
				return new KString(t.ToTitleCase(s));
			}));
			SetAttribute("swap_caze", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				String result = "";
				foreach (Char i in s)
					if (Char.IsUpper(i))
						result += Char.ToLower(i);
					else
						result += Char.ToUpper(i);
				return new KString(result);
			}));
			/*SetAttribute("ljust", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 1)
					return new KString(s.PadLeft((int)Converter.ToDouble(args[0],e)));
				else
					return new KString(s.PadLeft((int)Converter.ToDouble(args[0],e), args[1].ToString()[0]));
			}));
			SetAttribute("rjust", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 1)
					return new KString(s.PadRight((int)Converter.ToDouble(args[0],e)));
				else
					return new KString(s.PadRight((int)Converter.ToDouble(args[0],e), args[1].ToString()[0]));
			}));*/
			SetAttribute("chomp", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 0)
					return new KString(s.Trim());
				else
					return new KString(s.Trim(args[0].ToString().ToCharArray()));
			}));
			SetAttribute("rchomp", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 0)
					return new KString(s.TrimEnd());
				else
					return new KString(s.TrimEnd(args[0].ToString().ToCharArray()));
			}));
			SetAttribute("lchomp", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 0)
					return new KString(s.TrimStart());
				else
					return new KString(s.TrimStart(args[0].ToString().ToCharArray()));
			}));
			SetAttribute("reverse", new LambdaFun((e, args) => {
				Char[] a = e.Get("this").ToString().ToCharArray();
				Array.Reverse(a);
				return new KString(new String(a));
			}));
			/*SetAttribute("scan", new LambdaFun((e, args) => {
				String str = e.Get("this").ToString();
				System.Text.RegularExpressions.Regex regex = ((Regex)args[0]).value;
				List<Value> lst = new List<Value>();
				foreach (Match match in regex.Matches(str)) {
					lst.Add(new KString(match.Value));
				}
				return new List(lst);
			}));*/
			SetAttribute("squeeze", new LambdaFun((e, args) => {
				String str = e.Get("this").ToString();
				return new KString(String.Join("", str.Distinct()));
			}));
			SetAttribute("delete", new LambdaFun((e, args) => {
				return new KString(e.Get("this").ToString().Replace(args[0].ToString(), ""));
			}));
			SetAttribute("split", new LambdaFun((e, args) => {
				return new Vec(e.Get("this").ToString()
					.Split(args[0].ToString()[0])
					.Select(x => (Value)new KString(x)).ToList());
			}));

			SetAttribute("get_bytesize", new LambdaFun((e, args) => {
				String v = e.Get("this").ToString();
				return new Num(Encoding.Unicode.GetByteCount(v));
			}));
			SetAttribute("normalize", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				String v = e.Get("this").ToString();
				return new KString(v.Normalize());
			}));

			SetAttribute("casecmp", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				String v = e.Get("this").ToString(e);
				return new Num(String.Compare(v, args[0].ToString(), true));
			}));
			/*SetAttribute("center", new LambdaFun((e, args) => {
				//Checker.ExistsThis(e);

				String v = e.Get("this").ToString(e);
				String aligmenter = args.Length > 1 ? args[1].ToString() : " ";
				Int32 val = (int)Converter.ToDouble(args[0], e);
				Int32 num = (val - v.Length) / 2;
				String result = "";

				if (aligmenter.Length != 1) {
					int j = 0;
					for (int i = 0; i < num; i++) {
						result += aligmenter[j];
						j++;
						if (j == aligmenter.Length)
							j = 0;
					}
					result += v;
					j = 0;
					for (int i = 0; i < num; i++) {
						result += aligmenter[j];
						j++;
						if (j == aligmenter.Length)
							j = 0;
					}
				}
				else {
					for (int i = 0; i < num; i++) {
						result += aligmenter;
					}
					result += v;
					for (int i = 0; i < num; i++) {
						result += aligmenter;
					}
				}
				return new KString(result);
			}));*/
			/*SetAttribute("each", new LambdaFun((e, args) => {
				var v = e.Get("this").ToString();
				Fun f = (Fun)args[0];
				var res = GlobalizationEach(v).Select(x => f.Run(new Scope(), new KString(x.ToString()))).ToList();
				return new List(res);
			}));*/
			/*SetAttribute("each_line", new LambdaFun((e, args) => {
				var v = e.Get("this").ToString().Split(new String[] { !e.IsExsists("sep") ? Environment.NewLine : e.Get("sep").ToString() }, StringSplitOptions.None);
				Fun f = (Fun)args[0];
				var res = v.Select(x => f.Run(new Scope(), new KString(x.ToString()))).ToList();
				return new List(res);
			}));*/

			SetAttribute("num", new LambdaFun((e, args) => {
				String v = e.Get("this").ToString();
				return new Num(Double.Parse(v));
			}));

			SetAttribute("format", GetAttribute("%", null));
			// Add: method String#count [ref: Ruby]
			// Add: class FreezzeString, methods String#freeze and String#get_freeze?
			// Modified: String#index with regex
			// Modified: String#delete with regex
		}

		public IEnumerable<Value> GlobalizationEach(String str) {
			TextElementEnumerator iterator = StringInfo.GetTextElementEnumerator(str);
			while (iterator.MoveNext()) {
				yield return new KString(iterator.Current.ToString());
			}
		}
	}
}
