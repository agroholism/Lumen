using System;
using System.Collections.Generic;
using Lumen;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class LigthTypeGeneric : Expression {
		private System.String nameType;
		private System.String parameter;
		private Expression type;
		private Expression body;
		private Dictionary<System.String, Expression> generic;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public LigthTypeGeneric(System.String nameType, System.String parameter, Expression type, Expression body, Dictionary<System.String, Expression> generic) {
			this.nameType = nameType;
			this.parameter = parameter;
			this.type = type;
			this.body = body;
			this.generic = generic;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			throw new System.NotImplementedException();
		}

		public Value Eval(Scope e) {
			KType type = this.type.Eval(e) as KType;

			Fun f = new AnonymeDefine(new List<ArgumentMetadataGenerator> { new ArgumentMetadataGenerator(parameter, null, null) }, this.body).Eval(e) as Fun;
			var result = new LigthTypeType(nameType, type, f);

			e.Set(nameType, result);

			return Const.NULL;
		}
	}
}