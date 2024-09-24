using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PetrolTakip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-32Q9FH5;Initial Catalog=DbBenzin;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");

        void satislistele()
        {

            var labels = new[] { lblkursunsuz, lblmaxdiesel, lblprodiesel, lblotogaz };
            var progresbar = new[] { progressBar1, progressBar2, progressBar3, progressBar4 };
            var lblbar = new[] { lblbar1, lblbar2, lblbar3, lblbar4 };

            baglanti.Open();
            for (int i = 1; i <= 4; i++)
            {
                SqlCommand komut = new SqlCommand("select * from TBLBENZIN where ID=" + i, baglanti);
                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    labels[i - 1].Text = dr[3].ToString();
                    progresbar[i - 1].Value = Convert.ToInt32(dr[4]);
                    lblbar[i - 1].Text = dr[4] + " Litre";

                }
                dr.Close();
            }
            baglanti.Close();

            baglanti.Open();
            SqlCommand komut1 = new SqlCommand("select * from TBLKASA", baglanti);
            SqlDataReader dr1 = komut1.ExecuteReader();
            if (dr1.Read())
            {
                lblkasa.Text = dr1[0] + " TL";
            }

            baglanti.Close();

        }

        void alislistele()
        {
            var alislabels = new[] { lblaliskursunsuz, lblalismaxdiesel, lblalisprodiesel, lblalisotogaz };


            baglanti.Open();
            for (int i = 1; i <= 4; i++)
            {
                SqlCommand komut = new SqlCommand("select * from TBLBENZIN where ID=" + i, baglanti);
                SqlDataReader dr = komut.ExecuteReader();

                if (dr.Read())
                {
                    alislabels[i - 1].Text = dr[2].ToString();

                }
                dr.Close();
            }
            baglanti.Close();
        }

        void yakıtturu()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * From TBLBENZIN", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbyakıt.ValueMember = "ID";
            cmbyakıt.DisplayMember = "PETROLTUR";
            cmbyakıt.DataSource = dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            satislistele();
            yakıtturu();
            alislistele();
        }

        void yakitalis()
        {
            double alisfiyat = 0, litre, tutar;
            litre = Convert.ToDouble(txtyakıtdoldur.Text);
            
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select * from TBLBENZIN where PETROLTUR=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", cmbyakıt.Text);
            SqlDataReader dr = komut.ExecuteReader();
             while (dr.Read())
            {
                alisfiyat = Convert.ToDouble((dr[2])); 
            }
            baglanti.Close();

            tutar = litre * alisfiyat;

            baglanti.Open();
            SqlCommand komut1 = new SqlCommand("update TBLKASA set MIKTAR=MIKTAR-@p1", baglanti);
            komut1.Parameters.AddWithValue("@p1", tutar);
            komut1.ExecuteNonQuery();
            baglanti.Close();
            txtyakıtdoldur.Text = "";


        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            double kursunsuz95, litre, tutar;
            kursunsuz95 = Convert.ToDouble(lblkursunsuz.Text);
            litre = Convert.ToDouble(numericUpDown1.Value);
            tutar = kursunsuz95 * litre;
            txtkursunsuzfiyat.Text = tutar.ToString();

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            double maxdiesel, litre, tutar;
            maxdiesel = Convert.ToDouble(lblmaxdiesel.Text);
            litre = Convert.ToDouble(numericUpDown2.Value);
            tutar = maxdiesel * litre;
            txtmaxdiesel.Text = tutar.ToString();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            double prodiesel, litre, tutar;
            prodiesel = Convert.ToDouble(lblprodiesel.Text);
            litre = Convert.ToDouble(numericUpDown3.Value);
            tutar = prodiesel * litre;
            txtprodiesel.Text = tutar.ToString();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            double gaz, litre, tutar;
            gaz = Convert.ToDouble(lblotogaz.Text);
            litre = Convert.ToDouble(numericUpDown4.Value);
            tutar = gaz * litre;
            txtotogaz.Text = tutar.ToString();
        }

        void numeric1()
        {
            if (numericUpDown1.Value != 0)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into TBLHAREKET (PLAKA,BENZINTURU,LITRE,FIYAT)  values (@p1,@p2,@p3,@p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", txtplaka.Text);
                komut.Parameters.AddWithValue("@p2", "V/Max Kurşunsuz 95");
                komut.Parameters.AddWithValue("@p3", numericUpDown1.Value);
                komut.Parameters.AddWithValue("@p4", decimal.Parse(txtkursunsuzfiyat.Text));
                komut.ExecuteNonQuery();
                baglanti.Close();

                baglanti.Open();
                SqlCommand komut1 = new SqlCommand("update TBLKASA set MIKTAR=MIKTAR+@p1", baglanti);
                komut1.Parameters.AddWithValue("@p1", decimal.Parse(txtkursunsuzfiyat.Text));
                komut1.ExecuteNonQuery();
                baglanti.Close();
                txtkursunsuzfiyat.Text = "";

                baglanti.Open();
                SqlCommand komut2 = new SqlCommand("update TBLBENZIN set STOK=STOK-@p1 where PETROLTUR='V/Max Kurşunsuz 95'", baglanti);
                komut2.Parameters.AddWithValue("@p1", numericUpDown1.Value);
                komut2.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Satış Yapıldı.", "Bilgi");
                numericUpDown1.Value = 0;

            }
        }

        void numeric2()
        {
            if (numericUpDown2.Value != 0)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into TBLHAREKET (PLAKA,BENZINTURU,LITRE,FIYAT)  values (@p1,@p2,@p3,@p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", txtplaka.Text);
                komut.Parameters.AddWithValue("@p2", "V/Max Diesel");
                komut.Parameters.AddWithValue("@p3", numericUpDown2.Value);
                komut.Parameters.AddWithValue("@p4", decimal.Parse(txtmaxdiesel.Text));
                komut.ExecuteNonQuery();
                baglanti.Close();

                baglanti.Open();
                SqlCommand komut1 = new SqlCommand("update TBLKASA set MIKTAR=MIKTAR+@p1", baglanti);
                komut1.Parameters.AddWithValue("@p1", decimal.Parse(txtmaxdiesel.Text));
                komut1.ExecuteNonQuery();
                baglanti.Close();
                txtmaxdiesel.Text = "";

                baglanti.Open();
                SqlCommand komut2 = new SqlCommand("update TBLBENZIN set STOK=STOK-@p1 where PETROLTUR='V/Max Diesel'", baglanti);
                komut2.Parameters.AddWithValue("@p1", numericUpDown2.Value);
                komut2.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Satış Yapıldı.", "Bilgi");
                numericUpDown2.Value = 0;

            }
        }

        void numeric3()
        {
            if (numericUpDown3.Value != 0)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into TBLHAREKET (PLAKA,BENZINTURU,LITRE,FIYAT)  values (@p1,@p2,@p3,@p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", txtplaka.Text);
                komut.Parameters.AddWithValue("@p2", "V/Pro Diesel");
                komut.Parameters.AddWithValue("@p3", numericUpDown3.Value);
                komut.Parameters.AddWithValue("@p4", decimal.Parse(txtprodiesel.Text));
                komut.ExecuteNonQuery();
                baglanti.Close();

                baglanti.Open();
                SqlCommand komut1 = new SqlCommand("update TBLKASA set MIKTAR=MIKTAR+@p1", baglanti);
                komut1.Parameters.AddWithValue("@p1", decimal.Parse(txtprodiesel.Text));
                komut1.ExecuteNonQuery();
                baglanti.Close();
                txtprodiesel.Text = "";

                baglanti.Open();
                SqlCommand komut2 = new SqlCommand("update TBLBENZIN set STOK=STOK-@p1 where PETROLTUR='V/Pro Diesel'", baglanti);
                komut2.Parameters.AddWithValue("@p1", numericUpDown3.Value);
                komut2.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Satış Yapıldı.", "Bilgi");
                numericUpDown3.Value = 0;

            }
        }

        void numeric4()
        {
            if (numericUpDown4.Value != 0)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into TBLHAREKET (PLAKA,BENZINTURU,LITRE,FIYAT)  values (@p1,@p2,@p3,@p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", txtplaka.Text);
                komut.Parameters.AddWithValue("@p2", "PO/gaz Otogaz");
                komut.Parameters.AddWithValue("@p3", numericUpDown4.Value);
                komut.Parameters.AddWithValue("@p4", decimal.Parse(txtotogaz.Text));
                komut.ExecuteNonQuery();
                baglanti.Close();

                baglanti.Open();
                SqlCommand komut1 = new SqlCommand("update TBLKASA set MIKTAR=MIKTAR+@p1", baglanti);
                komut1.Parameters.AddWithValue("@p1", decimal.Parse(txtotogaz.Text));
                komut1.ExecuteNonQuery();
                baglanti.Close();
                txtotogaz.Text = "";

                baglanti.Open();
                SqlCommand komut2 = new SqlCommand("update TBLBENZIN set STOK=STOK-@p1 where PETROLTUR='PO/gaz Otogaz'", baglanti);
                komut2.Parameters.AddWithValue("@p1", numericUpDown4.Value);
                komut2.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Satış Yapıldı.", "Bilgi");
                numericUpDown4.Value = 0;

            }
        }





        private void btndepodoldur_Click(object sender, EventArgs e)
        {

            numeric1();
            numeric2();
            numeric3();
            numeric4();
            satislistele();
        }

        private void btnyakıtdoldur_Click(object sender, EventArgs e)
        {
            if (txtyakıtdoldur.Text == "")
            {
                MessageBox.Show("Lütfen yakıt miktarını giriniz", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {

                baglanti.Open();

                // Mevcut stok değerini al
                SqlCommand stokKomut = new SqlCommand("SELECT STOK FROM TBLBENZIN WHERE ID=@p1", baglanti);
                stokKomut.Parameters.AddWithValue("@p1", cmbyakıt.SelectedValue);

                decimal mevcutstok = (decimal)stokKomut.ExecuteScalar();

                // Yeni stok değerini hesapla
                decimal stogaeklenenyakit = decimal.Parse(txtyakıtdoldur.Text);
                decimal yeniStok = mevcutstok + stogaeklenenyakit;

                // Yeni stok değerinin 10,000'i geçip geçmediğini kontrol et
                if (yeniStok <= 10000)
                {
                    // Stok güncelleme
                    SqlCommand komut = new SqlCommand("UPDATE TBLBENZIN SET STOK=STOK+@p1 WHERE ID=@p2", baglanti);
                    komut.Parameters.AddWithValue("@p1", stogaeklenenyakit);
                    komut.Parameters.AddWithValue("@p2", cmbyakıt.SelectedValue);
                    komut.ExecuteNonQuery();

                    MessageBox.Show("Yakıt Eklendi");

                }
                else
                {
                    MessageBox.Show("Stok miktarınız 10.000 i geçemez");

                }
                baglanti.Close();

                


            }
            yakitalis();
            satislistele();
            txtyakıtdoldur.Text = "";
        }
    }
}

