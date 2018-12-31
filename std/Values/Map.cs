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
			this.value = new Dictionary<Value, Value>();
		}
		public override String ToString() {
			return this.ToString(null);
		}
		public Map(Dictionary<Value, Value> value) {
			this.value = value;
		}

		public Value Get(Value name) {
			return this.value[name];
		}

		public void Set(Value name, Value value) {
			this.value[name] = value;
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public Record Type => StandartModule.Map;

		public Boolean ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public Double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			if (this.value.Count == 0) {
				return "[:]";
			}

			StringBuilder sb = new StringBuilder("[");
			Int32 indexer = 0;
			foreach (KeyValuePair<Value, Value> i in this.value) {
				sb.Append(i.Key + " : " + i.Value.ToString(e));
				if (indexer < this.value.Count - 1) {
					sb.Append(", ");
				}

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
