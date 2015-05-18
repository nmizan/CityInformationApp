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
using System.Configuration;

namespace CityInformationApp
{
    public partial class CityInfoUI : Form
    {
        public CityInfoUI()
        {
            InitializeComponent();
            ShowAllCityInfo();
        }

        private string connectionString = ConfigurationManager.ConnectionStrings["CityInformationDB"].ConnectionString;

        private bool isUpdateModeOn = false;
        private int cityId;

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (nameTextBox.Text == "" || aboutTextBox.Text == "" || countryTextBox.Text == "")
            {
                MessageBox.Show("pls enter the information");
            }
            else
            {
                if (isUpdateModeOn)
                {
                    string name = nameTextBox.Text;
                    string about = aboutTextBox.Text;
                    string country = countryTextBox.Text;


                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = "UPDATE City SET About ='" + about + "', Country='" + country + "' WHERE id = '" +
                                   cityId + "'";




                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();
                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Updated Successfully!");

                        saveButton.Text = "Save";
                        cityId = 0;
                        isUpdateModeOn = false;
                        GetTextBoxesClear();
                        ShowAllCityInfo();

                    }
                    else
                    {
                        MessageBox.Show("Update Failed!");
                    }

                }

                else
                {

                    string name = nameTextBox.Text;
                    String about = aboutTextBox.Text;
                    string country = countryTextBox.Text;

                    if (IsCityNameExists(name))
                    {
                        MessageBox.Show("City Name already Exists!");
                    }
                    else if (nameTextBox.Text.Length < 4)
                    {
                        MessageBox.Show("City Name at least 4 character Long !");
                    }
                    else
                    {
                        SqlConnection connection = new SqlConnection(connectionString);

                        string query = "INSERT INTO City Values('" + name + "','" + about + "','" + country + "')";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        int rowAffected = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowAffected > 0)
                        {
                            MessageBox.Show(" Inserted successfully!");
                            GetTextBoxesClear();
                            ShowAllCityInfo();
                        }

                        else
                        {
                            MessageBox.Show("Insertion Failed !");
                        }
                    }



                }
            }
        }

        private void GetTextBoxesClear()
        {
            nameTextBox.Clear();
            aboutTextBox.Clear();
            countryTextBox.Clear();
        }

        public bool IsCityNameExists(string name)
        {

            SqlConnection connection = new SqlConnection(connectionString);


            string query = "SELECT * FROM City Where Name='" + name + "' ";
            bool isCityNameExists = false;

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                isCityNameExists = true;
                break;
            }
            reader.Close();
            connection.Close();
            return isCityNameExists;
        }

        public void ShowAllCityInfo()
        {


            SqlConnection connection = new SqlConnection(connectionString);


            string query = "SELECT * FROM City";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<City> cityList = new List<City>();
            while (reader.Read())
            {
                City cities = new City();

                cities.ID = int.Parse(reader["id"].ToString());
                cities.name = (reader["Name"].ToString());
                cities.about = (reader["About"].ToString());
                cities.country = (reader["Country"].ToString());

                cityList.Add(cities);

            }
            LoadCityListView(cityList);
            reader.Close();
            connection.Close();

        }

        public void LoadCityListView(List<City> citys)
        {
            cityListView.Items.Clear();
            foreach (var city in citys)
            {


                ListViewItem item = new ListViewItem(city.ID.ToString());
                item.SubItems.Add(city.name);
                item.SubItems.Add(city.about);
                item.SubItems.Add(city.country);

                cityListView.Items.Add(item);
            }


        }

        private void cityListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = cityListView.SelectedItems[0];
            int id = int.Parse(item.Text.ToString());
            City city = GetCityByID(id);

            if (city != null)
            {
                isUpdateModeOn = true;
                saveButton.Text = "Update";
                cityId = city.ID;

                nameTextBox.Text = city.name;
                aboutTextBox.Text = city.about;
                countryTextBox.Text = city.country;

            }
        }

        public City GetCityByID(int id)
        {

            SqlConnection connection = new SqlConnection(connectionString);


            string query = "SELECT * FROM City WHERE id='" + id + "'";


            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<City> cityList = new List<City>();
            while (reader.Read())
            {
                City cities = new City();

                cities.ID = int.Parse(reader["id"].ToString());
                cities.name = (reader["Name"].ToString());
                cities.about = (reader["About"].ToString());
                cities.country = (reader["Country"].ToString());

                cityList.Add(cities);

            }

            reader.Close();
            connection.Close();
            return cityList.FirstOrDefault();

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string search = searchTextBox.Text;

            if (cityRadioButton.Checked == true)
            {
                int id = 1;

                SqlConnection connection = new SqlConnection(connectionString);


                string query = "SELECT  Name,About,Country FROM City WHERE Name LIKE'" + search + "%'";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<City> cityList = new List<City>();

                cityListView.Items.Clear();


                while (reader.Read())
                {
                    City cities = new City();


                    cities.name = (reader["Name"].ToString());
                    cities.about = (reader["About"].ToString());
                    cities.country = (reader["Country"].ToString());

                    cityList.Add(cities);
                }
                reader.Close();
                connection.Close();

                foreach (var city in cityList)
                {

                    ListViewItem item = new ListViewItem(id.ToString());
                    item.SubItems.Add(city.name);
                    item.SubItems.Add(city.about);
                    item.SubItems.Add(city.country);

                    cityListView.Items.Add(item);
                    id++;
                }


            }


            else if (countryRadioButton.Checked == true)
            {
                int id = 1;

                SqlConnection connection = new SqlConnection(connectionString);


                string query = "SELECT  Name,About,Country FROM City WHERE Country LIKE'" + search + "%'";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<City> cityList = new List<City>();

                cityListView.Items.Clear();


                while (reader.Read())
                {
                    City cities = new City();


                    cities.name = (reader["Name"].ToString());
                    cities.about = (reader["About"].ToString());
                    cities.country = (reader["Country"].ToString());

                    cityList.Add(cities);
                }
                reader.Close();
                connection.Close();

                foreach (var city in cityList)
                {

                    ListViewItem item = new ListViewItem(id.ToString());
                    item.SubItems.Add(city.name);
                    item.SubItems.Add(city.about);
                    item.SubItems.Add(city.country);

                    cityListView.Items.Add(item);
                    id++;
                }

            }


            
      }
    }
}
        
                    

                      
          
           

       