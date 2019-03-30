using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using String = System.String;

namespace Lumen.Lang {
    public static class Converter {
        /// <summary> Converts any value to Boolean </summary>
        /// <param name="value">Value for converting</param>
        public static Boolean ToBoolean(this Value value) {
            if (value is Bool b) {
                return b.value;
            }

            if (value is Void) {
                return false;
            }

            return true;
        }

        /// <summary> Converts any value to Double </summary>
        /// <param name="value">Value for converting</param>
        public static Double ToDouble(this Value value, Scope scope) {
            if (value is Number num) {
                return num.value;
            }

            return ToDouble(ToNum(value, scope), scope);
        }

        /// <summary> Converts any value to Int </summary>
        /// <param name="value">Value for converting</param>
        public static Int32 ToInt(this Value value, Scope scope) {
            if (value is Number num)
                return (Int32)num.value;

            return ToInt(ToNum(value, scope), scope);
        }

        /// <summary> Converts any value to Num </summary>
        /// <param name="value">Value for converting</param>
        public static Number ToNum(this Value value, Scope scope) {
            if (value is Number num) {
                return num;
            } 

            IObject type = value.Type;

            if (type.TryGetField("num", out Value converterPrototype)
                    && converterPrototype is Fun converter) {
                Scope s = new Scope(scope) {
                    This = value
                };
                return ToNum(converter.Run(s), scope);
            }

            throw new LumenException(Exceptions.CONVERT_ERROR.F(value.Type, Prelude.Number));
        }

        public static BigFloat ToBigFloat(this Value value, Scope scope) {
            if (value is Number num) {
                return new BigFloat(num.value);
            } 

            throw new LumenException("невозможно преобразовать значение типа " + value.Type + " в значение типа Kernel.Fix");
        }

        public static Fun ToFunction(this Value value, Scope scope) {
            if (value is Fun fun) {
                return fun;
            }

         
            throw new LumenException(Exceptions.CONVERT_ERROR.F(value.Type, Prelude.Function));
        }

        public static Dictionary<Value, Value> ToMap(this Value value, Scope e) {
            if (value is Map klist) {
                return klist.value;
            }

            IObject type = value.Type;

            if (type.TryGetField("map", out var prf) && prf is Fun fun) {
                Scope s = new Scope(e) {
                    This = value
                };

                return ToMap(fun.Run(s), e);
            }

            throw new LumenException($"невозможно преобразовать объект типа {type} к объекту типа Kernel.List");
        }

        public static Array ToVec(this Value value, Scope e) {
            if (value is Array klist) {
                return klist;
            }

            if (value is IObject iobj) {
                if (iobj.TryGetField("vec", out Value f)) {
                    Scope s = new Scope(e) {
                        This = value
                    };
                    return ToVec((f as Fun).Run(s), e);
                }

                throw new LumenException($"невозможно преобразовать объект к объекту типа Kernel.List");
            }

            IObject type = value.Type;

            if (type.TryGetField("vec", out var prf) && prf is Fun fun) {
                Scope s = new Scope(e) {
                    This = value
                };
                return ToVec(fun.Run(s), e);
            }

            throw new LumenException($"невозможно преобразовать объект типа {type} к объекту типа Kernel.List");
        }

        public static LinkedList ToLinkedList(this Value value, Scope e) {
            if (value is List list) {
                return list.value;
            }

            if(value is Enumerator en) {
                return LinkedList.Create(en);
            }

            if (value is Array a) {
                return LinkedList.Create(a.value);
            }


            throw new LumenException($"невозможно преобразовать объект типа {value.Type} к объекту типа Kernel.List");
        }

        public static List<Value> ToList(this Value value, Scope e) {
            if (value is Array klist) {
                return klist.value;
            }

            IObject type = value.Type;

            if (type.TryGetField("vec", out var prf) && prf is Fun fun) {
                Scope s = new Scope(e) {
                    This = value
                };
                return ToList(fun.Run(s), e);
            } else if (type.TryGetField("seq", out var prf1) && prf1 is Fun fun1) {
                Scope s = new Scope(e) {
                    This = value
                };
                return ToSequence(fun1.Run(s), e).ToList();
            }

            throw new LumenException($"невозможно преобразовать объект типа {type} к объекту типа Kernel.List");
        }

