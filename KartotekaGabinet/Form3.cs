using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KartotekaGabinet
{
    public partial class Form3 : Form
    {
        public Form3(string name)
        {
            InitializeComponent();
            // Uzyskanie dzisiejszej daty
            DateTime today = DateTime.Today;

            // Konwersja daty na string
            textBoxDate.Text = today.ToString("yyyy-MM-dd");
            labelName.Text = name;
        }

        public Form3(List<string> Data, string name)
        {
            InitializeComponent();

            button1.Text = "ZAPISZ ZMIANY";

            // Ustawienie wartości stringów w kontrolkach TextBox
            textBoxDate.Text = Data[0];
            textBoxSurface.Text = Data[1];
            textBoxMaterial.Text = Data[3];
            textBoxComments.Text = Data[2];

            labelName.Text = name;
            button1.Text = "ZAPISZ ZMIANY";
        }
        // Właściwość przechowująca wprowadzoną wartość
        public string InputDate { get; private set; }
        public string InputSurface { get; private set; }
        public string InputComment { get; private set; }
        public string InputMaterial { get; private set; }
       


        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxDate.Text == string.Empty || textBoxSurface.Text == string.Empty || textBoxComments.Text == string.Empty)
                MessageBox.Show("Prosze uzupełnić pola wymagane!");
            else
            {
                // Przypisanie wartości z TextBox do właściwości InputText
                InputDate = textBoxDate.Text;
                InputSurface = textBoxSurface.Text;
                InputComment = textBoxComments.Text;
                InputMaterial = textBoxMaterial.Text;            
                // Ustawienie wyniku dialogu na OK
                DialogResult = DialogResult.OK;
            }

        }

       
        #region AutomaticComments
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxComments.Text == string.Empty)
                textBoxComments.Text = textBoxComments.Text + "WIZYTA KONTROLNA";
            else
            textBoxComments.Text = textBoxComments.Text + "; WIZYTA KONTROLNA";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBoxComments.Text == string.Empty)
                textBoxComments.Text = textBoxComments.Text + "LECZENIE ZACHOWAWCZE ZĘBA";
            else
                textBoxComments.Text = textBoxComments.Text + "; LECZENIE ZACHOWAWCZE ZĘBA";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBoxComments.Text == string.Empty)
                textBoxComments.Text = textBoxComments.Text + "LECZENIE ENDODONTYCZNE ZĘBA";
            else
                textBoxComments.Text = textBoxComments.Text + "; LECZENIE ENDODONTYCZNE ZĘBA";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBoxComments.Text == string.Empty)
                textBoxComments.Text = textBoxComments.Text + "HIGIENIZACJA I PROFILAKTYKA ZĘBÓW I JAMY USTNEJ";
            else
                textBoxComments.Text = textBoxComments.Text + "; HIGIENIZACJA I PROFILAKTYKA ZĘBÓW I JAMY USTNEJ";
        }


        private void buttonA5_Click(object sender, EventArgs e)
        {
            if (textBoxComments.Text == string.Empty)
                textBoxComments.Text = textBoxComments.Text + "LECZENIE CHIRURGICZNE ZĘBA";
            else
                textBoxComments.Text = textBoxComments.Text + "; LECZENIE CHIRURGICZNE ZĘBA";
        }

        private void buttonA6_Click(object sender, EventArgs e)
        {
            if (textBoxComments.Text == string.Empty)
                textBoxComments.Text = textBoxComments.Text + "LECZENIE PEDODONTYCZNE ZĘBÓW MLECZNYCH";
            else
                textBoxComments.Text = textBoxComments.Text + "; LECZENIE PEDODONTYCZNE ZĘBÓW MLECZNYCH";
        }

        #endregion

        //czyszczenie okna komentarzy
        private void buttonClearComment_Click(object sender, EventArgs e)
        {
            textBoxComments.Text = string.Empty;
        }
    }
}
