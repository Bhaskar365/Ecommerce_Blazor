using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.PayPal
{
    public class PayPalExecutedResponse
    {
        public string id { get; set; }
        public string intent { get; set; }
        public string state { get; set; }
        public string cart { get; set; }
        public Payer payer { get; set; }
        public Transaction[] transactions { get; set; }
        public DateTime create_time { get; set; }
        public Link[] links { get; set; }
        public class Payer
        {
            public string payment_method { get; set; }
            public string status { get; set; }
            public Payer_Info payer_info { get; set; }
        }
        public class Payer_Info
        {
            public string email { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string payer_id { get; set;; }
            public Shipping_Address shipping_address { get; set; }
            public string country_code { get; set; }
            public Billing_Address billing_address { get; set; }
        }
        public class Shipping_Address
        {
            public string recipient_name { get; set; }
            public string line1 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postal_code { get; set; }
            public string country_code { get; set; }
        }
        public class Transaction
        {
            public Amount amount { get; set; }
            public Payee payee { get; set; }
            public Item_List item_list { get; set; }
            public Related_Resources[] related_resources { get; set; }
        }
        public class Amount
        {
            public string total { get; set; }
            public string currency { get; set; }
            public Details details { get; set; }
        }
        public class Details
        {
            public string subtotal { get; set; }
        }
        public class Payee 
        {
            public string merchant_id { get; set; }
            public string email { get; set; }
        }
        public class Item_List 
        {
            public Shipping_Address1 shipping_address { get; set; }
        }

        public class Shipping_Address1 
        {
            public string recipient_name { get; set; }
            public string line1 { get; set; }

        }
    
    }
}
