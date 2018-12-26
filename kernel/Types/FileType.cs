using System;
using System.Linq;

namespace Lumen.Lang.Std {
	internal sealed class FileType : KType {
		public FileType() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.File"
				//BaseType = StandartModule.Object
			};

			Set("new", new LambdaFun((e, args) => {
				return new File(new System.IO.FileInfo(args[0].ToString(e)));
			}));

			Set("atime", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is KString) {
					return new DateTime(new System.IO.FileInfo(value.ToString(e)).LastAccessTime);
				}
				else if (value is File file) {
					return new DateTime(file.Inner.LastAccessTime);
				}
				return null;
			//	return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			Set("birthtime", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is KString) {
					return new DateTime(new System.IO.FileInfo(value.ToString(e)).CreationTime);
				}
				else if (value is File file) {
					return new DateTime(file.Inner.CreationTime);
				}

				return null;
				//return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			Set("ctime", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is KString) {
					return new DateTime(new System.IO.FileInfo(value.ToString(e)).LastWriteTime);
				}
				else if (value is File file) {
					return new DateTime(file.Inner.LastWriteTime);
				}

				return null;//Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			Set("delete", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is KString) {
					System.IO.File.Delete(value.ToString(e));

				}
				else if (value is File file) {
					System.IO.File.Delete(file.Inner.FullName);
				}

				return Const.NULL;
			}));

			Set("dir", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is KString) {
					return (KString)new System.IO.FileInfo(value.ToString(e)).DirectoryName;
				}
				else if (value is File file) {
					return (KString)file.Inner.DirectoryName;
				}

				return null;
				//return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			Set("exist?", new LambdaFun((e, args) => {
				Value value;

				if (e.ExistsInThisScope("file")) {
					value = e["file"];
				}
				else {
					//Checker.CheckArgsCount(args, 1, "аргумент file является обязательным при вызове функции", e);
					value = args[0];
				}

				if (value is KString) {
					return (Bool)new System.IO.FileInfo(value.ToString(e)).Exists;
				}
				else if (value is File file) {
					return (Bool)file.Inner.Exists;
				}

				return null;
				//return Checker.RaiseArgumentTypeError("file", "Kernel.String или Kernel.File", e);
			}));

			Set("read", new LambdaFun((e, args) => {
				return new Enumerator(System.IO.File.ReadLines(args[0].ToString(e)).Select(i => (KString)i));
			}));

			Set("read_all", new LambdaFun((e, args) => {
				return (KString)System.IO.File.ReadAllText(args[0].ToString(e));
			}));

			Set("add!", new LambdaFun((e, args) => {
				System.IO.File.AppendAllText(args[0].ToString(e), args[1].ToString(e));
				return Const.NULL;
			}));

			SetAttribute("get_dir", new LambdaFun((e, args) => {
				File value = e.Get("this") as File;

				return (KString)value.Inner.DirectoryName;
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

				return new Enumerator(System.IO.File.ReadLines(value.Inner.FullName).Select(i => (KString)i));
			}));

			SetAttribute("add!", new LambdaFun((e, args) => {
				System.IO.FileInfo file = ((e.Get("this") as File).Inner);
				System.IO.File.AppendAllText(file.FullName, args[0].ToString(e));
				return Const.NULL;
			}));
		}
	}
}
