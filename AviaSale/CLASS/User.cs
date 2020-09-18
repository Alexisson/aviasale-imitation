using MetroFramework;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows.Forms;
namespace AviaSale
{
    public partial class User : MetroFramework.Forms.MetroForm
    {
        int freeSeat, usedSeat, availableSeat;
        string query, aircraft, time, minutes, doc, pax;
        bool isActive = false;
        static string connstr = vars.connectString;
        MySqlConnection conn = new MySqlConnection(connstr);

        public User()
        {
            InitializeComponent();
            //TopMost = true;
            checkState();
            clearFilter();
            cityFilter();
            MainReport();
            this.StyleManager = metroStyleManager1;
            this.ControlBox = false;
            metroTabPage4.Parent = null;
            metroTabPage3.Parent = null;
            metroDateTime1.MinDate = DateTime.Now;
            metroDateTime2.MaxDate = DateTime.Now;
            metroDateTime3.MinDate = DateTime.Now;
            metroDateTime4.MaxDate = metroDateTime5.Value;
            metroDateTime5.MinDate = metroDateTime4.Value;
            metroComboBox5.DropDownHeight = 500;
            metroComboBox6.DropDownHeight = 500;
        }

        private void metroGrid1_DoubleClick(object sender, EventArgs e)
        {
            if (isActive)
            {
                metroTabPage3.Parent = metroTabControl2;
                metroTabPage2.Parent = null;
                metroTabControl2.SelectedTab = metroTabPage3;
            }

        }

        private void metroGrid1_Click(object sender, EventArgs e) => checkSeat();

        private void metroComboBox5_SelectedIndexChanged(object sender, EventArgs e) => filterAirport();

        private void User_Load(object sender, EventArgs e) => this.type_docTableAdapter1.Fill(this.asaleDataSet.type_doc);

        private void metroButton1_Click(object sender, EventArgs e)
        {
            metroLabel5.Visible = false;
            clearFilter();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            metroTextBox1.Clear();
            metroTextBox2.Clear();
            metroTextBox5.Clear();
            metroTextBox8.Clear();
            metroTextBox9.Clear();
            metroComboBox2.Text = "";
            metroComboBox3.Text = "";
            metroTabPage2.Parent = metroTabControl2;
            metroTabPage3.Parent = null;
            metroTabControl2.SelectedTab = metroTabPage2;
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            if (checkPaxInfo())
            {
                paxInfo();
                flightInfo();
                metroTabPage4.Parent = metroTabControl2;
                metroTabPage3.Parent = null;
                metroTabControl2.SelectedTab = metroTabPage4;
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            metroTabPage3.Parent = metroTabControl2;
            metroTabPage4.Parent = null;
            metroTabControl2.SelectedTab = metroTabPage3;
        }

        private void metroButton5_Click(object sender, EventArgs e) => purchaseTicket();

        private void metroButton6_Click(object sender, EventArgs e)
        {
            if (checkFlightFilter())
                searchFlight();
        }

        private void metroButton7_Click(object sender, EventArgs e) => newReport();

        private void metroButton8_Click(object sender, EventArgs e) => MainReport();

        private void MetroButton9_Click(object sender, EventArgs e)
        {
            LoginForm lf = new LoginForm();
            this.Close();
            lf.Show();
        }

        private void MetroTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') || (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)Keys.Back || e.KeyChar == '-' || e.KeyChar == '\'')
            {

            }
            else
                e.Handled = true;
        }

