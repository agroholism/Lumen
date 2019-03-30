using System.Collections.Generic;
using System;

namespace Lumen.Lang {
    public class TypeClass : Module {
        public String TypeParameter { get; set; }
        public List<Fun> Requirements { get; set; } = new List<Fun>();

        public Boolean IsTypeImplement(Module type) {
            foreach (Fun requirment in this.Requirements) {
                Boolean founded = false;
                foreach(KeyValuePair<String, Value> function in type.variables) {
                    if(function.Key == requirment.Name 
                        && function.Value is Fun f 
                        && f.Arguments.Count == requirment.Arguments.Count) {
                        founded = true;
                        break;
                    }
                }

                if (!founded) {
                    return false;
                }
            }

            return true;
        }
    }
}
