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
    public partial class Form1 : Form
    {
        public List<TraceSession> Sessions = new List<TraceSession>();
        public Form1()
        {
            InitializeComponent();
            servoGraph.MouseWheel += servoGraph_MouseWheel;
        }

        private void ImportSession_Click(object sender, EventArgs e)
        {
            TraceSession ts = new TraceSession();
            ts.Import();
            Sessions.Add(ts);
            TreeNode root = new TreeNode();
            root.Text = root.Name = "Trace: " + Sessions.Count.ToString();
            root.Tag = ts;
            int i = 0;
            foreach (var td in ts.TD.DataFrames)
            {
                TreeNode dfn = new TreeNode();
                dfn.Text = dfn.Name = "DataFrame: " + i.ToString();
                dfn.Tag = td;
                ++i;
                foreach(var ds in td.dataSignals)
                {
                    TreeNode dsn = new TreeNode();
                    dsn.Text = dsn.Name = ds.Description;
                    dsn.ToolTipText = ds.Name;
                    dsn.Tag = ds;
                    foreach(var s in ds.Signals)
                    {
                        TreeNode sub = new TreeNode();
                        sub.Text = sub.Name = s.Time;
                        sub.ToolTipText = s.Value.ToString();
                        sub.Tag = s;
                        dsn.Nodes.Add(sub);
                    }
                    dfn.Nodes.Add(dsn);
                }
                root.Nodes.Add(dfn);
            }
            SampleTree.Nodes.Add(root);
        }
        public bool IsXY;
        public bool IsXZ;
        public bool IsYZ;
        public double s1a1Min;
        public double s1a1Max;
        public double s1a2Min;
        public double s1a2Max;
        public double s2a1Min;
        public double s2a1Max;
        public double s2a2Min;
        public double s2a2Max;
        public void ChartIt()
        {

        }
        private void Show_click(object sender, EventArgs e)
        {
            try
            {
                servoGraph.Series["Series1"].Color = Color.FromArgb(120, 255, 0, 0);
                servoGraph.Series["Series2"].Color = Color.FromArgb(120, 0, 0, 255);
                makeSquare(servoGraph);
                if (s1a1.Nodes.Count < 1 || s1a2.Nodes.Count < 1 || s2a1.Nodes.Count < 1 || s2a2.Nodes.Count < 1)
                {
                    MessageBox.Show("Please Fill In Sample Data");
                    return;
                }
                if (s1a1.Nodes.Count > 1 || s1a2.Nodes.Count > 1 || s2a1.Nodes.Count > 1 || s2a2.Nodes.Count > 1)
                {
                    MessageBox.Show("Only (One) Item per Axis Sample is allowed");
                    return;
                }
                s1a1Points = GetPoints(s1a1.Tag as TraceSession.TraceData.DataFrame.DataSignal);
                s1a2Points = GetPoints(s1a2.Tag as TraceSession.TraceData.DataFrame.DataSignal);
                s2a1Points = GetPoints(s2a1.Tag as TraceSession.TraceData.DataFrame.DataSignal);
                s2a2Points = GetPoints(s2a2.Tag as TraceSession.TraceData.DataFrame.DataSignal);
                
                s1a1Min = s1a1Points.Min(sp => sp.YValues[0]);
                s1a1Max = s1a1Points.Max(sp => sp.YValues[0]);

                s1a2Min = s1a2Points.Min(sp => sp.YValues[0]);
                s1a2Max = s1a2Points.Max(sp => sp.YValues[0]);

                s2a1Min = s2a1Points.Min(sp => sp.YValues[0]);
                s2a1Max = s2a1Points.Max(sp => sp.YValues[0]);

                s2a2Min = s2a2Points.Min(sp => sp.YValues[0]);
                s2a2Max = s2a2Points.Max(sp => sp.YValues[0]);


               var a = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = s1a1Max+(s1a1Max*.03);
               var b =  servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = s1a1Min- (s1a1Min * .03);
                var c = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum;// = s1a2Max+10;
                var d = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum;// = s1a2Min-10;
                xmin.Text = b.ToString();
                xmax.Text = a.ToString();
                var change = a - b;

                c = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = s1a2Min+change-(change/2);
                d = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = s1a2Min- (change / 2);

                ymax.Text = c.ToString();
                ymin.Text = d.ToString();

                servoGraph.Series["Series1"].ChartType = SeriesChartType.Line;
                servoGraph.Series["Series2"].ChartType = SeriesChartType.Line;
                //servoGraph.Series["Series1"].YValuesPerPoint = 32;
                servoGraph.Series["Series1"].YValueType = ChartValueType.Double;
                servoGraph.Series["Series1"].XValueType = ChartValueType.Double;
               // servoGraph.Series["Series2"].YValuesPerPoint = 32;
                servoGraph.Series["Series2"].YValueType = ChartValueType.Double;
                servoGraph.Series["Series2"].XValueType = ChartValueType.Double;

                //make pairs and draw it out, check time stamps and move on if no change then keep moving
                



                //this does not work
                servoGraph.Series["Series1"].Points.Clear();
                servoGraph.Series["Series2"].Points.Clear();
                //pair up
                var path1 = Overloads.Pair(s1a1Points, s1a2Points);
                var path2 = Overloads.Pair(s2a1Points, s2a2Points,true);
               
                foreach (var p in path1)
                    servoGraph.Series["Series1"].Points.Add(p);
                foreach (var p in path2)
                   servoGraph.Series["Series2"].Points.Add(p);


                
               // servoGraph.ChartAreas[0].RecalculateAxesScale();
            }
            catch
            {

            }
        }
       

        public List<DataPoint> CreatePath(List<DataPoint> X, List<DataPoint> Y)
        {

            List<DataPoint> list = new List<DataPoint>();
            double x = X[0].YValues[0];
            double y = Y[0].YValues[0];
            list.Add(new DataPoint(Convert.ToDouble(x), Convert.ToDouble(y)));
            try
            {
                if (X.Count >= Y.Count) //if x is bigger
                {
                    int last = 0;
                    for (int i = 0; i < X.Count; ++i) //xcount is biggest
                    {
                        if (i < Y.Count) //have not run oyt of Y points yet
                        {

                            if (X[i].XValue < Y[i].XValue) //check times if x time is less than y time save x keep y...
                            {
                                x = X[i].YValues[0];
                            }
                            else
                            {
                                y = Y[i].YValues[0];
                            }
                            last = i;
                        }
                        else //if we ran out of Y (aka its holding position)
                        {

                            x = X[i].YValues[0];

                            //y = y; //use last value

                        }
                        //save x,y
                        list.Add(new DataPoint(Convert.ToDouble(x), Convert.ToDouble(y)));
                    }
                }
                else
                { //axis two greater count
                    for (int i = 0; i < Y.Count; ++i)
                    {
                        int last = 0;
                        if (i < X.Count)
                        {
                            if (X[i].XValue > Y[i].XValue) //check times
                            {
                                x = X[i].YValues[0];
                            }
                            else
                            {
                                y = Y[i].YValues[0];
                            }
                            last = i;
                        }
                        else //if we ran out of X (aka its holding position)
                        {

                            x = x;

                            y = Y[i].YValues[0];

                        }
                        //save x,y
                        list.Add(new DataPoint(Convert.ToDouble(x), Convert.ToDouble(y)));
                    }

                }
            }
            catch
            {

            }
            return list;
        }

        public List<DataPoint> CreatePathb(List<DataPoint> X, List<DataPoint> Y)
        {

            List<DataPoint> list = new List<DataPoint>();
            double x = X[0].YValues[0];
            double y = Y[0].YValues[0];
            list.Add(new DataPoint(Convert.ToDouble(x), Convert.ToDouble(y)));
            try
            {
                if (X.Count >= Y.Count)
                {
                    int last = 0;
                    for (int i = 0; i < X.Count; ++i)
                    {
                        if (i < Y.Count)
                        {
                            if (X[i].XValue > Y[i].XValue) //check times
                            {
                                x = X[i].YValues[0];
                            }
                            else
                            {
                                y = Y[i].YValues[0];
                            }
                            last = i;
                        }
                        else //if we ran out of Y (aka its holding position)
                        {
                            
                                x = X[i].YValues[0];
                           
                                y = y; //use last value
                            
                        }
                        //save x,y
                        list.Add(new DataPoint(Convert.ToDouble(x), Convert.ToDouble(y)));
                    }
                }
                else
                { //axis two greater count
                    for (int i = 0; i < Y.Count; ++i)
                    {
                        int last = 0;
                        if (i < X.Count)
                        {
                            if (X[i].XValue > Y[i].XValue) //check times
                            {
                                x = X[i].YValues[0];
                            }
                            else
                            {
                                y = Y[i].YValues[0];
                            }
                            last = i;
                        }
                        else //if we ran out of X (aka its holding position)
                        {
                            
                                x = x;
                           
                                y = Y[i].YValues[0];
                          
                        }
                        //save x,y
                        list.Add(new DataPoint(Convert.ToDouble(x), Convert.ToDouble(y)));
                    }

                }
            }
            catch
            {

            }
            return list;
        }
        public List<DataPoint> GetPoints(TraceSession.TraceData.DataFrame.DataSignal dataSignal)
        {
            List<DataPoint> l = new List<DataPoint>();
            foreach (var sig in dataSignal.Signals)
            {
                DataPoint dp = new DataPoint();

                dp.XValue = Convert.ToDouble(sig.Time);
                dp.YValues[0] = sig.Value;
                l.Add(dp);
            }
            return l;
        }

        public List<DataPoint> s1a1Points = new List<DataPoint>();
        public List<DataPoint> s1a2Points = new List<DataPoint>();
        public List<DataPoint> s2a1Points = new List<DataPoint>();
        public List<DataPoint> s2a2Points = new List<DataPoint>();
        public List<TreeNode> SelectedNodes = new List<TreeNode>();
        private void SampleTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            
            if (e.Node.Checked)
            {
                try { SelectedNodes.Add(e.Node); }
                catch { }
            }
            else
            {
                try { SelectedNodes.Remove(e.Node); }
                catch { }
            }
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void Drag_Enter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        public TreeNode GetDragNode(DragEventArgs e)
        {
            TreeNode node;
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                node = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                if (node.Tag is TraceSession.TraceData.DataFrame.DataSignal)
                {var n =node.Clone() as TreeNode;
                    n.Tag = node.Tag;
                    return n;
                }
                else
                {
                    MessageBox.Show("Sample is not a DataSignal Group");
                }
            }
            return null;
        }
        private void s1a1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var node = GetDragNode(e);
                if (node.Tag is TraceSession.TraceData.DataFrame.DataSignal)
                {
                    s1a1.Nodes.Add(node);
                    s1a1.Tag = node.Tag;
                }
            }
            catch
            {
                MessageBox.Show("Not a DataSignal Group");
            }       
        }

        private void s1a2_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var node = GetDragNode(e);
                if (node.Tag is TraceSession.TraceData.DataFrame.DataSignal)
                {
                    s1a2.Nodes.Add(node);
                    s1a2.Tag = node.Tag;
                }
            }
            catch
            {
                MessageBox.Show("Not a DataSignal Group");
            }
        }

        private void s2a1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var node = GetDragNode(e);
                if (node.Tag is TraceSession.TraceData.DataFrame.DataSignal)
                {
                    s2a1.Nodes.Add(node);
                    s2a1.Tag = node.Tag;
                }
            }
            catch
            {
                MessageBox.Show("Not a DataSignal Group");
            }
        }

        private void s2a2_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var node = GetDragNode(e);
                if (node.Tag is TraceSession.TraceData.DataFrame.DataSignal)
                {
                    s2a2.Nodes.Add(node);
                    s2a2.Tag = node.Tag;
                }
            }
            catch
            {
                MessageBox.Show("Not a DataSignal Group");
            }
        }

        private void SampleTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.DoDragDrop(e.Item, DragDropEffects.Copy);
        }
        public List<TreeNode> TreeSearch(List<TreeNode> TreeNodes, string contain)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode node in TreeNodes)
            {
                if (node.Text.ToUpper().Contains(contain.ToUpper()) || node.Name.ToUpper().Contains(contain.ToUpper()))
                {
                    nodes.Add(node);
                }
                if (node.Nodes.Count > 0)
                {
                    List<TreeNode> subs = new List<TreeNode>();
                    foreach (TreeNode sub in node.Nodes)
                        subs.Add(sub);
                    nodes.AddRange(TreeSearch(subs, contain));
                }
            }
            return nodes;
        }
        int detent = 0;
        public bool IsZoomingOut = false;
        public double ZoomFactor = 50;
        public double InfinityFactor = 5.999;
        public double m_InfinityFactor = 5.999;
        private void servoGraph_MouseWheel(object sender, MouseEventArgs e)
        {
           
            int change = e.Delta - detent;
            if (change < 0)
                IsZoomingOut = true;
            else
            { IsZoomingOut = false; }

            if (IsZoomingOut)
            {
                if (servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum - servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum < ZoomFactor && intoInfinity)
                {
                    m_InfinityFactor = ZoomFactor;
                    ZoomFactor = InfinityFactor;
                }
                else
                {
                    ZoomFactor = m_InfinityFactor;
                    intoInfinity = false;
                }
                servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - ZoomFactor;
            }
            else
            {
                if (intoInfinity == true)
                {
                    //devide by 2
                    var IF = InfinityFactor;
                    var a =servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum - InfinityFactor;
                    var b =servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum + InfinityFactor;// - (ZoomFactor / InfinityFactor);
                    var c =servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum - InfinityFactor;// + (ZoomFactor / InfinityFactor);
                   var d = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum + InfinityFactor;// - (ZoomFactor / InfinityFactor);
                    if (servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum == servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum ||
                            servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum < servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum)
                    {
                        //oops gone too small back up 1 level

                        
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + InfinityFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - InfinityFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + InfinityFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - InfinityFactor;
                        var xDif = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum - servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum;
                        var yDif = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum - servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum;
                        intoInfinity = true;
                        InfinityFactor = 0.998;// xDif *0.99;
                        //change infinityfactor
                    }
                }
                else
                {
                    //zoom in
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum - ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum + ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum - ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum + ZoomFactor;

                    //catch a min max problem and start zooming nito infinity ...devide by 2
                    if (servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum == servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum ||
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum < servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum)
                    {
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + ZoomFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - ZoomFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + ZoomFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - ZoomFactor;
                        intoInfinity = true;
                    }
                }
            }
            updateChartInfo(servoGraph);
        }
        public bool intoInfinity = false;
        private void updateChartInfo(Chart chart)
        {
            xmin.Text = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum.ToString();
            xmax.Text = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum.ToString();
           
            ymax.Text = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum.ToString();
            ymin.Text = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum.ToString();
        }
        private void zoomIn()
        {
           
            
             IsZoomingOut = false; 

            
                if (intoInfinity == true)
                {
                //devide by 2
                servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum - InfinityFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum + InfinityFactor;// - (ZoomFactor / InfinityFactor);
                servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum - InfinityFactor;// + (ZoomFactor / InfinityFactor);
                servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum + InfinityFactor;// - (ZoomFactor / InfinityFactor);
                if (servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum == servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum ||
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum < servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum)
                {
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + InfinityFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - InfinityFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + InfinityFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - InfinityFactor;
                    intoInfinity = true;
                }
            }
                else
                {
                //zoom in
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum - ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum + ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum - ZoomFactor;
                    servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum + ZoomFactor;

                    //catch a min max problem and start zooming nito infinity ...devide by 2
                    if (servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum == servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum ||
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum < servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum)
                    {
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + ZoomFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - ZoomFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + ZoomFactor;
                        servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - ZoomFactor;
                        intoInfinity = true;
                    }
                }
            updateChartInfo(servoGraph);
        }
        private void zoomOut()
        {
           

            IsZoomingOut = true;

            if (IsZoomingOut)
            {
                intoInfinity = false;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum + ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum - ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum + ZoomFactor;
                servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum - ZoomFactor;
            }
            updateChartInfo(servoGraph);
        }



        private void chartTracking_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Cross;
            this.servoGraph.Focus();
        }

        private void chartTracking_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
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
            this.Cursor = Cursors.Cross;
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right)
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
        public int Click_selectedPointIndex;
        public Series Click_selectedSeries;
        public DataPoint Click_selectedPoint;
        public int Start_selectedPointIndex;
        public Series Start_selectedSeries;
        public DataPoint Start_selectedPoint;
        public int End_selectedPointIndex;
        public Series End_selectedSeries;
        public DataPoint End_selectedPoint;
        public int LineCount = 0;
        private void servoGraph_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (DistanceBtn.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    double dx = servoGraph.ChartAreas["ChartArea1"].AxisX.PixelPositionToValue(e.X);
                    double dy = servoGraph.ChartAreas["ChartArea1"].AxisY.PixelPositionToValue(e.Y);
                    Start_selectedPoint = new DataPoint(dx, dy);
                }
            }
            
        
       
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right)
                Start = e.Location;
        }

        private void servoGraph_MouseUp(object sender, MouseEventArgs e)
        {
            
            if (DistanceBtn.Checked)
                {

                if (e.Button == MouseButtons.Left)
                {
                    try
                    {
                        double dx = servoGraph.ChartAreas["ChartArea1"].AxisX.PixelPositionToValue(e.X);
                        double dy = servoGraph.ChartAreas["ChartArea1"].AxisY.PixelPositionToValue(e.Y);
                        End_selectedPoint = new DataPoint(dx, dy);

                        ++LineCount;
                        servoGraph.Series.Add("Line" + LineCount.ToString());
                        servoGraph.Series["Line" + LineCount.ToString()].Points.AddXY(Start_selectedPoint.XValue, Start_selectedPoint.YValues[0]);
                        servoGraph.Series["Line" + LineCount.ToString()].Points.AddXY(End_selectedPoint.XValue, End_selectedPoint.YValues[0]);
                        //calculate distance
                        var side1 = Start_selectedPoint.XValue - End_selectedPoint.XValue;
                        if (side1 < 0)
                        {
                            side1 = side1 * (-1);
                        }
                        var side2 = Start_selectedPoint.YValues[0] - End_selectedPoint.YValues[0];
                        if (side2 < 0)
                        {
                            side2 = side2 * (-1);
                        }
                        var distance = Math.Sqrt(side1 * side1 + side2 * side2);
                        servoGraph.Series["Line" + LineCount.ToString()].Points[1].Label = distance.ToString();
                        servoGraph.Series["Line" + LineCount.ToString()].Points[1].LabelForeColor = Color.Lime;
                        servoGraph.Series["Line" + LineCount.ToString()].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                        servoGraph.Series["Line" + LineCount.ToString()].Color = Color.FromArgb(200, 255, 0, 0);
                    }
                    catch
                    {
                        MessageBox.Show("Error Distance Drawing");
                    }
                }
                }
            
            if (e.Button == MouseButtons.Left)
            {
                var result = servoGraph.HitTest(e.X, e.Y);
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    //End_selectedPointIndex = result.PointIndex;

                   // End_selectedSeries = result.Series;

                   // End_selectedPoint = result.Series.Points[End_selectedPointIndex];
                }
                if (DistanceBtn.Checked && Start_selectedPoint!=null && End_selectedPoint!=null)
                {
                    /*
                    ++LineCount;
                    servoGraph.Series.Add("Line" + LineCount.ToString());
                    servoGraph.Series["Line" + LineCount.ToString()].Points.AddXY(Start_selectedPoint.XValue, Start_selectedPoint.YValues[0]);                   
                    servoGraph.Series["Line" + LineCount.ToString()].Points.AddXY(End_selectedPoint.XValue, End_selectedPoint.YValues[0]);
                    //calculate distance
                    var side1 = Start_selectedPoint.XValue - End_selectedPoint.XValue;
                    if (side1<0)
                    {
                        side1 = side1 * (-1);
                    }
                    var side2 = Start_selectedPoint.YValues[0] - End_selectedPoint.YValues[0];
                    if (side2 < 0)
                    {
                        side2 = side2 * (-1);
                    }
                    var distance = Math.Sqrt(side1 * side1 + side2 * side2);
                    servoGraph.Series["Line" + LineCount.ToString()].Points[1].Label = distance.ToString();
                    servoGraph.Series["Line" + LineCount.ToString()].Points[1].LabelForeColor = Color.Lime;
                    servoGraph.Series["Line" + LineCount.ToString()].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    servoGraph.Series["Line" + LineCount.ToString()].Color = Color.FromArgb(200, 194, 215, 54);
                    */
                }
                
            }
            Start_selectedPoint = End_selectedPoint = null;
            // if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right)
            //    End = e.Location;
        }
        public enum axis
        {
            X,Y,Z,A,B,C
        }
        public enum valuetype
        {
            SETPOINT,ACTUAL
        }
        public struct auto
        {
            public axis Axis;
            public valuetype Type;
            public TraceSession.TraceData.DataFrame.DataSignal Data;
            public TreeNode Node;
        }
        private void Auto_click(object sender, EventArgs e)
        {
            try
            {
                var SetPointNodes = TreeSearch(SampleTree.Nodes.ToList(), "setpoint");
                var ActualPointNodes = TreeSearch(SampleTree.Nodes.ToList(), "position actual value meas.system");
                List<auto> Autos = new List<auto>();
                foreach(var node in SetPointNodes)
                {
                    auto a = new auto();
                    a.Data = node.Tag as TraceSession.TraceData.DataFrame.DataSignal;
                    a.Node = node.Clone() as TreeNode;
                    a.Node.Tag = node.Tag;
                    if (node.Contains("[Y"))
                        a.Axis = axis.Y;
                    if (node.Contains("[X"))
                        a.Axis = axis.X;
                    if (node.Contains("[Z"))
                        a.Axis = axis.Z;
                    if (node.Contains("[A"))
                        a.Axis = axis.A;
                    if (node.Contains("[B"))
                        a.Axis = axis.B;
                    if (node.Contains("[C"))
                        a.Axis = axis.C;
                    a.Type = valuetype.SETPOINT;
                    Autos.Add(a);
                }
                foreach (var node in ActualPointNodes)
                {
                    auto a = new auto();
                    a.Data = node.Tag as TraceSession.TraceData.DataFrame.DataSignal;
                    a.Node = node.Clone() as TreeNode;
                    a.Node.Tag = node.Tag;
                    if (node.Contains("[Y"))
                        a.Axis = axis.Y;
                    if (node.Contains("[X"))
                        a.Axis = axis.X;
                    if (node.Contains("[Z"))
                        a.Axis = axis.Z;
                    if (node.Contains("[A"))
                        a.Axis = axis.A;
                    if (node.Contains("[B"))
                        a.Axis = axis.B;
                    if (node.Contains("[C"))
                        a.Axis = axis.C;
                    a.Type = valuetype.ACTUAL;
                    Autos.Add(a);
                }
                var axis1 = Autos.Split(Autos[0].Axis); //now have split axis
                var axis2 = Autos.RemoveMulti(axis1); // inverse of axis1

                

                s1a1.Nodes.Clear();
                s1a1.Nodes.Add(axis1[0].Node);
                s1a1.Tag = axis1[0].Data;
                s1a2.Nodes.Clear();
                s1a2.Nodes.Add(axis2[0].Node);
                s1a2.Tag = axis2[0].Data;
                s2a1.Nodes.Clear();
                s2a1.Nodes.Add(axis1[1].Node);
                s2a1.Tag = axis1[1].Data;
                s2a2.Nodes.Clear();
                s2a2.Nodes.Add(axis2[1].Node);
                s2a2.Tag = axis2[1].Data;
                Show_click(null, null);

            }
            catch {

                MessageBox.Show("No Data To Display");
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ZoomFactor = trackBar1.Value;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SampleTree.Nodes.Clear();
            s1a1.Nodes.Clear();
            s1a2.Nodes.Clear();
            s2a1.Nodes.Clear();
            s2a2.Nodes.Clear();
            Sessions = new List<TraceSession>();
            servoGraph.Series.Clear();
            servoGraph.Series.Add("Series1");
            servoGraph.Series.Add("Series2");
            servoGraph.Series.Add("Ydif");
            servoGraph.Series.Add("Xdif");
            LineCount = 0;
        }

        private void Zoom_In_Click(object sender, EventArgs e)
        {
            zoomIn();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            zoomOut();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            var a = servoGraph.ChartAreas["ChartArea1"].AxisX.Maximum = s1a1Max;
            var b = servoGraph.ChartAreas["ChartArea1"].AxisX.Minimum = s1a1Min;
            var c = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum;// = s1a2Max+10;
            var d = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum;// = s1a2Min-10;
            xmin.Text = b.ToString();
            xmax.Text = a.ToString();
            var change = a - b;

            c = servoGraph.ChartAreas["ChartArea1"].AxisY.Maximum = s1a2Min + change - (change / 2);
            d = servoGraph.ChartAreas["ChartArea1"].AxisY.Minimum = s1a2Min - (change / 2);
        }

        private void InfiniteChooser_ValueChanged(object sender, EventArgs e)
        {
            InfinityFactor = Convert.ToDouble(InfiniteChooser.Value);
        }

        private void Clear_Chart_Click(object sender, EventArgs e)
        {
            servoGraph.Series.Clear();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            //var s = servoGraph.Series["Series1"];
            //double I = (servoGraph.ChartAreas[0].AxisX.Minimum /2)+ (servoGraph.ChartAreas[0].AxisX.Maximum / 2);
            //var a =s.FindValueOfXcursor(I);		max	0.030105249741495754	double
            try
            {
                var ydif = Overloads.CompareSeriesForVerticalDifferential(servoGraph.Series["Series1"], servoGraph.Series["Series2"], 1, 1, -0.9, 0.9);
                //check scale 
                var min = s1a2Min;
                var max = s1a2Max;
                var height = (max - min);
                var a = ydif[0].YValues[0];
                ydif.ThresholdY();
                var ymin = ydif.MinY();
                var ymax = ydif.MaxY();
                double high;
                if (Math.Abs(ymin) >= Math.Abs(ymax))
                    high = ymin;
                else
                    high = ymax;

                var factor = 1 / Math.Abs(high);

                var offset = height * 2;

                //ydif.ThresholdY();
                ydif.ScaleY(factor * 50);
                var shift = high * 2 + min - offset;
                ydif.ShiftY(shift);

                //check shift

                servoGraph.Series["Ydif"].Points.Clear();
                foreach (var dp in ydif)
                    servoGraph.Series["Ydif"].Points.Add(dp);
            }catch (Exception ex)
            {
                MessageBox.Show("Could Not Compare\r\n\r\n"+ex.ToString());
            }
            //var min = result.Min();
            //var max = result.Max();
            //var maxError = max - min;
        }

        private void servoGraph_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //center
                double dx = servoGraph.ChartAreas["ChartArea1"].AxisX.PixelPositionToValue(e.X);
                double dy = servoGraph.ChartAreas["ChartArea1"].AxisY.PixelPositionToValue(e.Y);
                DataPoint result = new DataPoint(dx, dy);

                servoGraph.ChartAreas["ChartArea1"].CenterDataPoint(result);
                //zoomin
                zoomIn();
            }
            if (e.Button == MouseButtons.Right)
            {
                //center
                double dx = servoGraph.ChartAreas["ChartArea1"].AxisX.PixelPositionToValue(e.X);
                double dy = servoGraph.ChartAreas["ChartArea1"].AxisY.PixelPositionToValue(e.Y);
                DataPoint result = new DataPoint(dx, dy);

                servoGraph.ChartAreas["ChartArea1"].CenterDataPoint(result);
                //zoomout
                zoomOut();
            }
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new License().ShowDialog();
        }
    }
  

}
