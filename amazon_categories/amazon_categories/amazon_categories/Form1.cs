using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace amazon_categories
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-HHRN7UL;Initial Catalog=Amazon;Integrated Security=True");
        IWebDriver driver = new ChromeDriver();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            driver.Navigate().GoToUrl("https://www.amazon.de/s?bbn=15759346031&rh=n%3A15759345031%2Cn%3A16254451031&dc&language=en&qid=1623956814&rnid=15759346031&ref=lp_15759346031_nr_n_21");
            var collection = driver.FindElements(By.XPath(".//ul[@aria-labelledby='n-title']/li[position()>2]/span/a"));
            foreach (var item in collection)
            {
                var link = item.GetAttribute("href");
                var status = 0;
                var name = item.Text.Replace("'","");
                string queery = "INSERT INTO amazon_category VALUES ('" + link + "','" + name + "','" + status + "')";
                Operation.DB_Operation(queery);
            }
            MessageBox.Show("Done");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fillDGV();
        }
        public void fillDGV()
        {
            string query = "SELECT * FROM SecondPhase_Brother1 where Status =0";

            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= dataGridView1.Rows.Count; i++)
            {
                var zipcode = dataGridView1.Rows[i].Cells[1].Value.ToString();
                int F_Id = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                driver.Navigate().GoToUrl(zipcode);

                var pagination = driver.FindElement(By.XPath(".//ul[@class='a-pagination']/li[position() = last() -1]")).Text;
                for (int j = 1; j < Convert.ToInt32(pagination); j++)
                {
                    var collection = driver.FindElements(By.XPath(".//div[@class='a-section a-spacing-none']/h2/a"));
                    foreach (var item in collection)
                    {
                        var city = item.GetAttribute("href");
                        var status = 0;
                        string queery = "INSERT INTO second_PhaseAmazon2 VALUES ('" + city + "','" + status + "','" + F_Id + "')";
                        Operation.DB_Operation(queery);
                    }
                    Thread.Sleep(5000);
                    up:
                    try
                    {
                        
                        
                        
                        
                        driver.FindElement(By.XPath(".//ul[@class='a-pagination']/li[position() = last()]")).Click();

                    }
                    catch { goto up; }
                    Thread.Sleep(5000);
                }
                Operation.DB_Operation("update amazon_category set status=1 where Link='" + zipcode + "'");
            }
            MessageBox.Show("Done");
        }

        private void button3_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var zipcode = dataGridView1.Rows[i].Cells[1].Value.ToString();
                int Id = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                up:
                try
                {
                    driver.Navigate().GoToUrl(zipcode);
                }
                catch 
                {
                    goto up;
                }
                Thread.Sleep(2000);
                string title = "", espn = "";

                //title = driver.FindElement(By.XPath(".//h1[@id='title']/span[@id='productTitle']")).Text.Replace("'","");
                var titles = driver.FindElements(By.XPath(".//h1[@id='title']/span[@id='productTitle']"));
                if(titles.Count>0)
                {
                    title=titles[0].Text.Replace("'", "");
                }

                var total_tr = driver.FindElements(By.XPath("//div[@id='audibleProductDetails']/div/table/tbody/tr"));
                if(total_tr.Count>0)
                {
                    foreach (var item in total_tr)
                    {
                        if (item.FindElement(By.XPath(".//th")).Text.Contains("ASIN"))
                        {
                            espn = item.FindElement(By.XPath(".//td")).Text;
                            break;
                        }

                    }

                }
                string queery = "";
                if (title != "" && espn!="")
                {
                     queery = "INSERT INTO Third_Phase2 VALUES (N'" + title + "','" + espn + "','" + Id + "')";
                    if (Operation.DB_Operation(queery))
                    {
                        Operation.DB_Operation("update second_PhaseAmazon2 set status=1 where CatId='" + Id + "'");
                    }
                }
                else
                {
                    Operation.DB_Operation("update second_PhaseAmazon2 set status=1 where CatId='" + Id + "'");
                }
            }
            MessageBox.Show("Done");
        }
    }
}
