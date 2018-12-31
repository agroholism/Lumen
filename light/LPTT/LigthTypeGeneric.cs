using System;
using System.Collections.Generic;
using Lumen;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class LigthTypeGeneric : Expression {
		private readonly System.String nameType;
		private readonly System.String parameter;
		private readonly Expression type;
		private readonly Expression body;
		private readonly Dictionary<System.String, Expression> generic;
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
			Record type = this.type.Eval(e) as Record;

			Fun f = new AnonymeDefine(new List<ArgumentMetadataGenerator> { new ArgumentMetadataGenerator(this.parameter, null, null) }, this.body).Eval(e) as Fun;
			LigthTypeType result = new LigthTypeType(this.nameType, type, f);

			e.Set(this.nameType, result);

			return Const.NULL;
		}
	}
}