using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello World!");
            MsgBundle bundle = new MsgBundle();
            bundle.username = "Hurray";
            bundle.passwd = "123";
            //bundle.FightMsg = "haha";
            string getJson = JsonConvert.SerializeObject(bundle);
            System.Console.WriteLine(getJson);

            MsgBundle newbundle = JsonConvert.DeserializeObject<MsgBundle>(getJson);
            System.Console.WriteLine(newbundle.username);
            Console.ReadKey();
        }
    }
}
