using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Typography_WCF
{
    [DataContract]
    public class Service
    {
        [DataMember]
        public int id;
        [DataMember]
        public string name;
        [DataMember]
        public double price;
        [DataMember]
        public PrintType printType;
    }

    [DataContract]
    public class Sale
    {
        [DataMember]
        public int id;
        [DataMember]
        public string name;
        [DataMember]
        public string description;
        [DataMember]
        public DateTime dateStart;
        [DataMember]
        public DateTime dateEnd;
        [DataMember]
        public int percent;
    }

    [DataContract]
    public class PrintType
    {
        [DataMember]
        public int id;
        [DataMember]
        public string name;
        [DataMember]
        public string description;
    }

    [DataContract]
    public class Person
    {
        [DataMember]
        public int id;
        [DataMember]
        public int state;
        [DataMember]
        public string name;
        [DataMember]
        public string email;
        [DataMember]
        public string telephone;
        [DataMember]
        public DateTime birthdayDate;
        [DataMember]
        public string token;
        [DataMember]
        public DateTime tokenDate;
        [DataMember]
        public string login;
        [DataMember]
        public string password;

    }

    [DataContract]
    public class Order
    {
        [DataMember]
        public int id;
        [DataMember]
        public Person client;
        [DataMember]
        public Service service;
        [DataMember]
        public int count;
        [DataMember]
        public DateTime date;
        [DataMember]
        public string state;
    }

    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public int id;
        [DataMember]
        public Person author;
        [DataMember]
        public Order order;
        [DataMember]
        public string message;
        [DataMember]
        public DateTime date;

    }
}
