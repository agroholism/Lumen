using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
    public static class Helper {
        public static IObject CreateConstructor(String name, Module baseType, IEnumerable<String> fields) {
            if (fields.Count() == 0) {
                return new SingletonConstructor(name, baseType);
            }

            return new Constructor(name, baseType, fields.ToList());
        }

        public static IObject CreateSome(Value value) {
            Instance result = new Instance(Prelude.Some);

            result.items[0] = value;

            return result;
        }
    }
}
