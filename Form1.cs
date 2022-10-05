using GMap.NET;
using GMap.NET.MapProviders;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        public MySqlConnection mysqlCon = new MySqlConnection("Server=localhost;Database=mydb;Uid=root;Pwd='1111';");
        int selectedId;//seçilen datagridview indexi içindeki id değişkenini tutan global değişken
        string baslangic = "";//başlangıç noktası koordinatının tutulması için global değişken

        //favoriler arası bağlantı kurmak için global değişken
        string fav_etiket;
        string fav_latt;
        string fav_long;


        public List<PointLatLng> points;
        public void datagridFill()
        {
            mysqlCon.Open();
            MySqlDataAdapter MyDA = new MySqlDataAdapter();
            string sqlSelectAll = "SELECT * from konumlar";
            MyDA.SelectCommand = new MySqlCommand(sqlSelectAll, mysqlCon);
            DataTable table = new DataTable();
            MyDA.Fill(table);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = table;
            dataGridView1.DataSource = bSource;
            mysqlCon.Close();
            foreach (DataGridViewBand band in dataGridView1.Columns)
            {
                band.ReadOnly = true;
            }
            // https://stackoverflow.com/questions/1025670/how-do-you-automatically-resize-columns-in-a-datagridview-control-and-allow-the
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            for (int i = 0; i <= dataGridView1.Columns.Count - 1; i++)
            {
                int colw = dataGridView1.Columns[i].Width;
                dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridView1.Columns[i].Width = colw;
            }

        }
        public void datagridFill_Fav()
        {
            mysqlCon.Open();
            MySqlDataAdapter MyDA = new MySqlDataAdapter();
            string sqlSelectAll = "SELECT * from favoriler";
            MyDA.SelectCommand = new MySqlCommand(sqlSelectAll, mysqlCon);
            DataTable table = new DataTable();
            MyDA.Fill(table);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = table;
            dataGridView2.DataSource = bSource;
            mysqlCon.Close();
            foreach (DataGridViewBand band in dataGridView2.Columns)
            {
                band.ReadOnly = true;
            }

            dataGridView2.DataSource = table;
            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView2.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            for (int i = 0; i <= dataGridView2.Columns.Count - 1; i++)
            {
                int colw = dataGridView2.Columns[i].Width;
                dataGridView2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridView2.Columns[i].Width = colw;
            }

        }
        public List<int> Distances(string orj, string[] dest, List<string> etiket)
        {
            List<int> kilometers = new List<int>();
            for (int i = 0; i < dest.Length; i++)
            {
                WebRequest request = WebRequest.Create("https://maps.googleapis.com/maps/api/directions/json?origin=" + orj + "&destination=" + dest[i] + "&units=metric&key=AIzaSyDevKkIHl38UNYteEKJyiYfW0P80kkqUG8");
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                StreamReader reader = new StreamReader(data);
                string responseFromServer = reader.ReadToEnd();
                JObject obj = JObject.Parse(responseFromServer);
                JToken distance = obj.SelectToken("$.routes.[0].legs[*].distance.value");
                kilometers.Add(Convert.ToInt32(distance));
                response.Close();
            }
            return kilometers;//gönderilen orjin ve çoklu dest noktaları arasındaki mesafeler   
        }
        public void mapPos(double x, double y)
        {
            map.Position = new PointLatLng(x, y);//CUMHURİYET PARKI
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            datagridFill();//konumlar datagrid doldurma
            datagridFill_Fav();//favoriler datagrid doldurma
            GMapProviders.GoogleMap.ApiKey = @"AIzaSyDevKkIHl38UNYteEKJyiYfW0P80kkqUG8";
            map.MapProvider = GMapProviders.GoogleMap;
            map.DragButton = MouseButtons.Left;
            map.ShowCenter = false;
            mapPos(40.82207230536177, 29.921615737616225);
            map.MinZoom = 0;
            map.MaxZoom = 18;
            map.Zoom = 16;
            points = new List<PointLatLng>();
        }


        List<string> etiket = new List<string>();//şehirlerin ismi tutuluyor
        GMapOverlay markers = new GMapOverlay("markers");

        private void button5_Click(object sender, EventArgs e)
        {
            List<string> latitute = new List<string>();
            List<string> longtitute = new List<string>();
            etiket.Clear();
            points.Clear();
            points = new List<PointLatLng>();



            //veri tabanındaki bilgileri oluşturulan list elemanlarına gönderen kod
            mysqlCon.Open();
            MySqlCommand kmt = new MySqlCommand("select * from `konumlar`", mysqlCon);
            kmt.ExecuteNonQuery();
            MySqlDataReader reader = kmt.ExecuteReader();
            while (reader.Read())
            {

                latitute.Add(reader.GetString("latitute"));
                longtitute.Add(reader.GetString("longtitute"));
                etiket.Add(reader.GetString("etiket"));


            }
            mysqlCon.Close();


            int count = 0;
            count = latitute.Count;//kaç adet koordinat olacak
            string[] coordinates = new string[count];//koordinat sayısı kadar büyüklüğünde string dizisi
            DirectedWeightedGraph g = new DirectedWeightedGraph();//directed graph constructor class


            foreach (var item in etiket)//konumların etiketleri ile node oluşturmak
            {
                g.InsertVertex(item);
            }

            //daha sonra tüm koordinatlar arası uzaklığı bulmak için distances metoduna gönderilecek konumların diziye aktarılması
            for (int i = 0; i < latitute.Count; i++)
            {

                richTextBox1.AppendText(latitute[i] + "," + longtitute[i] + "\n");//koordinatların yazılış şeması richtextbox
                coordinates[i] = latitute[i].Substring(0, 8) + "," + longtitute[i].Substring(0, 8);//koordinat dizisine gönderilen koordinatlar
            }


            for (int i = 0; i < latitute.Count; i++)//her node arasındaki mesafelerin apiye gönderilerek bulunması 
            {

                baslangic = latitute[i].Substring(0, 8) + "," + longtitute[i].Substring(0, 8);
                List<int> km = new List<int>();//return edilen nodeler arası mesafenin tutuldugu list            
                km = Distances(baslangic, coordinates, etiket);
                richTextBox1.AppendText("\n\nKORDİNAT: " + coordinates[i]);
                for (int j = 0; j < etiket.Count; j++)
                {
                    if (km[j] != 0)
                    {
                        g.InsertEdge(etiket[i], etiket[j], Convert.ToInt32(km[j]));
                        richTextBox1.AppendText("\nBAŞLANGIÇ: " + etiket[i] + " VARIŞ: " + etiket[j] + " MESAFE: " + km[j]);
                    }
                }
            }
            for (int i = 0; i < etiket.Count; i++)//sırayla tüm nodeler başlangıç olarak seçilir ve diğer noktalara en kısa yolu bulur
            {
                g.FindPaths(etiket[i]);
            }

            int[,] adjMat = g.matrix();//oluşturulan adjlisti returnleyip form ekranında görüntüleyen değişken

            List<int> ky = g.kisaYolR();//dönen en kısa yolları tutan değişken
            for (int k = 0; k < 6; k++)//dönen matrisi yazdıran döngü
            {
                for (int i = 0; i < 6; i++)
                {
                    richTextBox2.AppendText("\t" + adjMat[k, i]);
                }
                richTextBox2.AppendText("\n");
            }

            foreach (var item in ky)//en kısa yolları yazdıran döngü
            {
                richTextBox2.AppendText("\n" + item);
            }


            //gidilecek noktaların x,y positionlarını pointslatlong değişkenine gönderen satır    
            PointLatLng point;

            for (int i = 0; i < latitute.Count; i++)
            {
                point = new PointLatLng((Convert.ToDouble(latitute[i].Substring(0, 8)) / 100000), (Convert.ToDouble(longtitute[i].Substring(0, 8)) / 100000));
                points.Add(point);

                richTextBox1.AppendText(point.ToString());
            }
            mapPos((Convert.ToDouble(latitute[0].Substring(0, 8)) / 100000), (Convert.ToDouble(longtitute[0].Substring(0, 8)) / 100000));//haritayı başlangıç noktasına ayarlayan metod kullanımı



            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0)
                {
                    GMapMarker rect = new GMapMarkerRect(points[i]);//https://github.com/radioman/greatmaps/tree/master/Demo.WindowsForms/CustomMarkers
                    markers.Markers.Add(rect);
                    map.Overlays.Add(markers);
                }
                else
                {
                    GMapMarker marker = new GMapMarkerCircle(points[i]);//https://github.com/radioman/greatmaps/tree/master/Demo.WindowsForms/CustomMarkers
                    markers.Markers.Add(marker);
                    map.Overlays.Add(markers);
                }

            }



            MapRoute route;
            GMapRoute r;
            GMapOverlay routes;
            GDirections d;

            //tüm noktalar arasında oluşturulan en kısa yol
            double tempdis1 = 0;
            double tempdis2 = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                int min = i + 1;
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[j] != null)
                    {
                        route = GoogleMapProvider.Instance.GetRoute(points[i], points[j], true, false, 14);
                        if (j >= i + 2)
                        {
                            tempdis2 = route.Distance;
                        }
                        else
                        {
                            tempdis1 = route.Distance;
                        }
                        if (tempdis2 != 0 && tempdis2 < tempdis1)
                        {
                            min = j;
                            tempdis1 = tempdis2;
                        }

                    }
                }
                PointLatLng temp = points[min];
                points[min] = points[i + 1];
                points[i + 1] = temp;
            }
            bool highWay = false;
            bool walking;
            if (checkBox2.Checked == true)
            {
                highWay = true;
            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i + 1] != null)
                {
                    route = GoogleMapProvider.Instance.GetRoute(points[i], points[i + 1], highWay, false, 14);
                    r = new GMapRoute(route.Points, etiket[i])
                    {
                        Stroke = new Pen(Color.Red, 2)
                    };
                    routes = new GMapOverlay(etiket[i]);
                    routes.Routes.Add(r);


                    richTextBox2.AppendText(" " + r.Name.ToString() + " ");

                    map.Overlays.Add(routes);
                }
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            mysqlCon.Open();
            MySqlCommand kmt = new MySqlCommand("delete from `konumlar` where id='" + selectedId + "'", mysqlCon);
            kmt.ExecuteNonQuery();
            mysqlCon.Close();
            datagridFill();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mysqlCon.Open();
            MySqlCommand kmt = new MySqlCommand("delete from `konumlar`", mysqlCon);
            kmt.ExecuteNonQuery();
            mysqlCon.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mysqlCon.Open();
            MySqlCommand kmd = new MySqlCommand("insert into `konumlar`(etiket,latitute,longtitute) values" +
                "('" + textBox3.Text + "','" + textBox1.Text.Replace(",", ".") + "','" + textBox2.Text.Replace(",", ".") + "')", mysqlCon);
            kmd.ExecuteNonQuery();
            mysqlCon.Close();
            datagridFill();
            if (checkBox1.Checked == true)//checklenme durumunda koordinatı global değişkene gönderen satır
            {
                baslangic = textBox1.Text + "," + textBox2.Text;
            }
        }

        private void map_MouseClick(object sender, MouseEventArgs e)//haritadan sağ tıklanan kordinatları yazıyor
        {
            if (e.Button == MouseButtons.Right)
            {
                var point = map.FromLocalToLatLng(e.X, e.Y);
                double lat = point.Lat;
                double lng = point.Lng;
                textBox1.Text = lat + "";
                textBox2.Text = lng + "";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string id = dataGridView1.Rows[e.RowIndex].Cells["id"].Value.ToString();
                selectedId = Convert.ToInt32(id);
            }
            catch (Exception)
            {
                MessageBox.Show("Alan Seçiniz!!");
            }
        }

        private void button7_Click(object sender, EventArgs e)//temizleme buttonu tam işlevli değil eski noktalar hesaplanınca tekrar geliyor
        {
            while (map.Overlays.Count > 0)
            {
                map.Overlays.RemoveAt(0);
                map.Refresh();
            }
            map.Overlays.Remove(markers);
        }

        private void button9_Click(object sender, EventArgs e)//favoriler tablosundaki tıklanan datagridview nesnesini konumlar tablosuna ekler
        {
            mysqlCon.Open();
            MySqlCommand kmd = new MySqlCommand("insert into `konumlar`(etiket,latitute,longtitute) values" +
                "('" + fav_etiket + "','" + fav_latt + "','" + fav_long + "')", mysqlCon);
            kmd.ExecuteNonQuery();
            mysqlCon.Close();
            datagridFill_Fav();
            datagridFill();

        }

        private void button8_Click(object sender, EventArgs e)//favorilere ekle buttonuna tıklanınca girilen son konumu favoriler tablosuna ekler
        {
            mysqlCon.Open();
            MySqlCommand kmd = new MySqlCommand("insert into `favoriler`(fav_etiket,fav_latt,fav_long) values" +
                "('" + textBox3.Text + "','" + textBox1.Text.Replace(",", ".") + "','" + textBox2.Text.Replace(",", ".") + "')", mysqlCon);
            kmd.ExecuteNonQuery();
            mysqlCon.Close();
            datagridFill_Fav();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string id = dataGridView2.Rows[e.RowIndex].Cells["idfavoriler"].Value.ToString();
                fav_etiket = dataGridView2.Rows[e.RowIndex].Cells["fav_etiket"].Value.ToString();
                fav_latt = dataGridView2.Rows[e.RowIndex].Cells["fav_latt"].Value.ToString();
                fav_long = dataGridView2.Rows[e.RowIndex].Cells["fav_long"].Value.ToString();

                selectedId = Convert.ToInt32(id);
            }
            catch (Exception)
            {
                MessageBox.Show("Alan Seçiniz!!");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            mysqlCon.Open();
            MySqlCommand kmt = new MySqlCommand("delete from `favoriler` where id='" + selectedId + "'", mysqlCon);
            kmt.ExecuteNonQuery();
            mysqlCon.Close();
            datagridFill_Fav();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            map.SetPositionByKeywords(textBox4.Text.ToString());//çalışmıyor adrese göre harita
        }
    
        private void button11_Click(object sender, EventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Black);
            Brush b = new SolidBrush(Color.Red);
            g.DrawEllipse(p, 100, 100, 100, 100);
        }
    }
}
