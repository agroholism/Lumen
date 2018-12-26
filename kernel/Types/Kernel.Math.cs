using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	internal sealed class MathModule : Module {
		private static readonly Random mainRandomObject = new Random();

		internal MathModule() {
			/*Set("sin", new LambdaFun((e, args) => {
				return new Number(Math.Sin(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.sin") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("cos", new LambdaFun((e, args) => {
				return new Number(Math.Cos(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.cos") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("tan", new LambdaFun((e, args) => {
				return new Number(Math.Tan(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.tan") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("abs", new LambdaFun((e, args) => {
				return new Number(Math.Abs(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.abs") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("acos", new LambdaFun((e, args) => {
				return new Number(Math.Acos(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.acos") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("asin", new LambdaFun((e, args) => {
				return new Number(Math.Asin(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.asin") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("atan", new LambdaFun((e, args) => {
				return new Number(Math.Atan(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.atan") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("ceiling", new LambdaFun((e, args) => {
				return new Number(Math.Ceiling(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.ceiling") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("cosh", new LambdaFun((e, args) => {
				return new Number(Math.Cosh(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.cosh") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("exp", new LambdaFun((e, args) => {
				return new Number(Math.Exp(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.exp") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("floor", new LambdaFun((e, args) => {
				return new Number(Math.Floor(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.floor") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("log", new LambdaFun((e, args) => {
				return new Number(Math.Log(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.log") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("log10", new LambdaFun((e, args) => {
				return new Number(Math.Log10(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.log10") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("round", new LambdaFun((e, args) => {
				return new Number(Math.Round(Converter.ToDouble(e["x"], e), (Int32)Converter.ToDouble(e["y"], e)));
			}, "Kernel.Math.round") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number),
						new FunctionArgument("y", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("sinh", new LambdaFun((e, args) => {
				return new Number(Math.Sinh(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.sinh") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("sqrt", new LambdaFun((e, args) => {
				return new Number(Math.Sqrt(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.sqrt") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("tanh", new LambdaFun((e, args) => {
				return new Number(Math.Tanh(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.tanh") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("truncate", new LambdaFun((e, args) => {
				return new Number(Math.Truncate(Converter.ToDouble(e["x"], e)));
			}, "Kernel.Math.truncate") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("x", (Isable)StandartModule.Number)
					},
					returnedType = StandartModule.Number
				}
			});
			Set("random", new LambdaFun((e, args) => {
				Value up = e["up"];
				Value low = e["low"];

				return (Number)mainRandomObject.Next((Int32)Converter.ToDouble(up, e), (Int32)Converter.ToDouble(low, e));
			}, "Kernel.Math.random") {
				Metadata = new FunctionMetadata {
					args = new List<FunctionArgument> {
						new FunctionArgument("up", (Isable)StandartModule.Number),
						new FunctionArgument("low", (Isable)StandartModule.Number, (Number)0)
					},
					returnedType = StandartModule.Number
				}
			});

			Set("E", new Number(Math.E));
			Set("PI", new Number(Math.PI));*/
		}
	}
}
