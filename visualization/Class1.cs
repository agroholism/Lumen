using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using vion;

public static class Main {
    public static Module PlotType { get; } = new PlotType();

    public static IObject Point { get; } = Prelude.CreateConstructor("Point", PlotType, new List<String>());
    public static IObject Area { get; } = Prelude.CreateConstructor("Area", PlotType, new List<String>());
    public static IObject Line { get; } = Prelude.CreateConstructor("Line", PlotType, new List<String>());

    public static void Import(Scope scope, String s) {
        Prelude.Instance.SetField("PlotType", PlotType);

        PlotType.SetField("Point", Point);
        PlotType.SetField("Area", Area);
        PlotType.SetField("Line", Line);

        scope.Set("plot", new LambdaFun((e, args) => {
            Form form = new Form { Text = "", ShowIcon = false };

            Chart chart = new Chart {
                Dock = DockStyle.Fill
            };

            chart.ChartAreas.Add(new ChartArea("[graphics]"));

            var type = e["t"];

            var chType = SeriesChartType.Line;

            if(type == Main.Point) {
                chType = SeriesChartType.Point;
            } else if(type == Main.Area) {
                chType = SeriesChartType.Area;
            } else if (type == Main.Line) {
                chType = SeriesChartType.Line;
            }

            Series mySeriesOfPoint = new Series("Sinus") {
                ChartType = chType,
                ChartArea = "[graphics]"
            };

            DataPoint last = null;

            chart.GetToolTipText += (sender, ex) => {
                if (ex.HitTestResult.Series != null) {
                    ex.Text = ex.HitTestResult.Series.Points[ex.HitTestResult.PointIndex].YValues[0]
                        + " \n "
                        + ex.HitTestResult.Series.Points[ex.HitTestResult.PointIndex].XValue;

                    if (last != null) {
                        last.MarkerStyle = MarkerStyle.None;
                    }

                    last = ex.HitTestResult.Series.Points[ex.HitTestResult.PointIndex];
                    last.MarkerStyle = MarkerStyle.Circle;

                }
            };

            /*chart.ChartAreas[0].AxisX.Maximum = 3.14;
            chart.ChartAreas[0].AxisX.Minimum = -3.14;
            chart.ChartAreas[0].AxisX.Interval = 0.5;*/

            chart.Palette = ChartColorPalette.BrightPastel;
            //mySeriesOfPoint.IsValueShownAsLabel = true;

            //mySeriesOfPoint.ToolTip = "X = #VALX, Y = #VALY";

            var dataX = e["x"].ToSequence(scope);
            var dataY = e["y"].ToSequence(scope);

            foreach (var i in dataX.Zip(dataY, (x, y) => new { x, y })) {
                mySeriesOfPoint.Points.AddXY(i.x.ToDouble(scope), i.y.ToDouble(scope));
            }

            //Добавляем созданный набор точек в Chart
            chart.Series.Add(mySeriesOfPoint);

            form.Controls.Add(chart);
            form.ShowDialog();

            return Const.UNIT;
        }) {
            Arguments = new List<IPattern> {
                new NamePattern("t"),
                new NamePattern("x"),
                new NamePattern("y"),
            }
        });

    }
}

namespace vion {
    public class PlotType : Module {
        public static Module Instance { get; } = new PlotType();



        public PlotType() {
            this.name = "PlotType";
        }
    }

    public class Vion : Module {
        public static Vion Instance { get; } = new Vion();


        private Vion() {
            
        }
    }
}
