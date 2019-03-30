using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    public class Number : Value {
        internal Double value;

        public IObject Type => Prelude.Number;

        public Number(Double value) {
            this.value = value;
        }

        public static implicit operator Number(Double value) {
            return new Number(value);
        }

        public static implicit operator Number(Int32 value) {
            return new Number(value);
        }

        public static Number operator -(Number one) {
            return new Number(-one.value);
        }

        public static Number operator +(Number one, Number other) {
            return new Number(one.value + other.value);
        }

        public static Number operator +(Number one, Double other) {
            return new Number(one.value + other);
        }

        public static Number operator -(Number one, Number other) {
            return new Number(one.value - other.value);
        }

        public static Number operator -(Number one, Double other) {
            return new Number(one.value - other);
        }

        public static Number operator /(Number one, Number other) {
            return new Number(one.value / other.value);
        }

        public static Number operator /(Number one, Double other) {
            return new Number(one.value / other);
        }

        public static Number operator *(Number one, Number other) {
            return new Number(one.value * other.value);
        }

        public static Number operator *(Number one, Double other) {
            return new Number(one.value * other);
        }

        public static Boolean operator ==(Number one, Number other) {
            return one.value == other.value;
        }

        public static Boolean operator ==(Number one, Double other) {
            return one.value == other;
        }

        public static Boolean operator !=(Number one, Number other) {
            return one.value != other.value;
        }

        public static Boolean operator !=(Number one, Double other) {
            return one.value != other;
        }

        public static Boolean operator >(Number one, Number other) {
            return one.value > other.value;
        }

        public static Boolean operator >(Number one, Double other) {
            return one.value > other;
        }

        public static Boolean operator <(Number one, Number other) {
            return one.value < other.value;
        }

        public static Boolean operator <(Number one, Double other) {
            return one.value < other;
        }

        public static Boolean operator >=(Number one, Number other) {
            return one.value >= other.value;
        }

        public static Boolean operator >=(Number one, Double other) {
            return one.value >= other;
        }

        public static Boolean operator <=(Number one, Number other) {
            return one.value <= other.value;
        }

        public static Boolean operator <=(Number one, Double other) {
            return one.value <= other;
        }

        public Value Clone() {
            return new Number(this.value);
        }

        public String ToString(Scope e) {
            return this.value.ToString();
        }

        public override String ToString() {
            return this.ToString(null);
        }

        public override Boolean Equals(Object obj) => obj switch
        {
            Number num => this.value == num.value,
            _ => false
        };

        public Int32 CompareTo(Object obj) => obj switch
        {
            Number num => this.value.CompareTo(num.value),
            Value value => throw new LumenException(Exceptions.TYPE_ERROR.F(this.Type, value.Type)),
            _ => throw new LumenException(Exceptions.TYPE_ERROR.F(this.Type, obj.GetType()))
        };

        public override Int32 GetHashCode() {
            Int32 hashCode = 1927925191;
            hashCode = hashCode * -1521134295 + this.value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IObject>.Default.GetHashCode(this.Type);
            return hashCode;
        }
    }
}