        private void MetroTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') || (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)Keys.Back || e.KeyChar == '-' || e.KeyChar == '\'')
            {

            }
            else
                e.Handled = true;
        }

        private void metroTextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsLetterOrDigit(number) && number != 8)
                e.Handled = true;
        }

        private void metroTextBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') || (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == '_' || e.KeyChar == (char)Keys.Back || e.KeyChar == '-' || e.KeyChar == '.' || e.KeyChar == '@')
            {

            }
            else
                e.Handled = true;
        }

        private void MetroTextBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8 && number != 43)
                e.Handled = true;
        }

        private void metroDateTime4_ValueChanged(object sender, EventArgs e) => metroDateTime5.MinDate = metroDateTime4.Value;

        private void metroDateTime5_ValueChanged(object sender, EventArgs e) => metroDateTime4.MaxDate = metroDateTime5.Value;

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            if (metroToggle1.Checked)
                metroStyleManager1.Theme = MetroThemeStyle.Dark;
            else
                metroStyleManager1.Theme = MetroThemeStyle.Light;
        }
        
        private void clearFilter()
        {
            checkState();
            isActive = false;

            DataTable dt = new DataTable();
            query = "SELECT DEP.AIRLINE 'Авиакомпания', DEP.IATA 'IATA', DEP.FLIGHT_NUM '№ Рейса', TIME_FORMAT(DEP.SCHD_TIME, \"%H:%i\") 'Время отправления', " +
                    "DEP.DEP_CITY 'Город отправления', DEP.DEP_AP 'Аэропорт отправления', " +
                    "TIME_FORMAT(ARR.SCHD_TIME, \"%H:%i\") 'Время прибытия', ARR.ARR_CITY 'Город прибытия', ARR.ARR_AP 'Аэропорт прибытия'  " +
                    "FROM DEP INNER JOIN ARR ON DEP.FLIGHT_NUM = ARR.FLIGHT_NUM ORDER BY DEP.SCHD_TIME";
            conn.Open();
            MySqlCommand SCHD = new MySqlCommand(query, conn);
            MySqlDataReader reader = SCHD.ExecuteReader();
            dt.Load(reader);
            metroGrid1.DataSource = dt;
            disableSortGrid1();
            conn.Close();
        }
        private void paxInfo()
        {
            metroLabel26.Text = "DATE OF BIRTH: " + metroDateTime2.Value.ToString("yyyy-MM-dd");
            metroLabel27.Text = "PHONE NUMBER: " + metroTextBox9.Text;
            metroLabel28.Text = "E-MAIL: " + metroTextBox8.Text;
            switch (metroComboBox3.SelectedIndex)
            {
                case 0:
                    metroLabel8.Text = "MR." + metroTextBox2.Text.ToUpper() + " " + metroTextBox1.Text.ToUpper();
                    break;
                case 1:
                    metroLabel8.Text = "MS." + metroTextBox2.Text.ToUpper() + " " + metroTextBox1.Text.ToUpper();
                    break;
            }
            metroLabel9.Text = "DATE OF BIRTH: " + metroDateTime2.Value.ToString("yyyy-MM-dd");
            switch (metroComboBox2.SelectedIndex)
            {
                case 0:
                    metroLabel9.Text = "DOCUMENT: U" + metroTextBox5.Text;
                    break;
                case 1:
                    metroLabel9.Text = "DOCUMENT: P" + metroTextBox5.Text;
                    break;
                case 2:
                    metroLabel9.Text = "DOCUMENT: B" + metroTextBox5.Text;
                    break;
                case 3:
                    metroLabel9.Text = "DOCUMENT: O" + metroTextBox5.Text;
                    break;
            }
        }
        private void flightInfo()
        {
            checkState();
            conn.Open();
            query = @"SELECT AIRCRAFT.MANUFACTURER, AIRCRAFT.MODEL FROM AIRCRAFT
	                    INNER JOIN AIRLINE_AIRCRAFT 
                        ON AIRCRAFT.ID_AIRCRAFT = AIRLINE_AIRCRAFT.AIRCRAFT_ID_AIRCRAFT
                        INNER JOIN AIRLINE
                        ON AIRLINE_AIRCRAFT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE
                        INNER JOIN ARR_SCHD
                        ON AIRLINE_AIRCRAFT.ID_SET = ARR_SCHD.AIRLINE_AIRCRAFT_ID_SET
                        INNER JOIN AIRPORT 
                        ON ARR_SCHD.AIRPORT_ID_AIRPORT = AIRPORT.ID_AIRPORT
                        INNER JOIN FLIGHT
                        ON ARR_SCHD.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT
                        WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "' ";
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                aircraft = reader.GetString(0) + " " + reader.GetString(1);
            }
            conn.Close();
            DateTime t = DateTime.ParseExact(metroGrid1.CurrentRow.Cells[6].Value.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            DateTime t2 = DateTime.ParseExact(metroGrid1.CurrentRow.Cells[3].Value.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            time = getFlightTime(t, t2);
            metroLabel11.Text = "Авиакомпания: " + metroGrid1.CurrentRow.Cells[0].Value.ToString();
            metroLabel12.Text = "Рейс: " + metroGrid1.CurrentRow.Cells[1].Value.ToString() + metroGrid1.CurrentRow.Cells[2].Value.ToString();
            metroLabel13.Text = "Воздушное судно: " + aircraft;
            metroLabel14.Text = "Пункт вылета: " + metroGrid1.CurrentRow.Cells[4].Value.ToString() + " (" + metroGrid1.CurrentRow.Cells[5].Value.ToString() + ")";
            metroLabel15.Text = "Время вылета: " + metroGrid1.CurrentRow.Cells[3].Value.ToString();
            metroLabel16.Text = "Пункт прилета: " + metroGrid1.CurrentRow.Cells[7].Value.ToString() + " (" + metroGrid1.CurrentRow.Cells[8].Value.ToString() + ")";
            metroLabel17.Text = "Время прилета: " + metroGrid1.CurrentRow.Cells[6].Value.ToString();
            metroLabel19.Text = "Время полета: " + time;
            metroLabel20.Text = "Класс обслуживания: " + metroComboBox1.Text;
            metroLabel21.Text = "Цена: " + metroGrid1.CurrentRow.Cells[9].Value.ToString() + " тг.";
        }
        private void searchFlight()
        {
            checkState();
            if (checkSearchFlight())
            {
                isActive = true;
                if (metroDateTime1.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    switch (metroComboBox1.SelectedIndex)
                    {
                        case 0:
                            query = "SELECT  DEP_ECON.AIRLINE 'Авиакомпания', DEP_ECON.IATA 'IATA', DEP_ECON.FLIGHT_NUM '№ Рейса', " +
                                    "TIME_FORMAT(DEP_ECON.SCHD_TIME, \"%H:%i\") 'Время отправления', DEP_ECON.DEP_CITY 'Город отправления', DEP_ECON.DEP_AP 'Аэропорт отправления', " +
                                    "TIME_FORMAT(ARR.SCHD_TIME, \"%H:%i\") 'Время прибытия', ARR.ARR_CITY 'Город прибытия', ARR.ARR_AP 'Аэропорт прибытия', DEP_ECON.Y_PRICE 'Цена билета' " +
                                    "FROM DEP_ECON INNER JOIN ARR ON DEP_ECON.FLIGHT_NUM = ARR.FLIGHT_NUM " +
                                    "WHERE ARR.ARR_CITY = '" + metroComboBox6.Text + "' AND " +
                                    "DEP_ECON.DEP_CITY = '" + metroComboBox5.Text + "' AND " +
                                    "TIME_FORMAT(DEP_ECON.SCHD_TIME, \"%H:%i\") > '" + DateTime.Now.AddHours(3).ToShortTimeString() + "' " +
                                    "ORDER BY DEP_ECON.SCHD_TIME";
                            break;
                        case 1:
                            query = "SELECT  DEP_BUSINESS.AIRLINE 'Авиакомпания', DEP_BUSINESS.IATA 'IATA', DEP_BUSINESS.FLIGHT_NUM '№ Рейса', " +
                                    "TIME_FORMAT(DEP_BUSINESS.SCHD_TIME, \"%H:%i\") 'Время отправления', DEP_BUSINESS.DEP_CITY 'Город отправления', DEP_BUSINESS.DEP_AP 'Аэропорт отправления', " +
                                    "TIME_FORMAT(ARR.SCHD_TIME, \"%H:%i\") 'Время прибытия', ARR.ARR_CITY 'Город прибытия', ARR.ARR_AP 'Аэропорт прибытия', DEP_BUSINESS.C_PRICE 'Цена билета' " +
                                    "FROM DEP_BUSINESS INNER JOIN ARR ON DEP_BUSINESS.FLIGHT_NUM = ARR.FLIGHT_NUM " +
                                    "WHERE ARR.ARR_CITY = '" + metroComboBox6.Text + "' AND " +
                                    "DEP_BUSINESS.DEP_CITY = '" + metroComboBox5.Text + "' AND " +
                                    "DEP_BUSINESS.C_PRICE IS NOT NULL AND " +
                                    "TIME_FORMAT(DEP_BUSINESS.SCHD_TIME, \"%H:%i\") > '" + DateTime.Now.AddHours(3).ToShortTimeString() + "' " +
                                    "ORDER BY DEP_BUSINESS.SCHD_TIME";
                            break;
                    }
                }
                else
                {
                    switch (metroComboBox1.SelectedIndex)
                    {
                        case 0:
                            query = "SELECT  DEP_ECON.AIRLINE 'Авиакомпания', DEP_ECON.IATA 'IATA', DEP_ECON.FLIGHT_NUM '№ Рейса', " +
                                    "TIME_FORMAT(DEP_ECON.SCHD_TIME, \"%H:%i\") 'Время отправления', DEP_ECON.DEP_CITY 'Город отправления', DEP_ECON.DEP_AP 'Аэропорт отправления', " +
                                    "TIME_FORMAT(ARR.SCHD_TIME, \"%H:%i\") 'Время прибытия', ARR.ARR_CITY 'Город прибытия', ARR.ARR_AP 'Аэропорт прибытия', DEP_ECON.Y_PRICE 'Цена билета' " +
                                    "FROM DEP_ECON INNER JOIN ARR ON DEP_ECON.FLIGHT_NUM = ARR.FLIGHT_NUM " +
                                    "WHERE ARR.ARR_CITY = '" + metroComboBox6.Text + "' AND " +
                                    "DEP_ECON.DEP_CITY = '" + metroComboBox5.Text + "' " +
                                    "ORDER BY DEP_ECON.SCHD_TIME";
                            break;
                        case 1:
                            query = "SELECT  DEP_BUSINESS.AIRLINE 'Авиакомпания', DEP_BUSINESS.IATA 'IATA', DEP_BUSINESS.FLIGHT_NUM '№ Рейса', " +
                                    "TIME_FORMAT(DEP_BUSINESS.SCHD_TIME, \"%H:%i\") 'Время отправления', DEP_BUSINESS.DEP_CITY 'Город отправления', DEP_BUSINESS.DEP_AP 'Аэропорт отправления', " +
                                    "TIME_FORMAT(ARR.SCHD_TIME, \"%H:%i\") 'Время прибытия', ARR.ARR_CITY 'Город прибытия', ARR.ARR_AP 'Аэропорт прибытия', DEP_BUSINESS.C_PRICE 'Цена билета' " +
                                    "FROM DEP_BUSINESS INNER JOIN ARR ON DEP_BUSINESS.FLIGHT_NUM = ARR.FLIGHT_NUM " +
                                    "WHERE ARR.ARR_CITY = '" + metroComboBox6.Text + "' AND " +
                                    "DEP_BUSINESS.DEP_CITY = '" + metroComboBox5.Text + "' AND " +
                                    "DEP_BUSINESS.C_PRICE IS NOT NULL " +
                                    "ORDER BY DEP_BUSINESS.SCHD_TIME";
                            break;
                    }
                }
                conn.Open();
                DataTable dt = new DataTable();
                MySqlCommand SCHD = new MySqlCommand(query, conn);
                MySqlDataReader reader = SCHD.ExecuteReader();
                dt.Load(reader);
                metroGrid1.DataSource = dt;
                conn.Close();
                disableSortGrid1();
                setPrices();
            }
        }
        private void checkSeat()
        {
            checkState();
            if (isActive)
            {
                conn.Open();
                query = "SELECT COUNT(*) FROM PURCHASE_LOG " +
                        "INNER JOIN FLIGHT ON PURCHASE_LOG.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                        "INNER JOIN AIRLINE ON FLIGHT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE " +
                        "INNER JOIN CUSTOMER ON PURCHASE_LOG.CUSTOMER_ID_CUSTOMER = CUSTOMER.ID_CUSTOMER " +
                        "INNER JOIN TICKET ON CUSTOMER.ID_CUSTOMER = TICKET.CUSTOMER_ID_CUSTOMER " +
                        "WHERE PURCHASE_LOG.FLIGHT_CLASS = '" + metroComboBox1.Text + "' AND " +
                        "FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "' AND " +
                        "TICKET.FLIGHT_DATE = '" + metroDateTime1.Value.ToString("yyyy-MM-dd") + "'";
                MySqlCommand check1 = new MySqlCommand(query, conn);
                usedSeat = Convert.ToInt32(check1.ExecuteScalar());

                conn.Close();
                conn.Open();
                switch (metroComboBox1.SelectedIndex)
                {
                    case 0:
                        query = @"SELECT AIRLINE_AIRCRAFT.Y_CLASS FROM AIRLINE_AIRCRAFT
                                INNER JOIN AIRLINE
                                ON AIRLINE_AIRCRAFT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE
                                INNER JOIN ARR_SCHD
                                ON AIRLINE_AIRCRAFT.ID_SET = ARR_SCHD.AIRLINE_AIRCRAFT_ID_SET
                                INNER JOIN AIRPORT
                                ON ARR_SCHD.AIRPORT_ID_AIRPORT = AIRPORT.ID_AIRPORT
                                INNER JOIN FLIGHT
                                ON ARR_SCHD.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                                "WHERE AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "' AND " +
                                "FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + "";
                        break;
                    case 1:
                        query = @"SELECT AIRLINE_AIRCRAFT.C_CLASS FROM AIRLINE_AIRCRAFT
                                INNER JOIN AIRLINE
                                ON AIRLINE_AIRCRAFT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE
                                INNER JOIN ARR_SCHD
                                ON AIRLINE_AIRCRAFT.ID_SET = ARR_SCHD.AIRLINE_AIRCRAFT_ID_SET
                                INNER JOIN AIRPORT
                                ON ARR_SCHD.AIRPORT_ID_AIRPORT = AIRPORT.ID_AIRPORT
                                INNER JOIN FLIGHT
                                ON ARR_SCHD.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                                "WHERE AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "' AND " +
                                "FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + "";
                        break;
                }
                MySqlCommand check2 = new MySqlCommand(query, conn);
                freeSeat = Convert.ToInt32(check2.ExecuteScalar());
                conn.Close();
                availableSeat = freeSeat - usedSeat;
                metroLabel5.Visible = true;
                metroLabel5.Text = "Осталось мест: " + availableSeat.ToString();
            }
        }
        private void purchaseTicket()
        {
            checkState();
            if (checkPax())
            {
                conn.Open();
                query = "INSERT INTO customer(LAST_NAME, FIRST_NAME, BIRTHDAY, NUMDOC, EXPIRY_DATE, EMAIL, PHONE, sex, TYPE_DOC_ID_TYPE) VALUES " +
                        "('" + metroTextBox1.Text + "','" + metroTextBox2.Text + "', '" + metroDateTime2.Value.ToString("yyyy-MM-dd") + "', '" + metroTextBox5.Text + "', " +
                        "'" + metroDateTime3.Value.ToString("yyyy-MM-dd") + "', '" + metroTextBox9.Text + "', '" + metroTextBox8.Text + "', '" + metroComboBox3.Text + "', " +
                        "(SELECT ID_TYPE FROM TYPE_DOC WHERE TYPEDOC = '" + metroComboBox2.Text + "'));" +

                        "INSERT INTO TICKET (CUSTOMER_ID_CUSTOMER, FLIGHT_DATE, DEP_SCHD_ID_SCHD, ARR_SCHD_ID_SCHD) " +
                        "VALUES ((" +
                        "SELECT ID_CUSTOMER FROM CUSTOMER WHERE " +
                        "LAST_NAME = '" + metroTextBox1.Text + "' AND " +
                        "FIRST_NAME = '" + metroTextBox2.Text + "' AND " +
                        "NUMDOC = '" + metroTextBox5.Text + "' AND " +
                        "TYPE_DOC_ID_TYPE = (SELECT ID_TYPE FROM TYPE_DOC WHERE TYPEDOC = '" + metroComboBox2.Text + "')), " +
                        "'" + metroDateTime1.Value.ToString("yyyy-MM-dd") + "'," +
                        "(SELECT DEP_SCHD.ID_SCHD FROM DEP_SCHD " +
                        "INNER JOIN FLIGHT ON DEP_SCHD.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                        "INNER JOIN AIRLINE ON FLIGHT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE " +
                        "WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "'), " +
                        "(SELECT ARR_SCHD.ID_SCHD FROM ARR_SCHD " +
                        "INNER JOIN FLIGHT ON ARR_SCHD.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                        "INNER JOIN AIRLINE ON FLIGHT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE " +
                        "WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "')); " +

                        "INSERT INTO PURCHASE_LOG (PURCHASE_DATE, PURCHASE_TIME, FLIGHT_CLASS, CUSTOMER_ID_CUSTOMER, FLIGHT_ID_FLIGHT, FLIGHT_PRICE) VALUES " +
                        "('" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToLongTimeString() + "', '" + metroComboBox1.Text + "', " +
                        "(SELECT ID_CUSTOMER FROM CUSTOMER WHERE " +
                        "LAST_NAME = '" + metroTextBox1.Text + "' AND " +
                        "FIRST_NAME = '" + metroTextBox2.Text + "' AND " +
                        "NUMDOC = '" + metroTextBox5.Text + "' AND " +
                        "TYPE_DOC_ID_TYPE = (SELECT ID_TYPE FROM TYPE_DOC WHERE TYPEDOC = '" + metroComboBox2.Text + "'))," +
                        "(SELECT FLIGHT.ID_FLIGHT FROM FLIGHT " +
                        "INNER JOIN DEP_SCHD ON FLIGHT.ID_FLIGHT = DEP_SCHD.FLIGHT_ID_FLIGHT " +
                        "INNER JOIN AIRPORT ON DEP_SCHD.AIRPORT_ID_AIRPORT = AIRPORT.ID_AIRPORT " +
                        "INNER JOIN AIRLINE ON FLIGHT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE " +
                        "WHERE AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "' AND " +
                        "FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "DEP_SCHD.AIRPORT_ID_AIRPORT = (SELECT ID_AIRPORT FROM AIRPORT WHERE AIRPORT.AIRPORT = '" + metroGrid1.CurrentRow.Cells[5].Value.ToString() + "')), " +
                        "" + Convert.ToDouble(metroGrid1.CurrentRow.Cells[9].Value) + ");";
                MySqlCommand command2 = new MySqlCommand(query, conn);
                if (command2.ExecuteNonQuery() == 3)
                {
                    conn.Close();
                    DialogResult dialogResult = MetroMessageBox.Show(this, "Желаем приятного полета! Желаете распечатать билет?",
                                                "Спасибо за покупку!",
                                                MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        metroTabPage2.Parent = metroTabControl2;
                        metroTabPage4.Parent = null;
                        metroTabControl2.SelectedTab = metroTabPage2;
                        printPreviewDialog1.Document = printDocument1;
                        printPreviewDialog1.ShowDialog();
                    }
                    else
                    {
                        metroTabPage2.Parent = metroTabControl2;
                        metroTabPage4.Parent = null;
                        metroTabControl2.SelectedTab = metroTabPage2;
                    }
                }
                else
                {
                    MetroMessageBox.Show(this, "Ошибка при покупке", "Ошибка");
                    conn.Close();
                }
            }
            else
            {
                conn.Open();
                query = @"INSERT INTO TICKET (CUSTOMER_ID_CUSTOMER, FLIGHT_DATE, DEP_SCHD_ID_SCHD, ARR_SCHD_ID_SCHD) " +
                        "VALUES ((" +
                        "SELECT ID_CUSTOMER FROM CUSTOMER WHERE " +
                        "LAST_NAME = '" + metroTextBox1.Text + "' AND " +
                        "FIRST_NAME = '" + metroTextBox2.Text + "' AND " +
                        "NUMDOC = '" + metroTextBox5.Text + "' AND " +
                        "TYPE_DOC_ID_TYPE = (SELECT ID_TYPE FROM TYPE_DOC WHERE TYPEDOC = '" + metroComboBox2.Text + "')), " +
                        "'" + metroDateTime1.Value.ToString("yyyy-MM-dd") + "'," +
                        "(SELECT DEP_SCHD.ID_SCHD FROM DEP_SCHD " +
                        "INNER JOIN FLIGHT ON DEP_SCHD.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                        "INNER JOIN AIRLINE ON FLIGHT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE " +
                        "WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "'), " +
                        "(SELECT ARR_SCHD.ID_SCHD FROM ARR_SCHD " +
                        "INNER JOIN FLIGHT ON ARR_SCHD.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                        "INNER JOIN AIRLINE ON FLIGHT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE " +
                        "WHERE FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "')); " +

                        "INSERT INTO PURCHASE_LOG (PURCHASE_DATE, PURCHASE_TIME, FLIGHT_CLASS, CUSTOMER_ID_CUSTOMER, FLIGHT_ID_FLIGHT, FLIGHT_PRICE) VALUES " +
                        "('" + DateTime.Now.ToString("yyyy-MM-dd") + "', '" + DateTime.Now.ToLongTimeString() + "', '" + metroComboBox1.Text + "', " +
                        "(SELECT ID_CUSTOMER FROM CUSTOMER WHERE " +
                        "LAST_NAME = '" + metroTextBox1.Text + "' AND " +
                        "FIRST_NAME = '" + metroTextBox2.Text + "' AND " +
                        "NUMDOC = '" + metroTextBox5.Text + "' AND " +
                        "TYPE_DOC_ID_TYPE = (SELECT ID_TYPE FROM TYPE_DOC WHERE TYPEDOC = '" + metroComboBox2.Text + "'))," +
                        "(SELECT FLIGHT.ID_FLIGHT FROM FLIGHT " +
                        "INNER JOIN DEP_SCHD ON FLIGHT.ID_FLIGHT = DEP_SCHD.FLIGHT_ID_FLIGHT " +
                        "INNER JOIN AIRPORT ON DEP_SCHD.AIRPORT_ID_AIRPORT = AIRPORT.ID_AIRPORT " +
                        "INNER JOIN AIRLINE ON FLIGHT.AIRLINE_ID_AIRLINE = AIRLINE.ID_AIRLINE " +
                        "WHERE AIRLINE.AIRLINE = '" + metroGrid1.CurrentRow.Cells[0].Value.ToString() + "' AND " +
                        "FLIGHT.FLIGHT_NUM = " + Convert.ToInt32(metroGrid1.CurrentRow.Cells[2].Value) + " AND " +
                        "DEP_SCHD.AIRPORT_ID_AIRPORT = (SELECT ID_AIRPORT FROM AIRPORT WHERE AIRPORT.AIRPORT = '" + metroGrid1.CurrentRow.Cells[5].Value.ToString() + "')), " +
                        "" + Convert.ToDouble(metroGrid1.CurrentRow.Cells[9].Value) + ");";
                MySqlCommand command = new MySqlCommand(query, conn);
                if (command.ExecuteNonQuery() == 2)
                {
                    conn.Close();
                    DialogResult dialogResult = MetroMessageBox.Show(this, "Желаем приятного полета! Желаете распечатать билет?",
                                                "Спасибо за покупку!",
                                                MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        metroTabPage2.Parent = metroTabControl2;
                        metroTabPage4.Parent = null;
                        metroTabControl2.SelectedTab = metroTabPage2;
                        printPreviewDialog1.Document = printDocument1;
                        printPreviewDialog1.ShowDialog();
                    }
                    else
                    {
                        metroTabPage2.Parent = metroTabControl2;
                        metroTabPage4.Parent = null;
                        metroTabControl2.SelectedTab = metroTabPage2;
                    }
                }
                else
                {
                    MetroMessageBox.Show(this, "Ошибка при покупке", "Ошибка");
                    conn.Close();
                }
            }
        }
        private void filterAirport()
        {
            checkState();
            metroComboBox6.Items.Clear();
            conn.Open();
            query = "SELECT CITY FROM AIRPORT GROUP BY CITY HAVING CITY != '" + metroComboBox5.Text + "'  ORDER BY 1";
            MySqlCommand group1 = new MySqlCommand(query, conn);
            MySqlDataReader readergroup1 = group1.ExecuteReader();
            while (readergroup1.Read())
            {
                metroComboBox6.Items.Add(readergroup1.GetString("CITY"));
            }
            conn.Close();
        }
        private bool checkPax()
        {
            checkState();
            conn.Open();
            query = "SELECT COUNT(*) FROM CUSTOMER WHERE LAST_NAME = '" + metroTextBox1.Text + "' AND " +
                    "NUMDOC = '" + metroTextBox5.Text + "'";
            MySqlCommand CH = new MySqlCommand(query, conn);
            if (Convert.ToInt32(CH.ExecuteScalar()) == 0)
            {
                conn.Close();
                return true;

            }
            else
            {
                conn.Close();
                return false;
            }
        }
        private bool checkPaxInfo()
        {
            if (metroTextBox1.Text == "")
            {
                MetroMessageBox.Show(this, "Введите фамилию!", "Ошибка данных!");
                metroTextBox1.Focus();
                return false;
            }
            else if (metroTextBox2.Text == "")
            {
                MetroMessageBox.Show(this, "Введите имя!", "Ошибка данных!");
                metroTextBox2.Focus();
                return false;
            }
            else if (metroTextBox5.Text == "")
            {
                MetroMessageBox.Show(this, "Введите номер документа!", "Ошибка данных!");
                metroTextBox5.Focus();
                return false;
            }
            else if (metroTextBox9.Text == "")
            {
                MetroMessageBox.Show(this, "Введите телефон!", "Ошибка данных!");
                metroTextBox9.Focus();
                return false;
            }
            else if (metroTextBox8.Text == "")
            {
                MetroMessageBox.Show(this, "Введите E-Mail!", "Ошибка данных!");
                metroTextBox8.Focus();
                return false;
            }
            else if (metroComboBox1.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите пол!", "Ошибка данных!");
                metroComboBox1.Focus();
                return false;
            }
            else if (metroComboBox2.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите тип документа!", "Ошибка данных!");
                metroComboBox2.Focus();
                return false;
            }
            else
                return true;
        }
        private void MainReport()
        {
            checkState();
            conn.Open();
            query = "SELECT PURCHASE_LOG.PURCHASE_DATE 'Дата', PURCHASE_LOG.PURCHASE_TIME 'Время', PURCHASE_LOG.FLIGHT_CLASS 'Класс',PURCHASE_LOG.FLIGHT_PRICE 'Цена', " +
                    "DEP.AIRLINE 'Авиакомпания', FLIGHT.FLIGHT_NUM '№ Рейса', DEP.DEP_CITY 'Город отправления', ARR.ARR_CITY 'Город прибытия' " +
                    "FROM PURCHASE_LOG INNER JOIN FLIGHT " +
                    "ON PURCHASE_LOG.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                    "INNER JOIN DEP ON FLIGHT.FLIGHT_NUM = DEP.FLIGHT_NUM " +
                    "INNER JOIN ARR ON FLIGHT.FLIGHT_NUM = ARR.FLIGHT_NUM " +
                    "ORDER BY 1,2 ASC";
            DataTable dt = new DataTable();
            MySqlCommand SCHD = new MySqlCommand(query, conn);
            MySqlDataReader reader = SCHD.ExecuteReader();
            dt.Load(reader);
            metroGrid2.DataSource = dt;
            conn.Close();
            int i = 0;
            double sum = 0;
            while (i < metroGrid2.RowCount)
            {
                sum += Convert.ToDouble(metroGrid2.Rows[i].Cells[3].Value);
                i++;
            }
            disableSortGrid2();
            metroLabel29.Text = "Количество проданных билетов: " + metroGrid2.RowCount.ToString();
            metroLabel30.Text = "Сумма проданных билетов: " + sum.ToString();
        }
        private void cityFilter()
        {
            checkState();
            conn.Open();
            query = "SELECT CITY FROM AIRPORT GROUP BY CITY ORDER BY 1";
            MySqlCommand group1 = new MySqlCommand(query, conn);
            MySqlDataReader readergroup1 = group1.ExecuteReader();
            while (readergroup1.Read())
            {
                metroComboBox5.Items.Add(readergroup1.GetString("CITY"));
            }
            conn.Close();
        }
        private void newReport()
        {
            checkState();
            if (metroDateTime4.Value.ToString("yyyy-MM-dd") == metroDateTime5.Value.ToString("yyyy-MM-dd"))
                MetroMessageBox.Show(this, "Введите корректный период", "Ошибка!");
            else
            {
                conn.Open();
                query = "SELECT PURCHASE_LOG.PURCHASE_DATE 'Дата', PURCHASE_LOG.PURCHASE_TIME 'Время', PURCHASE_LOG.FLIGHT_CLASS 'Класс',PURCHASE_LOG.FLIGHT_PRICE 'Цена', " +
                        "DEP.AIRLINE 'Авиакомпания', FLIGHT.FLIGHT_NUM '№ Рейса', DEP.DEP_CITY 'Город отправления', ARR.ARR_CITY 'Город прибытия' " +
                        "FROM PURCHASE_LOG INNER JOIN FLIGHT " +
                        "ON PURCHASE_LOG.FLIGHT_ID_FLIGHT = FLIGHT.ID_FLIGHT " +
                        "INNER JOIN DEP ON FLIGHT.FLIGHT_NUM = DEP.FLIGHT_NUM " +
                        "INNER JOIN ARR ON FLIGHT.FLIGHT_NUM = ARR.FLIGHT_NUM " +
                        "WHERE PURCHASE_LOG.PURCHASE_DATE BETWEEN '" + metroDateTime4.Value.ToString("yyyy-MM-dd") + "' AND " +
                        "'" + metroDateTime5.Value.ToString("yyyy-MM-dd") + "'" +
                        "ORDER BY 1,2 ASC";
                DataTable dt = new DataTable();
                MySqlCommand SCHD = new MySqlCommand(query, conn);
                MySqlDataReader reader = SCHD.ExecuteReader();
                dt.Load(reader);
                metroGrid2.DataSource = dt;
                conn.Close();
                int i = 0;
                double sum = 0;
                while (i < metroGrid2.RowCount)
                {
                    sum += Convert.ToDouble(metroGrid2.Rows[i].Cells[3].Value);
                    i++;
                }
                disableSortGrid2();
                metroLabel29.Text = "Количество проданных билетов: " + metroGrid2.RowCount.ToString();
                metroLabel30.Text = "Сумма проданных билетов: " + sum.ToString();
            }
        }
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            DateTime t = DateTime.ParseExact(metroGrid1.CurrentRow.Cells[6].Value.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            DateTime t2 = DateTime.ParseExact(metroGrid1.CurrentRow.Cells[3].Value.ToString(), "HH:mm", CultureInfo.InvariantCulture);
            time = getFlightTime(t, t2);
            switch (metroComboBox3.SelectedIndex)
            {
                case 0:
                    pax = "MR." + metroTextBox2.Text.ToUpper() + " " + metroTextBox1.Text.ToUpper();
                    break;
                case 1:
                    pax = "MS." + metroTextBox2.Text.ToUpper() + " " + metroTextBox1.Text.ToUpper();
                    break;
            }

            switch (metroComboBox2.SelectedIndex)
            {
                case 0:
                    doc = "DOCUMENT: U" + metroTextBox5.Text;
                    break;
                case 1:
                    doc = "DOCUMENT: P" + metroTextBox5.Text;
                    break;
                case 2:
                    doc = "DOCUMENT: B" + metroTextBox5.Text;
                    break;
                case 3:
                    doc = "DOCUMENT: O" + metroTextBox5.Text;
                    break;
            }
            e.Graphics.DrawString("Маршрутная квитанция", new Font("Calibri", 40), Brushes.Black, new Point(50, 50));
            e.Graphics.DrawString("Внимание! Это не посадочный талон", new Font("Calibri", 16), Brushes.Black, new Point(50, 120));
            e.Graphics.DrawString("Passenger: " + pax, new Font("Calibri", 16), Brushes.Black, new Point(50, 210));
            e.Graphics.DrawString(doc, new Font("Calibri", 16), Brushes.Black, new Point(500, 210));
            e.Graphics.DrawString("Авиакомпания: " + metroGrid1.CurrentRow.Cells[0].Value.ToString(), new Font("Calibri", 16), Brushes.Black, new Point(50, 250));
            e.Graphics.DrawString("Рейс: " + metroGrid1.CurrentRow.Cells[1].Value.ToString() + " " + metroGrid1.CurrentRow.Cells[2].Value.ToString(), new Font("Calibri", 16), Brushes.Black, new Point(500, 250));
            e.Graphics.DrawString("Пункт вылета: " + metroGrid1.CurrentRow.Cells[4].Value.ToString() + " (" + metroGrid1.CurrentRow.Cells[5].Value.ToString() + ")", new Font("Calibri", 16), Brushes.Black, new Point(50, 290));
            e.Graphics.DrawString("Время вылета: " + metroGrid1.CurrentRow.Cells[3].Value.ToString(), new Font("Calibri", 16), Brushes.Black, new Point(300, 320));
            e.Graphics.DrawString("Пункт прилета: " + metroGrid1.CurrentRow.Cells[7].Value.ToString() + " (" + metroGrid1.CurrentRow.Cells[8].Value.ToString() + ")", new Font("Calibri", 16), Brushes.Black, new Point(50, 350));
            e.Graphics.DrawString("Время прилета: " + metroGrid1.CurrentRow.Cells[6].Value.ToString(), new Font("Calibri", 16), Brushes.Black, new Point(300, 380));
            e.Graphics.DrawString("Время полета: " + time, new Font("Calibri", 16), Brushes.Black, new Point(50, 410));
            e.Graphics.DrawString("Класс обслуживания: " + metroComboBox1.Text, new Font("Calibri", 16), Brushes.Black, new Point(50, 460));
            e.Graphics.DrawString("Итого: " + metroGrid1.CurrentRow.Cells[9].Value.ToString() + " тг.", new Font("Calibri", 16), Brushes.Black, new Point(500, 500));
        }
        private string getFlightTime(DateTime Time1, DateTime Time2)
        {
            TimeSpan diff = Time1.Subtract(Time2);
            if (Time1 < Time2)
                diff = diff + TimeSpan.FromDays(1);
            else
                diff = Time1.Subtract(Time2);
            if (diff.Minutes < 10)
                minutes = "0" + diff.Minutes;
            else
                minutes = diff.Minutes.ToString();
            time = diff.Hours + ":" + minutes;
            return time;
        }
        private void setPrices()
        {
            int days = ((TimeSpan)(metroDateTime1.Value - DateTime.Now)).Days;
            if (days <= 10)
                changePrice(1.5);
            else if (days > 10 && days <= 30)
                changePrice(1);
            else if (days > 30 && days <= 60)
                changePrice(0.85);
            else
                changePrice(0.6);
        }
        private void changePrice(double percent)
        {
            for (int i = 0; i < metroGrid1.RowCount; i++)
            {
                metroGrid1.Rows[i].Cells[9].Value = Convert.ToDouble(metroGrid1.Rows[i].Cells[9].Value) * percent;
            }
        }
        private void checkState()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }
        private void disableSortGrid1()
        {
            foreach (DataGridViewColumn column in metroGrid1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void disableSortGrid2()
        {
            foreach (DataGridViewColumn column in metroGrid2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private bool checkFlightFilter()
        {
            if (metroComboBox6.Text == "Выберите пункт отправления!")
            {
                MetroMessageBox.Show(this, "!");
                metroComboBox6.Focus();
                return false;
            }
            else if (metroComboBox5.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите пункт прибытия!");
                metroComboBox5.Focus();
                return false;
            }
            else if (metroComboBox6.Text == metroComboBox5.Text)
            {
                MetroMessageBox.Show(this, "Пункт отправления не может быть пунктом прибытия!");
                return false;
            }
            else if (metroComboBox1.Text == "")
            {
                MetroMessageBox.Show(this, "Выберите класс обслуживания!");
                metroComboBox1.Focus();
                return false;
            }
            return true;
        }
        private bool checkSearchFlight()
        {
            if (metroComboBox5.Text == "")
            {
                MetroMessageBox.Show(this, "Введите пункт вылета", "Ошибка!");
                metroComboBox5.Focus();
                return false;
            }
            else if (metroComboBox6.Text == "")
            {
                MetroMessageBox.Show(this, "Введите пункт прилета", "Ошибка!");
                metroComboBox6.Focus();
                return false;
            }
            else if (metroComboBox1.Text == "")
            {
                MetroMessageBox.Show(this, "Введите класс обслуживания", "Ошибка!");
                metroComboBox1.Focus();
                return false;
            }
            else
                return true;
        }
    }
}
