using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using Lumen.Lang.Std;

public static class Main {
	public static void Import(Scope scope, String s) {
		StandartModule.__Kernel__.Set("vion", vion.Vion.Instance);
	}
}

namespace vion {
	public class Vion : Module {
		public static Vion Instance { get; } = new Vion();

		private Vion() {
			Set("plot", new LambdaFun((scope, args) => {
				Form form = new Form {
					Text = "",
					ShowIcon = false
				};

				Chart chart = new Chart {
					Dock = DockStyle.Fill
				};

				chart.ChartAreas.Add(new ChartArea("[graphics]"));

				var type = scope["class"].ToString(scope);

				var chType = SeriesChartType.Line;

				switch (type) {
					case "area":
						chType = SeriesChartType.Area;
						break;
					case "bar":
						chType = SeriesChartType.Bar;
						break;
					case "box_plot":
						chType = SeriesChartType.BoxPlot;
						break;
					case "bubble":
						chType = SeriesChartType.Bubble;
						break;
					case "candlestick":
						chType = SeriesChartType.Candlestick;
						break;
					case "column":
						chType = SeriesChartType.Column;
						break;
					case "doughnut":
						chType = SeriesChartType.Doughnut;
						break;
					case "errorBar":
						chType = SeriesChartType.ErrorBar;
						break;
					case "fast_line":
						chType = SeriesChartType.FastLine;
						break;
					case "fast_point":
						chType = SeriesChartType.FastPoint;
						break;
					case "funnel":
						chType = SeriesChartType.Funnel;
						break;
					case "kagi":
						chType = SeriesChartType.Kagi;
						break;
					case "line":
						chType = SeriesChartType.Line;
						break;
					case "pie":
						chType = SeriesChartType.Pie;
						break;
					case "point":
						chType = SeriesChartType.Point;
						break;
						// todo
				}

				Series mySeriesOfPoint = new Series("Sinus") {
					ChartType = chType,
					ChartArea = "[graphics]"
				};

				DataPoint last = null;

				chart.GetToolTipText += (sender, e) => {
					if (e.HitTestResult.Series != null) {
						e.Text = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex].YValues[0]
							+ " \n "
							+ e.HitTestResult.Series.Points[e.HitTestResult.PointIndex].XValue;

						if (last != null) {
							last.MarkerStyle = MarkerStyle.None;
						}

						last = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
						last.MarkerStyle = MarkerStyle.Circle;

					}
				};

				/*chart.ChartAreas[0].AxisX.Maximum = 3.14;
				chart.ChartAreas[0].AxisX.Minimum = -3.14;
				chart.ChartAreas[0].AxisX.Interval = 0.5;*/

				chart.Palette = ChartColorPalette.BrightPastel;
				//mySeriesOfPoint.IsValueShownAsLabel = true;

				//mySeriesOfPoint.ToolTip = "X = #VALX, Y = #VALY";

				var dataX = scope["x"].ToIterator(scope);
				var dataY = scope["y"].ToIterator(scope);

				foreach (var i in dataX.Zip(dataY, (x, y) => new { x, y })) {
					mySeriesOfPoint.Points.AddXY(i.x.ToDouble(scope), i.y.ToDouble(scope));
				}

				//Добавляем созданный набор точек в Chart
				chart.Series.Add(mySeriesOfPoint);

				form.Controls.Add(chart);
				form.ShowDialog();

				return Const.VOID;
			}) {
				Arguments = new List<FunctionArgument> {
					new FunctionArgument("x"),
					new FunctionArgument("y"),
					new FunctionArgument("class", Const.VOID),
				}
			});
		}
	}
}
