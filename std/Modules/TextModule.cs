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

			this.SetMember(Op.USTAR, new LambdaFun((scope, args) => {
                return new Number(scope["self"].ToString().Length);
            }) {
				Arguments = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("x")
				}
			});

            this.SetMember(Op.PLUS, new LambdaFun((scope, args) => {
                return new Text(scope["self"].ToString() + scope["other"].ToString());
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember(Op.LSH, new LambdaFun((scope, args) => {
                Text str = scope["self"] as Text;
                str.value += scope["other"].ToString();
                return str;
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember(Op.STAR, new LambdaFun((scope, args) => {
                String str = scope["self"].ToString();
				Value other = scope["other"];

				if (other == Const.UNIT) {
					return new Number(str.Length);
				}


				Int32 count = (Int32)other.ToDouble(scope);

				StringBuilder buffer = new StringBuilder();
                for (Int32 i = 0; i < count; i++) {
                    buffer.Append(str);
                }

                return new Text(buffer.ToString());
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember("compare", new LambdaFun((scope, args) => {
                return new Number(scope["self"].CompareTo(scope["other"]));
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember(Op.MOD, new LambdaFun((scope, args) => {
                return new Text(String.Format(scope["self"].ToString(), scope["other"].ToStream(scope).ToArray()));
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember(Op.SLASH, new LambdaFun((scope, args) => {
				Value arg = scope["arg"];
				if (arg is Text text) {
					return new Array(
						scope["text"].ToString().Split(
							arg.ToString().ToCharArray(),
							StringSplitOptions.RemoveEmptyEntries
						).Select(x => (Value)new Text(x)).ToList());
				} else if (arg is Number num) {
					String t = scope["text"].ToString();
					Int32 numb = num.ToInt(scope);
					List<Value> result = new List<Value>();
					for(Int32 i = 0; i < t.Length; i += numb) {
						String sub = "";
						for(Int32 j = i; j < i + numb; j++) {
							sub += t[j];
						}
						result.Add(new Text(sub));
					}

					return new Array(result);
				}
				else {
					IEnumerable<Value> value = scope["text"].ToStream(scope);
					Fun func = scope["arg"].ToFunction(scope);

					return value.Aggregate((x, y) => func.Run(new Scope(scope), x, y));
				}
            }) {
                Arguments = new List<IPattern> {
					new NamePattern("text"),
					new NamePattern("arg")
				}
            });

            this.SetMember("inc", new LambdaFun((e, args) => {
                String value = e["text"].ToString();
                const String chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                Int32 last_char = value.Length - 1;
                while (true) {
                    Int32 index = chars.IndexOf(value[last_char]);
                    if (index == -1) {
                        last_char--;
                        if (last_char == -1) {
                            return new Text(value.Substring(0, value.Length - 1) + (Char)(1 + value[value.Length - 1]));
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
                        } else if (chars[index] == 'z') {
                            if (last_char == 0) {
                                value = "aa" + value.Substring(1);
                                break;
                            }
                            value = value.Substring(0, last_char) + "a" + value.Substring(last_char + 1);
                            last_char--;
                        } else if (chars[index] == '9') {
                            if (last_char == 0) {
                                value = "10" + value.Substring(1);
                                break;
                            }
                            value = value.Substring(0, last_char) + "0" + value.Substring(last_char + 1);
                            last_char--;
                        } else {
                            value = value.Substring(0, last_char) + (Char)(1 + chars[index]) + value.Substring(last_char + 1);
                            break;
                        }
                    }
                    break;
                }
                return new Text(value);
            }) {
                Arguments = new List<IPattern> {
					new NamePattern("text")
				}
            });
			// make get and set
			// match =~
			// make ranges

			/*   this.Set(Op.GETI, new LambdaFun((e, args) => {
                System.String value = e.Get("this").ToString();
                if (args.Length == 1) {
                    if (args[0] is Number) {
                        Int32 index = (Int32)Converter.ToDouble(args[0], e);
                        Int32 i = index;

                        index = index < 0 ? value.Length + index : index;

                        if (index >= value.Length || index < 0) {
                            throw new Exception("выход за пределы строки при срезе вида [i]. Требуемый индекс [" + i + "] превышает длину строки [" + value.Length + "]", stack: e);
                        }
                        return new Str(this.GlobalizationEach(value).Skip(index).First().ToString());
                    } else if (args[0] is Str) {
                        Int32 position = value.IndexOf(args[0].ToString());
                        if (position == -1) {
                            return Const.FALSE;
                        } else {
                            return new Number(position);
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
						}*
                } else if (args.Length == 2) {

                }
                return new Number(e.Get("this").ToString().Length);
            }));*/

			#endregion

			// Collection -> Text
			// let concat values = ...
			// Сцепляет Collection объектов в единую строку
			// Примечание:
			//  Для пустого Collection возвращает пустую строку
			// Исключения: 
			//	ConvertException: невозможно преобразовать объект к типу Stream
			this.SetMember("concat", new LambdaFun((scope, args) => {
				return new Text(String.Concat(scope["values"].ToStream(scope)));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("values")
                }
            });

			// Text -> Text
			// let toUpper self = ...
			// Переводит строку в верхний регистр
			this.SetMember("upperCase", new LambdaFun((scope, args) => {
				return new Text(scope["self"].ToString().ToUpper());
			}) {
				Arguments = Const.Self
			});

			// Text -> Text
			// let toLower self = ...
			// Переводит строку в нижний регистр
			this.SetMember("lowerCase", new LambdaFun((scope, args) => {
				return new Text(scope["self"].ToString().ToLower());
			}) {
				Arguments = Const.Self
			});

			// Text -> Text
			// let capitalize self = ...
			this.SetMember("capitalizeCase", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString().ToLower();
				return new Text(Char.ToUpper(self[0]) + self.Substring(1));
			}) {
				Arguments = Const.Self
			});

			// Text -> Text
			// let toTitleCase self = ...
			this.SetMember("titleCase", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();
				TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

				return new Text(textInfo.ToTitleCase(self));
			}) {
				Arguments = Const.Self
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
				Arguments = Const.Self
			});

			// Text -> Text -> Boolean
			// let contains substring string = ...
			// Определяет, содержится ли подстрока substring в строке string
			this.SetMember("contains", new LambdaFun((scope, args) => {
				return new Bool(scope["this"].ToString().Contains(scope["other"].ToString()));
			}) {
				Arguments = Const.SelfOther
			});

			// Text -> Boolean
			this.SetMember("isEmpty", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Bool("" == self);
			}) {
				Arguments = Const.Self
			});

			// Text -> Text -> Boolean
			// let contains string prefix = ...
			// Определяет, начинается ли строка string со строки prefix
			this.SetMember("isStartsWith", new LambdaFun((scope, args) => {
				return new Bool(scope["this"].ToString().StartsWith(scope["other"].ToString()));
			}) {
				Arguments = Const.SelfOther
			});

			// Text -> Text -> Boolean
			// let contains string suffix = ...
			// Определяет, кончается ли строка string на строку prefix
			this.SetMember("isEndsWith", new LambdaFun((scope, args) => {
				return new Bool(scope["this"].ToString().EndsWith(scope["other"].ToString()));
			}) {
				Arguments = Const.SelfOther
			});

			// Text -> Stream
			// let getCahrs self = ...
			this.SetMember("getChars", new LambdaFun((scope, args) => {
				String self = scope["self"].ToString();

				return new Stream(self.Select<Char, Value>(x => new Number(x)));
			}) {
				Arguments = Const.Self
			});

			this.SetMember("toStream", new LambdaFun((e, args) => {
				String v = e["values"].ToString();
				return new Stream(this.GlobalizationEach(v));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values")
				}
			});

			this.SetMember("fromStream", new LambdaFun((e, args) => {
				IEnumerable<Value> v = e["stream"].ToStream(e);
				return new Text(String.Concat(v));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("stream")
				}
			});

			this.SetMember("toText", new LambdaFun((e, args) => {
				return e["self"];
			}) {
				Arguments = Const.Self
			});

          /*  this.SetMember("isAscii", new LambdaFun((e, args) => {
                return (Bool)(e["this"].ToString()[0] < 255);
            }) {
                Arguments = Const.Self
            });*/
 
            this.SetMember("replace", new LambdaFun((e, args) => {
                return new Text(e["this"].ToString().Replace(e["other"].ToString(), e["xother"].ToString()));
            }) {
                Arguments = new List<IPattern> {
                    Const.Self[0],
                    Const.SelfOther[1],
                    new NamePattern("xother")
                }
            });

            this.SetMember("index", new LambdaFun((e, args) => {
                return new Number(e["this"].ToString().IndexOf(e["other"].ToString()));
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember("size", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                return new Number(s.Length);
            }) {
                Arguments = Const.Self
            });

            this.SetMember("tr", new LambdaFun((e, args) => {
				String text = e["text"].ToString();

				String source = e["src"].ToString();
				String to = e["res"].ToString();

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
				Arguments = new List<IPattern> {
					new NamePattern("text"),
					new NamePattern("src"),
					new NamePattern("res")
				}
			});
            this.SetMember("isWhiteSpace", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (s.Length == 1) {
                    return new Bool(Char.IsWhiteSpace(s[0]));
                }

                return new Array(s.Select<Char, Value>(x => new Bool(Char.IsWhiteSpace(x))).ToList());
            }));
          
			this.SetMember("ljust", new LambdaFun((e, args) => {
				String s = e["text"].ToString();
				return args.Length == 1
					? new Text(s.PadLeft((Int32)Converter.ToDouble(args[0], e)))
					: new Text(s.PadLeft((Int32)Converter.ToDouble(args[0], e), args[1].ToString()[0]));
			}));
			this.SetMember("rjust", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 1)
					return new Text(s.PadRight((int)Converter.ToDouble(args[0],e)));
				else
					return new Text(s.PadRight((int)Converter.ToDouble(args[0],e), args[1].ToString()[0]));
			}));
            this.SetMember("chomp", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (args.Length == 0) {
                    return new Text(s.Trim());
                } else {
                    return new Text(s.Trim(args[0].ToString().ToCharArray()));
                }
            }));
            this.SetMember("rchomp", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (args.Length == 0) {
                    return new Text(s.TrimEnd());
                } else {
                    return new Text(s.TrimEnd(args[0].ToString().ToCharArray()));
                }
            }));
            this.SetMember("lchomp", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (args.Length == 0) {
                    return new Text(s.TrimStart());
                } else {
                    return new Text(s.TrimStart(args[0].ToString().ToCharArray()));
                }
            }));
            this.SetMember("reverse", new LambdaFun((e, args) => {
                Char[] a = e.Get("this").ToString().ToCharArray();
                System.Array.Reverse(a);
                return new Text(new String(a));
            }));
            /*Set("scan", new LambdaFun((e, args) => {
				String str = e.Get("this").ToString();
				System.Text.RegularExpressions.Regex regex = ((Regex)args[0]).value;
				List<Value> lst = new List<Value>();
				foreach (Match match in regex.Matches(str)) {
					lst.Add(new KString(match.Value));
				}
				return new List(lst);
			}));*/
            this.SetMember("squeeze", new LambdaFun((e, args) => {
                String str = e.Get("this").ToString();
                return new Text(String.Join("", str.Distinct()));
            }));
            this.SetMember("delete", new LambdaFun((e, args) => {
                return new Text(e.Get("this").ToString().Replace(args[0].ToString(), ""));
            }));
            this.SetMember("split", new LambdaFun((e, args) => {
                return new Array(e.Get("this").ToString()
                    .Split(args[0].ToString()[0])
                    .Select(x => (Value)new Text(x)).ToList());
            }));

            this.SetMember("get_bytesize", new LambdaFun((e, args) => {
                String v = e.Get("this").ToString();
                return new Number(Encoding.Unicode.GetByteCount(v));
            }));
            this.SetMember("normalize", new LambdaFun((e, args) => {
				String v = e.Get("this").ToString();
				return new Text(v.Normalize());
			}) {
				Arguments = Const.Self
			});

            this.SetMember("casecmp", new LambdaFun((scope, args) => {
				String v = scope["x"].ToString();

				return new Number(String.Compare(v, scope["y"].ToString(), true));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});
            /*Set("center", new LambdaFun((e, args) => {
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
            /*Set("each", new LambdaFun((e, args) => {
				var v = e.Get("this").ToString();
				Fun f = (Fun)args[0];
				var res = GlobalizationEach(v).Select(x => f.Run(new Scope(), new KString(x.ToString()))).ToList();
				return new List(res);
			}));*/
            /*Set("each_line", new LambdaFun((e, args) => {
				var v = e.Get("this").ToString().Split(new String[] { !e.IsExsists("sep") ? Environment.NewLine : e.Get("sep").ToString() }, StringSplitOptions.None);
				Fun f = (Fun)args[0];
				var res = v.Select(x => f.Run(new Scope(), new KString(x.ToString()))).ToList();
				return new List(res);
			}));*/

            this.SetMember("toNumber", new LambdaFun((e, args) => {
                String v = e.Get("self").ToString();
                try {
                    return new Number(Double.Parse(v));
                } catch {
                    return Const.UNIT;
                }
            }) {
                Arguments = Const.Self
            });

            this.IncludeMixin(Prelude.Ord);
			this.IncludeMixin(Prelude.Collection);
            // Add: method String#count [ref: Ruby]
            // Add: class FreezzeString, methods String#freeze and String#get_freeze?
            // Modified: String#index with regex
            // Modified: String#delete with regex
            // encode decode
        }

        public IEnumerable<Value> GlobalizationEach(String str) {
            TextElementEnumerator iterator = StringInfo.GetTextElementEnumerator(str);
            while (iterator.MoveNext()) {
                yield return new Text(iterator.Current.ToString());
            }
        }
    }
}
