using System.Drawing;
using System;

namespace Lumen.Lang.Libraries.Visual {
    public class ColorModule : Module {
        public static IType Red { get; private set; }
        public static IType AliceBlue { get; private set; }
        public static IType AntiqueWhite { get; private set; }
        public static IType Aqua { get; private set; }
        public static IType Aquamarine { get; private set; }
        public static IType Azure { get; private set; }
        public static IType Beige { get; private set; }
        public static IType Bisque { get; private set; }
        public static IType Black { get; private set; }
        public static IType BlanchedAlmond { get; private set; }
        public static IType Blue { get; private set; }
        public static IType BurlyWood { get; private set; }
        public static IType Cyan { get; private set; }
        public static IType DarkBlue { get; private set; }
        public static IType DarkGoldenrod { get; private set; }
        public static IType DarkGray { get; private set; }
        public static IType DarkOliveGreen { get; private set; }
        public static IType DeepSkyBlue { get; private set; }
        public static IType Green { get; private set; }
        public static IType DodgerBlue { get; private set; }
        public static IType DimGray { get; private set; }
        public static IType DeepPink { get; private set; }
        public static IType DarkViolet { get; private set; }
        public static IType DarkTurquoise { get; private set; }
        public static IType DarkSlateGray { get; private set; }
        public static IType DarkSlateBlue { get; private set; }
        public static IType DarkSalmon { get; private set; }
        public static IType DarkSeaGreen { get; private set; }
        public static IType DarkRed { get; private set; }
        public static IType DarkOrchid { get; private set; }
        public static IType DarkOrange { get; private set; }
        public static IType DarkMagenta { get; private set; }
        public static IType DarkKhaki { get; private set; }
        public static IType DarkGreen { get; private set; }
        public static IType DarkCyan { get; private set; }
        public static IType Crimson { get; private set; }
        public static IType Cornsilk { get; private set; }
        public static IType CornflowerBlue { get; private set; }
        public static IType Coral { get; private set; }
        public static IType Chocolate { get; private set; }
        public static IType Chartreuse { get; private set; }
        public static IType CadetBlue { get; private set; }
        public static IType Brown { get; private set; }
        public static IType BlueViolet { get; private set; }
        public static IType White { get; private set; }

        public static IType RGB { get; private set; }
        public static IType ARGB { get; private set; }

