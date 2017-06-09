namespace TraceCompare
{
    partial class ChartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartForm));
            this.servoGraph = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.servoGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // servoGraph
            // 
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisX.LineWidth = 5;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.MajorTickMark.LineColor = System.Drawing.Color.Lime;
            chartArea1.AxisX.MajorTickMark.LineWidth = 2;
            chartArea1.AxisX.Maximum = 10D;
            chartArea1.AxisX.MaximumAutoSize = 100F;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.MinorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisX.ScaleBreakStyle.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.ScrollBar.BackColor = System.Drawing.Color.Gray;
            chartArea1.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.ScrollBar.LineColor = System.Drawing.Color.Red;
            chartArea1.AxisY.Interval = 1D;
            chartArea1.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.Maximum = 10D;
            chartArea1.AxisY.MaximumAutoSize = 100F;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.AxisY.MinorGrid.Enabled = true;
            chartArea1.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.ScrollBar.BackColor = System.Drawing.Color.Gray;
            chartArea1.AxisY.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.ScrollBar.LineColor = System.Drawing.Color.Red;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.Name = "ChartArea1";
            this.servoGraph.ChartAreas.Add(chartArea1);
            this.servoGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.servoGraph.Location = new System.Drawing.Point(0, 0);
            this.servoGraph.Name = "servoGraph";
            this.servoGraph.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.MarkerSize = 10;
            series1.Name = "Series1";
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Name = "Series2";
            this.servoGraph.Series.Add(series1);
            this.servoGraph.Series.Add(series2);
            this.servoGraph.Size = new System.Drawing.Size(784, 750);
            this.servoGraph.TabIndex = 21;
            this.servoGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.servoGraph_MouseDown);
            this.servoGraph.MouseEnter += new System.EventHandler(this.chartTracking_MouseEnter);
            this.servoGraph.MouseLeave += new System.EventHandler(this.chartTracking_MouseLeave);
            this.servoGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.servoGraph_MouseMove);
            this.servoGraph.MouseUp += new System.Windows.Forms.MouseEventHandler(this.servoGraph_MouseUp);
            this.servoGraph.Resize += new System.EventHandler(this._Chart_Resize);
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 750);
            this.Controls.Add(this.servoGraph);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chart";
            ((System.ComponentModel.ISupportInitialize)(this.servoGraph)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart servoGraph;
    }
}