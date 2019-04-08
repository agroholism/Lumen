using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

namespace Lumen.Studio {
    public class LumenLanguage : Language {
        internal LumenLanguage() {
            this.Name = "Lumen";
            this.Extensions = new List<String> { ".lm" };
            this.IdentOn = true;

            this.Styles.Add(new Regex("(\"\")|\".*?[^\\\\]\"", RegexOptions.Compiled | RegexOptions.Singleline), Settings.String);
            this.Styles.Add(new Regex("//.*"), Settings.Comment);
            this.Styles.Add(new Regex("#\\w+"), Settings.Type);
            this.Styles.Add(new Regex("'\\w+"), Settings.Type);
            this.Styles.Add(new Regex("\\b(open|deriving|class|tailrec|with|as|type|mut|async|await|raise|match|module|next|try|except|finally|global|break|not|self|and|is|let|module|or|xor|where|new|do|end|this|if|else|for|in|while|from|return|true|false|void)\\b"), Settings.Keyword);
            this.Styles.Add(new Regex("\\b(Unit|File|Number|Fail|Ok|Resut|Sequence|Text|Some|None|Optional|Array|Boolean|Map|List|Function)\\b"), Settings.Type);
            this.Styles.Add(new Regex("\\b(Functor|Monad|Applicative|Comonad|Contravariant|Alternative|Ord|Enum)\\b"), Settings.Interface);
        }

        internal override void OnTextChanged(TextBoxManager manager, Range rng) {
            FastColoredTextBox textBox = manager.TextBox;

            textBox.Range.ClearFoldingMarkers();

            foreach ((String, String) i in this.Folding) {
                textBox.Range.SetFoldingMarkers(i.Item1, i.Item2);
            }

            if (this.IdentOn) {
                this.Ident(manager.TextBox);
            }

            this.RefsAndCommentsFolding(manager.TextBox);

            rng.ClearStyle(StyleIndex.ALL);

            foreach (KeyValuePair<Regex, Style> i in this.Styles) {
				if (i.Key.Options.HasFlag(RegexOptions.Singleline)) {
					manager.TextBox.Range.SetStyle(i.Value, i.Key); // or visible range?
				}
				else {
					rng.SetStyle(i.Value, i.Key);
				}
            }

            foreach (Bookmark i in textBox.Bookmarks) {
                textBox.GetLine(i.LineIndex).ClearStyle(Settings.UnactiveBreakpoint, Settings.Type, Settings.Keyword, Settings.String);
                textBox.GetLine(i.LineIndex).SetStyle(Settings.UnactiveBreakpoint);
            }

            manager.RebuildMenu();
        }

        private void RefsAndCommentsFolding(FastColoredTextBox textBox) {
            for (Int32 i = 0; i < textBox.LinesCount; i++) {
                Line line = textBox[i];

                if (Regex.IsMatch(line.Text, "^#ref.*", RegexOptions.Multiline)) {
                    Int32 importSectionBegin = i;
                    i++;

                    while (Regex.IsMatch(textBox[i].Text, "\\s*#ref.*")) {
                        i++;
                    }

                    Int32 importSectionEnd = i - 1;

                    if (importSectionEnd - importSectionBegin > 0) {
                        textBox[importSectionBegin].FoldingStartMarker = "m" + importSectionBegin;
                        textBox[importSectionEnd].FoldingEndMarker = "m" + importSectionBegin;
                    }
                } else if (Regex.IsMatch(line.Text, "^//.*", RegexOptions.Multiline)) {
                    Int32 commentBegin = i;
                    i++;
                    try {
                        while (Regex.IsMatch(textBox[i].Text, "^//.*")) {
                            i++;
                        }
                    } catch {

                    }
                    Int32 commentEnd = i - 1;

                    if (commentEnd - commentBegin > 0) {
                        textBox[commentBegin].FoldingStartMarker = "m" + commentBegin;
                        textBox[commentEnd].FoldingEndMarker = "m" + commentBegin;
                    }
                }
            }
        }

