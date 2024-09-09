using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KartotekaGabinet
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(List<string> Data)
        {
            InitializeComponent();

            button1.Text = "ZAPISZ ZMIANY";

            // Ustawienie wartości stringów w kontrolkach TextBox
            textBoxFirstName.Text = Data[0];
            textBoxName.Text = Data[1];
            textBoxPhone.Text = Data[2];
            textBoxPesel.Text = Data[3];
            textBoxAdress.Text = Data[4];
            textBoxNIP.Text = Data[5];
            textBoxBirthDate.Text = Data[6];
            textBoxComments.Text = Data[7];


        }
        // Właściwość przechowująca wprowadzoną wartość
        public string InputName { get; private set; }
        public string InputPhone { get; private set; }
        public string InputPesel { get; private set; }
        public string InputAdress { get; private set; }
        public string InputNip { get; private set; }
        public string InputBirthDate { get; private set; }
        public string InputComments { get; private set; }



        private void button1_Click_1(object sender, EventArgs e)
        {
            if (textBoxFirstName.Text == string.Empty || textBoxName.Text == string.Empty)
                MessageBox.Show("Proszę wpisać IMIĘ i NAZWISKO");
            else
            {
                // Przypisanie wartości z TextBox do właściwości InputText
                InputName = textBoxFirstName.Text.ToUpper() + " " + textBoxName.Text.ToUpper();
                InputPhone = textBoxPhone.Text.ToUpper();
                InputPesel = textBoxPesel.Text.ToUpper();
                InputAdress = textBoxAdress.Text.ToUpper();
                InputNip = textBoxNIP.Text.ToUpper();
                InputBirthDate = textBoxBirthDate.Text.ToUpper();
                InputComments = textBoxComments.Text.ToUpper();
                // Ustawienie wyniku dialogu na OK
                DialogResult = DialogResult.OK;
            }
        }
        // czyszczenie okienek
        private void button2_Click(object sender, EventArgs e)
        {
            textBoxFirstName.Text = string.Empty;
            textBoxName.Text = string.Empty;
            textBoxPhone.Text = string.Empty;
            textBoxPesel.Text = string.Empty;
            textBoxAdress.Text = string.Empty;
            textBoxNIP.Text = string.Empty;
            textBoxBirthDate.Text = string.Empty;
            textBoxComments.Text = string.Empty;
        }
    
    }
}
