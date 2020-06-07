using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using Typography_WCF;

namespace TypographyWCFHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(Typography_WCF.WorkClasses));
            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;
            host.Description.Behaviors.Add(behavior);
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), new Uri("http://localhost:8733/Design_Time_Addresses/Typography_WCF/Service1/mex"));
            host.Open();

            Console.WriteLine("Служба запущенна");

            Console.ReadLine();

            host.Close();
        }
    }
}
