using GaneShop.Pages.Admin.Parfumuri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GaneShop.Pages
{
    public class CardModel : PageModel
    {


        public List<OrderItem> listOrderItems= new List<OrderItem>();


        private Dictionary<String, int> getParfumuriDictionary()
        {
            var ParfumuriDictionary =new Dictionary<String, int>();
            string cookieValue = Request.Cookies["shopping_card"] ?? "";

            if (cookieValue.Length > 0)
            {
                string[] parfumuriIdArray = cookieValue.Split('-');


                for(int i = 0; i< parfumuriIdArray.Length; i++)
                {
                    string parfumuriID = parfumuriIdArray[i];
                    if(ParfumuriDictionary.ContainsKey(parfumuriID))
                    {
                        ParfumuriDictionary[parfumuriID] += 1;

                    }
                    else
                    {
                        ParfumuriDictionary.Add(parfumuriID, 1);
                    }
                }




            }



            return ParfumuriDictionary;
        }


        public void OnGet()
        {
            var ParfumuriDictionary=getParfumuriDictionary();

            try
            {
                string connectionString = "Data Source=.\\sqlexpress01;Initial Catalog=bestshop;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string sql = "SELECT * FROM Parfumuri WHERE id=@id";
                    foreach (var KeyValuePair in ParfumuriDictionary)
                    {

                        Console.WriteLine();

                        string parfumuriID = KeyValuePair.Key;
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {

                            command.Parameters.AddWithValue("@id", parfumuriID);
                            using(SqlDataReader reader = command.ExecuteReader()) 
                            { 
                                if(reader.Read())
                                {
                                    OrderItem item = new OrderItem();
                                    item.ParfumuriInfo.ID = reader.GetInt32(0);
                                    item.ParfumuriInfo.Title = reader.GetString(1);
                                    item.ParfumuriInfo.Cod_Parfum = reader.GetString(2);
                                    item.ParfumuriInfo.Pret = reader.GetDecimal(3);
                                    item.ParfumuriInfo.Descriere = reader.GetString(4);
                                    item.ParfumuriInfo.categorie = reader.GetString(5);
                                    item.ParfumuriInfo.imagine = reader.GetString(6);
                                    item.ParfumuriInfo.created_at = reader.GetDateTime(7).ToString("MM/dd/YYYY");

                                    item.numCopies=KeyValuePair.Value;
                                    item.totalPrice = item.numCopies * item.ParfumuriInfo.Pret;
                                 

                                    listOrderItems.Add(item);   
                                }
                            
                            }

                        }
                    }

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }





        }
    }

    public class OrderItem
    {
        public ParfumuriInfo ParfumuriInfo = new ParfumuriInfo();
        public int numCopies = 0;
        public decimal totalPrice = 0;

    }

}
