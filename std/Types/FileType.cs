using System;
using System.Linq;

namespace Lumen.Lang.Std {
	internal sealed class FileType : Record {
		public FileType() {
			this.meta = new TypeMetadata {
				Name = "Kernel.File"
				//BaseType = StandartModule.Object
			};

			SetAttribute("()", new LambdaFun((e, args) => {
				return new File(new System.IO.FileInfo(args[0].ToString(e)));
			}));

			SetAttribute("atime", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is Str) {
					return new DateTime(new System.IO.FileInfo(value.ToString(e)).LastAccessTime);
				}
				else if (value is File file) {
					return new DateTime(file.Inner.LastAccessTime);
				}
				return null;
			//	return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			SetAttribute("birthtime", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is Str) {
					return new DateTime(new System.IO.FileInfo(value.ToString(e)).CreationTime);
				}
				else if (value is File file) {
					return new DateTime(file.Inner.CreationTime);
				}

				return null;
				//return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			SetAttribute("ctime", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is Str) {
					return new DateTime(new System.IO.FileInfo(value.ToString(e)).LastWriteTime);
				}
				else if (value is File file) {
					return new DateTime(file.Inner.LastWriteTime);
				}

				return null;//Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			SetAttribute("delete", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is Str) {
					System.IO.File.Delete(value.ToString(e));

				}
				else if (value is File file) {
					System.IO.File.Delete(file.Inner.FullName);
				}

				return Const.VOID;
			}));

			SetAttribute("dir", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is Str) {
					return (Str)new System.IO.FileInfo(value.ToString(e)).DirectoryName;
				}
				else if (value is File file) {
					return (Str)file.Inner.DirectoryName;
				}

				return null;
				//return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			SetAttribute("exist?", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is Str) {
					return (Bool)new System.IO.FileInfo(value.ToString(e)).Exists;
				}
				else if (value is File file) {
					return (Bool)file.Inner.Exists;
				}

				return null;
				//return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			SetAttribute("read", new LambdaFun((e, args) => {
				return new Enumerator(System.IO.File.ReadLines(args[0].ToString(e)).Select(i => (Str)i));
			}));

			SetAttribute("read_all", new LambdaFun((e, args) => {
				return (Str)System.IO.File.ReadAllText(args[0].ToString(e));
			}));

			SetAttribute("add!", new LambdaFun((e, args) => {
				System.IO.File.AppendAllText(args[0].ToString(e), args[1].ToString(e));
				return Const.VOID;
			}));

			SetAttribute("get_dir", new LambdaFun((e, args) => {
				File value = e.Get("this") as File;

				return (Str)value.Inner.DirectoryName;
			}));

			SetAttribute("get_exists?", new LambdaFun((e, args) => {
				File value = e.Get("this") as File;

				return (Bool)value.Inner.Exists;
			}));

			SetAttribute("set_exists?", new LambdaFun((e, args) => {
				File value = e.Get("this") as File;

				if (Converter.ToBoolean(args[0])) {
					value.Inner.Create().Close();
				}

				return (Bool)true;
			}));

			SetAttribute("to_i", new LambdaFun((e, args) => {
				File value = e.Get("this") as File;

				return new Enumerator(System.IO.File.ReadLines(value.Inner.FullName).Select(i => (Str)i));
			}));

			SetAttribute("add!", new LambdaFun((e, args) => {
				System.IO.FileInfo file = ((e.Get("this") as File).Inner);
				System.IO.File.AppendAllText(file.FullName, args[0].ToString(e));
				return Const.VOID;
			}));
		}
	}
}
