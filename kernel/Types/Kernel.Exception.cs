using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Lang.Std { 
	public sealed class ExceptionType : KType {
		internal ExceptionType() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.Exception",
				//BaseType = StandartModule.Object
			};

			Set("new", new LambdaFun((scope, args) => {
				return new Exception(args[0].ToString(scope), this, null);
			}));

			SetAttribute("get_message", new LambdaFun((e, args) => (KString)(e.This as Exception).Message));

			SetAttribute("to_s", new LambdaFun((e, args) => (KString)(e.This as Exception).ToString(e)));
		}
	}

	public class LexerException : KType {
		public LexerException() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.LexerException",
				//BaseType = StandartModule.Exception
			};

			Set("new", new LambdaFun((scope, args) => {
				return new Exception(args[0].ToString(scope), this, null);
			}));
		}
	}

	public class ArgumentExceptionType : KType {
		public ArgumentExceptionType() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.ArgumentException",
				//BaseType = StandartModule.Exception
			};

			Set("new", new LambdaFun((scope, args) => {
				return new Exception(args[0].ToString(scope), this, null);
			}));
		}
	}

	public class TypeException : KType {
		public TypeException() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.TypeException",
				//BaseType = StandartModule.Exception
			};

			Set("new", new LambdaFun((scope, args) => {
				return new Exception($"expected value of type {args[0]} given {args[1]}", this, null);
			}));
		}
	}

	public class CastExceptionType : KType {
		public CastExceptionType() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.CastException",
				//BaseType = StandartModule.Exception
			};

			Set("new", new LambdaFun((scope, args) => {
				return new Exception($"expected value of type {args[0]} given {args[1]}", this, null);
			}));
		}
	}
}
