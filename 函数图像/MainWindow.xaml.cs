using CustomComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace 函数图像
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private struct LineInfo
        {
            public List<double> Data;
            public string Info;
        }
        int suanfa = 0;
        public MainWindow()
        {
            InitializeComponent();
            MyMessageBox.Show("aaa");
            
        }
        private void CreateLineData(double b, double m, double c)
        {
            double p = -25;
            List<double> a = new List<double>();
            string infos = "";
            for (int j = 0; j < 500; j++)
            {
                switch (suanfa)
                {
                    case 0:
                        a.Add(b * Math.Pow(p, m) + c);
                        infos = "0|Y=" + b + "X**" + m + "+" + c;
                        break;
                    case 1:
                        a.Add(b *Math.Sin(Math.Pow(p, m)) + c);
                        infos = "1|Y=" + b + "X**" + m + "+" + c;
                        break;
                    case 2:
                        a.Add(b * Math.Cos(Math.Pow(p, m)) + c);
                        infos = "2|Y=" + b + "X**" + m + "+" + c;
                        break;
                    case 3:
                        a.Add(b * Math.Tan(Math.Pow(p, m)) + c);
                        infos = "3|Y=" + b + "X**" + m + "+" + c;
                        break;
                }

                p += 0.1;
            }
            Top.Add(a.Max());
            LineInfo info = new LineInfo
            {
                Data = a,
                Info = infos
            };
            Lines.Add(info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b">x的倍数</param>
        /// <param name="m">x的指数幂</param>
        /// <param name="c">常数</param>

        private void Draw()
        {
            paint.Children.Clear();
            if (Top.Count > 0)
            {
                Top.Sort();
                max = Top[Top.Count / 2];
                double num = 500 / 500;
                for (int t = 0; t < Lines.Count; t++)
                {
                    Polyline polyline = new Polyline
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 5.0,
                        Opacity = 0.5
                    };
                    polyline.MouseEnter += Polyline_MouseEnter;
                    polyline.MouseLeave += Polyline_MouseLeave;
                    polyline.MouseDown += Polyline_MouseDown;
                    Point[] array = new Point[500];
                    PointCollection pointCollection = new PointCollection();
                    for (int i = 0; i < Lines[t].Data.Count; i++)
                    {
                        PointCollection pointCollection2 = new PointCollection();
                        array[i].Y = 250 - Lines[t].Data[i] / max * 250.0;
                        array[i].X = num * i;
                        pointCollection.Add(array[i]);
                    }
                    polyline.Tag = Lines[t];
                    polyline.Points = pointCollection;
                    paint.Children.Add(polyline);
                }
            }
        }

        private void Polyline_MouseLeave(object sender, MouseEventArgs e)
        {
            Polyline polyline = sender as Polyline;
            polyline.Stroke = Brushes.Blue;
            polyline.Opacity = 0.5;
        }

        private void Polyline_MouseEnter(object sender, MouseEventArgs e)
        {
            Polyline polyline = sender as Polyline;
            polyline.Stroke = Brushes.Red;
            polyline.Opacity = 1;
        }

        private void Polyline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Polyline polyline = sender as Polyline;
            LineInfo info = (LineInfo)polyline.Tag;
            string[] ress = info.Info.Split('|');
            int ss = int.Parse(ress[0]);
            string[] fi = ress[1].Split(new char[] { '=', '+', '*', 'X' });
            double b = double.Parse(fi[1]);
            double m = double.Parse(fi[4]);
            double c = double.Parse(fi[5]);
            string tt="";
            switch (ss)
            {
                case 0:tt = "基本的函数:"+b+"X**"+m+"+"+c;
                    break;
                case 1:
                    tt = "Sin()函数:" + b + "Sin(X**" + m + ")+" + c; 
                    break;
                case 2:
                    tt = "Cos()函数:" + b + "Cos(X**" + m + ")+" + c; ;
                    break;
                case 3:
                    tt = "Tan()函数:" + b + "Tan(X**" + m + ")+" + c; ;
                    break;
            }
            switch (MyMessageBox.Show("是否删除此条函数线？\n" + tt, "提示", MyMessageBox.MyMessageBoxButton.ConfirmNOButton, "删除该曲线", "取消", "查找值"))
            {
                case MyMessageBox.MyMessageBoxResult.Comfirm:
                    temp = info.Info;
                    int x = Lines.FindIndex(pre);
                    Lines.RemoveAt(x);
                    temp = "";
                    Draw();
                    break;
                case MyMessageBox.MyMessageBoxResult.Buttons:
                    InputBox inputBox = new InputBox();
                    string res = inputBox.Show("请输入X的值：", "查找Y值");

                    if (isNum(res) == true)
                    {
                        double xv = double.Parse(res);

                        switch (ss)
                        {
                            case 0:
                                MessageBox.Show("当X=" + xv + "时\nY=" + (b * Math.Pow(xv, m) + c).ToString(), "结果");
                                break;
                            case 1:
                                MessageBox.Show("当X=" + xv + "时\nY=" + (b *Math.Sin( Math.Pow(xv, m)) + c).ToString(), "结果");
                                break;
                            case 2:
                                MessageBox.Show("当X=" + xv + "时\nY=" + (b * Math.Cos(Math.Pow(xv, m)) + c).ToString(), "结果");
                                break;
                            case 3:
                                MessageBox.Show("当X=" + xv + "时\nY=" + (b * Math.Tan(Math.Pow(xv, m)) + c).ToString(), "结果");
                                break;
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("输入错误", "错误");
                    }
                    break;
            }
        }

        private static string temp = "";
        private static Predicate<LineInfo> pre = new Predicate<LineInfo>(FindData);

        private static bool FindData(LineInfo l)
        {
            if (l.Info.Equals(temp))
                return true;
            else
                return false;
        }
        private List<LineInfo> Lines = new List<LineInfo>();
        private double max = 0.0;
        private List<double> Top = new List<double>();

        private void button_Click(object sender, RoutedEventArgs e)
        {
            double b, m, c;
            string bb = beishu.Text, mm = mi.Text, cc = changshu.Text;
            bool Fcheck = true;
            if (bb.Equals(""))
            {
                bb = "1";
            }
            if (mm.Equals(""))
            {
                mm = "1";
            }
            if (cc.Equals(""))
            {
                cc = "0";
            }
            bool[] Check = { false, false, false };
            Check[0] = isNum(bb);
            Check[1] = isNum(mm);
            Check[2] = isNum(cc);
            foreach (bool ok in Check)
            {
                if (ok == false)
                {
                    Fcheck = false;
                    MessageBox.Show("输入有误", "错误");
                }
            }
            if (Fcheck == true)
            {
                double.TryParse(bb, out b);
                double.TryParse(mm, out m);
                double.TryParse(cc, out c);
                CreateLineData(b, m, c);
                Draw();

            }
        }
        private bool isNum(string z)
        {
            bool result = false;
            char[] array = z.ToArray();
            int index = 0;
            if (array[0] == '-')
                index = 1;
            else if (array[0] == '.')
            {
                return false;
            }
            else if (array[array.Length - 1] == '.')
                return false;
            for (; index < array.Length; index++)
            {
                if ((array[index] >= '0' && array[index] <= '9') || array[index] == '.')
                {
                    result = true;
                    continue;
                }
                result = false;
                break;
            }
            return result;
        }

        private void paint_MouseMove(object sender, MouseEventArgs e)
        {

            Point point = e.GetPosition(paint);
            this.Title = "X:" + point.X + "      Y:" + point.Y;
        }

        private void radioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton r = sender as RadioButton;
            suanfa = int.Parse(r.Tag.ToString());
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Lines.Clear();
            Top.Clear();
            max = 0;
            Draw();
        }
    }
}
