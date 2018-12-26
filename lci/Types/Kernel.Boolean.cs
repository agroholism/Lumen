using System;
using System.Collections.Generic;

namespace StandartLibrary {
	internal sealed class RBoolean : KType {
		internal RBoolean() {
			this.meta = new TypeMetadata {
				Name = "std.bool"
			};

			#region operators

			SetAttribute(Op.NOT, new LambdaFun((scope, args) => {
				Boolean value = Converter.ToBoolean(scope.This);
				return new Bool(!value);
			}));

			SetAttribute(Op.OR, new LambdaFun((scope, args) => {
				Boolean value = Converter.ToBoolean(scope.This);
				Boolean other = Converter.ToBoolean(scope["other"]);

				return new Bool(value || other);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.XOR, new LambdaFun((scope, args) => {
				Boolean value = Converter.ToBoolean(scope.This);
				Boolean other = Converter.ToBoolean(scope["other"]);

				return new Bool(value ^ other);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.AND, new LambdaFun((scope, args) => {
				Boolean value = Converter.ToBoolean(scope.This);
				Boolean other = Converter.ToBoolean(scope["other"]);

				return new Bool(value && other);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.EQL, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (!(other is Bool)) {
					return Const.FALSE;
				}

				return (Bool)(Converter.ToBoolean(scope.Get("this")) == Converter.ToBoolean(other));
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.NOT_EQL, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (!(other is Bool)) {
					return Const.TRUE;
				}

				return (Bool)(Converter.ToBoolean(scope.Get("this")) != Converter.ToBoolean(other));
			}, "Boolean.!=") { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.SHIP, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return (Num)scope.This.CompareTo(other);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.LT, new LambdaFun((e, args) => {
				Boolean value = Converter.ToBoolean(e.This);
				Boolean other = Converter.ToBoolean(e["other"]);

				return new Bool(value == false && other == true);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.LTEQ, new LambdaFun((e, args) => {
				Boolean value = Converter.ToBoolean(e.This);
				Boolean other = Converter.ToBoolean(e["other"]);

				return new Bool(value == false && other == true || value == other);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });

			SetAttribute(Op.GTEQ, new LambdaFun((e, args) => {
				Boolean value = Converter.ToBoolean(e.This);
				Boolean other = Converter.ToBoolean(e["other"]);

				return new Bool(value == true && other == false || value == other);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") }});

			SetAttribute(Op.GT, new LambdaFun((e, args) => {
				Boolean value = Converter.ToBoolean(e.This);
				Boolean other = Converter.ToBoolean(e["other"]);

				return new Bool(value == true && other == false);
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("other") } });
			#endregion

			SetAttribute("str", new LambdaFun((e, args) => new KString(e.This.ToString(e))));
			SetAttribute("num", new LambdaFun((e, args) => (Num)(Converter.ToBoolean(e.This) ? 1 : 0)));

			Set("new", new LambdaFun((e, args) => {
				return new Bool(Converter.ToBoolean(e["obj"]));
			}) { Arguments = new List<FunctionArgument> { new FunctionArgument("obj", Const.FALSE) } });
		}
	}
}