        public ColorModule() {
            this.Name = "Visual.Color";

            Red = Helper.CreateConstructor("Visual.Color.Red", this, new String[0]);
            AliceBlue = Helper.CreateConstructor("Visual.Color.AliceBlue", this, new String[0]);
            AntiqueWhite = Helper.CreateConstructor("Visual.Color.AntiqueWhite", this, new String[0]);
            Aqua = Helper.CreateConstructor("Visual.Color.Aqua", this, new String[0]);
            Aquamarine = Helper.CreateConstructor("Visual.Color.Aquamarine", this, new String[0]);
            Azure = Helper.CreateConstructor("Visual.Color.Azure", this, new String[0]);
            Beige = Helper.CreateConstructor("Visual.Color.Beige", this, new String[0]);
            Bisque = Helper.CreateConstructor("Visual.Color.Bisque", this, new String[0]);
            Black = Helper.CreateConstructor("Visual.Color.Black", this, new String[0]);
            BlanchedAlmond = Helper.CreateConstructor("Visual.Color.BlanchedAlmond", this, new String[0]);
            Blue = Helper.CreateConstructor("Visual.Color.Blue", this, new String[0]);
            BlueViolet = Helper.CreateConstructor("Visual.Color.BlueViolet", this, new String[0]);
            Brown = Helper.CreateConstructor("Visual.Color.Brown", this, new String[0]);
            BurlyWood = Helper.CreateConstructor("Visual.Color.BurlyWood", this, new String[0]);
            CadetBlue = Helper.CreateConstructor("Visual.Color.CadetBlue", this, new String[0]);
            Chartreuse = Helper.CreateConstructor("Visual.Color.Chartreuse", this, new String[0]);
            Chocolate = Helper.CreateConstructor("Visual.Color.Chocolate", this, new String[0]);
            Coral = Helper.CreateConstructor("Visual.Color.Coral", this, new String[0]);
            CornflowerBlue = Helper.CreateConstructor("Visual.Color.CornflowerBlue", this, new String[0]);
            Cornsilk = Helper.CreateConstructor("Visual.Color.Cornsilk", this, new String[0]);
            Crimson = Helper.CreateConstructor("Visual.Color.Crimson", this, new String[0]);
            Cyan = Helper.CreateConstructor("Visual.Color.Cyan", this, new String[0]);
            DarkBlue = Helper.CreateConstructor("Visual.Color.DarkBlue", this, new String[0]);
            DarkCyan = Helper.CreateConstructor("Visual.Color.DarkCyan", this, new String[0]);
            DarkGoldenrod = Helper.CreateConstructor("Visual.Color.DarkGoldenrod", this, new String[0]);
            DarkGray = Helper.CreateConstructor("Visual.Color.DarkGray", this, new String[0]);
            DarkGreen = Helper.CreateConstructor("Visual.Color.DarkGreen", this, new String[0]);
            DarkKhaki = Helper.CreateConstructor("Visual.Color.DarkKhaki", this, new String[0]);
            DarkMagenta = Helper.CreateConstructor("Visual.Color.DarkMagenta", this, new String[0]);
            DarkOliveGreen = Helper.CreateConstructor("Visual.Color.DarkOliveGreen", this, new String[0]);
            DarkOrange = Helper.CreateConstructor("Visual.Color.DarkOrange", this, new String[0]);
            DarkOrchid = Helper.CreateConstructor("Visual.Color.DarkOrchid", this, new String[0]);
            DarkRed = Helper.CreateConstructor("Visual.Color.DarkRed", this, new String[0]);
            DarkSalmon = Helper.CreateConstructor("Visual.Color.DarkSalmon", this, new String[0]);
            DarkSeaGreen = Helper.CreateConstructor("Visual.Color.DarkSeaGreen", this, new String[0]);
            DarkSlateBlue = Helper.CreateConstructor("Visual.Color.DarkSlateBlue", this, new String[0]);
            DarkSlateGray = Helper.CreateConstructor("Visual.Color.DarkSlateGray", this, new String[0]);
            DarkTurquoise = Helper.CreateConstructor("Visual.Color.DarkTurquoise", this, new String[0]);
            DarkViolet = Helper.CreateConstructor("Visual.Color.DarkViolet", this, new String[0]);
            DeepPink = Helper.CreateConstructor("Visual.Color.DeepPink", this, new String[0]);
            DeepSkyBlue = Helper.CreateConstructor("Visual.Color.DeepSkyBlue", this, new String[0]);
            DimGray = Helper.CreateConstructor("Visual.Color.DimGray", this, new String[0]);
            DodgerBlue = Helper.CreateConstructor("Visual.Color.DodgerBlue", this, new String[0]);
            Green = Helper.CreateConstructor("Visual.Color.Green", this, new String[0]);
            White = Helper.CreateConstructor("Visual.Color.White", this, new String[0]);

            RGB = Helper.CreateConstructor("Visual.Color.RGB", this, new[] { "R", "G", "B" });

            ARGB = Helper.CreateConstructor("Visual.Color.ARGB", this, new[] { "A", "R", "G", "B" });

            this.SetMember("RGB", RGB);
            this.SetMember("ARGB", ARGB);
            this.SetMember("Green", Green);
            this.SetMember("DodgerBlue", DodgerBlue);
            this.SetMember("Red", Red);
            this.SetMember("Green", Green);
            this.SetMember("Blue", Blue);
            this.SetMember("Black", Black);
            this.SetMember("White", White);
        }

