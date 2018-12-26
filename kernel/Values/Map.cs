using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Lang.Std {
	[Serializable]
	public class Map : Value {
		public Dictionary<Value, Value> value;

		public Map() {
			value = new Dictionary<Value, Value>();
		}
		public override String ToString() {
			return this.ToString(null);
		}
		public Map(Dictionary<Value, Value> value) {
			this.value = value;
		}

		public Value Get(Value name) {
			return value[name];
		}

		public void Set(Value name, Value value) {
			this.value[name] = value;
		}

		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		public KType Type => StandartModule.Map;

		public bool ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public string ToString(Scope e) {
			if (value.Count == 0)
				return "[:]";
			StringBuilder sb = new StringBuilder("[");
			int indexer = 0;
			foreach (var i in value) {
				sb.Append(i.Key + " : " + i.Value.ToString(e));
				if (indexer < value.Count - 1)
					sb.Append(", ");
				indexer++;
			}
			sb.Append("]");
			return sb.ToString();
		}

		public Value Clone() {
			Dictionary<Value, Value> result = new Dictionary<Value, Value>();
			foreach(KeyValuePair<Value, Value> i in this.value) {
				result.Add(i.Key, i.Value);
			}
			return new Map(result);
		}
	}
}
