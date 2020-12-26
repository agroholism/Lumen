using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal sealed class TextModule : Module {
		internal TextModule() {
			this.Name = "Text";

			#region operators

			this.SetMember(Constants.PLUS, new LambdaFun((scope, args) => {
				return new Text(scope["self"].ToString() + scope["other"].ToString());
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember(Constants.STAR, new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				Value other = scope["other"];

				// Unary operator
				if (other == Const.UNIT) {
					return new Number(self.Length);
				}

				Int32 times = (Int32)other.ToDouble(scope);

				StringBuilder buffer = new StringBuilder();
				for (Int32 i = 0; i < times; i++) {
					buffer.Append(self);
				}

				return new Text(buffer.ToString());
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("compare", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				String other = scope["other"].ToString();

				return new Number(String.CompareOrdinal(self, other));
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember(Constants.MOD, new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				Value other = scope["other"];

				return new Text(String.Format(self, other.ToSeq(scope).ToArray()));
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember(Constants.SLASH, new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				Value other = scope["other"];

				if (other is Text text) {
					String[] parts =
						self.Split(new[] { other.ToString() }, StringSplitOptions.RemoveEmptyEntries);

					return new MutArray(parts.Select(x => (Value)new Text(x)).ToList());
				}
				else if (other is Number num) {
					Int32 maxLength = num.ToInt(scope);

					List<Value> result = new List<Value>();
					StringBuilder buffer = new StringBuilder();
					for (Int32 i = 0; i < self.Length; i += maxLength) {
						buffer.Clear();

						for (Int32 j = i; j < self.Length && j < i + maxLength; j++) {
							buffer.Append(self[j]);
						}

						result.Add(new Text(buffer.ToString()));
					}

					return new MutArray(result);
				}

				throw new LumenException("expect Text or Number");
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember(Constants.GETI, new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				List<Value> indices = scope["indices"].ToList(scope);

				if (indices.Count == 1) {
					Value index = indices[0];

					if (index is Fun fun) {
						return new Seq(from ch in self
										  select new Text(ch.ToString()) into value
										  where fun.Call(new Scope(scope), value).ToBoolean()
										  select value);
					}

					if (index is Number) {
						Int32 intIndex = Helper.Index(index.ToInt(scope), self.Length);

						if (intIndex < 0 || intIndex >= self.Length) {
							throw Helper.IndexOutOfRange();
						}

						return new Text(self[intIndex].ToString());
					}

					StringBuilder buffer = new StringBuilder();

					foreach (Value i in index.ToSeq(scope)) {
						if (i is Number) {
							Int32 newIndex = Helper.Index(i.ToInt(scope), self.Length);

							if (newIndex < 0 || newIndex >= self.Length) {
								throw Helper.IndexOutOfRange();
							}

							buffer.Append(self[newIndex]);
						}
						else {
							throw new LumenException(Exceptions.TYPE_ERROR.F(Prelude.Number, i.Type));
						}
					}

					return new Text(buffer.ToString());
				}
				else if (indices.Count == 2) {
					Int32 firstIndex = Helper.Index(indices[0].ToInt(scope), self.Length);
					Int32 secondIndex = indices[1].ToInt(scope); // should by positive!

					return new Text(self.Substring(firstIndex, secondIndex));
				}

				throw new LumenException("function Text.getIndex supports only one ot two arguments");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("indices"),
					new NamePattern("self")
				}
			});
			#endregion

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new Text("");
			}) {
				Parameters = new List<IPattern> { }
			});

			this.SetMember("clone", new LambdaFun((scope, args) => {
				return new Text(scope["self"].ToString());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("empty", new LambdaFun((scope, args) => {
				return new Text("");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			// Collection -> Text
			// let concat values = ...
			// Сцепляет Collection объектов в единую строку
			// Примечание:
			//  Для пустого Collection возвращает пустую строку
			// Исключения: 
			//	ConvertException: невозможно преобразовать объект к типу Stream
			this.SetMember("concat", new LambdaFun((scope, args) => {
				return new Text(String.Concat(scope["values"].ToSeq(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("values")
				}
			});

			// Text -> Text
			// let upperCase self = ...
			// Переводит строку в верхний регистр
			this.SetMember("upperCase", new LambdaFun((scope, args) => {
				return new Text(scope["self"].ToString().ToUpper());
			}) {
				Parameters = Const.Self
			});

			// Text -> Text
			// let lowerCase self = ...
			// Переводит строку в нижний регистр
			this.SetMember("lowerCase", new LambdaFun((scope, args) => {
				return new Text(scope["self"].ToString().ToLower());
			}) {
				Parameters = Const.Self
			});

			// Text -> Text
			// let capitalCase self = ...
			this.SetMember("capitalCase", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString().ToLower();
				return new Text(Char.ToUpper(self[0]) + self.Substring(1));
			}) {
				Parameters = Const.Self
			});

			// Text -> Text
			// let titleCase self = ...
			this.SetMember("titleCase", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

				return new Text(textInfo.ToTitleCase(self));
			}) {
				Parameters = Const.Self
			});

			// Text -> Text
			// let swapCase self = ...
			this.SetMember("swapCase", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				StringBuilder result = new StringBuilder();
				foreach (Char i in self) {
					result.Append(Char.IsUpper(i) ? Char.ToLower(i) : Char.ToUpper(i));
				}

				return new Text(result.ToString());
			}) {
				Parameters = Const.Self
			});

			// Text -> Text -> Boolean
			// let contains substring self = ...
			// Определяет, содержится ли подстрока substring в строке string
			this.SetMember("contains", new LambdaFun((scope, args) => {
				return new Logical(scope["self"].ToString().Contains(scope["substring"].ToString()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("substring")
				}
			});

			// Text -> Boolean
			this.SetMember("isEmpty", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Logical("" == self);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("isWhiteSpace", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Logical(String.IsNullOrWhiteSpace(self));
			}) {
				Parameters = Const.Self
			});

			// Text -> Text -> Boolean
			// let contains prefix self = ...
			// Определяет, начинается ли строка string со строки prefix
			this.SetMember("isStartsWith", new LambdaFun((scope, args) => {
				return new Logical(scope["self"].ToString().StartsWith(scope["prefix"].ToString()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("prefix")
				}
			});

			// Text -> Text -> Boolean
			// let contains suffix self = ...
			// Определяет, кончается ли строка string на строку prefix
			this.SetMember("isEndsWith", new LambdaFun((scope, args) => {
				return new Logical(scope["self"].ToString().EndsWith(scope["suffix"].ToString()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("suffix")
				}
			});

			// Text -> Stream
			// let getCahrs self = ...
			this.SetMember("getChars", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Seq(self.Select<Char, Value>(x => new Number(x)));
			}) {
				Parameters = Const.Self
			});

			this.SetMember("indexOf", new LambdaFun((scope, args) => {
				return new Number(scope["self"].ToString().IndexOf(scope["subtext"].ToString()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("subtext")
				}
			});

			this.SetMember("lastIndexOf", new LambdaFun((scope, args) => {
				return new Number(scope["self"].ToString().LastIndexOf(scope["subtext"].ToString()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("subtext")
				}
			});


			this.SetMember("size", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Number(self.Length);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("translate", new LambdaFun((scope, args) => {
				String text = scope["text"].ToString();

				String source = scope["src"].ToString();
				String to = scope["res"].ToString();

				StringBuilder buff = new StringBuilder();

				for (Int32 i = 0; i < source.Length; i++) {
					if (i + 1 < source.Length && source[i + 1] == '-') {
						if (Char.IsLetterOrDigit(source[i]) && Char.IsLetterOrDigit(source[i + 2])) {
							for (Int32 j = source[i]; j <= source[i + 2]; j++) {
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
				buff.Clear();

				for (Int32 i = 0; i < to.Length; i++) {
					if (i + 1 < to.Length && to[i + 1] == '-') {
						if (Char.IsLetterOrDigit(to[i]) && Char.IsLetterOrDigit(to[i + 2])) {
							for (Int32 j = to[i]; j <= (Int32)to[i + 2]; j++) {
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

				buff.Clear();
				for (Int32 i = 0; i < text.Length; i++) {
					Int32 position = source.IndexOf(text[i]);
					if (position != -1) {
						buff.Append(position >= to.Length ? to[to.Length - 1] : to[position]);
					}
					else {
						buff.Append(text[i]);
					}
				}
				return new Text(buff.ToString());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("text"),
					new NamePattern("src"),
					new NamePattern("res")
				}
			});


			this.SetMember("replace", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				String what = scope["what"].ToString();
				String with = scope["with"].ToString();

				return new Text(self.Replace(what, with));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("what"),
					new NamePattern("with"),
					Const.Self[0]
				}
			});

			this.SetMember("delete", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.Replace(scope["subtext"].ToString(), ""));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("subtext")
				}
			});


			this.SetMember("trim", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.Trim());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("trimEnd", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.TrimEnd());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("trimStart", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.TrimStart());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});


			static String Pad(String self, Int32 len, String aligmenter = " ") {
				static String MakePadderForOneSide(Int32 byEachSide, String aligmenter) {
					StringBuilder buffer = new StringBuilder();
					for (Int32 i = 0, j = 0; i < byEachSide; i++) {
						buffer.Append(aligmenter[j]);
						j = (j == aligmenter.Length - 1) ? 0 : j + 1;
					}
					return buffer.ToString();
				}

				Int32 byEachSide = (len - self.Length) / 2;
				StringBuilder buffer = new StringBuilder();

				if (aligmenter.Length != 1) {
					String padder = MakePadderForOneSide(byEachSide, aligmenter);
					buffer
						.Append(padder)
						.Append(self)
						.Append(padder);
				}
				else {
					buffer
						.Append(aligmenter[0], byEachSide)
						.Append(self)
						.Append(aligmenter[0], byEachSide);
				}

				return buffer.ToString();
			}

			this.SetMember("pad", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				Int32 totalLenght = scope["length"].ToInt(scope);

				return new Text(Pad(self, totalLenght));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("length")
				}
			});

			this.SetMember("padStart", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.PadLeft(scope["lenght"].ToInt(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("lenght")
				}
			});

			this.SetMember("padEnd", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.PadRight(scope["lenght"].ToInt(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("lenght")
				}
			});

			this.SetMember("padWith", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				String with = scope["with"].ToString();
				Int32 totalLenght = scope["length"].ToInt(scope);

				return new Text(Pad(self, totalLenght, with));
			}) {
				Parameters = new List<IPattern> {
						new NamePattern("self"),
						new NamePattern("with"),
					new NamePattern("length")
				}
			});

			this.SetMember("padStartWith", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.PadLeft(scope["lenght"].ToInt(scope), scope["with"].ToString()[0]));
			}) {
				Parameters = new List<IPattern> {
						new NamePattern("self"),
					new NamePattern("with"),
					new NamePattern("lenght")
				}
			});

			this.SetMember("padEndWith", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				return new Text(self.PadRight(scope["lenght"].ToInt(scope), scope["with"].ToString()[0]));
			}) {
				Parameters = new List<IPattern> {
						new NamePattern("self"),
					new NamePattern("with"),
					new NamePattern("lenght")
				}
			});


			this.SetMember("reverse", new LambdaFun((scope, args) => {
				Char[] chars = scope["self"].ToString().ToCharArray();
				System.Array.Reverse(chars);
				return new Text(new String(chars));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("compareIgnoreCase", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Number(String.Compare(self, scope["other"].ToString(), true));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("other")
				}
			});

			this.SetMember("iterLine", new LambdaFun((scope, args) => {
				Fun action = scope["action"].ToFunction(scope);
				String[] self = scope["self"].ToString().Split(
					new[] { Environment.NewLine },
					StringSplitOptions.None);

				foreach (String i in self) {
					action.Call(new Scope(scope), new Text(i));
				}

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
						new NamePattern("self"),
					new NamePattern("action")
				}
			});


			this.SetMember("toSeq", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Seq(this.GlobalizationEach(self));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("fromSeq", new LambdaFun((scope, args) => {
				IEnumerable<Value> stream = scope["stream"].ToSeq(scope);
				return new Text(String.Concat(stream));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("stream")
				}
			});

			this.SetMember("toText", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Parameters = Const.Self
			});


			this.AppendImplementation(Prelude.Ord);
			this.AppendImplementation(Prelude.Collection);
			this.AppendImplementation(Prelude.Clone);
			this.AppendImplementation(Prelude.Default);
		}

		public IEnumerable<Value> GlobalizationEach(String str) {
			TextElementEnumerator iterator = StringInfo.GetTextElementEnumerator(str);

			while (iterator.MoveNext()) {
				yield return new Text(iterator.Current.ToString());
			}
		}
	}
}
