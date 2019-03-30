using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
    public class Array : Value {
        internal List<Value> value;

        public Int32 Count => this.value.Count;
        public IObject Type => Prelude.Array;

        public virtual Value this[Int32 index] {
            get {
                Int32 i = index;
                index = index < 0 ? this.value.Count + index : index;

                if (index >= this.value.Count || index < 0) {
                    throw new LumenException("выход за пределы списка при срезе вида [i]. Требуемый индекс [" + i + "] превышает длину списка [" + this.value.Count + "]");
                }

                return this.value[index];
            }
            set {
                index = index < 0 ? this.value.Count + index : index;

                if (index >= this.value.Count) {
                    for (Int32 j = this.value.Count; j < index; j++) {
                        this.value.Add(Const.UNIT);
                    }

                    this.value.Add(value);

                } else if (index < 0) {
                    this.value.InsertRange(0, new Array(-index - 1).value);
                    this.value.Insert(0, value);
                } else {
                    this.value[index] = value;
                }
            }
        }

        public virtual Value this[Int32 first, Int32 second] {
            get {
                Int32 i = first;
                Int32 j = second;

                if (first < 0) {
                    first = this.value.Count + first;
                }

                if (second < 0) {
                    second = this.value.Count + second;
                }

                if (second != this.value.Count) {
                    second++;
                }

                if (first > this.value.Count || second > this.value.Count || first < 0 || second < 0) {
                    throw new LumenException("выход за пределы списка при срезе вида [i:j]: границы требуемого диапазонa [" + i + ":" + j + "] превышают длину списка [" + this.value.Count + "]");
                }

                if (first >= second) {
                    return new Array();
                }

                List<Value> val = new List<Value>();

                for (Int32 index = first; index < second; index++) {
                    val.Add(this.value[index]);
                }

                return new Array(val);
            }
            set {
                Int32 i = first;
                Int32 j = second;

                if (first < 0) {
                    first = this.value.Count + first;
                }

                if (second < 0) {
                    second = this.value.Count + second;
                }

                if (i == 0) {
                    if (second != this.value.Count) {
                        second++;
                    }
                }

                if (first > this.value.Count || second > this.value.Count) {
                    throw new LumenException("выход за пределы списка при срезе вида [i:j]: границы требуемого диапазонa [" + i + ":" + j + "] превышают длину списка [" + this.value.Count + "]");
                }

                this.value.RemoveRange(first, second);

                this.value.Insert(first, value);
            }
        }

        public Array(Int32 size) {
            this.value = new List<Value>(size);
            for (Int32 i = 0; i < size; i++) {
                this.value.Add(Const.UNIT);
            }
        }

        public Array(params Value[] args) {
            this.value = args.ToList();
        }

        public Array(List<Value> value) {
            this.value = value;
        }

        public String ToString(Scope e) {
            return "[|" + String.Join("; ", this.value) + "|]";
        }

        public override Boolean Equals(Object obj) {
            if (obj is Array) {
                List<Value> v1 = this.value;
                List<Value> v2 = ((Array)obj).value;

                if (v1.Count != v2.Count) {
                    return false;
                }

                for (Int32 i = 0; i < v1.Count; i++) {
                    if (!v1[i].Equals(v2[i])) {
                        return false;
                    }
                }

                return true;
            } else if (obj is List<Value>) {
                List<Value> v1 = this.value;
                List<Value> v2 = (List<Value>)obj;

                if (v1.Count != v2.Count) {
                    return false;
                }

                for (Int32 i = 0; i < v1.Count; i++) {
                    if (!v1[i].Equals(v2[i])) {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        public Int32 CompareTo(Object obj) {
            if (obj is Value) {
                Scope e = new Scope(null);
                e.Set("this", this);
                //return (int)Converter.ToDouble(((Fun)Type.Get("<=>", null)).Run(e, (Value)obj), null);
            }
            throw new LumenException("notcomparable");
        }

        public override Int32 GetHashCode() {
            Int32 res = 0;

            foreach (Value i in this.value) {
                res |= i.GetHashCode();
            }

            return res;
        }

        public Value Clone() {
            List<Value> result = new List<Value>();
            foreach (Value i in this.value) {
                result.Add(i);
            }
            return new Array(result);
        }

        public override String ToString() {
            return this.ToString(null);
        }
    }
}
