using System;
using System.Collections.Generic;
using System.Linq;

using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	public class FunctionDefineStatement : Expression {
		public String NameFunction;
		public List<ArgumentMetadataGenerator> Args;
		public Expression Body;
		public Expression returnedType;
		public String doc;
		public List<Expression> otherContacts;
		public Int32 line;
		public String file;


		public Expression Optimize(Scope scope) {
			return this;
		}
		public FunctionDefineStatement(String NameFunction, List<ArgumentMetadataGenerator> Args, Expression Body, Expression returnedType) {
			this.NameFunction = NameFunction;
			this.Args = Args;
			this.Body = Body;
			this.returnedType = returnedType;
		}

		public FunctionDefineStatement(string NameFunction, List<ArgumentMetadataGenerator> Args, Expression Body, Expression returnedType, List<Expression> otherContacts, Int32 line, String file) : this(NameFunction, Args, Body, returnedType) {
			this.otherContacts = otherContacts;
			this.line = line;
			this.file = file;
		}

		public override String ToString() {
			String result = "fun " + this.NameFunction + "(" + String.Join(", ", this.Args.Select(i => i.name)) + ")" + "{" + this.Body + "}";
			return result;
		}

		public Value Eval(Scope e) {
			List<FunctionArgument> args = new List<FunctionArgument>();

			KType exte = null;

			foreach (ArgumentMetadataGenerator i in this.Args) {
				var arg = i.EvalArgumnet(e);
			/*	if (arg.name == "this" && arg.type != null && arg.type is StandartLibrary.KType) {
					exte = (StandartLibrary.KType)arg.type;
					continue;
				}*/
				args.Add(arg);
			}

			List<string> s = new List<string>() { "self", "_", "this", "base", "value", "kwargs", "args" };

			BlockE expre = new BlockE();

			foreach (var i in args) {
				string mutname = i.name.Replace("*", "");
				if (mutname == "this")
					throw new StandartLibrary.Exception("Параметр функции не может иметь имя this", stack: e);
				s.Add(mutname);
			}

			/*	if (this.otherContacts != null) {
					foreach (Expression i in this.otherContacts) {
						expre.Add(new ConditionE(i, new UnknownExpression(), new RaiseE(null, new ValueE("контракт " + i.ToString() + " не выполнен"), "", -1)));
					}
				}*/

			if (this.returnedType != null || expre.expressions.Count > 0) {
				expre.Add(Body);
			}

			if (returnedType != null || expre.expressions.Count > 0)
				Body = expre;

			List<String> visible = new List<String> { "this", "self", "null", "true", "false", "args", "kwargs", "_" };
			visible.AddRange(this.Args.Select(i => i.name.Replace("*", "")));

			UserFun v = new UserFun {
				Arguments = args,
				condition = otherContacts.Count > 0 ? otherContacts[0] : null,
				body = this.Body?.Closure(visible, e)
			};
			v.Set("name", (KString)NameFunction, AccessModifiers.PRIVATE, e);

			if (exte != null) {
				exte.SetAttribute(NameFunction, v);
			}
			else {
				if (this.Body == null) {
					e.Set(this.NameFunction, v);
				}
				else if (e.ExistsInThisScope(NameFunction) && e[NameFunction] is UserFun uf) {
					if (uf.body == null) {
						uf.body = v.body;
					}
					/*else {
						Dispatcher dis = new Dispatcher();
						dis.Add(v);
						dis.Add(uf);
						e.Set(this.NameFunction, dis);
					}*/
				}
			/*	else if (e.ExistsInThisScope(NameFunction) && e[NameFunction] is Dispatcher disp) {
					disp.funcs.Insert(0, v);
				}*/
				else
					e.Set(this.NameFunction, v);
			}

			return v;
		}

		/*public class Dispatcher : SystemFun {
			public List<Fun> funcs = new List<Fun>();

			public void Add(UserFun fun) {
				this.funcs.Add(fun);
			}

			public override Value Run(Scope e, params Value[] args) {
				Int32 argsCount = 0;
				List<String> vars = new List<string> { "self", "args", "kwargs", "this" };
				foreach(var i in e.variables) {
					if (!vars.Contains(i.Key)) {
						argsCount++;
					}
				}
				argsCount += args.Length;

				List<Fun> candidates = new List<Fun>();

				foreach(Fun i in this.funcs) {
					if(i.Metadata.args.Count == argsCount) {
						candidates.Add(i);
					}
				}

				Dictionary<String, Object> arguments = new Dictionary<String, Object>();
				Int32 counter = 0;

				foreach (FunctionArgument i in candidates[0].Metadata.args) {
					if (e.ExistsInThisScope(i.name.Replace("*", ""))) {
						arguments.Add(i.name.Replace("*", ""), e.Get(i.name.Replace("*", "")));
						continue;
					}

					if (counter < args.Length) {
						if (i.name.StartsWith("*")) {
							List<Value> v = new List<Value>();

							for (Int32 z = counter; z < args.Length; z++) {
								v.Add(args[z]);
								counter++;
							}

							arguments.Add(i.name.Substring(1, i.name.Length - 1), new List(v));
							continue;
						}

						arguments.Add(i.name, args[counter]);
						counter++;
					}
					else {
						arguments.Add(i.name, i.defaultValue);
					}
				}

				counter = 0;

				foreach (FunctionArgument i in candidates[0].Metadata.args) {
					if (e.ExistsInThisScope(i.name.Replace("*", ""))) {
						continue;
					}

					if (counter < args.Length || (args.Length == 0 && counter == args.Length)) {
						if (i.name.StartsWith("*")) {
							List<Value> v = new List<Value>();

							for (Int32 z = counter; z < args.Length; z++) {
								v.Add(args[z]);
								counter++;
							}

							e.Set(i.name.Substring(1, i.name.Length - 1), new List(v));
							continue;
						}

						if (args.Length != 0) {
							e.Set(i.name, args[counter]);
							counter++;
						}
						else {
							if (i.defaultValue != null) {
								if (i.defaultValue is Value val) {
									e.Set(i.name, val);
								}
								else if (i.defaultValue is Expression exp) {
									e.Set(i.name, exp.Eval(e));
								}
							}
							else {
								e.Set(i.name, Const.NULL);
							}

							counter++;
						}
					}
					else {
						if (i.defaultValue != null) {
							if (i.defaultValue is Value val) {
								e.Set(i.name, val);
							}
							else if (i.defaultValue is Expression exp) {
								e.Set(i.name, exp.Eval(e));
							}
						}
						else {
							e.Set(i.name, Const.NULL);
						}
					}
				}


				foreach (UserFun i in candidates) {
					if (i.Metadata.IsMatch(arguments)) {
						if (i.condition != null && !Converter.ToBoolean(i.condition.Eval(e))) {
							continue;
						}
						return i.Run(e, args);
					}
				}

				throw new StandartLibrary.Exception("dispatch", stack: e);
			}
		}
		*/
		internal static Expression MakeClosure(List<String> zex, Expression i, Scope e) {
			return i.Closure(zex, e);

			if (i is Namespace) {
				zex.Add(((Namespace)i).name);
			}

			if (i is DecoratorE dece) {
				return new DecoratorE(dece.expsOfDecos.Select(x => MakeClosure(zex, x, e)).ToList(), MakeClosure(zex, dece.func, e));
			}

			if (i is AnonymeDefine) {
				var z = new List<string> { "self", "_", "this" };

				foreach (var x in ((AnonymeDefine)i).Args)
					z.Add(x.name.Replace("*", ""));
				foreach (string x in zex)
					z.Add(x);

				return new AnonymeDefine(((AnonymeDefine)i).Args, MakeClosure(z, ((AnonymeDefine)i).Body, e));
			}

			if (i is FunctionDefineStatement fde) {
				/*    var z = new List<string> { "self" };*/

				/* foreach (string x in fde.Args.GetNames())
                     z.Add(x.Replace("&", "").Replace("*", ""));
                     */
				/*    foreach (string x in zex)
                        z.Add(x);*/

				zex.Add(fde.NameFunction);
				//   z.Add(fde.NameFunction);

				// ((FunctionDefineStatement)i).Body = ReWrite(fde.NameFunction, z, fde.Body, e);
			}

			if (i is WhileExpression we)
				return new WhileExpression(MakeClosure(zex, we.condition, e), MakeClosure(zex, we.body, e));

			if (i is DotApplicate dapp) {
				return new DotApplicate(MakeClosure(zex, dapp.res, e), dapp.exps.Select(x => MakeClosure(zex, x, e)).ToList());
			}

			if (i is Assigment ae) {
				zex.Add(ae.id);
				return new Assigment(ae.id, MakeClosure(zex, ae.exp, e), ae.line, ae.file);
			}

			if (i is SpreadE)
				return new SpreadE(MakeClosure(zex, ((SpreadE)i).expression, e));

			if (i is ListForGen lfg) {
				return new ListForGen(lfg.names, MakeClosure(zex, lfg.container, e), MakeClosure(zex, lfg.body, e));
			}

			if (i is ConditionE cde)
				return new ConditionE(MakeClosure(zex, cde.condition, e), MakeClosure(zex, ((ConditionE)i).trueExpression, e), MakeClosure(zex, ((ConditionE)i).falseExpression, e));

			if (i is DotExpression dte)
				return new DotExpression(MakeClosure(zex, dte.expression, e), dte.nameVariable);

			if (i is IdExpression ce) {
				if (!ce.id.StartsWith("$"))
					if (!(zex.Contains(ce.id))) {
						if (!e.IsExsists(ce.id))
							throw new StandartLibrary.Exception($"\n\tFILE: {ce.file} LINE: {ce.line}\n\t неизвестный идентификатор {ce.id}", stack: e);
						return new ValueE(e.Get(ce.id));
					}
				return i;
			}

			if (i is BinaryExpression) {
				BinaryExpression z = (BinaryExpression)i;
				return new BinaryExpression(MakeClosure(zex, z.expressionOne, e), MakeClosure(zex, z.expressionTwo, e), z.operation, z.line, z.file);
			}

			return i;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			visible.Add(NameFunction);
			return new FunctionDefineStatement(NameFunction, Args.Select(i => new ArgumentMetadataGenerator(i.name, i.type?.Closure(visible, thread), i.defaultValue?.Closure(visible, thread))).ToList(), Body.Closure(visible, thread), returnedType?.Closure(visible, thread));
		}
	}
}