        public static IEnumerable<Value> ToIterator(this Value val, Int32 count, Scope e) {
            if (val is Enumerator seq) {
                foreach (Value i in seq.innerValue) {
                    yield return i;
                }
            } else {
                if (val is IObject iobj) {
                    if (iobj.TryGetField("seq", out Value f)) {
                        Scope s = new Scope(e) {
                            This = val
                        };

                        foreach (Value i in ToSequence((f as Fun).Run(s), e)) {
                            yield return i;
                        }
                        yield break;
                    }
                }

                if (val.Type.TryGetField("seq", out var prf) && prf is Fun fun) {
                    Scope s = new Scope(e) {
                        This = val
                    };

                    foreach (Value i in ToIterator(fun.Run(s, new Number(count)), count, e)) {
                        yield return i;
                    }
                }
            }
        }

        public static IEnumerable<Value> ToSequence(this Value val, Scope e) {
            if (val is Enumerator seq) {
                foreach (Value i in seq.innerValue) {
                    yield return i;
                }
            } else if (val is List l) {
                foreach (Value i in l.value) {
                    yield return i;
                }
            } else if (val is Array a) {
                foreach (Value i in a.value) {
                    yield return i;
                }
            } else if (val is Text t) {
                foreach (Char i in t.value) {
                    yield return new Text(i.ToString());
                }
            } else {
                if (val is IObject iobj) {
                    if (iobj.TryGetField("Sequence", out Value f)) {
                        Scope s = new Scope(e) {
                            This = val
                        };

                        foreach (Value i in ToSequence((f as Fun).Run(s), e)) {
                            yield return i;
                        }

                        yield break;
                    }
                }

                if (val.Type.TryGetField("Sequence", out Value prf) && prf is Fun fun) {
                    foreach (Value i in ToSequence(fun.Run(new Scope(e), val), e)) {
                        yield return i;
                    }
                }
            }
        }

        public static Number ToValue(Int32 value) {
            return new Number(value);
        }

        public static Number ToValue(Double value) {
            return new Number(value);
        }

        public static Text ToValue(String value) {
            return new Text(value);
        }

        static String bukv = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        #region BaseConverter

        #endregion

        /// <summary>
        /// Переводит из деятичной системы счисления в систему счисления с основанием N
        /// </summary>
        /// <param name="number">Число, которое переводим </param>
        /// <param name="sys">Система счисления, в которую переводим</param>
        /// <returns>Возвращает переведенное число в строковом формате</returns>
        public static String ToN(String number, String sys) {
            String newNum = "";
            Int32 num = Convert.ToInt32(number);
            Int32 chast = Convert.ToInt32(number);
            ArrayList numTemp = new ArrayList();
            while (chast > 0) {
                chast = chast / Convert.ToInt32(sys);
                numTemp.Add(num - chast * Convert.ToInt32(sys));
                num = chast;
            }
            Int32 j;
            for (j = numTemp.Count - 1; j >= 0; j--) {
                newNum += newCh(numTemp[j].ToString(), "to");
            }

            return newNum;
        }
        /// <summary>
        /// Функция, заменяет буквы на числа и наоборот
        /// </summary>
        /// <param name="sym">Число, над которым нужно работать</param>
        /// <param name="otk">В какую сторону осуществляется действие относительно десятичной системы счисления</param>
        /// <returns>Возвращает букву, если числу соответствует буква и наоборот, иначе число</returns>
        public static String newCh(String sym, String otk) {
            String s = "";
            if (otk == "to") {
                if (Convert.ToInt32(sym) > 10) {
                    s += bukv.Substring(Convert.ToInt32(sym) - 10, 1);
                } else {
                    s += sym;
                }
            } else if (otk == "from") {
                if (bukv.IndexOf(sym) == -1) {
                    s += sym;
                } else {
                    s += (bukv.IndexOf(sym) + 10).ToString();
                }
            }
            return s;
        }

        /// <summary>
        /// Переводит системы счисления с основанием N в деятичную систему счисления 
        /// </summary>
        /// <param name="number">Число, которое переводим </param>
        /// <param name="sys">Система счисления, из которой переводим</param>
        /// <returns>Возвращает переведенное число в строковом формате</returns>
        public static String FromN(String number, String sys) {
            Int32 newNum = 0;
            String temp = "";
            Int32 t;
            Int32 i;
            for (i = 0; i < number.Length; i++) {
                temp = "";
                temp += newCh(number.Substring(i, 1), "from");
                t = (Int32)System.Math.Pow(Convert.ToDouble(sys), Convert.ToDouble(number.Length - (i + 1)));
                newNum += Convert.ToInt32(temp) * t;
            }
            return newNum.ToString();
        }

        public static String FromTo(String number, String sysN, String sysK) {
            String temp = "";
            temp = FromN(number, sysN);
            temp = ToN(temp, sysK);
            return temp;
        }
    }
}
