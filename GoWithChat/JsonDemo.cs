using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

/*
namespace GoWithChat
{
    public partial class JsonDemo : Form
    {
        public JsonDemo()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string jsonText = "['JSON!',1,true,{property:'value'}]";
 
            JsonReader reader = new JsonReader(new StringReader(jsonText));
 
            Console.WriteLine("TokenType\t\tValueType\t\tValue");
 
            while (reader.Read())
            {
                Console.WriteLine(reader.TokenType + "\t\t" + WriteValue(reader.ValueType) + "\t\t" + WriteValue(reader.Value))
            }
        }
    }
}
 * http://www.cnblogs.com/sbxwylt/archive/2008/12/31/1366199.html
 * */
