using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Mosaic
{
    public partial class Form1 : Form
    {
        int count = 600000;
        private Button selected = null;
        Bitmap picture = null;
        private Bitmap btn7 = null;
        private Bitmap btn8 = null;
        private Bitmap btn9 = null;
        private Bitmap btn10 = null;

        private Button btnFromIndex(int index) // Получаем объект кнопки по id
        {
            switch (index)
            {
                case 0: return button10;
                case 1: return button8;
                case 2: return button7;
                case 3: return button9;
            }
            return null;
        }

        private void reload() // подготавливаем программу к новому запуску
        {
            button7.BackgroundImage = null;
            button8.BackgroundImage = null;
            button9.BackgroundImage = null;
            button10.BackgroundImage = null;

            button1.Visible = true;
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
            button7.Visible = false;
            button8.Visible = false;
            button9.Visible = false;
            button10.Visible = false;

            pictureBox2.BackgroundImage = null;

            timer1.Enabled = false;
            timer1.Stop();
            timer1.Tick -= new EventHandler(timer1_Tick);
            count = 600000;

            label1.Visible = false;
            pictureBox3.Visible = false;
            TLabel.Visible = false;
        }

        private void randomizeParts() // перемешиваем части мозайки
        {
            Random rand = new Random();
            Button tbtn1 = null, tbtn2 = null;
            Image timg = null;
            int i = 0;
            while (i < 6 || button7.BackgroundImage == btn7)
            {
                tbtn1 = btnFromIndex(rand.Next(0, 3));
                timg = tbtn1.BackgroundImage;
                tbtn2 = btnFromIndex(rand.Next(0, 3));
                tbtn1.BackgroundImage = tbtn2.BackgroundImage;
                tbtn2.BackgroundImage = timg;
                ++i;
            }
        }

        private bool checkFinish() // проверяем, верно ли собрана мозайка
        {
            if (button7.BackgroundImage == btn7 &&
                button8.BackgroundImage == btn8 &&
                button9.BackgroundImage == btn9 &&
                button10.BackgroundImage == btn10) return true;
            else return false;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // обработчик кнопки "загрузить изображение"
        {
            OpenFileDialog imDiag = new OpenFileDialog();
            try
            {
                if (imDiag.ShowDialog() == DialogResult.OK)
                {
                    picture = new Bitmap(imDiag.FileName);
                }
                pictureBox2.Image = new Bitmap(picture, new Size(pictureBox2.Width, pictureBox2.Height));
                //делим изображение на 4 части
                int w = picture.Width;
                int h = picture.Height;
                btn10 = new Bitmap(picture.Clone(new RectangleF(0, 0, w / 2, h / 2),
                        picture.PixelFormat), new Size(button7.Width, button7.Height));
                btn9 = new Bitmap(picture.Clone(new RectangleF(0, h / 2, w / 2, h / 2),
                        picture.PixelFormat), new Size(button7.Width, button7.Height));
                btn8 = new Bitmap(picture.Clone(new RectangleF(w / 2, 0, w / 2, h / 2),
                        picture.PixelFormat), new Size(button7.Width, button7.Height));
                btn7 = new Bitmap(picture.Clone(new RectangleF(w / 2, h / 2, w / 2, h / 2),
                        picture.PixelFormat), new Size(button7.Width, button7.Height));

                button1.Visible = false;
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
                button7.Visible = true;
                button8.Visible = true;
                button9.Visible = true;
                button10.Visible = true;

                button7.BackgroundImage = btn7;
                button8.BackgroundImage = btn8;
                button9.BackgroundImage = btn9;
                button10.BackgroundImage = btn10;

                randomizeParts();

                //активируем таймер
                timer1.Enabled = true;
                timer1.Start();
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = 1;
             
                label1.Visible = true;
                pictureBox3.Visible = true;
                TLabel.Visible = true;
            }
            catch
            {
                MessageBox.Show("Вы выбрали некорректный файл. Попробуйте ещё раз");
                reload();
            }
        }

        private void button_Click(object sender, EventArgs e) //обработчик нажатия на кнопку мозайки
        {
            Button b = (Button)sender;
            Image timg = null;
            if (selected == null)
                selected = b;
            else
            {
                timg = selected.BackgroundImage;
                selected.BackgroundImage = b.BackgroundImage;
                b.BackgroundImage = timg;
                selected = null;
                if (checkFinish())
                {
                    reload();
                    MessageBox.Show("Всё правильно, с победой!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e) // если прошло > 10 минут (проигрыш)
        {
            count -= 1000;
            if (count != 0 && count > 0)
            {
                TLabel.Text = count / 60000 + ":" + ((count % 60000) >= 10000 ? (count % 60000) : 0)/1000;
            }
            else
            {
                reload();
                MessageBox.Show("Вы проиграли");
            }
            timer1.Interval = 1000;

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_DragDrop(object sender, DragEventArgs e) // обработчик DragDrop (аналогичен обработчику кнопке 
                                                                    // "загрузить изображение"
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    picture = new Bitmap(Image.FromFile(files[0]));
                    pictureBox2.Image = new Bitmap(picture, new Size(pictureBox2.Width, pictureBox2.Height));
                    int w = picture.Width;
                    int h = picture.Height;
                    btn10 = new Bitmap(picture.Clone(new RectangleF(0, 0, w / 2, h / 2),
                            picture.PixelFormat), new Size(button7.Width, button7.Height));
                    btn9 = new Bitmap(picture.Clone(new RectangleF(0, h / 2, w / 2, h / 2),
                            picture.PixelFormat), new Size(button7.Width, button7.Height));
                    btn8 = new Bitmap(picture.Clone(new RectangleF(w / 2, 0, w / 2, h / 2),
                            picture.PixelFormat), new Size(button7.Width, button7.Height));
                    btn7 = new Bitmap(picture.Clone(new RectangleF(w / 2, h / 2, w / 2, h / 2),
                            picture.PixelFormat), new Size(button7.Width, button7.Height));

                    button1.Visible = false;
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = true;
                    button7.Visible = true;
                    button8.Visible = true;
                    button9.Visible = true;
                    button10.Visible = true;

                    button7.BackgroundImage = btn7;
                    button8.BackgroundImage = btn8;
                    button9.BackgroundImage = btn9;
                    button10.BackgroundImage = btn10;

                    randomizeParts();

                    timer1.Enabled = true;
                    timer1.Start();
                    timer1.Tick += new EventHandler(timer1_Tick);
                    timer1.Interval = 1;

                    label1.Visible = true;
                    pictureBox3.Visible = true;
                    TLabel.Visible = true;
                }
                catch
                {
                    MessageBox.Show("Вы выбрали некорректный файл. Попробуйте ещё раз");
                    reload();
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
