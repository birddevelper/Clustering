using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
namespace K_mean
{
    public partial class Form1 : Form
    {

        ArrayList datapoints;
        ArrayList means;
        Boolean isChanged = false;
        Boolean bad_mean = false;
        int max_x = 0;
        int max_y = 0;
        int min_x = int.MaxValue;
        int min_y = int.MaxValue;
        private static Random rand = new Random();
        Color[] colors = new Color[] { Color.Blue, Color.Green, Color.Orange, Color.White, Color.Violet, Color.Cyan, Color.DeepPink, Color.DarkGray, Color.PaleGreen, Color.PaleVioletRed, Color.Plum, Color.Beige, Color.Chocolate, Color.DarkSeaGreen, Color.Brown, Color.MintCream, Color.MistyRose, Color.Orchid, Color.Salmon, Color.Sienna, Color.SlateGray, Color.Gold, Color.YellowGreen, Color.PeachPuff,Color.Navy,Color.Moccasin,Color.Indigo,Color.Honeydew,Color.Cornsilk,Color.Bisque, Color.Cornsilk,Color.Teal};
        public Form1()
        {
            InitializeComponent();
            reset_data();
            reset_means();
            Form1.CheckForIllegalCrossThreadCalls = false;
        }

        private void pic_pln_Click(object sender, EventArgs e)
        {
          
        }

        private void pic_pln_MouseDown(object sender, MouseEventArgs e)
        {
              Graphics gf = pic_pln.CreateGraphics();
              gf.DrawEllipse(new Pen(Color.Red, 2f),e.X-1 ,e.Y-1,2, 2 );
              datapoints.Add(new data(new Point(e.X, e.Y)));
              if (e.X > max_x) max_x = e.X;
              if (e.Y > max_y) max_y = e.Y;
              if (e.X < min_x) min_x = e.X;
              if (e.Y < min_y) min_y = e.Y;
        }

        /////////////////////////////////// random number generator////////////////////////////////
        public static int rInt(int incLB ,int exclUB)
        {
           // int t = rand.Next(incLB, exclUB);
            //return t;
            Random rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));

            int rnd = rndNum.Next(incLB, exclUB);

