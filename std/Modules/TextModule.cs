using Lumen.Lang.Expressions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Lumen.Lang {
    internal sealed class TextModule : Module {
        internal TextModule() {
            this.name = "prelude.Text";

            #region operators

            this.SetField(Op.USTAR, new LambdaFun((scope, args) => {
                return new Number(scope.This.ToString(scope).Length);
            }) {
                Arguments = Const.This
            });

            this.SetField(Op.PLUS, new LambdaFun((scope, args) => {
                return new Text(scope.This.ToString(scope) + scope["other"].ToString(scope));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.LSH, new LambdaFun((scope, args) => {
                Text str = scope.This as Text;
                str.value += scope["other"].ToString(scope);
                return str;
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.STAR, new LambdaFun((scope, args) => {
                String str = scope.This.ToString(scope);
                Int32 count = (Int32)scope["other"].ToDouble(scope);

                StringBuilder buffer = new StringBuilder();
                for (Int32 i = 0; i < count; i++) {
                    buffer.Append(str);
                }

                return new Text(buffer.ToString());
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("compare", new LambdaFun((scope, args) => {
                return new Number(scope.This.CompareTo(scope["other"]));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.MOD, new LambdaFun((scope, args) => {
                return new Text(String.Format(scope.This.ToString(scope), scope["other"].ToSequence(scope).ToArray()));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.SLASH, new LambdaFun((scope, args) => {
                return new Array(
                    scope.This.ToString(scope).Split(
                        scope["other"].ToString(scope).ToCharArray(),
                        StringSplitOptions.RemoveEmptyEntries
                    ).Select(x => (Value)new Text(x)).ToList());
            }) {
                Arguments = Const.ThisOther
            });

            // make it later
            this.SetField("inc", new LambdaFun((e, args) => {
                String value = e.Get("this").ToString();
                String chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
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
                Arguments = Const.ThisOther
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

            this.SetField("concat", new LambdaFun((e, args) => {
                String[] z = new String[args.Length + 1];

                z[0] = e.Get("this").ToString(e);
                Int32 index = 1;
                foreach (Value i in e["other"].ToSequence(e)) {
                    z[index] = i.ToString(e);
                    index++;
                }

                return new Text(String.Concat(z));
            }) {
                Arguments = new List<IPattern> {
                    Const.This[0],
                    new NamePattern("*other")
                }
            });

            this.SetField("Sequence", new LambdaFun((e, args) => {
                String v = e.Get("this").ToString();

                if (args.Length > 0) {
                    Double wait = Converter.ToDouble(args[0], e);
                    Int32 index = 0;
                    if (wait == 2) {
                        return new Enumerator(this.GlobalizationEach(v).Select(i => new Array(new List<Value> { i, new Number(index++) })));
                    } else if (wait > 2) {
                        return new Enumerator(this.GlobalizationEach(v).Select(i => new Array(new List<Value> { i, new Number(index++), new Number((Int32)i.ToString()[0]) })));
                    }
                }

                return new Enumerator(this.GlobalizationEach(v));
            }));
            this.SetField("String", new LambdaFun((e, args) => {
                return e.This;
            }));

            this.SetField("upper", new LambdaFun((e, args) => {
                return new Text(e.This.ToString(e).ToUpper());
            }) { Arguments = Const.This });
            this.SetField("lower", new LambdaFun((e, args) => {
                return new Text(e.This.ToString(e).ToLower());
            }) { Arguments = Const.This });

            this.SetField("isAscii", new LambdaFun((e, args) => {
                return (Bool)(e.This.ToString(e)[0] < 255);
            }) {
                Arguments = Const.This
            });
            
            this.SetField("contains", new LambdaFun((e, args) => {
                return new Bool(e.This.ToString(e).Contains(e["other"].ToString(e)));
            }) {
                Arguments = Const.ThisOther
            });
            this.SetField("isStartsWith", new LambdaFun((e, args) => {
                return new Bool(e.This.ToString(e).StartsWith(e["other"].ToString(e)));
            }) {
                Arguments = Const.ThisOther
            });
            this.SetField("isEndsWith", new LambdaFun((e, args) => {
                return new Bool(e.This.ToString(e).EndsWith(e["other"].ToString(e)));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("replace", new LambdaFun((e, args) => {
                return new Text(e.This.ToString(e).Replace(e["other"].ToString(e), e["xother"].ToString(e)));
            }) {
                Arguments = new List<IPattern> {
                    Const.This[0],
                    Const.ThisOther[1],
                    new NamePattern("xother")
                }
            });

            this.SetField("index", new LambdaFun((e, args) => {
                return new Number(e.This.ToString(e).IndexOf(e["other"].ToString(e)));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("get_chars", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                return new Array(s.Select<Char, Value>(x => new Number(x)).ToList());
            }));

            this.SetField("isEmpty", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                return new Bool("" == s);
            }) {
                Arguments = Const.This
            });

            this.SetField("size", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                return new Number(s.Length);
            }) {
                Arguments = Const.This
            });

            this.SetField("tr", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();

                String source = args[0].ToString();
                String to = args[1].ToString();

                StringBuilder buff = new StringBuilder();

                for (Int32 i = 0; i < source.Length; i++) {
                    if (i + 1 < source.Length && source[i + 1] == '-') {
                        if (Char.IsLetterOrDigit(source[i]) && Char.IsLetterOrDigit(source[i + 2])) {
                            for (Int32 j = (Int32)source[i]; j <= (Int32)source[i + 2]; j++) {
                                buff.Append((Char)j);
                            }
                            i += 2;
                        }
                    } else {
                        buff.Append(source[i]);
                    }
                }

                source = buff.ToString();
                Console.WriteLine(source);
                buff.Clear();

                for (Int32 i = 0; i < to.Length; i++) {
                    if (i + 1 < to.Length && to[i + 1] == '-') {
                        if (Char.IsLetterOrDigit(to[i]) && Char.IsLetterOrDigit(to[i + 2])) {
                            for (Int32 j = (Int32)to[i]; j <= (Int32)to[i + 2]; j++) {
                                buff.Append((Char)j);
                            }
                            i += 2;
                        }
                    } else {
                        buff.Append(to[i]);
                    }
                }

                to = buff.ToString();
                Console.WriteLine(to);
                buff.Clear();
                for (Int32 i = 0; i < s.Length; i++) {
                    Int32 position = source.IndexOf(s[i]);
                    if (position != -1) {
                        buff.Append(position >= to.Length ? to[to.Length - 1] : to[position]);
                    } else {
                        buff.Append(s[i]);
                    }
                }
                return new Text(buff.ToString());
            }));
            this.SetField("white_space?", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (s.Length == 1) {
                    return new Bool(Char.IsWhiteSpace(s[0]));
                }

                return new Array(s.Select<Char, Value>(x => new Bool(Char.IsWhiteSpace(x))).ToList());
            }));
            this.SetField("capitalize", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString().ToLower();
                return new Text(Char.ToUpper(s[0]) + s.Substring(1));
            }));
            this.SetField("title_case", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                TextInfo t = CultureInfo.CurrentCulture.TextInfo;
                return new Text(t.ToTitleCase(s));
            }));
            this.SetField("swap_caze", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                String result = "";
                foreach (Char i in s) {
                    if (Char.IsUpper(i)) {
                        result += Char.ToLower(i);
                    } else {
                        result += Char.ToUpper(i);
                    }
                }

                return new Text(result);
            }));
            /*Set("ljust", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 1)
					return new KString(s.PadLeft((int)Converter.ToDouble(args[0],e)));
				else
					return new KString(s.PadLeft((int)Converter.ToDouble(args[0],e), args[1].ToString()[0]));
			}));
			Set("rjust", new LambdaFun((e, args) => {
				String s = e.Get("this").ToString();
				if (args.Length == 1)
					return new KString(s.PadRight((int)Converter.ToDouble(args[0],e)));
				else
					return new KString(s.PadRight((int)Converter.ToDouble(args[0],e), args[1].ToString()[0]));
			}));*/
            this.SetField("chomp", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (args.Length == 0) {
                    return new Text(s.Trim());
                } else {
                    return new Text(s.Trim(args[0].ToString().ToCharArray()));
                }
            }));
            this.SetField("rchomp", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (args.Length == 0) {
                    return new Text(s.TrimEnd());
                } else {
                    return new Text(s.TrimEnd(args[0].ToString().ToCharArray()));
                }
            }));
            this.SetField("lchomp", new LambdaFun((e, args) => {
                String s = e.Get("this").ToString();
                if (args.Length == 0) {
                    return new Text(s.TrimStart());
                } else {
                    return new Text(s.TrimStart(args[0].ToString().ToCharArray()));
                }
            }));
            this.SetField("reverse", new LambdaFun((e, args) => {
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
            this.SetField("squeeze", new LambdaFun((e, args) => {
                String str = e.Get("this").ToString();
                return new Text(System.String.Join("", str.Distinct()));
            }));
            this.SetField("delete", new LambdaFun((e, args) => {
                return new Text(e.Get("this").ToString().Replace(args[0].ToString(), ""));
            }));
            this.SetField("split", new LambdaFun((e, args) => {
                return new Array(e.Get("this").ToString()
                    .Split(args[0].ToString()[0])
                    .Select(x => (Value)new Text(x)).ToList());
            }));

            this.SetField("get_bytesize", new LambdaFun((e, args) => {
                String v = e.Get("this").ToString();
                return new Number(Encoding.Unicode.GetByteCount(v));
            }));
            this.SetField("normalize", new LambdaFun((e, args) => {
				String v = e.Get("this").ToString();
				return new Text(v.Normalize());
			}) {
				Arguments = Const.This
			});

            this.SetField("casecmp", new LambdaFun((e, args) => {
                //Checker.ExistsThis(e);

                String v = e.Get("this").ToString(e);
                return new Number(System.String.Compare(v, args[0].ToString(), true));
            }));
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

            this.SetField("Number", new LambdaFun((e, args) => {
                String v = e.Get("this").ToString();
                try {
                    return new Number(Double.Parse(v));
                } catch {
                    return Const.UNIT;
                }
            }) {
                Arguments = Const.This
            });

            this.Derive(Prelude.Ord);

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
