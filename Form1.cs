using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace WindowsFormsApplication35
{
    public partial class Form1 : Form
    {
        public static int Mesafe;
        List<PictureBox> items = new List<PictureBox>();
        Point[] points = new Point[50];
        Graphics gObject;
        Graphics gText;

        public Form1()
        {
            InitializeComponent();
        }
        public static MySqlConnectionStringBuilder build = new MySqlConnectionStringBuilder();
        public static MySqlConnection baglanti;
        int SehirSayisi = 0;
        Random Rastgele = new Random();
        int ToplamMesafe = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            build.Server = "localhost";
            build.UserID = "root";
            build.Database = "kuryedatabase";
            build.Password = "safranbolu78";
            baglanti = new MySqlConnection(build.ToString());
            BilgileriOku(); baglanti.Close();


        }
        
        private void MakePicturebox(MouseEventArgs e)
        {
            
            int X, Y;
            X = e.X;
            Y = e.Y;
            PictureBox point = new PictureBox();
            point.Height = 25;
            point.Width = 25;
            if (items.Count() > 0)
            {
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                gp.AddEllipse(0, 0, point.Width - 3, point.Height - 3);
                Region rg = new Region(gp);
                point.Region = rg;

                for (int i = 0; i < points.Length; i++)
                {
                    if (items.Count() == i)
                    {
                        points[i] = new Point(X, Y);
                    }
                }
                
            }
            
            if (items.Count() == 0)
            {
                points[0] = new Point(X, Y);
            }
            point.BackColor = Color.Black;
            point.Location = new Point(X, Y); 
            items.Add(point);
            this.Controls.Add(point);
            point.BringToFront();
           
          }
        private void MakeText(PaintEventArgs e)
        {
            Font drawFont = new Font("Arial", 32);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            float x = 150.0F;
            float y = 50.0F;
            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            e.Graphics.DrawString(textBox3.Text, drawFont, drawBrush, x, y, drawFormat);



        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
           
            if (checkBox1.Checked == true && textBox3.Text != "")
            {
                
                int X, Y;
                string SehirAdi;
                int SehirNo;
                X = e.X;
                Y = e.Y;
                SehirAdi = textBox3.Text;
                
                BilgileriKaydet(SehirAdi, X, Y);

            }

        }
        //VT bilgileri kaydediyoruz.
        public void BilgileriKaydet(string SehirAdi, int X, int Y)
        {
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            MySqlCommand command = new MySqlCommand("INSERT INTO Konumlar(SehirAdi, X, Y) VALUES ('" +
           SehirAdi + "'," + X + "," + Y + ")", baglanti);
            MySqlDataReader reader = command.ExecuteReader();
            
            baglanti.Close();
            BilgileriOku();
        }

        public void BilgileriOku()
        {
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            //Sorgu (Emir Listesi)
           
            MySqlCommand command = new MySqlCommand("SELECT * FROM Konumlar", baglanti);

            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Sayfaya yükleniyor
            listBox1.Items.Clear();
            while (Okuyucu.Read())
            {
                listBox1.Items.Add(Okuyucu["ID"].ToString() + "," +
               Okuyucu["SehirAdi"].ToString() + "," + Okuyucu["X"].ToString() + "," +
               Okuyucu["Y"].ToString());
            }
            checkBox1.Checked = false;
            baglanti.Close();
            Okuyucu.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            //Veritabanından Diziye okutuyoruz.
            string[,] Sehirler = new string[100, 4];
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
                baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand command = new MySqlCommand("SELECT * FROM Konumlar", baglanti);
            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Diziye yükleniyor
            int i = 0;
            while (Okuyucu.Read())
            {

                Sehirler[i, 0] = Okuyucu["ID"].ToString();
                Sehirler[i, 1] = Okuyucu["SehirAdi"].ToString();
                Sehirler[i, 2] = Okuyucu["X"].ToString();
                Sehirler[i, 3] = Okuyucu["Y"].ToString();
                i = i + 1;
            }
            SehirSayisi = i;
            //Mesafe Hesaplama
            for (int j = 0; j < SehirSayisi; j++)
            {
                int ID1 = Convert.ToInt16(Sehirler[j, 0]);
                string Sehir1 = Sehirler[j, 1];
                int X1 = Convert.ToInt16(Sehirler[j, 2]);
                int Y1 = Convert.ToInt16(Sehirler[j, 3]);
                for (int k = 0; k < SehirSayisi; k++)
                {
                    int ID2 = Convert.ToInt16(Sehirler[k, 0]);
                    string Sehir2 = Sehirler[k, 1];
                    int X2 = Convert.ToInt16(Sehirler[k, 2]);
                    int Y2 = Convert.ToInt16(Sehirler[k, 3]);
                    Mesafe = Convert.ToInt16(Math.Sqrt((Y2 - Y1) * (Y2 - Y1) +
                   (X2 - X1) * (X2 - X1)));
                    if (Mesafe != 0 && Mesafe<200)
                    {
                        listBox2.Items.Add(Sehir1 + "," + Sehir2 + "=" +
                       Mesafe.ToString());
                        baglanti.Close();
                        baglanti.Open();
                        MySqlCommand commandD = new MySqlCommand("INSERT INTO mesafeler(IDsehir1, IDsehir2, Mesafe) VALUES(" + ID1 + ", " + ID2 + ", " + Mesafe + ")", baglanti);
                        commandD.ExecuteNonQuery();


                    }
                }
            }
            baglanti.Close();
            Okuyucu.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand command = new MySqlCommand("SELECT * FROM mesafeler", baglanti);
            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Sayfaya yükleniyor
            listBox2.Items.Clear();
            while (Okuyucu.Read())
            {
                listBox2.Items.Add(Okuyucu["IDsehir1"].ToString() + "," +
               Okuyucu["IDsehir2"].ToString() + "=" + Okuyucu["Mesafe"].ToString());
            }
            baglanti.Close();
            Okuyucu.Close();

        }
        //KROMOZOMLARI OLUŞTUR.

        private void button3_Click(object sender, EventArgs e)
        {
            //int BaslangicGenSayisi = 100;
            //string A ,B;
            string YeniKromozom = null;

            string EskiKromozom = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16";
            //Kaç tane ilk kromozom oluşturacaksa o kadar dönecek
            for (int i = 0; i < 25; i++)
            {
                YeniKromozom = YeniKromozomOlustur(EskiKromozom);
                listBox3.Items.Add(YeniKromozom + "=" + ToplamMesafe);
                EskiKromozom = YeniKromozom;
            }

        }

        public string YeniKromozomOlustur(string EskiKromozom)
        {
            string YeniDizi = null;
            int Mesafe = 0;
            string[] Dizi1 = EskiKromozom.Split(',');
            ArrayList Dizi2 = new ArrayList();
            foreach (string Eleman in Dizi1)
            {
                if (Eleman != "")
                {
                    Dizi2.Add(Eleman);
                }
            }
            int IDsehir1 = -1, IDsehir2 = -1; //sıfırlamalar
            ToplamMesafe = 0;
            do
            {
                try
                {
                    int RastgeleSayi = Rastgele.Next(0, Dizi2.Count);
                    YeniDizi = YeniDizi + Dizi2[RastgeleSayi] + ",";
                    IDsehir1 = Convert.ToInt32(Dizi2[RastgeleSayi]);
                    if (IDsehir2 != -1) //Ilk değer okunmazken.
                    {
                        Mesafe = VT_IkiSehirMesafesiniOku(IDsehir1, IDsehir2);
                     //   MessageBox.Show(Mesafe.ToString());
                        ToplamMesafe = ToplamMesafe + Mesafe;
                    }
                    IDsehir2 = IDsehir1;
                    Dizi2.RemoveAt(RastgeleSayi);
                }
                catch { }
            } while (Dizi2.Count > 0);
            return YeniDizi;
        }

        //VT İKİ ŞEHİR ARASINDAKİ MESAFEYİ OKUYOR
        public int VT_IkiSehirMesafesiniOku(int IDsehir1, int IDsehir2)
        {
            Mesafe = 0;
            // Bağlantı adresini tanımlama (Köprü kuruluyor)
            baglanti.Open();
            //Sorgu (Emir Listesi)
          
            MySqlCommand command = new MySqlCommand("SELECT Mesafe FROM mesafeler WHERE IDsehir1=" + IDsehir1 + " AND IDsehir2=" + IDsehir2 + "", baglanti);

            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Sayfaya yükleniyor
            while (Okuyucu.Read())
            {
                Mesafe = Convert.ToInt32(Okuyucu["Mesafe"]);
            }
            baglanti.Close();
            Okuyucu.Close();
      //     MessageBox.Show("İki Nokta arasındaki Mesafe"+Mesafe.ToString());
            return Mesafe;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Point point = pictureBox2.PointToClient(Cursor.Position);
            Bitmap bmp = new Bitmap(pictureBox2.Image);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawString(textBox1.Text, new Font("Verdava", 10,
            FontStyle.Regular), new SolidBrush(Color.Black), point.X, point.Y);
            pictureBox2.Image = bmp;
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked == true && textBox3.Text != "")
            { 
                MakePicturebox(e);
                

            }
            else
            {
                MessageBox.Show("Bilgileri doldurun ve Onay verin");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand command = new MySqlCommand("TRUNCATE TABLE konumlar", baglanti);
            //Okuyucu nesnesi (Kamyon)
            MySqlDataReader Okuyucu = command.ExecuteReader();
            //Bilgiler Diziye yükleniyor
            listBox1.Items.Clear();
            listBox2.Items.Clear();
           
            baglanti.Close();
            baglanti.Open();
            //Sorgu (Emir Listesi)
            MySqlCommand commandd = new MySqlCommand("TRUNCATE TABLE mesafeler", baglanti);
            //Okuyucu nesnesi (Kamyon)
            commandd.ExecuteReader();
            baglanti.Close();
            

        }

   

        private void button7_Click(object sender, EventArgs e)
        {

            
            gObject = pictureBox2.CreateGraphics();
            Brush red = new SolidBrush(Color.Red);
            Brush blue = new SolidBrush(Color.Blue);
            Pen redPen = new Pen(red, 4);
            Pen bluePen = new Pen(blue, 4);
            MessageBox.Show(items.Count().ToString());
            for (int i = 0; i < items.Count(); i++)
            {
                    for (int j = 0; j < items.Count(); j++)
                    {
                    int ikinoktaarası = 0;
                    ikinoktaarası = VT_IkiSehirMesafesiniOku((i + 1), (j + 1));
                    MessageBox.Show(i.ToString() +"  -  "+ j.ToString()+"=Mesafesi="+ikinoktaarası.ToString());
                            if(ikinoktaarası <200 && ikinoktaarası!=0)
                            {
                                gObject.DrawLine(redPen, points[i], points[j]);
                                ikinoktaarası = 0;
                            }
                            else
                            {
                            continue;
                            }
                        
                    }
            }
            
        }
        public void dijkstra(int hedef,int depo=1)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 2; i <= items.Count; i++)
            {
                dijkstra(i);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            VT_IkiSehirMesafesiniOku(1,2);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
           
        }
    }
}
