using ProjectB.sakila;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectB
{
    public class EF_mysql_conn
    {
        string product;

        public EF_mysql_conn( string product )
        {
            Console.WriteLine( "In constructor" );

            var context = new withprimarykeyContext();

            var sale1 = new Sale()
            {
                Product = product,
                Price = 300
            };

            context.Add<Sale>(sale1);
            context.SaveChanges();

        }

    }
}
