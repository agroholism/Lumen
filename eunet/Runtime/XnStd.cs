using System;
using System.Collections.Generic;
using System.Linq;

using Argent.Xenon.Runtime.Interop;

namespace Argent.Xenon.Runtime {
	public class XnStd : Namespace {
		static Dictionary<KsTypeable, KsTypeable> cahce = new Dictionary<KsTypeable, KsTypeable>();
		static Dictionary<KsTypeable, KsTypeable> mutcache = new Dictionary<KsTypeable, KsTypeable>();

		#region TYPES
		public static KsTypeable ObjectType { get; } = new Contract {
			Name = "object",
			Checker = x => !(x is Nil)
		};

		public static KsType ListType { get; } = new KsType {
			Name = "list",
			Checker = x => x is XnList || x is XnImmutList
		};

		public static KsType SequenceType { get; } = new KsType {
			Name = "sequence",
			Checker = x => x is Sequence
		};

		public static KsType IteratorType { get; } = new KsType {
			Name = "iterator",
			Checker = x => x is Iterator
		};

		public static KsType AtomType { get; } = new KsType {
			Name = "atom",
			Checker = x => x is Atom
		};

		public static KsType IntegerType { get; } = new KsType {
			Name = "integer",
			Checker = x => x is Atom atom && atom.internalValue % 1 == 0,
			EigenNamespace = new Namespace {

			}
		};

		public static KsType TextType { get; } = new KsType {
			Name = "text",
			Checker = x => x is Text
		};

		public static KsType FunctionType { get; } = new KsType {
			Name = "function",
			Checker = x => x is Function,
			EigenNamespace = new Namespace {

			}
		};

		public static KsType TypeType { get; } = new KsType {
			Name = "type",
			Checker = x => x is KsType,
			EigenNamespace = new Namespace {

			}
		};

		public static KsType NamespaceType { get; } = new KsType {
			Name = "namespace",
			Checker = x => x is Namespace,
			EigenNamespace = new Namespace {

			}
		};

		public static KsType VoidType { get; } = new KsType {
			Name = "void",
			Checker = x => x == Nil.NilIns,
			EigenNamespace = new Namespace {

			}
		};

		#endregion
		public static XnStd EU_STD = new XnStd();

