using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace TraceCompare
{
    public class TraceSession
    {
        public void Import()
        {
            TraceSession ts = this;
            XmlDocument doc = new XmlDocument();
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                doc.Load(ofd.FileName);
                //PULL DATA FRAME
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/traceSession");

                foreach (XmlNode node in nodes)
                {
                    //parse out the version
                    ts.Version = node.Attributes[0].InnerText;
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        //parse out the kinds
                        if (child.Name == "traceCaptureSetup")
                        {

                        }
                        if (child.Name == "traceDisplaySetup")
                        {

                        }
                        if (child.Name == "traceData")
                        {
                            foreach (XmlNode attr in child.Attributes)
                            {
                                //values
                                if (attr.Name == "dataFrames")
                                {
                                    ts.TD.dataFrames = attr.Value;
                                }
                            }
                            foreach (XmlNode dataFrame in child.ChildNodes)
                            {

                                TraceSession.TraceData.DataFrame df = new TraceSession.TraceData.DataFrame();
                                foreach (XmlNode data in dataFrame.ChildNodes)
                                {
                                    if (data.Name == "frameHeader")
                                    {
                                        df.startTime = data.Attributes.GetNamedItem("startTime").Value;
                                        df.startTriggerInc = Convert.ToSingle(data.Attributes.GetNamedItem("startTriggerInc").Value);
                                        df.stopTriggerInc = Convert.ToSingle(data.Attributes.GetNamedItem("stopTriggerInc").Value);
                                        df.stopInc = Convert.ToSingle(data.Attributes.GetNamedItem("stopInc").Value);
                                    }
                                    if (data.Name == "domain")
                                    {
                                        df.Domain = data.Attributes.GetNamedItem("e1").Value;
                                    }
                                    if (data.Name == "dataSignal")
                                    {
                                        TraceSession.TraceData.DataFrame.DataSignal ds = new TraceSession.TraceData.DataFrame.DataSignal();
                                        ds.Id = data.Attributes.GetNamedItem("id")?.Value;
                                        ds.Key = data.Attributes.GetNamedItem("key")?.Value;
                                        ds.Event = data.Attributes.GetNamedItem("event")?.Value;
                                        ds.EventChannel = data.Attributes.GetNamedItem("eventChannel")?.Value;
                                        ds.Description = data.Attributes.GetNamedItem("description")?.Value;
                                        ds.Domain = data.Attributes.GetNamedItem("domain")?.Value;
                                        ds.Name = data.Attributes.GetNamedItem("name")?.Value;
                                        ds.DataType = data.Attributes.GetNamedItem("dataType")?.Value;
                                        ds.Interval = data.Attributes.GetNamedItem("interval")?.Value;
                                        ds.UnitsType = data.Attributes.GetNamedItem("unitsType")?.Value;
                                        ds.DisplayRes = data.Attributes.GetNamedItem("displayRes")?.Value;
                                        ds.DataPointCount = data.Attributes.GetNamedItem("datapointCount")?.Value;
                                        df.dataSignals.Add(ds);
                                    }
                                    if (data.Name == "rec") //actual signals that need t obe parsed baised on "dataSignal"
                                    {
                                        foreach (XmlNode attr in data.Attributes)
                                        {
                                            foreach (var ds in df.dataSignals)
                                            {
                                                if (ds.Id == attr.Name)
                                                {
                                                    TraceSession.TraceData.DataFrame.DataSignal.Signal signal = new TraceSession.TraceData.DataFrame.DataSignal.Signal();
                                                    signal.Time = data.Attributes[0].Value.ToString();
                                                    try
                                                    {
                                                        signal.Value = Convert.ToSingle(attr.Value);
                                                        ds.Signals.Add(signal);
                                                    }
                                                    catch { }
                                                }
                                            }
                                        }
                                    }
                                }
                                ts.TD.DataFrames.Add(df);
                            }
                        }

                    }
                    // var a = node.SelectSingleNode("author").InnerText; //select single item
                }
            }
        }
       
        public TraceCaptureSetup TCS = new TraceCaptureSetup();
        public TraceDisplaySetup TDS = new TraceDisplaySetup();
        public TraceData TD = new TraceData();
        public string Version = "";
        public class TraceCaptureSetup
        {
            public class CollectionSettings
            {
                public string AppendData;
                public string Endless;
                public string AtCapacity;
                public string AutoRestart;
                public string Capacity;
                public string PowerOnStart;
                public string TimeLimit;
                public string DeferOffload;
            }
            public class SessionSettings
            {
                public string SessionName;
                public string ArchiveSetting;
                public string ClientPrivilegeRead;
                public string ClientPrivilegeManage;
                public string ExpirationPeriod;
            }
            public class SampleRateSettings
            {
                public string Ipo1SampleRate;
                public string Ipo2SampleRate;
                public string ServoSampleRate;
            }
            public class SignalSettings
            {
                List<TraceDisplaySetup.Signal> Signals = new List<TraceDisplaySetup.Signal>();
            }
            public class TriggerSettings
            {

            }
        }
        public class TraceDisplaySetup
        {
            public List<Signal> Signals = new List<Signal>();
            public class Signal
            {
                public string Key;
                public string Domain;
                public string Event;
                public string EventChannel;
                public string Name;
                public string Description;
                public string DataType;
                public string UnitsType;
                public string WaveFormKey;
                public string Color;
                public string LineStyle;
                public string PlotYOption;
                public string SynchScale;
                public string DisplayRes;
                public string Bitmask;
                public string IsReference;
                public string CompositeDef;
                public string AxisDisplay;
            }
            public class GraphWindow
            {
                //FILL IN
            }
        }
        public class TraceData
        {
            public string dataFrames = "";
            public int iDataFrame
            {
                get { return Convert.ToInt32(dataFrames); }
                set { dataFrames = value.ToString(); }
            }
            public List<DataFrame> DataFrames = new List<DataFrame>();
            public class DataFrame
            {
                public string startTime;
                public float stopInc;
                public float startTriggerInc;
                public float stopTriggerInc;
                public string Domain;
                public List<DataSignal> dataSignals = new List<DataSignal>();
                public class DataSignal
                {
                    public string Id;
                    public string Key;
                    public string Event;
                    public string EventChannel;
                    public string Description;
                    public string Domain;
                    public string Name;
                    public string DataType;
                    public string Interval;
                    public string UnitsType;
                    public string DisplayRes;
                    private string m_datapointcount = "";
                    private int m_count;
                    public int Count
                    {
                        get { return m_count; }
                        set
                        {
                            m_count = value;
                            m_datapointcount = value.ToString();
                        }
                    }
                    public string DataPointCount
                    {
                        get { return m_datapointcount; }
                        set
                        {
                            m_datapointcount = value;
                            m_count = Convert.ToInt32(value);
                        }
                    }
                    public List<Signal> Signals = new List<Signal>();
                    public class Signal
                    {
                        public string Time;
                        public float Value;
                    }
                }

            }

        }
    }
    public static class Overloads
    {
        public static List<System.Windows.Forms.TreeNode> ToList(this System.Windows.Forms.TreeNodeCollection Nodes)
        {
            List<TreeNode> subs = new List<TreeNode>();
            foreach (TreeNode sub in Nodes)
                subs.Add(sub);
            return subs;
        }
        public static bool Contains(this TreeNode node, string key)
        {
            if (node.Text.ToUpper().Contains(key.ToUpper()))
                return true;
            else
                return false;
                    
        }
        public static List<Form1.auto> Split(this List<Form1.auto> autos, Form1.axis axis)
        {
            List<Form1.auto> ret = new List<Form1.auto>();
            foreach(var a in autos)
            {
                if (a.Axis == axis)
                {
                    ret.Add(a);
                }
            }
            return ret;
        }
        public static List<Form1.auto> RemoveMulti(this List<Form1.auto> autos, List<Form1.auto> rems)
        {
            foreach (var rem in rems)
            { autos.Remove(rem); }
            return autos;
        }
        private static bool Ax1Normal = true;
        public static List<DataPoint> Pair(List<DataPoint> axis1, List<DataPoint> axis2, bool IsPath2=false)
        {
            double TOL = 0.005;
            List<Form1.auto> autos = new List<Form1.auto>();
            List<DataPoint> Ax1;
            List<DataPoint> Ax2;
            if (IsPath2)
            {
                if (Ax1Normal)
                {
                    Ax1 = axis1;
                    Ax2 = axis2;
                }
                else
                {
                    Ax1 = axis2;
                    Ax2 = axis1;
                }
            }
            else
            {
                if (axis1.Count > axis2.Count)
                {
                    Ax1 = axis1;
                    Ax2 = axis2;
                    Ax1Normal = true;
                }
                else
                {
                    Ax1 = axis2;
                    Ax2 = axis1;
                    Ax1Normal = false;
                }
            }
            List<DataPoint> ret = new List<DataPoint>();
            foreach (var a1 in Ax1)
            {
                DataPoint dp = new DataPoint();
                foreach(var a2 in Ax2)
                {
                    if (a1.XValue == a2.XValue)
                    {
                        dp.XValue = a1.YValues[0];
                        dp.YValues[0] = a2.YValues[0];
                        ret.Add(dp);
                    }
                }
                
            }


            return ret;
        }
        //Chart Range Manipulation 
        public static void CenterDataPoint(this ChartArea graph, DataPoint dp)
        {            
            double x = 0;
            double y = 0;
            //Find Center
            x = (graph.AxisX.Maximum - graph.AxisX.Minimum) / 2; //center of X
            x += graph.AxisX.Minimum;
            y = (graph.AxisY.Maximum - graph.AxisY.Minimum) / 2; //center of Y
            y += graph.AxisY.Minimum;
            //Create Shift
            var yShift = (dp.YValues[0] - y);
            var xShift = (dp.XValue - x);
            //Shift
            graph.AxisX.Maximum = graph.AxisX.Maximum + xShift;
            graph.AxisX.Minimum = graph.AxisX.Minimum +xShift;

            graph.AxisY.Maximum = graph.AxisY.Maximum + yShift;
            graph.AxisY.Minimum = graph.AxisY.Minimum +yShift;
        }
        //Data Point Manipulation
        public static void ScaleY(this List<DataPoint> points,double scaleFactor)
        {
            foreach(var dp in points)
            {
                dp.YValues[0] = dp.YValues[0] * scaleFactor;
            }
        }
        public static void ScaleX(this List<DataPoint> points, double scaleFactor)
        {
            foreach (var dp in points)
            {
                dp.XValue = dp.XValue * scaleFactor;
            }
        }
        public static void ShiftY(this List<DataPoint> points, double shiftFactor)
        {
            foreach (var dp in points)
            {
                dp.YValues[0] = dp.YValues[0] + shiftFactor;
            }
        }
        public static void ShiftX(this List<DataPoint> points, double shiftFactor)
        {
            foreach (var dp in points)
            {
                dp.XValue = dp.XValue + shiftFactor;
            }
        }
        public static double AverageY(this List<DataPoint> points)
        {
            List<double> list = new List<double>();
            foreach(var dp in points)
            {
                list.Add(dp.YValues[0]);
            }
            return list.Average();
        }
        public static double MaxY(this List<DataPoint> points)
        {
            List<double> list = new List<double>();
            foreach (var dp in points)
            {
                list.Add(dp.YValues[0]);
            }
            return list.Max();
        }
        public static double MinY(this List<DataPoint> points)
        {
            List<double> list = new List<double>();
            foreach (var dp in points)
            {
                list.Add(dp.YValues[0]);
            }
            var a = list.Min();
            return list.Min();
        }
        public static void ThresholdY(this List<DataPoint> points, double lowThreshold = -0.09, double highThreshold = 0.09)
        {
         
            foreach (var dp in points)
            {
                if (dp.YValues[0]< lowThreshold || dp.YValues[0] >highThreshold)
                {
                    dp.YValues[0] = 0.0;
                }
            }
           
        }

        //Trig Intersects
        public static double sub_Diference_intersect(this Series series, double xCursorPos, DataPoint p2, DataPoint p1)
        {
            //LETS DO TRIG!!!!!!

            //side 1

            var AM = p2.XValue - p1.XValue;
            //side 2
            var BM = p2.YValues[0] - p1.YValues[0];
            if (AM < 0) AM = AM * (-1);
            if (BM < 0) BM = BM * (-1);

            //hypotenuse of s1 and s2
            var CM = Math.Sqrt(AM * AM + BM * BM);

            //set cursor value
            var XC = xCursorPos;

            var A1 = XC - p1.XValue;
            double B1;
            var angle = Math.Atan2(AM, BM) * 180 / Math.PI;
            angle = 180 - 90 - angle;
            B1 = A1 * Math.Tan(angle * Math.PI / 180);

            //check for negative value

            return B1;

        }        
        //Main Intersect Funtions
        public static double XCursorIntersectValue(this Series series, double xCursorPos)
        {
            var line = FindNearestNeighborsXValue(series,xCursorPos);
            if (line.dp2 == null && line.dp1 != null) // out of range of series
            {
                return -1;
            }
            if (line.dp1 == null || line.dp2 == null) return -1;
            var Yvalue = YValueIntersect(series,line.dp1, line.dp2, xCursorPos);
           // Yval.Text = Yvalue.ToString();
            return Yvalue;
        }
        public static double YCursorIntersectValue(this Series series, double yCursorPos)
        {
            var line = FindNearestNeighborsYValue(series, yCursorPos);
            if (line.dp2 == null && line.dp1 != null) // out of range of series
            {
                return -1;
            }
            if (line.dp1 == null || line.dp2 == null) return -1;
            var Yvalue = XValueIntersect(series, line.dp1, line.dp2, yCursorPos);
            // Yval.Text = Yvalue.ToString();
            return Yvalue;
        }

        private static Line FindNearestNeighborsXValue(this Series series, double xCursorPos)
        {
            Line line = new Line();
            bool high = false;
            //bool low = false;
            foreach (DataPoint dp in series.Points)
            {
                if (dp.XValue <= xCursorPos)
                {
                    // if (!low)
                    // {
                    line.dp1 = dp;
                    // low = true;
                    // }       
                }
                if (dp.XValue > xCursorPos)
                {
                    if (!high)
                    {
                        line.dp2 = dp;
                        high = true;
                    }
                }
            }
            return line;
        }
        private static Line FindNearestNeighborsYValue(this Series series, double yCursorPos)
        {
            Line line = new Line();
            bool high = false;
            //bool low = false;
            foreach (DataPoint dp in series.Points)
            {
                if (dp.YValues[0] <= yCursorPos)
                {
                    // if (!low)
                    // {
                    line.dp1 = dp;
                    // low = true;
                    // }       
                }
                if (dp.YValues[0] > yCursorPos)
                {
                    if (!high)
                    {
                        line.dp2 = dp;
                        high = true;
                    }
                }
            }
            return line;
        }

        public static double YValueIntersect(this Series series, DataPoint p1, DataPoint p2, double xCursorPos)
        {
            double Yvalue = 0;
            if (p1.YValues[0] == p2.YValues[0]) //is straight line
            {
                return p1.YValues[0];
            }
            else
            {
                double B1 = 0;
                B1 = sub_Diference_intersect(series,xCursorPos, p2, p1);
                //if line is up
                if (p1.YValues[0] < p2.YValues[0])
                {
                    Yvalue = p1.YValues[0] + B1;
                }
                else
                if (p1.YValues[0] > p2.YValues[0])
                {
                    Yvalue = p1.YValues[0] - B1;
                }
                //if line is up

            }



            return Yvalue;
        }
        public static double XValueIntersect(this Series series, DataPoint p1, DataPoint p2, double yCursorPos)
        {
            double Yvalue = 0;
            if (p1.YValues[0] == p2.YValues[0]) //is straight line
            {
                return p1.YValues[0];
            }
            else
            {
                double B1 = 0;
                B1 = sub_Diference_intersect(series, yCursorPos, p2, p1);
                //if line is up
                if (p1.YValues[0] < p2.YValues[0])
                {
                    Yvalue = p1.YValues[0] + B1;
                }
                else
                if (p1.YValues[0] > p2.YValues[0])
                {
                    Yvalue = p1.YValues[0] - B1;
                }
                //if line is up

            }



            return Yvalue;
        }

        public static List<DataPoint> ScanSeriesVertical(this Series series, double interval, double start, double end)
        {
            double idx = start;
            List<DataPoint> DPS = new List<DataPoint>();
            do
            {                
                var result = series.XCursorIntersectValue(idx);
                DataPoint dp = new DataPoint(idx, result);
                DPS.Add(dp);
                idx += interval;
            } while (idx<=end);
            return DPS;
        }
        public static List<DataPoint> ScanSeriesHorizontal(this Series series, double interval, double start, double end)
        {
            double idx = start;
            List<DataPoint> DPS = new List<DataPoint>();
            do
            {
                var result = series.YCursorIntersectValue(idx);
                DataPoint dp = new DataPoint(idx, result);
                DPS.Add(dp);
                idx += interval;
            } while (idx <= end);
            return DPS;
        }
        //STARTING POINT
        public static List<DataPoint> CompareSeriesForVerticalDifferential(Series series1, Series series2, double interval=0.001, double scaleFactor=1, double lowThreshold=-0.09, double highthreshold=0.09)
        {

            List<double> Error = new List<double>();
            List<DataPoint> ydif = new List<DataPoint>();
            try
            {
                var path1 = series1.ScanSeriesVertical(interval, series1.Points.First().XValue, series1.Points.Last().XValue);
                var path2 = series2.ScanSeriesVertical(interval, series1.Points.First().XValue, series1.Points.Last().XValue);
                for (int i = 0; i < path1.Count; ++i)
                {
                    double er = path1[i].YValues[0] - path2[i].YValues[0];
                    if (er > lowThreshold && er < highthreshold) //out of range for Y;
                    {
                        er = er * scaleFactor;
                        Error.Add(er);
                    }
                    else
                    {
                        er = er * scaleFactor;
                        Error.Add(0.00);
                    }
                    ydif.Add(new DataPoint(path1[i].XValue, er));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could Not Compare/r/n"+ex.ToString());
            }
            return ydif;
        }
        public static List<DataPoint> CompareSeriesForHorizontalDifferential(Series series1, Series series2, double interval = 0.001, double scaleFactor = 1, double lowThreshold = -0.09, double highthreshold = 0.09)
        {

            List<double> Error = new List<double>();
            List<DataPoint> ydif = new List<DataPoint>();
            try
            {
                double s1MinY;
                double s1MaxY;
                s1MinY = MinY(series1.Points.ToList());
                s1MaxY = MaxY(series1.Points.ToList());
                double s2MinY;
                double s2MaxY;
                s2MinY = MinY(series2.Points.ToList());
                s2MaxY = MaxY(series2.Points.ToList());
                var path1 = series1.ScanSeriesHorizontal(interval, s1MinY, s1MaxY);
                var path2 = series2.ScanSeriesHorizontal(interval, s2MinY, s2MaxY);
                for (int i = 0; i < path1.Count; ++i)
                {
                    double er = path1[i].YValues[0] - path2[i].YValues[0];

                    if (er > lowThreshold && er < highthreshold) //out of range for Y;
                    {
                        er = er * scaleFactor;
                        Error.Add(er);
                    }
                    else
                    {
                        er = er * scaleFactor;
                        Error.Add(0.00);
                    }
                    ydif.Add(new DataPoint(path1[i].XValue, er));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could Not Compare/r/n" + ex.ToString());
            }
            return ydif;
        }
        public class Window
        {
            public Window(double w, double h)
            {
                Width = w;
                Height = h;
            }
            public double Width;
            public double Height;

        }
        public class Grid
        {
                   
        }
        //EXPERIMENTAL
        public static double YValue(this DataPoint dp, int idx=0)
        {
            return dp.YValues[idx];
        }
        
        public static void XYFOO(this ChartArea chart,Series series1, Series series2, double interval = 0.001)
        {
            //build grid to find intersects at regualr interval
            double xMin = chart.AxisX.Minimum;
            double xMax = chart.AxisX.Maximum;
            double yMin = chart.AxisY.Minimum;
            double yMax = chart.AxisY.Maximum;
            Window window = new Window(xMax-xMin,yMax-yMin);

            //create Grid with even interval spacing
            var XI = window.Width / interval;
            XI = Math.Round(XI, MidpointRounding.AwayFromZero); //round up
            var YI = window.Width / interval;
            YI = Math.Round(YI, MidpointRounding.AwayFromZero); //round up
            XI = (XI * interval) - interval;
            YI = (YI * interval) - interval;
            Window adjustedWindow = new Window(XI, YI);

            //Fill grid
            List<DataPoint> GridValues = new List<DataPoint>();
            double X = 0;
            double Y = 0;
            for (int y = 0; y <= adjustedWindow.Height; ++y)
            {
                for (int x = 0; x <= adjustedWindow.Width; ++x)
                {                    
                    DataPoint dp = new DataPoint(X, Y);
                    GridValues.Add(dp);
                    X += interval;
                }
                Y += interval;
            }

            //Follow Series Index and find nearest xy grid point
            List<DataPoint> Path1 = new List<DataPoint>();
            foreach(var dp in series1.Points)
            {
                DataPoint lowX;
                DataPoint lowY;
                DataPoint HighX;
                DataPoint HighY;

                foreach(var gp in GridValues)
                {
                    //
                }
            }

    }

    }
    public class Line
    {
        public Line() { }
        public Line(DataPoint p1, DataPoint p2)
        {
            dp1 = p1;
            dp2 = p2;
        }
        public DataPoint dp1;
        public DataPoint dp2;
    }
}
