using ProjectB.sakila;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectB
{
    public class EF_mysql_conn
    {
        public EF_mysql_conn()
        {
            Console.WriteLine( "In constructor" );

            var context = new withprimarykeyContext();

            var sale1 = new Sale()
            {
                Product = "clio",
                Price = 200000
            };

            context.Add<Sale>(sale1);
            context.SaveChanges();

        }

    }
}
