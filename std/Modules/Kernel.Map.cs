using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen.Lang {
    internal class MapClass : Module {
        internal MapClass() {
            this.name = "Kernel.Map";

            this.SetField("[]", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
                if (dict.TryGetValue(args[0], out Value result)) {
                    return result;
                }
                throw new LumenException("данный ключ отсутствует в словаре");
            }));

            this.SetField("get_keys", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
                return new Array(dict.Keys.ToList());
            }));

            this.SetField("get_values", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
                return new Array(dict.Values.ToList());
            }));

           /* LambdaFun to_l = new LambdaFun((e, args) => {
                Expando obj = e.Get("this") as Expando;
                List<Value> result = new List<Value> {
                    obj.Get("key", AccessModifiers.PUBLIC, e),
                    obj.Get("value", AccessModifiers.PUBLIC, e)
                };
                return new Array(result);
            });
            */

         /*   this.Set("to_i", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
                return new Enumerator(dict.Select(x => new Expando { ["key", e] = x.Key, ["value", e] = x.Value, ["to_l", e] = to_l }));
            }));*/

            /*this.Set("to_s", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = Converter.ToMap(e.Get("this"), e);
                return new String("[" + String.Join(", ", dict) + "]");
            }));*/
            this.SetField("contains", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
                return (Bool)dict.ContainsKey(args[0]);
            }));
            this.SetField("get?", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
                IObject obj = args[1] as IObject;
                if (dict.TryGetValue(args[0], out Value res)) {
                    (obj.GetField("=", e) as Fun).Run(new Scope(e), res);
                    return (Bool)true;
                }
                (obj.GetField("=", e) as Fun).Run(new Scope(e), Const.UNIT);
                return (Bool)false;
            }));
        }
    }
}