            return rnd;
        }
        
        /////////////////////redraw data points ////////
        public void draw_points(ArrayList dpw)
        {
            foreach (data dp in dpw)
            {
                Graphics gf = pic_pln.CreateGraphics();
                gf.DrawEllipse(new Pen(Color.Red, 2f), dp.point.X - 1, dp.point.Y - 1, 2, 2);
            }
          

        }
        
        // This method clears center points
        public void reset_means()
        {
            means = new ArrayList();
        }


        // This method clears the board and all data points
        public void reset_data()
        {
            datapoints = new ArrayList();
             max_x = 0;
             max_y = 0;
             min_x = int.MaxValue;
             min_y = int.MaxValue;

        }

        /////////////////////////////////////genereat random mean//////////////////////////////
        public void generate_mean(int meanNum)
        {
            for (int i = 0; i < meanNum; )
            {
                int x=rInt( min_x, max_x);
                int y = rInt(min_y, max_y);
                bad_mean = false;
                foreach (data m in means)
                {
                    if (distance(new Point(x, y), m.point) < distance(new Point(min_x, min_y), new Point(max_x, max_y)) / meanNum)
                        bad_mean = true;
                }
                if (!bad_mean)
                {
                    means.Add(new data(new Point(x, y), i));
                    Graphics gf = pic_pln.CreateGraphics();
                    gf.DrawEllipse(new Pen(colors[i], 2f), x - 4, y - 4, 8, 8);
                    i++;
                }

            }

        }
        //////////////////////////////////// distance measuring ///////////////////////////
        public double distance(Point p1,Point p2)
        {
            return Math.Sqrt(Math.Pow((p1.X-p2.X),2)+Math.Pow((p1.Y-p2.Y),2));
        }
        ///////////////////////////////////// find best mean for a point /////////////////////////
        public int find_nearest_mean(Point p)
        {
                int nearst_mean=0;
                double nearest_distnace=double.MaxValue;
                foreach(data mean in means )
                {
                    if(distance(p,mean.point)<nearest_distnace) { nearest_distnace=distance(p,mean.point); nearst_mean=mean.set;}
                }
            return nearst_mean;
        }
        //////////////////////////////////// Cluster assigment ////////////////////////////
        public void cluster_assigment()
        {
            foreach(data dp in datapoints )
                {
                    int prior_set = dp.set;
                    dp.set = find_nearest_mean(dp.point);
                    if (dp.set != prior_set) isChanged = true;
                    Graphics gf = pic_pln.CreateGraphics();
                    gf.DrawEllipse(new Pen(colors[dp.set], 2f), dp.point.X - 1, dp.point.Y - 1, 2, 2);
                }
        }
        ///////////////////////// moving mean to best place ///////////////////////////////
        public void fix_means(int meanNum)
        {
           
            for (int i = 0; i < meanNum; i++)
            { 
                int avg_x = 0; int avg_y = 0;
                int cnt=0;
                foreach (data dp in datapoints)
                {

                    if (dp.set == ((data)means[i]).set) { avg_x += dp.point.X; avg_y += dp.point.Y; cnt++; }
                }
                try
                {
                    ((data)means[i]).point.X = avg_x / cnt;
                    ((data)means[i]).point.Y = avg_y / cnt;
                }
                catch (Exception)
                {
                    
                                  
                }
                Graphics gf = pic_pln.CreateGraphics();
                gf.DrawEllipse(new Pen(colors[i], 2f), ((data)means[i]).point.X - 4, ((data)means[i]).point.Y - 4, 8, 8);
            }

        }
        ////////////////////////////////////// clear panle///////////////////////////
        public void clearboard()
        {

            Graphics graphic =  pic_pln.CreateGraphics();
            graphic.Clear(Color.Black);
        }
        ///////////////////////////////////// k-mean algorithm ///////////////////////////

        public void k_mean_alg()
        {
            lbl_info.Text = "Working...";
            // Generate K Random Mean
            int k = int.Parse(txt_mean_cnt.Text);
            int itr = 0;
            generate_mean(k);
            isChanged = true;
            while (isChanged) {
                Thread.Sleep(5000-trk_speed.Value);
                clearboard();
                //itr++;
                isChanged = false;
                // mean assigment
                cluster_assigment();
                //fix the means
                fix_means(k);

            } 

            lbl_info.Text = "Finished";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(datapoints.Count==0)
            {
                MessageBox.Show("No Data Points is Found!");
                return;
            }
            if (txt_mean_cnt.Text == "")
            {
                MessageBox.Show("Please Enter Means Count!!");
                return;
            }
            clearboard();
            draw_points(datapoints);
            reset_means();
             // start algorithm as a thread
            Thread th = new Thread(new ThreadStart(k_mean_alg));
            th.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reset_data();
            reset_means();
            clearboard();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        ////////////////////////////////// new algorithm subroutins ///
        
         public void draw_points(ArrayList S,ArrayList mymeans){
          //   foreach (data dp in S)
             {
             //    Graphics gf = pic_pln.CreateGraphics();
             //    gf.DrawEllipse(new Pen(colors[dp.set-1], 2f), dp.point.X - 1, dp.point.Y - 1, 2, 2);
             }
             foreach (data m in mymeans)
             {
                 Graphics gf = pic_pln.CreateGraphics();
                 gf.DrawEllipse(new Pen(colors[(m.set-1)%32], 2f), m.point.X - 4, m.point.Y - 4, 8, 8);
             }

        }

        //////////////////////// remove points from S ////////////
         public void remove_points_from_S(ArrayList S, ArrayList remove_list)
         {
             remove_list.Reverse();
             foreach (int index in remove_list)
             {
                 S.RemoveAt(index);
             }
         }

        //////////////////////////////// take data point randomly
        public data take_data_point(ArrayList S, ArrayList S2, int set)
        {
            int i = rInt(0, S.Count);
            data dp = (data)S[i];


            if (neigh_count(dp, r) > nighbour_min)
            {
                ((data)S[i]).set = set;
                Graphics gf = pic_pln.CreateGraphics();
                gf.DrawEllipse(new Pen(colors[(((data)S[i]).set - 1) % 32], 2f), ((data)S[i]).point.X - 1, ((data)S[i]).point.Y - 1, 2, 2);
                S2.Add(dp);
                
            }
                S.RemoveAt(i);
                return (dp);
            
        }


        ////////////////////////////////neighbour count calculate ///////////////////

        public int neigh_count(data d,int r)
        {
            int cnt=0;
            foreach (data x in datapoints)
            {
                if(distance(d.point, x.point) <= r) cnt++;
            }
            return cnt;
        }
        //////////////////////////////////////// border coloring //////////////////////

        public void Border_coloring(ArrayList br,ArrayList s2)
        {
            
            foreach (data bp in br)
            {
                int[] sets = new int[500];
                foreach (data x in s2)
                {
                    if (distance(bp.point, x.point) <= r) sets[x.set]++;
                }
                bp.set = maximum_set(sets);
            }
               foreach (data dp in br)
            {
                if (dp.set != 0)
                {
                    Graphics gf = pic_pln.CreateGraphics();
                    gf.DrawEllipse(new Pen(colors[(dp.set - 1) % 32], 2f), dp.point.X - 1, dp.point.Y - 1, 2, 2);
                }
                else
                {
                    Graphics gf = pic_pln.CreateGraphics();
                    gf.DrawEllipse(new Pen(Color.Red, 2f), dp.point.X - 1, dp.point.Y - 1, 2, 2);
                }
            }


        }
        //////////////////////////////////maximum in araylist /////////////////////////
        public int maximum_set(int[] x)
        {
            int maxd = x[0];
            int maxi=0;
            for (int i=0; i< x.Length;i++)
            {
                if (x[i] > maxd) { maxd = x[i]; maxi = i; }

            }
            return maxi;
        }

        //////////////////////////////////Heuristic Algorithm/////////////////////////
        int r;
        int nighbour_min;
        public void mosy_alg()
        {
            lbl_info.Text = "Working...";
            ///////iniate S /////////////
            ArrayList S=new ArrayList();
            ArrayList S2 = new ArrayList();
            ArrayList mymeans = new ArrayList();
            ArrayList border_point = new ArrayList();
            S.AddRange(datapoints);
            r = int.Parse(txt_r.Text);
            nighbour_min = int.Parse(txt_neigh.Text);
            //// initiate queue
            Queue<data> q = new System.Collections.Generic.Queue<data>();

            //////Start of algorithm
            int ci = 1;
            while (S.Count != 0)
            {
                ///// take a data point randomly
                data dp = take_data_point(S,S2,ci);
                int cluster_count = 1;
                int sum_x = 0;
                int sum_y = 0;
                if (neigh_count(dp, r) > nighbour_min)
                {

                    sum_x = dp.point.X;
                    sum_y = dp.point.Y;
                    q.Enqueue(dp);
                }
                else
                {
                    border_point.Add(dp);
                }
                while (q.Count != 0)
                {
                    dp = q.Dequeue();
                    ArrayList remove_list = new ArrayList();
                    Boolean nth = false;
                    //if (neigh_count(dp, r) > nighbour_min) nth = true;
                    
                        for (int ii = 0; ii < S.Count; ii++)
                        {
                            try
                            {
                                if (distance(dp.point, ((data)S[ii]).point) <= r)  // check the distance from point picked of queue  to point in S
                                {
                                    // put the point in the queue
                                    if (/*nth &&*/ neigh_count((data)S[ii], r) > nighbour_min)
                                    {
                                        q.Enqueue(((data)S[ii]));
                                        //set point's set
                                        ((data)S[ii]).set = ci;
                                        Graphics gf = pic_pln.CreateGraphics();
                                        Thread.Sleep((5000 - trk_speed.Value) / 5);
                                        gf.DrawEllipse(new Pen(colors[(((data)S[ii]).set - 1) % 32], 2f), ((data)S[ii]).point.X - 1, ((data)S[ii]).point.Y - 1, 2, 2);
                                        sum_x += ((data)S[ii]).point.X;
                                        sum_y += ((data)S[ii]).point.Y;
                                        cluster_count++;
                                    }
                                    else
                                    {
                                        border_point.Add((data)S[ii]);
                                    }
                                    // remove from S
                                    S2.Add(((data)S[ii]));
                                    remove_list.Add(ii); //S.RemoveAt(ii);//S.Remove(((data)S[ii]));
                                    

                                }
                            }
                            catch (Exception ee)
                            {
                                MessageBox.Show("error  " + ee.Message);
                            }
                        }

                        remove_points_from_S(S, remove_list);
                    
                   
                }
                data mi=new data(new Point(sum_x/cluster_count,sum_y/cluster_count),ci);
                mymeans.Add(mi);
                ci++;
            }
            //clearboard();
            Border_coloring(border_point, S2);
            draw_points(S2, mymeans);
            lbl_info.Text = "Finished";
        }

        ///////////////////////////////////////////////////////////




        private void button2_Click_1(object sender, EventArgs e)
        {
            if (datapoints.Count == 0)
            {
                MessageBox.Show("No Data Points is Found!");
                return;
            }
            if (txt_r.Text == "")
            {
                MessageBox.Show("Please Enter Radius!!");
                return;
            }
            clearboard();
            draw_points(datapoints);
            Thread th = new Thread(new ThreadStart(mosy_alg));
            th.Start(); 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                path = ofd.FileName;
                reset_data();
                reset_means();
                clearboard();
                Thread th = new Thread(new ThreadStart(load_dataset));
                th.Start();
            }
        }
        string path = "";
        ///////////////////////////////////////////////////////////loading dataset
        public void load_dataset()
        {
           // OpenFileDialog ofd = new OpenFileDialog();
           // DialogResult result = ofd.ShowDialog();
           // if (result == DialogResult.OK) // Test result.
            {
                
                System.IO.FileStream fs= System.IO.File.Open(path, System.IO.FileMode.Open);
                System.IO.StreamReader sr= new System.IO.StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    string s=sr.ReadLine();
                    string[] par=s.Split('\t');
                    int x = (int)Math.Round(float.Parse(par[0]),0,MidpointRounding.AwayFromZero)*10+100;
                    int y = (int)Math.Round(float.Parse(par[1]), 0, MidpointRounding.AwayFromZero)*10;
                    if (x > 10000) x /= 1000;
                    if (y > 10000) y /= 1000;
                    Graphics gf = pic_pln.CreateGraphics();
                    gf.DrawEllipse(new Pen(Color.Red, 2f), x - 1, y - 1, 2, 2);
                    datapoints.Add(new data(new Point(x, y)));
                    if (x > max_x) max_x = x;
                    if (y > max_y) max_y = y;
                    if (x < min_x) min_x = x;
                    if (y < min_y) min_y = y;
                }

                sr.Close();
            }



        }

    }
}