        internal override List<AutocompleteItem> GetAutocompleteItems(TextBoxManager manager) {
            List<AutocompleteItem> items = new List<AutocompleteItem> {
				new AutocompleteItem("prelude", (Int32)Images.MODULE, "prelude", "module prelude = ...", "Содержит базовые типы, модули и функции."),

                new AutocompleteItem("print", (Int32)Images.FUNCTION, "print", "let print x = ...", "Выводит аргументы в стандартный поток вывода"),
                new AutocompleteItem("read", (Int32)Images.FUNCTION, "read", "let read () = ...", ""),

                new AutocompleteItem("writeFile", (Int32)Images.FUNCTION, "writeFile", "let writeFile text fileName = ...", "Производит попытку записи текста text в файл fileName.\r\nЕсли выполнение успешно - возвращает (Some fileName), иначе - None"),
                new AutocompleteItem("createFile", (Int32)Images.FUNCTION, "createFile", "let createFile fileName = ...", "Производит попытку создания файла\r\nЕсли выполнение успешно - возвращает (Some fileName), иначе - None"),
                new AutocompleteItem("readFile", (Int32)Images.FUNCTION, "readFile", "let readFile fileName = ...", "Производит попытку считывания содержимого файла в единый текстовый объект.\r\nЕсли выполнение успешно - возвращает (Some Text), иначе - None"),
                new AutocompleteItem("readLines", (Int32)Images.FUNCTION, "readLines", "let readLines fileName = ...", "        "),
                new AutocompleteItem("readArray", (Int32)Images.FUNCTION, "readArray", "let readArray fileName = ...", "        "),

                new AutocompleteItem("Ord", (Int32)Images.MODULE, "Ord", "type class Ord", "Модуль, содержащий функции сравнения на основе оператора <=>."),
                new Mod("Ord.<", (Int32)Images.FUNCTION, "<", "let < x y = ...", "Производит сравнение двух значений."),
                new Mod("Ord.<=", (Int32)Images.FUNCTION, "<=", "let <= x y = ...", "Производит сравнение двух значений."),
                new Mod("Ord.>", (Int32)Images.FUNCTION, ">", "let > x y = ...", "Производит сравнение двух значений."),
                new Mod("Ord.>=", (Int32)Images.FUNCTION, ">=", "let >= x y = ...", "Производит сравнение двух значений."),
                new Mod("Ord.compare", (Int32)Images.FUNCTION, "compare", "let compare x y = ...", "Производит сравнение двух значений."),
                new Mod("Ord.min", (Int32)Images.FUNCTION, "min", "let min x y = ...", "Возвращает минимальное из двух значений"),
                new Mod("Ord.max", (Int32)Images.FUNCTION, "max", "let max x y = ...", "Возвращает максимальное из двух значений"),

                new AutocompleteItem("Sequence", (Int32)Images.MODULE, "Sequence", "module Sequence = ...", "Модуль для перечисляемых типов."),
                new Mod("Sequence.*", (Int32)Images.FUNCTION, "*", "let * s = ...", "Возвращает количество элементов в последовательности."),
                new Mod("Sequence./", (Int32)Images.FUNCTION, "/", "let / s f = ...", "Поведение аналогично функции reduce."),
                new Mod("Sequence.*", (Int32)Images.FUNCTION, "*", "let * a (Text s) = ...", "Конкатенирует последовательность при помощи строки s"),
                new Mod("Sequence.*", (Int32)Images.FUNCTION, "*", "let * a (Number n) = ...", "Повторяет последовательность n раз"),
                new Mod("Sequence.*", (Int32)Images.FUNCTION, "*", "let * a f = ...", "Аналог функции map"),
                new Mod("Sequence.min", (Int32)Images.FUNCTION, "min", "let min x y = ...", "[description not founded]"),
                new Mod("Sequence.max", (Int32)Images.FUNCTION, "max", "let max x y = ...", "[description not founded]"),
                new Mod("Sequence.between", (Int32)Images.FUNCTION, "between", "let between x y z = ...", "[description not founded]"),

                new AutocompleteItem("Number", (Int32)Images.MODULE, "Number", "type _ = Number ...", "[description not founded]"),
                new Mod("Number.~", (Int32)Images.FUNCTION, "~", "let ~ x = ...", "[description not founded]"),
                new Mod("Number.+", (Int32)Images.FUNCTION, "+", "let + x = ...", "[description not founded]"),
                new Mod("Number.+", (Int32)Images.FUNCTION, "+", "let + x y = ...", "[description not founded]"),
                new Mod("Number.-", (Int32)Images.FUNCTION, "-", "let - x = ...", "[description not founded]"),
                new Mod("Number.-", (Int32)Images.FUNCTION, "-", "let - x y = ...", "[description not founded]"),
                new Mod("Number./", (Int32)Images.FUNCTION, "/", "let / x y = ...", "[description not founded]"),
                new Mod("Number.*", (Int32)Images.FUNCTION, "*", "let * x y = ...", "[description not founded]"),
                new Mod("Number.^", (Int32)Images.FUNCTION, "^", "let ^ x y = ...", "[description not founded]"),
                new Mod("Number.>>", (Int32)Images.FUNCTION, ">>", "let >> x y = ...", "[description not founded]"),
                new Mod("Number.<<", (Int32)Images.FUNCTION, "<<", "let << x y = ...", "[description not founded]"),

                new Mod("Number.sin", (Int32)Images.FUNCTION, "sin", "let sin x = ...", "[description not founded]"),
                new Mod("Number.cos", (Int32)Images.FUNCTION, "cos", "let cos x = ...", "[description not founded]"),
                new Mod("Number.tan", (Int32)Images.FUNCTION, "cos", "let tan x = ...", "[description not founded]"),
                new Mod("Number.sinh", (Int32)Images.FUNCTION, "sinh", "let sinh x = ...", "[description not founded]"),
                new Mod("Number.cosh", (Int32)Images.FUNCTION, "cosh", "let cosh x = ...", "[description not founded]"),
                new Mod("Number.tanh", (Int32)Images.FUNCTION, "tanh", "let tanh x = ...", "[description not founded]"),
                new Mod("Number.acos", (Int32)Images.FUNCTION, "acos", "let acos x = ...", "[description not founded]"),
                new Mod("Number.asin", (Int32)Images.FUNCTION, "asin", "let asin x = ...", "[description not founded]"),
                new Mod("Number.atan", (Int32)Images.FUNCTION, "atan", "let atan x = ...", "[description not founded]"),
                new Mod("Number.log", (Int32)Images.FUNCTION, "log", "let log x = ...", "[description not founded]"),
                new Mod("Number.rem", (Int32)Images.FUNCTION, "rem", "let rem x = ...", "[description not founded]"),
                new Mod("Number.mod", (Int32)Images.FUNCTION, "mod", "let mod x = ...", "[description not founded]"),
                new Mod("Number.between", (Int32)Images.FUNCTION, "between", "let between x y z = ...", "[description not founded]"),

                new AutocompleteItem("PI", (Int32)Images.VARIABLE, "PI", "let PI = ...", "[description not founded]"),
                new AutocompleteItem("E", (Int32)Images.VARIABLE, "E", "let E = ...", "[description not founded]"),

                new AutocompleteItem("Text", (Int32)Images.MODULE, "Text", "type _ = Text ...", "[description not founded]"),

                new AutocompleteItem("List", (Int32)Images.MODULE, "List", "type List = List ...", "(Functor, Applicative, Aliternative, Monad, Comonad, Ord, Enum)"),
                new Mod("List.+", (Int32)Images.FUNCTION, "+", "let + l1 l2 = ...", "Складывает два списка"),
                new Mod("List./", (Int32)Images.FUNCTION, "/", "let / l f = ...", "Редуцирует список"),
                new Mod("List.chunkBySize", (Int32)Images.FUNCTION, "chunkBySize", "let chunkBySize l s = ...", "Разбивает список на меньшие списки размера s"),
                new Mod("List.collect", (Int32)Images.FUNCTION, "collect", "let collect l f = ...", "Применяет функцию f к каждому элементу списка (функция должна возвращать список) и объединяет результаты"),
                new Mod("List.map", (Int32)Images.FUNCTION, "map", "let map l f = ...", "Применяет функцию f к каждому элементу списка"),
                new Mod("List.filter", (Int32)Images.FUNCTION, "filter", "let filter l f = ...", "Фильтрует список на основе предиката f"),
                new Mod("List.reduce", (Int32)Images.FUNCTION, "reduce", "let reduce s f = ...", "Редуцирует список"),
                new Mod("List.first", (Int32)Images.FUNCTION, "first", "let first s f = ...", "Возвращает первый элемент списка"),
                new Mod("List.last", (Int32)Images.FUNCTION, "last", "let last s f = ...", "Возвращает последний элемент списка"),
                new Mod("List.head", (Int32)Images.FUNCTION, "head", "let head l = ...", "Возвращает голову списка"),
                new Mod("List.tail", (Int32)Images.FUNCTION, "tail", "let tail l = ...", "Возвращает хвост списка"),
                new Mod("List.contains", (Int32)Images.FUNCTION, "contains", "let contains l e = ...", "Определяет содержится ли элемент в списке"),
                new Mod("List.count", (Int32)Images.FUNCTION, "count", "let count l x = ...", "Возвращает число, представляющее количество элементов в указанной последовательности удовлетворяют условию."),
                new Mod("List.action", (Int32)Images.FUNCTION, "action", "let action l act = ...", "Возвращает"),
                new Mod("List.isAll", (Int32)Images.FUNCTION, "isAll", "let isAll l f = ...", "Возвращает"),
                new Mod("List.isAny", (Int32)Images.FUNCTION, "isAny", "let isAny l f = ...", "Возвращает"),
                new Mod("List.find", (Int32)Images.FUNCTION, "find", "let find l f = ...", "Возвращает"),
                new Mod("List.findAll", (Int32)Images.FUNCTION, "findAll", "let findAll l f = ...", "Возвращает"),
                new Mod("List.min", (Int32)Images.FUNCTION, "min", "let min l = ...", "Возвращает"),
                new Mod("List.max", (Int32)Images.FUNCTION, "max", "let max l = ...", "Возвращает"),
                new Mod("List.sort", (Int32)Images.FUNCTION, "sort", "let sort l = ...", "Сортирует список"),
                new Mod("List.sortBy", (Int32)Images.FUNCTION, "sortBy", "let sortBy l f = ...", "Сортирует список по функции f"),
                new Mod("List.sortDescending", (Int32)Images.FUNCTION, "sortDescending", "let sortDescending l = ...", "Сортирует список по функции f в порядке убывания"),
                new Mod("List.sortByDescending", (Int32)Images.FUNCTION, "sortByDescending", "let sortByDescending l f = ...", "Сортирует список по функции f в порядке убывания"),

                new AutocompleteItem("Array", (Int32)Images.MODULE, "Array", "type Array = ...", "Определяет операции на массивами"),
                new Mod("Array.*", (Int32)Images.FUNCTION, "*", "let * a = ...", "Возвращает длину массива"),
                new Mod("Array.+", (Int32)Images.FUNCTION, "+", "let + a s = ...", "Конкатенирует две последовательности."),
                new Mod("Array.-", (Int32)Images.FUNCTION, "-", "let - a s = ...", "Удаляет из массива все элементы, содержащиеся в последовательности s."),
                new Mod("Array./", (Int32)Images.FUNCTION, "/", "let / l f = ...", "Редуцирует массив"),
                new Mod("Array.*", (Int32)Images.FUNCTION, "*", "let * a (Text s) = ...", "Конкатенирует массив при помощи строки s"),
                new Mod("Array.*", (Int32)Images.FUNCTION, "*", "let * a (Number n) = ...", "Повторяет массив n раз"),
                new Mod("Array.*", (Int32)Images.FUNCTION, "*", "let * a f = ...", "Аналог функции map"),
                new Mod("Array.average", (Int32)Images.FUNCTION, "average", "let average l = ...", "Находит среднее из элементов массива"),
                new Mod("Array.map", (Int32)Images.FUNCTION, "map", "let map l f = ...", "Применяет функцию f к каждому элементу массива"),
                new Mod("Array.filter", (Int32)Images.FUNCTION, "filter", "let filter l f = ...", "Фильтрует массив на основе предиката f"),
                new Mod("Array.reduce", (Int32)Images.FUNCTION, "reduce", "let reduce s f = ...", "Редуцирует список"),
                new Mod("Array.first", (Int32)Images.FUNCTION, "first", "let first s f = ...", "Возвращает первый элемент массив"),
                new Mod("Array.last", (Int32)Images.FUNCTION, "last", "let last s f = ...", "Возвращает последний элемент массив"),
                new Mod("Array.contains", (Int32)Images.FUNCTION, "contains", "let contains l e = ...", "Определяет содержится ли элемент в массиве"),
                new Mod("Array.count", (Int32)Images.FUNCTION, "count", "let count l x = ...", "Возвращает число, представляющее количество элементов в указанной последовательности удовлетворяют условию."),
                new Mod("Array.action", (Int32)Images.FUNCTION, "action", "let action l act = ...", "Возвращает"),
                new Mod("Array.isAll", (Int32)Images.FUNCTION, "isAll", "let isAll l f = ...", "Возвращает"),
                new Mod("Array.isAny", (Int32)Images.FUNCTION, "isAny", "let isAny l f = ...", "Возвращает"),
                new Mod("Array.find", (Int32)Images.FUNCTION, "find", "let find l f = ...", "Возвращает"),
                new Mod("Array.findAll", (Int32)Images.FUNCTION, "findAll", "let findAll l f = ...", "Возвращает"),
                new Mod("Array.min", (Int32)Images.FUNCTION, "min", "let min l = ...", "Возвращает"),
                new Mod("Array.max", (Int32)Images.FUNCTION, "max", "let max l = ...", "Возвращает"),
                new Mod("Array.sort", (Int32)Images.FUNCTION, "sort", "let sort l = ...", "Сортирует массив"),
                new Mod("Array.sortBy", (Int32)Images.FUNCTION, "sortBy", "let sortBy l f = ...", "Сортирует массив по функции f"),
                new Mod("Array.sortDescending", (Int32)Images.FUNCTION, "sortDescending", "let sortDescending l = ...", "Сортирует массив по функции f в порядке убывания"),
                new Mod("Array.sortByDescending", (Int32)Images.FUNCTION, "sortByDescending", "let sortByDescending l f = ...", "Сортирует массив по функции f в порядке убывания"),

                new AutocompleteItem("Monad", (Int32)Images.MODULE, "Monad", "type class Monad", "        "),
                new AutocompleteItem("ApplicativeFunctor", (Int32)Images.MODULE, "ApplicativeFunctor", "type class ApplicativeFunctor", "        "),
                new AutocompleteItem("Functor", (Int32)Images.MODULE, "Functor", "type class Functor", "        "),
                new Mod("List.Array", (Int32)Images.FUNCTION, "Array", "let Array l = ...", "Преобразует список в массив"),
                new Mod("List.Sequence", (Int32)Images.FUNCTION, "Sequence", "let Sequence l = ...", "Преобразует список в последовательность"),

                new AutocompleteItem("Text", (Int32)Images.MODULE, "Text", "type _ = Text ...", "Модуль для работы с текстовыми данными"),
                new Mod("Text.*", (Int32)Images.FUNCTION, "*", "let * x = ...", "Возвращает длину строки"),
                new Mod("Text.+", (Int32)Images.FUNCTION, "+", "let + x y = ...", "Конткатенирует две строки"),
                new Mod("Text.%", (Int32)Images.FUNCTION, "%", "let % x y = ...", "Форматирует текст"),
                new Mod("Text.*", (Int32)Images.FUNCTION, "*", "let * x y = ...", "Повторяет строку заданное число раз"),
                new Mod("Text.<<", (Int32)Images.FUNCTION, "<<", "let << x y = ...", "Добавляет строку к существующей"),

                new Mod("Text.upper", (Int32)Images.FUNCTION, "upper", "let upper x = ...", "Переводит строку в верхний регистр"),
                new Mod("Text.lower", (Int32)Images.FUNCTION, "lower", "let lower x = ...", "Переводит строку в нижний регистр"),
                new Mod("Text.contains", (Int32)Images.FUNCTION, "contains", "let contains x y = ...", "Определяет содержится ли подстрока в строке"),
                new Mod("Text.isStartsWith", (Int32)Images.FUNCTION, "isStartsWith", "let isStartsWith x y = ...", "Определяет начинается ли текст с указанной подстроки"),
                new Mod("Text.isEndsWith", (Int32)Images.FUNCTION, "isEndsWith", "let isEndsWith x y = ...", "Определяет заканчивается ли текст указанной подстрокой"),
                new Mod("Text.replace", (Int32)Images.FUNCTION, "replace", "let replace x y z = ...", "Заменяет все вхождения строки y на строку z"),
                new Mod("Text.isEmpty", (Int32)Images.FUNCTION, "isEmpty", "let isEmpty x = ...", "Определяет является ли текст пустым"),

                new AutocompleteItem("Unit", (Int32)Images.MODULE, "Unit", "type Unit = ()", "[description not founded]"),

                new AutocompleteItem("Boolean", (Int32)Images.MODULE, "Boolean", "type Boolean = true | false", "[description not founded]"),
                new Mod("Boolean.true", (Int32)Images.TYPE, "true", "type Boolean = true | false", "[description not founded]"),
                new Mod("Boolean.false", (Int32)Images.TYPE, "false", "type Boolean = true | false", "[description not founded]"),

                new AutocompleteItem("true", (Int32)Images.TYPE, "true", "type Boolean = true | false", "[description not founded]"),

                new AutocompleteItem("false", (Int32)Images.TYPE, "false", "type Boolean = true | false", "[description not founded]"),

                new AutocompleteItem("Fail", (Int32)Images.TYPE, "Fail", "type _ = Fail message", "[description not founded]"),
            };

            String f(String s) {
                String[] ss = s.Split('.');

                return ss[ss.Length - 1];
            }

          /* try {
                var res = ldoc.Program.Parse(manager.TextBox.Text);

                foreach (var i in res) {
                    if(i.Type == ldoc.Program.Type.FUNCTION) {
                        if (i.Name.Contains(".")) {
                            items.Add(new Mod(i.Name, (Int32)Images.FUNCTION, f(i.Name), i.Declaration, i.Summary ?? "\t\t\t"));
                        } else {
                            items.Add(new AutocompleteItem(i.Name, (Int32)Images.FUNCTION, i.Name, i.Declaration, i.Summary ?? "\t\t\t"));
                        }
                    } else if (i.Type == ldoc.Program.Type.MODULE) {
                        if (i.Name.Contains(".")) {
                            items.Add(new Mod(i.Name, (Int32)Images.MODULE, f(i.Name), i.Declaration, i.Summary ?? "\t\t\t"));
                        } else {
                            items.Add(new AutocompleteItem(i.Name, (Int32)Images.MODULE, i.Name, i.Declaration, i.Summary ?? "\t\t\t"));
                        }
                    } else if (i.Type == ldoc.Program.Type.CONSTRUCTOR) {
                        if (i.Name.Contains(".")) {
                            items.Add(new Mod(i.Name, (Int32)Images.SUBTYPE, f(i.Name), i.Declaration, i.Summary ?? "\t\t\t"));
                        } else {
                            items.Add(new AutocompleteItem(i.Name, (Int32)Images.SUBTYPE, i.Name, i.Declaration, i.Summary ?? "\t\t\t"));
                        }
                    }
                }
            } catch { }
            */
            return items;
        }
    }
}
