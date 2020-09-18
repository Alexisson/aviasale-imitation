using MetroFramework;
using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AviaSale
{
    public partial class LoginForm : MetroFramework.Forms.MetroForm 
    {
        static string connstr = vars.connectString;
        MySqlConnection conn = new MySqlConnection(connstr);
        string passwd, query, path = @"D:\OSPanel\Open Server x64.exe";
        bool priv;

        public LoginForm()
        {
            InitializeComponent();
            this.ControlBox = false;
            //TopMost = true;
            checkResult();
        }

        private void metroButton1_Click(object sender, EventArgs e) => userLogin();

        private void MetroButton2_Click(object sender, EventArgs e) => Application.Exit();

        private void Label1_Click(object sender, EventArgs e) => checkResult();

        private void userLogin()
        {
            Admin admin = new Admin();
            User user = new User();
            conn.Open();
            query = @"SELECT * FROM user WHERE login_user = '" + metroTextBox1.Text + "'";
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                passwd = reader.GetString(2);
                priv = reader.GetBoolean(3);
            }

            if (metroTextBox2.Text == passwd)
            {
                this.Hide();
                if (priv)
                    admin.Show();
                else
                    user.Show();
                conn.Close();
            }
            else
                MetroMessageBox.Show(this, "Логин и/или пароль неверны!", "Ошибка!");
            conn.Close();
        }
        private bool checkState()
        {
            try
            {
                conn.Open();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }
        private void checkResult()
        {
            if (checkState())
            {
                metroButton1.Enabled = true;
                UpdateServer.Visible = false;
            }
            else
            {
                if (File.Exists(path))
                    Process.Start(path);
                else
                    MetroMessageBox.Show(this,"Не удалось запустить сервер Open Server","Ошибка базы данных!");
                metroButton1.Enabled = false;
                UpdateServer.Visible = true;
            }
        }
    }
}
