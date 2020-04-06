using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Lang {
    public class Map : Value {
        public Dictionary<Value, Value> value;

        public Map() {
            this.value = new Dictionary<Value, Value>();
        }

        public Map(Dictionary<Value, Value> value) {
            this.value = value;
        }

		public override Boolean Equals(Object obj) {
			if(obj is Map map) {
				if(ReferenceEquals(this, map)) {
					return true;
				}

				if(map.value.Count != this.value.Count) {
					return false;
				}

				return this.value.Zip(map.value, (p1, p2) => p1.Key.Equals(p2.Key) && p1.Value.Equals(p2.Value)).All(x => x);
			}
			return base.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return -342344534;
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

        public IType Type => Prelude.Map;

        public override String ToString() {
            if (this.value.Count == 0) {
                return "[:]";
            }

            StringBuilder sb = new StringBuilder("[");
            Int32 indexer = 0;
            foreach (KeyValuePair<Value, Value> i in this.value) {
                sb.Append(i.Key + " : " + i.Value.ToString());
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
            foreach (KeyValuePair<Value, Value> i in this.value) {
                result.Add(i.Key, i.Value);
            }
            return new Map(result);
        }

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}
	}
}
