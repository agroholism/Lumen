using System;
using System.Collections.Generic;
using Lumen;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class LigthType : Expression {
		internal System.String nameType;
		internal System.String parameter;
		internal Expression type;
		internal Expression body;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public LigthType(System.String nameType, System.String parameter, Expression type, Expression body) {
			this.nameType = nameType;
			this.parameter = parameter;
			this.type = type;
			this.body = body;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			visible.Add(parameter);
			visible.Add(nameType);
			return new LigthType(nameType, parameter, this.type.Closure(visible, scope), this.body.Closure(visible, scope));
		}

		public Value Eval(Scope e) {
			Record type = this.type.Eval(e) as Record;

			Fun f = new AnonymeDefine(new List<ArgumentMetadataGenerator> { new ArgumentMetadataGenerator(parameter, null, null) }, this.body).Eval(e) as Fun;
			var result = new LigthTypeType(nameType, type, f);

			e.Set(nameType, result);

			return Const.NULL;
		}
	}

	class LigthTypeType : Record {
		Record inner;
		Fun matchFun;

		public LigthTypeType(string name, Record inner, Fun matchFun) {
			this.inner = inner;
			this.matchFun = matchFun;
			this.meta = new TypeMetadata {
				Name = name
			};
		}
	}
}