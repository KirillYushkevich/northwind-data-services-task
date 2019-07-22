using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ODataTask1Core
{
    class Program
    {
        static async Task Main(string[] args)
        {
            NorthwindModel.NorthwindEntities entities = new NorthwindModel.NorthwindEntities(new Uri("https://services.odata.org/V3/Northwind/Northwind.svc"));
            var taskFactory = new TaskFactory<IEnumerable<NorthwindModel.Customer>>();
            var customers = (await taskFactory.FromAsync(entities.Customers.BeginExecute(null, null), (iar) => // breakpoint #2.1
            {
                return entities.Customers.EndExecute(iar); // breakpoint #2.2
            })).ToArray();
            Console.WriteLine("{0} customers in the service found.", customers.Length);
            Console.ReadLine(); // breakpoint #2.3
        }
    }
}