		public XnStd() {
			this.Set("function", FunctionType);
			this.Set("text", TextType);
			this.Set("atom", AtomType);
			this.Set("integer", IntegerType);
			this.Set("list", ListType);
			this.Set("object", ObjectType);
			this.Set("type", TypeType);
			this.Set("void", VoidType);
			this.Set("sequence", SequenceType);
			this.Set("iterator", IteratorType);

			this.Set("nil", Nil.NilIns);

			InitSequenceType();
			InitTextType();
			InitAtomType();

			IteratorType.EigenNamespace = new Namespace {
				["get_current"] = new SystemFunction((scope, args) => {
					return (scope.Get("self") as Iterator).InnerValue.Current;
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = IteratorType
					}
				},
				["move"] = new SystemFunction((scope, args) => {
					return new Atom((scope.Get("self") as Iterator).InnerValue.MoveNext() ? 1 : 0);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = IteratorType
					}
				},
			};

			ListType.EigenNamespace = new Namespace {
				["new"] = new SystemFunction((scope, args) => {
					return new XnList();
				}) {
					arguments = new Dictionary<String, KsTypeable> {
					}
				},

				["len"] = new SystemFunction((scope, args) => {
					return new Atom((scope.Get("self") as XnList).internalValue.Count);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = ListType
					}
				},

				["map"] = new SystemFunction((scope, args) => {
					Function fn = scope.Get("fn") as Function;
					return new XnList((scope.Get("self") as XnList).internalValue.Select(i => fn.Run(new Scope(), i)).ToList());
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = ListType,
						["fn"] = FunctionType
					}
				},

				["filter"] = new SystemFunction((scope, args) => {
					Function fn = scope.Get("fn") as Function;
					return new XnList((scope.Get("self") as XnList).internalValue.Where(i => AsBool(fn.Run(new Scope(), i))).ToList());
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = ListType,
						["fn"] = FunctionType
					}
				},

				["fold"] = new SystemFunction((scope, args) => {
					Function fn = scope.Get("fn") as Function;
					return (scope.Get("self") as XnList).internalValue.Aggregate((i, j) => fn.Run(new Scope(), i, j));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = ListType,
						["fn"] = FunctionType
					}
				},

				["append"] = new SystemFunction((scope, args) => {
					(scope.Get("self") as XnImmutList).internalValue.Add(scope.Get("elem"));

					return new Atom(1);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = GetMutable(ListType),
						["elem"] = GetNullable(ObjectType)
					}
				},
			};

			this.Set("range", new SystemFunction((scope, args) =>
				new Sequence(Enumerable.Range(
					(Int32)((Atom)scope.Get("from")).internalValue,
					(Int32)((Atom)scope.Get("to")).internalValue).Select(i => (XnObject)new Atom(i)))) {
				arguments = new Dictionary<String, KsTypeable> {
					["from"] = GetNullable(ObjectType),
					["to"] = IntegerType
				}
			});

			this.Set("repeat", new SystemFunction((scope, args) => Repeat(scope.Get("obj"), (Atom)scope.Get("count"))) {
				arguments = new Dictionary<String, KsTypeable> {
					["obj"] = GetNullable(ObjectType),
					["count"] = IntegerType
				}
			});

			this.Set("immut", new SystemFunction((scope, args) => {
				return new XnImmutList((scope.Get("obj") as XnList).internalValue);
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["obj"] = ListType
				}
			});

			this.Set("print", new SystemFunction((scope, args) => {
				Console.WriteLine(scope.Get("obj"));
				return new Atom(0);
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["obj"] = GetNullable(ObjectType)
				}
			});

			this.Set("read", new SystemFunction((scope, args) => {
				return new Text(Console.ReadLine());
			}) {
				arguments = new Dictionary<String, KsTypeable>()
			});

			this.Set("before", new SystemFunction((scope, args) => {
				Function fn = scope.Get("fn") as Function;
				Function origin = scope.Get("origin") as Function;

				if (origin is BeforeAfterFunction baf) {
					baf.before = fn;
				} else {
					origin = new BeforeAfterFunction {
						origin = origin,
						before = fn
					};
				}

				return origin;
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["fn"] = FunctionType,
					["origin"] = FunctionType
				}
			});

			this.Set("after", new SystemFunction((scope, args) => {
				Function fn = scope.Get("fn") as Function;
				Function origin = scope.Get("origin") as Function;

				if (origin is BeforeAfterFunction baf) {
					baf.after = fn;
				}
				else {
					origin = new BeforeAfterFunction {
						origin = origin,
						after = fn
					};
				}

				return origin;
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["fn"] = FunctionType,
					["origin"] = FunctionType
				}
			});

			this.Set("override", new SystemFunction((scope, args) => {
				Function fn = scope.Get("fn") as Function;

				return fn;
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["fn"] = FunctionType,
					["origin"] = GetNullable(FunctionType)
				}
			});

			this.Set("extern_type", new SystemFunction((scope, args) => {
				Type type = System.Type.GetType(scope.Get("name").ToString());

				if (type != null) {
					ClrType resultType = new ClrType(type) {
						Checker = x => x is ClrObject s && s.value.GetType().Equals(type),
						Name = type.Name,
						type = type
					};

					resultType.EigenNamespace = new Namespace {
						["new"] = new SystemFunction((s, a) => {
							return new ClrObject(type.GetConstructor(new Type[0]).Invoke(new Type[0]));
						}) {
							arguments = new Dictionary<string, KsTypeable>()
						},

						["as_text"] = new SystemFunction((s, a) => {
							return new Text((s.Get("self") as ClrObject).value.ToString());
						}) {
							arguments = new Dictionary<string, KsTypeable> {
								["self"] = ObjectType
							}
						},
					};

					return resultType;
				}

				return new Atom(0);
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["name"] = TextType
				}
			});

			this.Set("extern_func", new SystemFunction((scope, args) => {
				Type t = (scope.Get("type") as ClrType).type;

				System.Reflection.MethodInfo m = t.GetMethod(scope.Get("name").ToString(), (scope.Get("sign") as XnList).internalValue.Select(i => (i as ClrType).type).ToArray());

				if (m != null) {
					SystemFunction res = new SystemFunction((s, a) => {
						XnObject[] eus = new XnObject[a.Length - 1];
						Array.Copy(a, 1, eus, 0, a.Length - 1);
						return new ClrObject(m.Invoke(a[0].AsClr(), eus.Select(Utils.AsClr).ToArray()));
					}) {
						arguments = new Dictionary<String, KsTypeable> {
							["self"] = ObjectType
						}
					};

					Int32 x = 0;
					foreach (XnObject i in (scope.Get("sign") as XnList).internalValue) {
						res.arguments["i" + x] = ObjectType;
						x++;
					}

					return res;
				}

				return new Atom(0);
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["type"] = ObjectType,
					["name"] = TextType,
					["sign"] = ListType,
				}
			});

			this.Set("as_eu_obj", new SystemFunction((scope, args) => {
				return scope.Get("obj").AsEu();
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["obj"] = ObjectType
				}
			});

			this.Set("len", new SystemFunction((scope, args) => {
				if (scope.Get("obj") is Text text) {
					return new Atom(text.internalValue.Length);
				}

				if (scope.Get("obj") is XnList seq) {
					return new Atom(seq.internalValue.Count);
				}

				throw new XenonException(Exceptions.INVALID_OPERATION);
			}) {
				arguments = new Dictionary<String, KsTypeable> {
					["obj"] = ObjectType
				}
			});
		}