        internal static Color ToSystemColor(IType obj, Scope s) {
            if(obj == AliceBlue) {
                return Color.AliceBlue;
            }

            if (obj == AntiqueWhite) {
                return Color.AntiqueWhite;
            }

            if (obj == Aqua) {
                return Color.Aqua;
            }

            if (obj == Aquamarine) {
                return Color.Aquamarine;
            }

            if (obj == Azure) {
                return Color.Azure;
            }

            if (obj == Beige) {
                return Color.Beige;
            }

            if (obj == Bisque) {
                return Color.Bisque;
            }

            if (obj == Black) {
                return Color.Black;
            }

            if (obj == BlanchedAlmond) {
                return Color.BlanchedAlmond;
            }

            if (obj == Blue) {
                return Color.Blue;
            }

            if (obj == BlueViolet) {
                return Color.BlueViolet;
            }

            if (obj == Brown) {
                return Color.Brown;
            }

            if (obj == BurlyWood) {
                return Color.BurlyWood;
            }

            if (obj == CadetBlue) {
                return Color.CadetBlue;
            }

            if (obj == Chartreuse) {
                return Color.Chartreuse;
            }

            if (obj == Chocolate) {
                return Color.Chocolate;
            }

            if (obj == Coral) {
                return Color.Coral;
            }

            if (obj == CornflowerBlue) {
                return Color.CornflowerBlue;
            }

            if (obj == Cornsilk) {
                return Color.Cornsilk;
            }

            if (obj == Crimson) {
                return Color.Crimson;
            }

            if (obj == Cyan) {
                return Color.Cyan;
            }

            if (obj == DarkBlue) {
                return Color.DarkBlue;
            }

            if (obj == DarkCyan) {
                return Color.DarkCyan;
            }

            if (obj == DarkGoldenrod) {
                return Color.DarkGoldenrod;
            }

            if (obj == DarkGray) {
                return Color.DarkGray;
            }

            if (obj == DarkGreen) {
                return Color.DarkGreen;
            }

            if (obj == DarkKhaki) {
                return Color.DarkKhaki;
            }

            if (obj == DarkMagenta) {
                return Color.DarkMagenta;
            }

            if (obj == DarkOliveGreen) {
                return Color.DarkOliveGreen;
            }

            if (obj == DarkOrange) {
                return Color.DarkOrange;
            }

            if (obj == DarkOrchid) {
                return Color.DarkOrchid;
            }

            if (obj == DarkRed) {
                return Color.DarkRed;
            }

            if (obj == DarkSalmon) {
                return Color.DarkSalmon;
            }

            if (obj == DarkSeaGreen) {
                return Color.DarkSeaGreen;
            }

            if (obj == DarkSlateBlue) {
                return Color.DarkSlateBlue;
            }

            if (obj == DarkSlateGray) {
                return Color.DarkSlateGray;
            }

            if (obj == DarkTurquoise) {
                return Color.DarkTurquoise;
            }

            if (obj == DarkViolet) {
                return Color.DarkViolet;
            }

            if (obj == DeepPink) {
                return Color.DeepPink;
            }

            if (obj == DeepSkyBlue) {
                return Color.DeepSkyBlue;
            }

            if (obj == Red) {
                return Color.Red;
            }

            if (obj == DimGray) {
                return Color.DimGray;
            }

            if (obj == DodgerBlue) {
                return Color.DodgerBlue;
            }

            if (obj == White) {
                return Color.White;
            }

            /* if (obj == Firebrick) {
                 return Color.Firebrick;
             }

             if (obj == FloralWhite) {
                 return Color.FloralWhite;
             }

             if (obj == Fuchsia) {
                 return Color.Fuchsia;
             }

             if (obj == Gainsboro) {
                 return Color.Gainsboro;
             }

             if (obj == GhostWhite) {
                 return Color.GhostWhite;
             }

             if (obj == Gold) {
                 return Color.Gold;
             }

             if (obj == Goldenrod) {
                 return Color.Goldenrod;
             }

             if (obj == Gray) {
                 return Color.Gray;
             }

             if (obj == Green) {
                 return Color.Green;
             }

             if (obj == GreenYellow) {
                 return Color.GreenYellow;
             }

             if (obj == Honeydew) {
                 return Color.Honeydew;
             }

             if (obj == HotPink) {
                 return Color.HotPink;
             }

             if (obj == IndianRed) {
                 return Color.IndianRed;
             }

             if (obj == Indigo) {
                 return Color.Indigo;
             }

             if (obj == Ivory) {
                 return Color.Ivory;
             }

             if (obj == Khaki) {
                 return Color.Khaki;
             }

             if (obj == Lavender) {
                 return Color.Lavender;
             }

             if (obj == LavenderBlush) {
                 return Color.LavenderBlush;
             }

             if (obj == LawnGreen) {
                 return Color.LawnGreen;
             }

             if (obj == LemonChiffon) {
                 return Color.LemonChiffon;
             }

             if (obj == LightBlue) {
                 return Color.LightBlue;
             }

             if (obj == LightCoral) {
                 return Color.LightCoral;
             }

             if (obj == LightCyan) {
                 return Color.LightCyan;
             }

             if (obj == LightGoldenrodYellow) {
                 return Color.LightGoldenrodYellow;
             }

             if (obj == LightGray) {
                 return Color.LightGray;
             }

             if (obj == LightGreen) {
                 return Color.LightGreen;
             }

             if (obj == LightPink) {
                 return Color.LightPink;
             }

             if (obj == Aqua) {
                 return Color.LightSalmon;
             }

             if (obj == Aqua) {
                 return Color.LightSeaGreen;
             }

             if (obj == Aqua) {
                 return Color.LightSkyBlue;
             }

             if (obj == Aqua) {
                 return Color.LightSlateGray;
             }

             if (obj == Aqua) {
                 return Color.LightSteelBlue;
             }

             if (obj == Aqua) {
                 return Color.LightYellow;
             }

             if (obj == Aqua) {
                 return Color.Lime;
             }

             if (obj == Aqua) {
                 return Color.LimeGreen;
             }

             if (obj == Aqua) {
                 return Color.Linen;
             }

             if (obj == Aqua) {
                 return Color.Magenta;
             }

             if (obj == Aqua) {
                 return Color.Maroon;
             }

             if (obj == Aqua) {
                 return Color.MediumAquamarine;
             }

             if (obj == Aqua) {
                 return Color.MediumBlue;
             }

             if (obj == Aqua) {
                 return Color.MediumOrchid;
             }

             if (obj == Aqua) {
                 return Color.MediumPurple;
             }

             if (obj == Aqua) {
                 return Color.MediumSeaGreen;
             }

             if (obj == Aqua) {
                 return Color.MediumSlateBlue;
             }

             if (obj == Aqua) {
                 return Color.MediumSpringGreen;
             }

             if (obj == Aqua) {
                 return Color.MediumTurquoise;
             }


             if (obj == Aqua) {
                 return Color.MediumVioletRed;
             }

             if (obj == Aqua) {
                 return Color.MidnightBlue;
             }

             if (obj == Aqua) {
                 return Color.MintCream;
             }

             if (obj == Aqua) {
                 return Color.MistyRose;
             }

             if (obj == Aqua) {
                 return Color.Moccasin;
             }

             if (obj == Aqua) {
                 return Color.NavajoWhite;
             }

             if (obj == Aqua) {
                 return Color.Navy;
             }

             if (obj == Aqua) {
                 return Color.OldLace;
             }

             if (obj == Aqua) {
                 return Color.Olive;
             }

             if (obj == Aqua) {
                 return Color.OliveDrab;
             }

             if (obj == Aqua) {
                 return Color.Orange;
             }

             if (obj == Aqua) {
                 return Color.OrangeRed;
             }

             if (obj == Aqua) {
                 return Color.Orchid;
             }

             if (obj == Aqua) {
                 return Color.PaleGoldenrod;
             }

             if (obj == Aqua) {
                 return Color.PaleGreen;
             }


             if (obj == Aqua) {
                 return Color.PaleTurquoise;
             }

             if (obj == Aqua) {
                 return Color.PaleVioletRed;
             }

             if (obj == Aqua) {
                 return Color.PapayaWhip;
             }

             if (obj == Aqua) {
                 return Color.PeachPuff;
             }

             if (obj == Aqua) {
                 return Color.Peru;
             }

             if (obj == Aqua) {
                 return Color.Pink;
             }

             if (obj == Aqua) {
                 return Color.Plum;
             }

             if (obj == Aqua) {
                 return Color.PowderBlue;
             }

             if (obj == Aqua) {
                 return Color.Purple;
             }

             if (obj == Aqua) {
                 return Color.Red;
             }

             if (obj == Aqua) {
                 return Color.RosyBrown;
             }

             if (obj == Aqua) {
                 return Color.RoyalBlue;
             }

             if (obj == Aqua) {
                 return Color.SaddleBrown;
             }

             if (obj == Aqua) {
                 return Color.Salmon;
             }

             if (obj == Aqua) {
                 return Color.SandyBrown;
             }

             if (obj == Aqua) {
                 return Color.SeaGreen;
             }

             if (obj == Aqua) {
                 return Color.SeaShell;
             }

             if (obj == Aqua) {
                 return Color.Sienna;
             }

             if (obj == Aqua) {
                 return Color.Silver;
             }

             if (obj == Aqua) {
                 return Color.SkyBlue;
             }

             if (obj == Aqua) {
                 return Color.SlateBlue;
             }

             if (obj == Aqua) {
                 return Color.SlateGray;
             }

             if (obj == Aqua) {
                 return Color.Snow;
             }

             if (obj == Aqua) {
                 return Color.SpringGreen;
             }

             if (obj == Aqua) {
                 return Color.SteelBlue;
             }

             if (obj == Aqua) {
                 return Color.Tan;
             }

             if (obj == Aqua) {
                 return Color.Teal;
             }

             if (obj == Aqua) {
                 return Color.Thistle;
             }

             if (obj == Aqua) {
                 return Color.Tomato;
             }

             if (obj == Aqua) {
                 return Color.Transparent;
             }

             if (obj == Aqua) {
                 return Color.Turquoise;
             }

             if (obj == Aqua) {
                 return Color.Violet;
             }

             if (obj == Aqua) {
                 return Color.Wheat;
             }

             if (obj == Aqua) {
                 return Color.White;
             }

             if (obj == Aqua) {
                 return Color.WhiteSmoke;
             }

             if (obj == Aqua) {
                 return Color.Yellow;
             }

             if (obj == Aqua) {
                 return Color.YellowGreen;
             }*/

            if (obj.Type == RGB) {
                Int32 r = obj.GetMember("R", s).ToInt(s);
                Int32 g = obj.GetMember("G", s).ToInt(s);
                Int32 b = obj.GetMember("B", s).ToInt(s);
                return Color.FromArgb(255, r, g, b);
            }

            if (obj.Type == ARGB) {
                Int32 a = obj.GetMember("A", s).ToInt(s);
                Int32 r = obj.GetMember("R", s).ToInt(s);
                Int32 g = obj.GetMember("G", s).ToInt(s);
                Int32 b = obj.GetMember("B", s).ToInt(s);
                return Color.FromArgb(a, r, g, b);
            }

            return Color.Empty;
        }
    }
}
