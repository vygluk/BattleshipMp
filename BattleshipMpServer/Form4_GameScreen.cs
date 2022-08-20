using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace BattleshipMp
{
    public partial class Form4_GameScreen : Form
    {
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;
        List<Button> gameBoardButtons;
        List<Button> myBoardButtons;
        bool areEnabledButtons = true;
        List<string> AllSelectedButtonList;
        bool myExit = false;

        //  Oyun ekranı nesnesi oluşturulurken constructor yardımıyla Form2'den seçili butonların listesini al ve rengini değiştir.
        public Form4_GameScreen(List<string> list)
        {
            InitializeComponent();
            this.AllSelectedButtonList = list;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        //  Hamle yaparken mouse işaretçisini kırmızı bir hedef görseliyle değiştir. buton üzerinden çıktığında normal işaretçiye dön.
        private void button_mousehover(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(new Bitmap(Application.StartupPath + @"\Images\target.png"), 20, 20);
            ((Button)sender).Cursor = new Cursor(bm.GetHicon());
        }
        private void button_mouseleave(object sender, EventArgs e)
        {
            ((Button)sender).Cursor = Cursors.Default;
        }


        private void Form4_GameScreen_Load(object sender, EventArgs e)
        {
            // 1 // Hamle yapılan alandaki butonları ve benim seçtiğim gemileri gösteren butonların hepsini listede tut.
            gameBoardButtons = groupBox2.Controls.OfType<Button>().ToList();
            myBoardButtons = groupBox1.Controls.OfType<Button>().ToList();

            // 2 // Form2'den gelen buton listesine göre gemilerin yerlerini "Benim Gemilerim" bölümünde rengini değiştir.
            foreach (var item in AllSelectedButtonList)
            {
                groupBox1.Controls.Find(item, true)[0].BackColor = Color.DarkGray;
            }

            // 3 // Server - Client arasında veri alışverişini sağlayacak olan Stream'leri oluştur ve bunları global değişkenlere ata.
            // backgorundWorker1 nesnesi her zaman gelen veriyi dinleyeceği için arka planda sürekli çalışıyor durumda olacak.
            try
            {
                STR = new StreamReader(Server.client.GetStream());
                STW = new StreamWriter(Server.client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            // 4 // Oyuna ilk başlarken sıranın kimde olduğunu belirlemek için 0 veya 1 olacak şekilde random sayı üret. Oyun başlangıcında bunu karşı tarafa gönder.
            // Hamle sırası belirlendikten sonra hamle tahtasındaki butonları aktif veya pasif hale getir.
            Random random = new Random();
            int rnd = random.Next(0, 2);
            if (rnd == 0)
            {
                areEnabledButtons = true;
                SwitchGameButtonsEnabled();
            }
            else
            {
                areEnabledButtons = false;
                SwitchGameButtonsEnabled();
            }
            AttackToEnemy(rnd.ToString());

            timer1.Start();
        }

        // 5 // Gelen bilgileri bağlantı sağlandığı takdirde sürekli oku. Gelen bilgi boş değilse "AttackFromEnemy()" metodunu çalıştır.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Server.client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();

                    if (recieve != "")
                    {
                        AttackFromEnemy(recieve);
                    }
                    recieve = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        // 6 // Asıl işlemlerin yapıldığı metod burası. Gerekli açıklamalar method içerisinde. Hamle yapıldığında bu metoda 1 den fazla sayıda iterasyonla ulaşılır.
        // Yine açıklama kısmında belirtildi.
        private void AttackFromEnemy(string recieve)
        {
            //  Dördüncü adımda sıranın kimde olduğunu belirleyen veriyi oku. Veri 0'sa oyuna Server; Veri 1'se Client başlayacak.
            if (recieve == "0")
            {
                areEnabledButtons = true;
                SwitchGameButtonsEnabled();
                return;
            }
            else if (recieve == "1")
            {
                areEnabledButtons = false;
                SwitchGameButtonsEnabled();
                return;
            }

            //  Arayüzde bulunan RichTextBox nesnesine hamle yapan tarafın hedefi vurup vurmadığı logu yazılır.
            //  Eğer karavana ise "O" görseliyle butonun arka planı döşenir. Tam tersiyse "X" görseli kullanılır.
            //  Bu işlem 2. iterasyonda gerçekleşir.
            else if (recieve.Contains("karavana:"))
            {
                //  String işlemleri oyuncunun gemileri ve oyunun oynandığı tahta ayrı olduğu için alınan buton isimlerini düzenlemek için.
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                richTextBox1.AppendText("Karavana\n");
                return;
            }
            else if (recieve.Contains("isabet:"))
            {
                string result = recieve.Substring(recieve.Length - 2, 2);
                result = result + result.Substring(result.Length - 1);
                gameBoardButtons.FirstOrDefault(x => x.Name == result).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");
                richTextBox1.AppendText("İsabet\n");
                return;
            }

            // Tüm kutular vurulduğu takdirde bile oyunun bitmesi için Amiral gemisinin vurulmuş olmasına bakıldığı için; gelen veri "youwin"
            // ise, bu mesajı alan taraf oyunu kazanmış olur. Yeni oyun için hazırlık aşamasına geçip geçmek istemediği sorulur. Duruma göre işlem yapılır.
            // Bu okunan veri eğer oyun bitiyorsa 3. iterasyonda alınmış olur.
            else if (recieve.Contains("youwin"))
            {
                DialogResult res = MessageBox.Show("Kazandın. Hazırlık ekranına dönmek ister misin?", "Server - Oyun Sonucu", MessageBoxButtons.YesNo);
                {
                    if (res == DialogResult.Yes)
                    {
                        myExit = true;
                        this.Close();
                    }
                    else
                        Environment.Exit(1);
                }
                return;
            }
            else if (recieve.Contains("exit"))
            {
                MessageBox.Show("Rakip oyunu terketti. Hazırlık aşamasına yönlendirileceksiniz.");
                this.Close();
            }

            //  Hamlenin sonucu için tutulan değişkenler.
            bool isShot = false;
            string shotButtonName = "";
            string shottedShip = "";
            ShipButtons deletingButton = null;

            //  Yukarıdaki if koşulları tutmadığı takdirde metod bunu hamle yapıldı olarak algılar.
            //  Karşıdan yapılan hamle buton ismiyle birlikte gelir. Daha sonra "Ship" modelindeki yapıya göre gemiye ait butonlar arasında gezinir.
            //  Yapılan hamlenin hedefi vurup vurmadığına göre üstteki değişkenlere değer atar.
            foreach (var item1 in Form2_PreparatoryScreen.shipList)
            {
                foreach (var item2 in item1.shipPerButton)
                {
                    foreach (string item3 in item2.buttonNames)
                    {
                        if (item3 == recieve.Substring(0, recieve.Length - 1))
                        {

                            isShot = true;
                            shotButtonName = item3;
                            shottedShip = item1.shipName;
                            deletingButton = item2;
                            break;
                        }
                        else
                        {
                            string xxx = recieve.Substring(0, recieve.Length - 1);
                            shotButtonName = xxx;
                        }
                    }
                }
            }


            //  Eğer hedef vurulmuşsa Kendi hedef tahtasındaki butonu işaretler. Sonrasında karşıya isabet verisi yollar. Yine aynı AttackFromEnemy() metodu
            //  gelen isabet ya da karavana verisine göre göre işlem yapar. + if(recieve.Contain(karavana)) statement.
            //  Vurulan gemiye ait butonu listeden siler.
            if (isShot)
            {
                myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\x.png");

                AttackToEnemy("isabet:" + shotButtonName);

                deletingButton.buttonNames.Remove(shotButtonName);

                //  Vurulan gemi Amiral ise Amiral'e ait tüm butonlar kontrol edilir. Eğer hepsi vurulmuşsa oyun sona erer.
                if (shottedShip == "Amiral")
                {
                    foreach (var item in Form2_PreparatoryScreen.shipList.FirstOrDefault(x => x.shipName == "Amiral").shipPerButton)
                    {
                        if (item.buttonNames.Count > 0)
                        {
                            return;
                        }
                        AttackToEnemy("youwin");
                        DialogResult res = MessageBox.Show("Kaybettin. Hazırlık ekranına dönmek ister misin?", "Server - Oyun Sonucu", MessageBoxButtons.YesNo);
                        {
                            if (res == DialogResult.Yes)
                            {
                                myExit = true;
                                this.Close();
                            }
                            else
                                Environment.Exit(1);
                        }
                        return;
                    }
                }
                return;
            }

            //  Hedef vurulmamışsa sadece ilgili butonun arka planı değiştirilir ve 2. iterasyonda yine bu kısmın okuması için karşıya veri gönderilir.
            else
            {
                myBoardButtons.FirstOrDefault(x => x.Name == shotButtonName).BackgroundImage = Image.FromFile(Application.StartupPath + @"\Images\o.png");
                AttackToEnemy("karavana:" + shotButtonName);
                return;
            }
        }

        //  Veriyi gönderecek olan nesne. Sadece hamle yapıldığı zaman çalıştırılır. Onun dışında beklemede kalır.
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Server.client.Connected)
            {
                STW.WriteLine(TextToSend);
            }
            else
            {
                MessageBox.Show("Mesaj Gönderilemedi!!");
            }

            backgroundWorker2.CancelAsync();
        }

        //  Hamlelerin yapılacağı tüm butonların Click eventleri bu metodla ilişkilidir. Tıklandığında karşıya buton ismini gönderir.
        private void button_click(object sender, EventArgs e)
        {
            AttackToEnemy(((Button)sender).Name);
        }

        //  Veri gönderilirken buton isminde düzenleme yapar. A11 şeklinde buton ismini A1 olarak gönderir.
        private void AttackToEnemy(string buttonName)
        {
            if (buttonName == null)
            {
                return;
            }

            else if (buttonName.Length == 3)
            {
                TextToSend = buttonName.Substring(0, 2);
            }

            TextToSend = buttonName;
            backgroundWorker2.RunWorkerAsync();

            SwitchGameButtonsEnabled();
        }

        //  Hamle sırasına göre Hamle tahtasındaki butonları aktif veya pasif yapacak metod.
        private void SwitchGameButtonsEnabled()
        {
            if (areEnabledButtons == true)
            {
                foreach (var item in gameBoardButtons)
                {
                    item.Enabled = false;
                }
                labelAttackTurn.Text = "BEKLE...";
                areEnabledButtons = false;
            }
            else
            {
                foreach (var item in gameBoardButtons)
                {
                    item.Enabled = true;
                }
                labelAttackTurn.Text = "HAMLE YAP";
                areEnabledButtons = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Server.listener != null && (Server.client == null && !Server.client.Connected))
            {
                MessageBox.Show("Client bağlantısı koptu.");
            }
        }

        //  Form kapatılırken. Oyun bittiği için mi? Yoksa oyun devam ederken mi kapanıyor bunu kontrol et. Eğer devam sırasında ise karşıya oyuncunun
        //  oyunu terk ettiği bilgisini gönder.
        private void Form4_GameScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!myExit)
            {
                AttackToEnemy("exit");
            }
            Form2_PreparatoryScreen frm2 = new Form2_PreparatoryScreen();
            frm2.Show();
        }
    }
}
