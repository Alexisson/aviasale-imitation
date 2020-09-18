using MetroFramework;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace AviaSale
{
    public partial class Admin : MetroFramework.Forms.MetroForm
    {
        int c, y;
        string query, t1, t2;
        static string connstr = vars.connectString;
        MySqlConnection conn = new MySqlConnection(connstr);

        public Admin()
        {
            InitializeComponent();
            //TopMost = true;
            filterModel();
            this.ControlBox = false;
            metroComboBox1.DropDownHeight = 500;
            metroComboBox2.DropDownHeight = 500;
            metroComboBox3.DropDownHeight = 500;
            metroComboBox4.DropDownHeight = 500;
            metroComboBox5.DropDownHeight = 500;
            metroComboBox6.DropDownHeight = 500;
            metroComboBox7.DropDownHeight = 200;
            metroComboBox8.DropDownHeight = 200;
            metroComboBox9.DropDownHeight = 200;
            metroComboBox10.DropDownHeight = 200;       
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            this.airportTableAdapter1.Fill(this.asaleDataSet.airport);
            this.aircraftTableAdapter1.Fill(this.asaleDataSet.aircraft);
            this.airlineTableAdapter1.Fill(this.asaleDataSet.airline);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (checkAirline())
                newAirline();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            if (checkAircrafAirline())
                newAirlineAircraft();
        }

        private void metroButton3_Click(object sender, EventArgs e) => newFlight();

        private void metroButton4_Click(object sender, EventArgs e)
        {
            if(checkAirport())
                newAirport();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            if(checkUser())
                newUser();
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm lf = new LoginForm();
            lf.Show();
        }

        private void metroComboBox4_SelectedIndexChanged(object sender, EventArgs e) => filterModel();

        private void metroTextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
                e.Handled = true;
        }

        private void metroTextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
                e.Handled = true;
        }

        private void metroTextBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
                e.Handled = true;
        }

        private void MetroTextBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
                e.Handled = true;
        }

        private void metroTextBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8)
                e.Handled = true;
        }

        private void newFlight()
        {
            if (checkForm())
            {
                query = "SELECT COUNT(*) FROM FLIGHT " +
                    "WHERE FLIGHT.AIRLINE_ID_AIRLINE = " + metroComboBox4.SelectedValue + " AND " +
                    "FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroTextBox7.Text) + "";
                conn.Open();
                MySqlCommand CH = new MySqlCommand(query, conn);
                if (Convert.ToInt32(CH.ExecuteScalar()) == 0)
                {
                    conn.Close();
                    conn.Open();
                    switch (metroTextBox6.Text)
                    {
                        case "":
                            query = "INSERT INTO FLIGHT(AIRLINE_ID_AIRLINE, FLIGHT_NUM, Y_PRICE) VALUES " +
                                    "(" + metroComboBox4.SelectedValue + "," + Convert.ToInt32(metroTextBox7.Text) + ", " + Convert.ToDouble(metroTextBox8.Text) + ");";
                            break;
                        default:
                            query = "INSERT INTO FLIGHT(AIRLINE_ID_AIRLINE, FLIGHT_NUM, C_PRICE, Y_PRICE) VALUES " +
                                    "(" + metroComboBox4.SelectedValue + "," + Convert.ToInt32(metroTextBox7.Text) + ", " + Convert.ToDouble(metroTextBox6.Text) + "," + Convert.ToDouble(metroTextBox8.Text) + ");";
                            break;
                    }

                    MySqlCommand command = new MySqlCommand(query, conn);
                    if (command.ExecuteNonQuery() == 1)
                    {
                        conn.Close();
                        arrDep();
                    }
                    else
                    {
                        MetroMessageBox.Show(this, "Не удалось создать рейс!", "Ошибка");
                        conn.Close();
                    }
                }
                else
                {
                    conn.Close();
                    arrDep();
                }
            } 
        }
        private void filterModel()
        {
            metroComboBox3.Items.Clear();
            conn.Open();
            query = "SELECT AIRCRAFT.MODEL " +
                    "FROM AIRLINE INNER JOIN(AIRCRAFT INNER JOIN AIRLINE_AIRCRAFT ON AIRCRAFT.ID_AIRCRAFT = AIRLINE_AIRCRAFT.AIRCRAFT_ID_AIRCRAFT) ON AIRLINE.ID_AIRLINE = AIRLINE_AIRCRAFT.AIRLINE_ID_AIRLINE " +
                    "WHERE AIRLINE_AIRCRAFT.AIRLINE_ID_AIRLINE = (SELECT ID_AIRLINE FROM AIRLINE WHERE AIRLINE = '" + metroComboBox4.Text + "');";
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                metroComboBox3.Items.Add(reader[0].ToString());
            }
            conn.Close();
        }
        private void newUser()
        {
            conn.Open();
            if (metroCheckBox1.Checked)
                query = @"INSERT INTO USER(LOGIN_USER, PASSWORD_USER, PRIV) VALUES ('" + metroTextBox13.Text + "','" + metroTextBox12.Text + "', true)";
            else
                query = @"INSERT INTO USER(LOGIN_USER, PASSWORD_USER, PRIV) VALUES ('" + metroTextBox13.Text + "','" + metroTextBox12.Text + "', false)";

            MySqlCommand command = new MySqlCommand(query, conn);
            if (command.ExecuteNonQuery() == 1)
            {
                MetroMessageBox.Show(this, "Данные добавлены успешно!", "Информация");
                metroTextBox13.Clear();
                metroTextBox12.Clear();
                metroCheckBox1.Checked = false;
                conn.Close();
            }
        }
        private void newAirport()
        {
            conn.Open();
            query = @"INSERT INTO AIRPORT(AIRPORT, CITY, IATA) VALUES ('" + metroTextBox10.Text + "','" + metroTextBox11.Text + "','" + metroTextBox9.Text + "')";
            MySqlCommand command = new MySqlCommand(query, conn);
            if (command.ExecuteNonQuery() == 1)
            {
                metroTextBox9.Clear();
                metroTextBox10.Clear();
                metroTextBox11.Clear();
                MetroMessageBox.Show(this, "Данные добавлены успешно!", "Информация");
                conn.Close();
            }
        }
        private void newAirlineAircraft()
        {
            conn.Open();
            query = @"SELECT COUNT(*) FROM AIRLINE_AIRCRAFT WHERE AIRLINE_ID_AIRLINE= " + metroComboBox1.SelectedValue + " AND " +
                    "AIRCRAFT_ID_AIRCRAFT = " + metroComboBox2.SelectedValue + "";
            MySqlCommand CH = new MySqlCommand(query, conn);
            if (Convert.ToInt32(CH.ExecuteScalar()) == 0)
            {
                conn.Close();
                if (metroTextBox4.Text == "")
                    c = 0;
                else
                    c = Convert.ToInt32(metroTextBox4.Text);
                if (metroTextBox5.Text == "")
                    y = 0;
                else
                    y = Convert.ToInt32(metroTextBox5.Text);
                conn.Open();
                query = @"INSERT INTO AIRLINE_AIRCRAFT(AIRLINE_ID_AIRLINE, AIRCRAFT_ID_AIRCRAFT, C_CLASS, Y_CLASS) VALUES 
                        (" + metroComboBox1.SelectedValue + "," + metroComboBox2.SelectedValue + "," + c + "," + y + ")";
                MySqlCommand command = new MySqlCommand(query, conn);
                if (command.ExecuteNonQuery() == 1)
                {
                    MetroMessageBox.Show(this, "Данные добавлены успешно!", "Информация");
                    metroTextBox4.Clear();
                    metroTextBox5.Clear();
                }
                else
                    MetroMessageBox.Show(this, "Ошибка добавления данных!", "Ошибка");
            }
            else
                MetroMessageBox.Show(this, "Данные уже существуют в базе данных!", "Внимание");
            conn.Close();
        }
        private void newAirline()
        {
            conn.Open();
            query = @"INSERT INTO AIRLINE(AIRLINE, ICAO, IATA) VALUES ('" + metroTextBox1.Text + "','" + metroTextBox2.Text + "','" + metroTextBox3.Text + "')";
            MySqlCommand command = new MySqlCommand(query, conn);
            if (command.ExecuteNonQuery() == 1)
            {
                MetroMessageBox.Show(this, "Данные добавлены успешно!", "Информация");
                metroTextBox1.Clear();
                metroTextBox2.Clear();
                metroTextBox3.Clear();
            }
            else
                MetroMessageBox.Show(this, "Ошибка добавления данных!", "Ошибка");
            conn.Close();
        }
        private void arrDep()
        {
            if (checkForm())
            {
                t1 = metroComboBox7.Text + ":" + metroComboBox8.Text;
                t2 = metroComboBox10.Text + ":" + metroComboBox9.Text;
                query = "SELECT COUNT(*) " +
                        "FROM FLIGHT " +
                        "WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroTextBox7.Text) + " AND " +
                        "FLIGHT.AIRLINE_ID_AIRLINE = " + metroComboBox4.SelectedValue + "";
                conn.Open();
                MySqlCommand check1 = new MySqlCommand(query, conn);
                if (Convert.ToInt32(check1.ExecuteScalar()) == 1)
                {
                    conn.Close();
                    conn.Open();
                    query = "INSERT INTO DEP_SCHD(AIRPORT_ID_AIRPORT,  SCHD_TIME, FLIGHT_ID_FLIGHT, AIRLINE_AIRCRAFT_ID_SET) VALUES " +
                           "(" + metroComboBox5.SelectedValue + ",'" + t1 + "', " +
                           "(SELECT FLIGHT.ID_FLIGHT " +
                           "FROM FLIGHT " +
                           "WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroTextBox7.Text) + " AND " +
                           "FLIGHT.AIRLINE_ID_AIRLINE = " + metroComboBox4.SelectedValue + ")," +
                           "(SELECT AIRLINE_AIRCRAFT.ID_SET FROM AIRLINE_AIRCRAFT INNER JOIN AIRCRAFT " +
                           "ON AIRLINE_AIRCRAFT.AIRCRAFT_ID_AIRCRAFT = AIRCRAFT.ID_AIRCRAFT " +
                           "WHERE AIRLINE_AIRCRAFT.AIRLINE_ID_AIRLINE = " + metroComboBox4.SelectedValue + " AND " +
                           "AIRLINE_AIRCRAFT.AIRCRAFT_ID_AIRCRAFT = (SELECT ID_AIRCRAFT FROM AIRCRAFT WHERE MODEL = '" + metroComboBox3.Text + "'))); " +
                           "INSERT INTO ARR_SCHD(AIRPORT_ID_AIRPORT,  SCHD_TIME, FLIGHT_ID_FLIGHT, AIRLINE_AIRCRAFT_ID_SET) VALUES " +
                           "(" + metroComboBox6.SelectedValue + ",'" + t2 + "', " +
                           "(SELECT FLIGHT.ID_FLIGHT " +
                           "FROM FLIGHT " +
                           "WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroTextBox7.Text) + " AND " +
                           "FLIGHT.AIRLINE_ID_AIRLINE = " + metroComboBox4.SelectedValue + ")," +
                           "(SELECT AIRLINE_AIRCRAFT.ID_SET FROM AIRLINE_AIRCRAFT INNER JOIN AIRCRAFT " +
                           "ON AIRLINE_AIRCRAFT.AIRCRAFT_ID_AIRCRAFT = AIRCRAFT.ID_AIRCRAFT " +
                           "WHERE AIRLINE_AIRCRAFT.AIRLINE_ID_AIRLINE = " + metroComboBox4.SelectedValue + " AND " +
                           "AIRLINE_AIRCRAFT.AIRCRAFT_ID_AIRCRAFT = (SELECT ID_AIRCRAFT FROM AIRCRAFT WHERE MODEL = '" + metroComboBox3.Text + "'))); ";
                    MySqlCommand cmd1 = new MySqlCommand(query, conn);
                    if (cmd1.ExecuteNonQuery() == 2)
                    {
                        MetroMessageBox.Show(this, "Данные добавлены успешно!", "Информация");
                        metroTextBox6.Clear();
                        metroTextBox7.Clear();
                        metroTextBox8.Clear();
                    }
                    else
                        MetroMessageBox.Show(this, "Ошибка добавления данных!", "Ошибка");
                    conn.Close();
                }
                else
                    MetroMessageBox.Show(this, "Авиарейс уже существует!");
                conn.Close();
            }          
        }
        private bool checkForm()
        {
            if (metroComboBox3.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите тип воздушного судна!");
                metroComboBox3.Focus();
                return false;
            }
            else if(metroComboBox5.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите пункт отправления!");
                metroComboBox5.Focus();
                return false;
            }
            else if (metroComboBox6.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите пункт прибытия!");
                metroComboBox6.Focus();
                return false;
            }
            else if (metroComboBox6.Text == metroComboBox5.Text)
            {
                MetroMessageBox.Show(this, "Пункт отправления не может быть пунктом прибытия!");
                return false;
            }
            else if (metroComboBox7.Text == "" && metroComboBox8.Text == "")
            {
                MetroMessageBox.Show(this, "Введите корректное время отправления!");
                return false;
            }
            else if (metroComboBox9.Text == "" && metroComboBox10.Text == "")
            {
                MetroMessageBox.Show(this, "Введите корректное время прибытия!");
                return false;
            }
            else if (metroTextBox8.Text == "")
            {
                MetroMessageBox.Show(this, "Введите цену эконом класса!");
                metroTextBox8.Focus();
                return false;
            }
            else
                return true;
        }
        private bool checkAirline()
        {
            if (metroTextBox1.Text == "")
            {
                MetroMessageBox.Show(this, "Введите название авиакомпании!");
                metroTextBox1.Focus();
                return false;
            }
            else if (metroTextBox2.Text == "")
            {
                MetroMessageBox.Show(this, "Введите код ICAO!");
                metroTextBox2.Focus();
                return false;
            }
            else if (metroTextBox3.Text == "")
            {
                MetroMessageBox.Show(this, "Введите код IATA!");
                metroTextBox3.Focus();
                return false;
            }
            else
                return true;
        }
        private bool checkAircrafAirline()
        {
            if (metroComboBox1.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите авиакомпанию!");
                metroComboBox1.Focus();
                return false;
            }
            else if (metroComboBox2.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите модель воздушного судна!");
                metroComboBox2.Focus();
                return false;
            }
            else if (metroTextBox5.Text == "")
            {
                MetroMessageBox.Show(this, "Введите количество мест в эконом классе!");
                metroTextBox5.Focus();
                return false;
            }
            else
                return true;
        }
        private bool checkAirport()
        {
            if (metroTextBox11.Text == "")
            {
                MetroMessageBox.Show(this, "Введите город!");
                metroTextBox11.Focus();
                return false;
            }
            else if (metroTextBox10.Text == "")
            {
                MetroMessageBox.Show(this, "Введите название аэропорта!");
                metroTextBox10.Focus();
                return false;
            }
            else if (metroTextBox9.Text == "")
            {
                MetroMessageBox.Show(this, "Введите код IATA!");
                metroTextBox9.Focus();
                return false;
            }
            else
                return true;
        }
        private bool checkUser()
        {
            if (metroTextBox13.Text == "")
            {
                MetroMessageBox.Show(this, "Введите логин!");
                metroTextBox13.Focus();
                return false;
            }
            else if (metroTextBox12.Text == "")
            {
                MetroMessageBox.Show(this, "Введите пароль");
                metroTextBox12.Focus();
                return false;
            } 
            else
                return true;
        }
    }
}
