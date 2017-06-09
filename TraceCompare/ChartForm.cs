using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace TraceCompare
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
            servoGraph.MouseWheel += servoGraph_MouseWheel;
        }
        int detent = 0;
        public bool IsZoomingOut = false;
        public int ZoomFactor = 1;
        private void servoGraph_MouseWheel(object sender, MouseEventArgs e)
        {
            int change = e.Delta - detent;
            if (change < 0)
                IsZoomingOut = true;
            else
            { IsZoomingOut = false; }

            if (IsZoomingOut)
            {
                servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - ZoomFactor;
            }
            else
            {
                servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum - ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum + ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum - ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum + ZoomFactor;
                if (servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum == servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum)
                {
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - ZoomFactor;
                }
            }
            
        }
        private void chartTracking_MouseEnter(object sender, EventArgs e)
        {
            this.servoGraph.Focus();
        }

        private void chartTracking_MouseLeave(object sender, EventArgs e)
        {
            this.servoGraph.Parent.Focus();
        }

        public ElementPosition ipp0 = null;
        public void makeSquare(Chart chart)
        {
            ChartArea ca = chart.ChartAreas[0];

            // store the original value:
            if (ipp0 == null) ipp0 = ca.InnerPlotPosition;

            // get the current chart area :
            ElementPosition cap = ca.Position;

            // get both area sizes in pixels:
            Size CaSize = new Size((int)(cap.Width * servoGraph.ClientSize.Width / 100f),
                                    (int)(cap.Height * servoGraph.ClientSize.Height / 100f));

            Size IppSize = new Size((int)(ipp0.Width * CaSize.Width / 100f),
                                    (int)(ipp0.Height * CaSize.Height / 100f));

            // we need to use the smaller side:
            int ippNewSide = Math.Min(IppSize.Width, IppSize.Height);

            // calculate the scaling factors
            float px = ipp0.Width / IppSize.Width * ippNewSide;
            float py = ipp0.Height / IppSize.Height * ippNewSide;

            // use one or the other:
            if (IppSize.Width < IppSize.Height)
                ca.InnerPlotPosition = new ElementPosition(ipp0.X, ipp0.Y, ipp0.Width, py);
            else
                ca.InnerPlotPosition = new ElementPosition(ipp0.X, ipp0.Y, px, ipp0.Height);

        }

        private void _Chart_Resize(object sender, EventArgs e)
        {
            makeSquare(servoGraph);

        }

        private void ChartCursorSelected(double x, double y)
        {
            //txtChartSelect.Text = x.ToString("F4") + ", " + y.ToString("F4");
        }
        private void ChartCursorMoved(double x, double y)
        {
            //txtChartValue.Text = x.ToString("F4") + ", " + y.ToString("F4");
        }

        private void servoGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                //adjust min max by this much
                End = e.Location;
                Point dif = new Point(Start.X - End.X, Start.Y - End.Y);
                double scale = 800.0;
                double Xfactor = dif.X / scale;
                double Yfactor = dif.Y / scale;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + Xfactor;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum + Xfactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum - Yfactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - Yfactor;

            }
        }
        public Point Start;
        public Point End;
        private void servoGraph_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                Start = e.Location;
        }

        private void servoGraph_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                End = e.Location;
        }
    }
}
