using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

/*
 * Сокращаем regex, exception до минимума
 * Убираем Arguments
 * Не слишком ли много Fun?
 * HENUMERABLE
 * Добесколько интерфейсовавить н
 * Основные функции сосредоточить в ядре
 */

namespace Lumen.Lang.Std {
	internal sealed class FunctionClass : KType {
		internal FunctionClass() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.Function"
			};

			SetAttribute("rec", new LambdaFun((e, args) => throw new GotoE(args)));

			SetAttribute("+=", new LambdaFun((e, args) => {
				Fun fun1 = Converter.ToFunction(e.Get("this"), e);
				Fun fun2 = Converter.ToFunction(args[0], e);

				if(fun1 is LambdaFun lf) {
					lf.value = (scope, arguments) => fun2.Run(new Scope(scope), fun1.Run(new Scope(scope), arguments));
					return lf;
				}

				return new LambdaFun((scope, arguments) => fun2.Run(new Scope(scope), fun1.Run(new Scope(scope), arguments)));
			}));

			SetAttribute("<<", new LambdaFun((e, args) => {
				Fun fun1 = Converter.ToFunction(e.Get("this"), e);
				Fun fun2 = Converter.ToFunction(args[0], e);

				return new LambdaFun((scope, arguments) => fun1.Run(new Scope(scope), fun2.Run(new Scope(scope), arguments)));
			}));

			SetAttribute(">>", new LambdaFun((e, args) => {
				Fun fun1 = Converter.ToFunction(e.Get("this"), e);
				Fun fun2 = Converter.ToFunction(args[0], e);

				return new LambdaFun((scope, arguments) => fun2.Run(new Scope(scope), fun1.Run(new Scope(scope), arguments)));
			}));
		}
	}
}
