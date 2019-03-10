using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.IO;

namespace WebApplication2
{
    public delegate void ExecuteData();

    public partial class Json_mysql_asp : System.Web.UI.Page
    {
        public ExecuteData Executedata;
        DataTable dt = new DataTable();
        MySqlConnection cnn = new MySqlConnection("server=localhost;database=apiuser;user id =root;password =123;");
        MySqlCommand cmd;
        MySqlDataReader datareader;

        public Json_mysql_asp()
        {


            string query = System.IO.File.ReadAllText(@"d:\bricquette\dole.txt");

            cnn.Open();
            cmd = new MySqlCommand(query, cnn);
            cmd.ExecuteNonQuery();
            cnn.Close();


            Executedata += (ClearData);
            Executedata += (SetDataColumn);
            Executedata += (FetchDataTable);
            Executedata += (PageRender);
        }

        protected void Page_Load()
        {
            //Json_mysql_asp jma = new Json_mysql_asp();
            this.Executedata();
        }

        protected void PageLoad()
        {
            //string connectionString = "Localhost;Port=3306;user id =root;password =12345678;database=cadmacro_shopplanvishalfeb04;Pooling =True;Min Pool Size =0;Max Pool Size =200;Connect Timeout =30;ConnectionReset =False;Allow User Variables=True;Convert Zero Datetime=True;"


            try
            {
                
                string jsonData = string.Empty;
                string url = "https://jsonplaceholder.typicode.com/todos/";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    jsonData = reader.ReadToEnd();
                }

                jsonData = jsonData.TrimStart('[');
                jsonData = jsonData.TrimStart(']');
                
                foreach (string item in jsonData.Split('}'))
                {
                    if (item.Contains("{"))
                    {
                        string data = (item + "}").TrimStart(',');
                        var myDetails = JsonConvert.DeserializeObject<MyPlaceHolderDetail>(data);
                        cnn.Open();

                        string query = "insert into tbluser values('" + myDetails.userId + "','" + myDetails.id + "','" + myDetails.title + "'," + myDetails.completed + ")";
                        cmd = new MySqlCommand(query, cnn);
                        cmd.ExecuteNonQuery();
                        cnn.Clone();
                    }

                }

            }
            catch (Exception ex)
            {
                Response.Write("connection not open");
            }
            finally
            {
                cnn.Close();
                //FetchDataTable();
                //PageRender();
            }

        }
        private void ClearData()
        {
            try
            {
                cnn.Open();
                string query = "delete from tbluser;";
                cmd = new MySqlCommand(query, cnn);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                cnn.Close();
            }
        }

        private void FetchDataTable()
        {
            try
            {
                cnn.Open();

                string query = "select * from tbluser;";
                cmd = new MySqlCommand(query, cnn);
                datareader = cmd.ExecuteReader();

                int i = 0;
                while (datareader.Read())
                {
                    dt.Rows.Add();

                    dt.Rows[i]["Userid"] = Convert.ToString(datareader["Userid"]);
                    dt.Rows[i]["id"] = Convert.ToString(datareader["id"]);
                    dt.Rows[i]["title"] = Convert.ToString(datareader["title"]);
                    dt.Rows[i++]["completed"] = Convert.ToString(datareader["completed"]);

                }
            }
            finally
            {
                cnn.Close();
            }
        }
        private void SetDataColumn()
        {
            dt.Columns.Add("Userid");
            dt.Columns.Add("id");

            dt.Columns.Add("title");
            dt.Columns.Add("completed");

        }
        private void PageRender()
        {
            //<tr>
            //<th>User ID</th>
            //<th>ID</th>
            //<th>Title</th>
            //<th>Completed</th>
            //</tr>

            HttpContext.Current.Response.Write("<table style='width:100%'>         <tr>             <th>User ID</th>             <th>ID</th>             <th>Title</th>             <th>Completed</th>             </tr>");
            

            foreach (DataRow row in dt.Rows)
            {

                HttpContext.Current.Response.Write("<tr> 			<td>" + Convert.ToString(row["userid"]) + "</td> 			<td>" + Convert.ToString(row["id"]) + "</td> 			<td>" + Convert.ToString(row["title"]) + "</td> 			<td>" + Convert.ToString(row["completed"]) + "</td> 			</tr> 			 			");

                //HttpContext.Current.Response.Write(Convert.ToString(row["userid"]));
                //HttpContext.Current.Response.Write("<br>");
                //HttpContext.Current.Response.Write(Convert.ToString(row["id"]));
                //HttpContext.Current.Response.Write("<br>");
                //HttpContext.Current.Response.Write(Convert.ToString(row["title"]));
                //HttpContext.Current.Response.Write("<br>");
                //HttpContext.Current.Response.Write(Convert.ToString(row["completed"]));
                //HttpContext.Current.Response.Write("<br>");
                //HttpContext.Current.Response.Write("<br>");
                //HttpContext.Current.Response.Write("<br>");
                //HttpContext.Current.Response.Write("<br>");
                //HttpContext.Current.Response.Write("<br>");


                
            }

        }
    }

    #region myplaceholderdetail
    public class MyPlaceHolderDetail
    {
        public string userId
        {
            get;
            set;
        }
        public string id
        {
            get;
            set;
        }


        public string title
        {
            get;
            set;
        }
        public bool completed
        {
            get;
            set;
        }
    }
#endregion
}