		class BeforeAfterFunction : Function {
			internal Function origin;
			internal Function before;
			internal Function after;

			public KsTypeable Type => XnStd.FunctionType;

			public XnObject Run(Scope scope, params XnObject[] arguments) {
				XnObject result = this.before?.Run(scope, arguments) ?? this.origin.Run(scope, arguments);

				return this.after?.Run(new Scope(), result) ?? result;
			}
		}

		private static void InitAtomType() {
			AtomType.EigenNamespace = new Namespace {
				[Operation.PLUS] = new SystemFunction((scope, args) => {
					return new Atom(AsDouble(scope.Get("self")) + AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.SUBSTRACT] = new SystemFunction((scope, args) => {
					return new Atom(AsDouble(scope.Get("self")) - AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.DIV] = new SystemFunction((scope, args) => {
					return new Atom(AsDouble(scope.Get("self")) / AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.MUL] = new SystemFunction((scope, args) => {
					return new Atom(AsDouble(scope.Get("self")) * AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},

				[Operation.EQUALS] = new SystemFunction((scope, args) => {
					XnObject other = scope.Get("other");

					if (other is Atom) {
						return new Bool(AsDouble(scope.Get("self")) == AsDouble(other));
					}

					return new Bool(false);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = ObjectType
					}
				},
				[Operation.NOT_EQUALS] = new SystemFunction((scope, args) => {
					XnObject other = scope.Get("other");

					if (other is Atom) {
						return new Bool(AsDouble(scope.Get("self")) != AsDouble(other));
					}

					return new Bool(true);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = ObjectType
					}
				},

				[Operation.LESS] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) < AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.LESSEQ] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) <= AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.GREATER] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) > AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.GREATEREQ] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) >= AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},

				["parse"] = new SystemFunction((scope, args) => {
					if (Double.TryParse((scope.Get("self") as Text).internalValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result)) {
						return new Atom(result);
					}

					return Nil.NilIns;
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType
					}
				},

				["as"] = new SystemFunction((scope, args) => {
					Atom self = (Atom)scope.Get("self");
					XnObject type = scope.Get("type");

					if (type == TextType) {
						return new Text(self.ToString());
					}

					if (type == IntegerType) {
						if (self.internalValue % 1 == 0) {
							return self;
						}
					}

					return Nil.NilIns;
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["type"] = ObjectType
					}
				},

				["as_bool"] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("srlf")) != 0);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType
					}
				},
			};
		}

		private static void InitTextType() {
			TextType.EigenNamespace = new Namespace {
				[Operation.PLUS] = new SystemFunction((scope, args) => {
					return new Text(scope.Get("self").ToString() + scope.Get("other").ToString());
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType,
						["other"] = ObjectType
					}
				},
				[Operation.DIV] = new SystemFunction((scope, args) => {
					return new Sequence(scope.Get("self").ToString()
						.Split(new String[] { scope.Get("other").ToString() }, StringSplitOptions.RemoveEmptyEntries).Select(i => new Text(i)));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType,
						["other"] = TextType
					}
				},

				[Operation.EQUALS] = new SystemFunction((scope, args) => {
					XnObject other = scope.Get("other");

					if (other is Text) {
						return new Bool(AsDouble(scope.Get("self")) == AsDouble(other));
					}

					return new Bool(false);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType,
						["other"] = ObjectType
					}
				},
				[Operation.NOT_EQUALS] = new SystemFunction((scope, args) => {
					XnObject other = scope.Get("other");

					if (other is Text) {
						return new Bool(AsDouble(scope.Get("self")) != AsDouble(other));
					}

					return new Bool(true);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType,
						["other"] = ObjectType
					}
				},

				[Operation.LESS] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) < AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType,
						["other"] = TextType
					}
				},
				[Operation.LESSEQ] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) <= AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.GREATER] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) > AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},
				[Operation.GREATEREQ] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) >= AsDouble(scope.Get("other")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = AtomType,
						["other"] = AtomType
					}
				},

				["as"] = new SystemFunction((scope, args) => {
					Text self = scope.Get("self") as Text;
					XnObject type = scope.Get("type");

					if (type == SequenceType) {
						return new Sequence(self.internalValue.Select(i => new Text(i.ToString())));
					}

					if (type == ListType) {
						return new XnList(self.internalValue.Select(i => (XnObject)new Text(i.ToString())).ToList());
					}

					if (type == TextType) {
						return self;
					}

					return Nil.NilIns;
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType,
						["type"] = ObjectType
					}
				},

				["to_upper"] = new SystemFunction((scope, args) => {
					return new Text((scope.Get("self") as Text).internalValue.ToUpper());
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType
					}
				},

				["to_lower"] = new SystemFunction((scope, args) => {
					return new Text((scope.Get("self") as Text).internalValue.ToLower());
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType
					}
				},

				["reverse"] = new SystemFunction((scope, args) => {
					return new Text(new String((scope.Get("self") as Text).internalValue.Reverse().ToArray()));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType
					}
				},

				["as_bool"] = new SystemFunction((scope, args) => {
					return new Bool(AsDouble(scope.Get("self")) != 0);
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = TextType
					}
				},
			};
		}

		private static void InitSequenceType() {
			SequenceType.EigenNamespace = new Namespace {
				["new"] = new SystemFunction((scope, args) => {
					return new Sequence(AsSequence(scope.Get("obj")));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["obj"] = ObjectType
					}
				},

				["as"] = new SystemFunction((scope, args) => {
					Sequence self = scope.Get("self") as Sequence;
					XnObject type = scope.Get("type");

					if (type == IteratorType) {
						return new Iterator(self.xns.GetEnumerator());
					}

					if (type == ListType) {
						return new XnList(self.xns.ToList());
					}

					if (type == TextType) {
						return new Text(self.ToString());
					}

					return Nil.NilIns;
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType,
						["type"] = ObjectType
					}
				},

				["as_iterator"] = new SystemFunction((scope, args) => {
					return new Iterator((scope.Get("self") as Sequence).xns.GetEnumerator());
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType
					}
				},

				["as_list"] = new SystemFunction((scope, args) => {
					return new XnList((scope.Get("self") as Sequence).xns.ToList());
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType
					}
				},

				["map"] = new SystemFunction((scope, args) => {
					IEnumerable<XnObject> xns = AsSequence(scope.Get("self"));
					Function fn = scope.Get("fn") as Function;

					return new Sequence(xns.Select(i => fn.Run(new Scope(), i)));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType,
						["fn"] = FunctionType
					}
				},

				["filter"] = new SystemFunction((scope, args) => {
					IEnumerable<XnObject> xns = AsSequence(scope.Get("self"));
					Function fn = scope.Get("fn") as Function;

					return new Sequence(xns.Where(i => AsBool(fn.Run(new Scope(), i))));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType,
						["fn"] = FunctionType
					}
				},

				["fold"] = new SystemFunction((scope, args) => {
					IEnumerable<XnObject> xns = AsSequence(scope.Get("self"));
					Function fn = scope.Get("fn") as Function;

					return xns.Aggregate((i, j) => fn.Run(new Scope(), i, j));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType,
						["fn"] = FunctionType
					}
				},

				["take"] = new SystemFunction((scope, args) => {
					IEnumerable<XnObject> xns = AsSequence(scope.Get("self"));
					Int32 fn = (Int32)AsDouble(scope.Get("count"));

					return new Sequence(xns.Take(fn));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType,
						["count"] = IntegerType
					}
				},

				["take_while"] = new SystemFunction((scope, args) => {
					IEnumerable<XnObject> xns = AsSequence(scope.Get("self"));
					Function fn = scope.Get("fn") as Function;

					return new Sequence(xns.TakeWhile(i => AsBool(fn.Run(new Scope(), i))));
				}) {
					arguments = new Dictionary<String, KsTypeable> {
						["self"] = SequenceType,
						["fn"] = FunctionType
					}
				},
			};
		}

		public static XnList Repeat(XnObject object1, Atom atom) {
			List<XnObject> result = new List<XnObject>();

			for (Int32 i = 0; i < atom.internalValue; i++) {
				result.Add(object1);
			}

			return new XnList(result);
		}

		public static IEnumerable<XnObject> AsSequence(XnObject euObject) {
			if (euObject is XnList seq) {
				return seq.internalValue;
			}

			if (euObject is Sequence sequence) {
				return sequence.xns;
			}

			if (euObject.Type is KsType ksType && ksType.EigenNamespace.variables.TryGetValue("as_stream", out var x)) {
				//if(x is )
			}

			throw new Exception();
		}

		public static Double AsDouble(XnObject xnObject) {
			if (xnObject is Atom atom) {
				return atom.internalValue;
			}

			throw new XenonException(Exceptions.INVALID_OPERATION);
		}

		public static KsTypeable GetNullable(KsTypeable typeable) {
			if (cahce.TryGetValue(typeable, out KsTypeable result)) {
				return result;
			}

			result = new Contract {
				Name = typeable.Name + "?",
				Checker = new Func<XnObject, Boolean>(x => x is Nil || typeable.Satisfaction(x))
			};

			cahce[typeable] = result;
			return result;
		}

		public static KsTypeable GetMutable(KsTypeable typeable) {
			if (mutcache.TryGetValue(typeable, out KsTypeable result)) {
				return result;
			}

			result = new Contract {
				Name = "immut[" + typeable.Name + "]",
				Checker = x => x is ImmutObject && typeable.Satisfaction(x)
			};

			mutcache[typeable] = result;
			return result;
		}

		public static Boolean AsBool(XnObject object1) {
			if (object1 is Bool b) {
				return b.internalValue;
			}

			if (object1 is Atom atom1) {
				return atom1.internalValue != 0;
			}
			else if (object1 is XnList sequence1) {
				return sequence1.internalValue.Count != 0;
			}

			throw new XenonException(Exceptions.INVALID_OPERATION);
		}

		public static XnObject Add(XnObject object1, XnObject object2) {
			if (object1 is Atom atom1) {
				if (object2 is Atom atom2) {
					return Add(atom1, atom2);
				}
				else if (object2 is XnList sequence1) {
					return Add(atom1, sequence1);
				}
			}
			else if (object1 is XnList sequence1) {
				if (object2 is Atom atom2) {
					return Add(sequence1, atom2);
				}
				else if (object2 is XnList sequence2) {
					return Add(sequence1, sequence2);
				}
			}
			else if (object1 is Text text1) {

			}

			throw new XenonException(Exceptions.INVALID_OPERATION);
		}

		public static Atom Add(Atom atom1, Atom atom2) {
			return new Atom(atom1.internalValue + atom2.internalValue);
		}

		public static XnList Add(Atom atom1, XnList sequence1) {
			return Add(Repeat(atom1, new Atom(sequence1.internalValue.Count)), sequence1);
		}

		public static XnList Add(XnList sequence1, Atom atom1) {
			return Add(sequence1, Repeat(atom1, new Atom(sequence1.internalValue.Count)));
		}

		public static XnList Add(XnList sequence1, XnList sequence2) {
			if (sequence1.internalValue.Count == sequence2.internalValue.Count) {
				return new XnList(sequence1.internalValue.Zip(sequence2.internalValue, Add).ToList());
			}

			throw new XenonException(Exceptions.INCORRECT_LENGTH);
		}
	}

	public class Nil : XnObject {
		public KsTypeable Type => throw new NotImplementedException();

		public static Nil NilIns { get; } = new Nil();

		private Nil() {

		}

		public override String ToString() {
			return "nil";
		}
	}
}
