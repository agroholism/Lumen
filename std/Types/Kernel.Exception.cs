using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Lang.Std { 
	public sealed class ExceptionType : Record {
		internal ExceptionType() {
			this.meta = new TypeMetadata {
				Name = "Kernel.Exception",
				//BaseType = StandartModule.Object
			};

			SetAttribute("()", new LambdaFun((scope, args) => {
				return new Exception(args[0].ToString(scope), this, null);
			}));

			SetAttribute("get_message", new LambdaFun((e, args) => (Str)(e.This as Exception).Message));

			SetAttribute("to_s", new LambdaFun((e, args) => (Str)(e.This as Exception).ToString(e)));
		}
	}

	public class LexerException : Record {
		public LexerException() {
			this.meta = new TypeMetadata {
				Name = "Kernel.LexerException",
				//BaseType = StandartModule.Exception
			};

			SetAttribute("new", new LambdaFun((scope, args) => {
				return new Exception(args[0].ToString(scope), this, null);
			}));
		}
	}

	public class ArgumentExceptionType : Record {
		public ArgumentExceptionType() {
			this.meta = new TypeMetadata {
				Name = "Kernel.ArgumentException",
				//BaseType = StandartModule.Exception
			};

			SetAttribute("new", new LambdaFun((scope, args) => {
				return new Exception(args[0].ToString(scope), this, null);
			}));
		}
	}

	public class TypeException : Record {
		public TypeException() {
			this.meta = new TypeMetadata {
				Name = "Kernel.TypeException",
				//BaseType = StandartModule.Exception
			};

			SetAttribute("new", new LambdaFun((scope, args) => {
				return new Exception($"expected value of type {args[0]} given {args[1]}", this, null);
			}));
		}
	}

	public class CastExceptionType : Record {
		public CastExceptionType() {
			this.meta = new TypeMetadata {
				Name = "Kernel.CastException",
				//BaseType = StandartModule.Exception
			};

			SetAttribute("new", new LambdaFun((scope, args) => {
				return new Exception($"expected value of type {args[0]} given {args[1]}", this, null);
			}));
		}
	}
}
