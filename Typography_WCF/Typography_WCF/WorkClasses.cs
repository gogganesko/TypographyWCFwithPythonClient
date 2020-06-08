using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Typography_WCF
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

    public class WorkClasses : IClasses
    {


        //public int globalServiceID = 0;
        public List<Service> services = new List<Service>();
        public List<Service> getServices()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                string sql = "select * from services";
                SQLiteCommand command = new SQLiteCommand(sql, Connect);
                Connect.Open(); // открыть соединение
                command = new SQLiteCommand(sql, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                services.Clear();
                while (reader.Read())
                {
                    Service service = new Service();
                    service.id = Convert.ToInt32(reader["id"]);
                    service.price = Convert.ToDouble(reader["Price"]);
                    service.name = Convert.ToString(reader["Name"]);
                    printTypes = getPrintTypes();
                    service.printType = printTypes.Find(x => x.id == Convert.ToInt32(reader["PrintTypeId"]));
                    services.Add(service);
                }
                return services;  
            }                
        }

        //public int globalSaleID = 0;
        public List<Sale> sales = new List<Sale>();
        public List<Sale> getSales()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                string sql = "select * from Sales";
                SQLiteCommand command = new SQLiteCommand(sql, Connect);
                Connect.Open(); // открыть соединение
                command = new SQLiteCommand(sql, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                sales.Clear();
                while (reader.Read())
                {
                    Sale sale = new Sale();
                    sale.id = Convert.ToInt32(reader["id"]);
                    sale.name = Convert.ToString(reader["Name"]);
                    sale.percent = Convert.ToInt32(reader["Percent"]);
                    sale.dateStart = Convert.ToDateTime(reader["DateStart"]);
                    sale.dateEnd = Convert.ToDateTime(reader["DateEnd"]);
                    sales.Add(sale);
                }
                return sales;
            }
        }

        //public int globalPrintTypeID = 0;
        private List<PrintType> printTypes = new List<PrintType>();
        private List<PrintType> getPrintTypes()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                string sql = "select * from PrintTypes";
                SQLiteCommand command = new SQLiteCommand(sql, Connect);
                Connect.Open(); // открыть соединение
                command = new SQLiteCommand(sql, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                printTypes.Clear();
                while (reader.Read())
                {
                    PrintType printType = new PrintType();
                    printType.id = Convert.ToInt32(reader["id"]);
                    printType.name = Convert.ToString(reader["Name"]);
                    printTypes.Add(printType);
                }
                return printTypes;
            }
        }

        //public int globaPesronlID = 0;
        private List<Person> persons = new List<Person>();
        private List<Person> getPersons()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                string sql = "select * from Persons";
                SQLiteCommand command = new SQLiteCommand(sql, Connect);
                Connect.Open(); // открыть соединение
                command = new SQLiteCommand(sql, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                persons.Clear();
                while (reader.Read())
                {
                    Person person = new Person();
                    person.id = Convert.ToInt32(reader["id"]);
                    person.state = Convert.ToInt32(reader["State"]);                    
                    person.email = Convert.ToString(reader["Email"]);
                    person.telephone = Convert.ToString(reader["Telephone"]);
                    person.birthdayDate = Convert.ToDateTime(reader["BirthdayDate"]);
                    if ((Convert.ToString(reader["TokenDate"]))!="")
                    {
                        person.tokenDate = Convert.ToDateTime(reader["TokenDate"]);
                    }                    
                    person.login = Convert.ToString(reader["Login"]);
                    person.password = Convert.ToString(reader["Password"]);
                    if ((Convert.ToString(reader["Token"])) != "")
                    {
                        person.token = Convert.ToString(reader["Token"]);
                    }
                    person.name = Convert.ToString(reader["Name"]);
                    persons.Add(person);
                }
                return persons;
            }
        }


        public int auth(string login, string password)//функция авторизации, возвращает 1 - если авторизован, 0 - ошибка
        {
            int isAuth = 0;
            List<Person> allPersons = getPersons();
            Person authuser = allPersons.Find(x => x.login == login && x.password == password);
            if (authuser != null)
            {
                string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); //генерация уникального токена на C#
                authuser.token = token;
                authuser.tokenDate = DateTime.Now;
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
                {

                    string sql = String.Format(@"Update Persons Set Token = ""{0}"", TokenDate = ""{1}"" Where id = {2}", authuser.token, authuser.tokenDate.ToString("yyyy-MM-dd HH:MM"), Convert.ToInt32(authuser.id));
                    SQLiteCommand command = new SQLiteCommand(sql, Connect);
                    Connect.Open(); // открыть соединение
                    command = new SQLiteCommand(sql, Connect);
                    command.ExecuteNonQuery();
                }
                isAuth = 1;
            }
            else
            {
                isAuth = 0;
            }
            return isAuth;
        }

        public int logout(int personID)//функция выхода, возвращает 1 если попытка выхода прошла успешно, 0 - ошибка
        {
            int isLoggedOut = 0;
            List<Person> allPersons = getPersons();
            Person authuser = allPersons.Find(x => x.id == personID);
            if (authuser != null && checkToken(authuser) == true)
            {
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
                {

                    string sql = String.Format(@"Update Persons Set Token = ""{0}"", TokenDate = ""{1}"" Where id = {2}", "", "", personID);
                    SQLiteCommand command = new SQLiteCommand(sql, Connect);
                    Connect.Open(); // открыть соединение
                    command = new SQLiteCommand(sql, Connect);
                    command.ExecuteNonQuery();
                }
                isLoggedOut = 1;
            }
            else
            {
                isLoggedOut = 0;
            }
            return isLoggedOut;
        }


        private bool checkToken(Person user)
        {
            string token = "";
            DateTime tokenDate = new DateTime();
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                string sql = String.Format(@"Select * From Persons where Persons.id = ""{0}""", user.id);
                Connect.Open(); // открыть соединение
                SQLiteCommand command = new SQLiteCommand(sql, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    token = Convert.ToString(reader["Token"]);
                    tokenDate = Convert.ToDateTime(reader["TokenDate"]);
                }
            }
            if (user.token == token && user.tokenDate == tokenDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Person getCurrentUser(string login, string password)
        {
            Person currentUser = getPersons().First(x => x.login == login && x.password == password);
            if (checkToken(currentUser) == true)
            {
                return currentUser;
            }
            else return null;
        }

        public List<Order> getOrdersOfCurrentUser(Person user)
        {
            if (checkToken(user) == true)
            {
                List<Order> userOrders = getOrders().FindAll(x => x.client.id == user.id);
                return userOrders;
            }
            else
            {
                return null;
            }
        }

        public List<ChatMessage> GetChatMessagesOfOrder(Order order)
        {
            List<ChatMessage> chatMessages = getChatMessages().FindAll(x => x.order.id == order.id);
            Person currentUser = getPersons().First(x => x.id == order.client.id);
            if (checkToken(currentUser) == true)
            {
                return chatMessages;
            }
            else return null;
            
        }

        public void changePwd(int personID, string password)//функция смены пароля
        {
            List<Person> allPersons = getPersons();
            Person authuser = allPersons.Find(x => x.id == personID);
            if (checkToken(authuser) == true)
            {
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
                {
                    string sql = String.Format(@"Update Persons Set Password = ""{0}"" Where id = {1}", password, personID);
                    SQLiteCommand command = new SQLiteCommand(sql, Connect);
                    Connect.Open(); // открыть соединение
                    command = new SQLiteCommand(sql, Connect);
                    command.ExecuteNonQuery();
                }
                authuser.password = password;
            }

        }


        private List<Order> orders = new List<Order>();
        private List<Order> getOrders()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                string sql = "select * from Orders";
                SQLiteCommand command = new SQLiteCommand(sql, Connect);
                Connect.Open(); // открыть соединение
                command = new SQLiteCommand(sql, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                orders.Clear();
                while (reader.Read())
                {
                    Order order = new Order();
                    order.id = Convert.ToInt32(reader["id"]);
                    persons = getPersons();
                    order.client = persons.Find(x => x.id == Convert.ToInt32(reader["PersonID"]));
                    services = getServices();
                    order.service = services.Find(x => x.id == Convert.ToInt32(reader["ServiceID"]));
                    order.count = Convert.ToInt32(reader["Count"]);
                    order.date = Convert.ToDateTime(reader["Date"]);
                    order.state = Convert.ToString(reader["State"]);
                    orders.Add(order);
                }
                return orders;
            }
        }

        public void addOrder(Order order)
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                Person currentUser = getPersons().First(x => x.id == order.client.id);
                if (checkToken(currentUser) == true)
                {
                    persons = getPersons();
                    services = getServices();
                    int personID = persons.Find(x => x.id == order.client.id).id;
                    int serviceID = services.Find(x => x.id == order.service.id).id;
                    string sql = String.Format(@"Insert into Orders (PersonID, ServiceID, Count, Date, State) values ({0}, {1}, {2}, ""{3}"", ""{4}"")", personID.ToString(), serviceID.ToString(), order.count.ToString(), order.date.ToString("yyyy-MM-dd HH:MM"), order.state.ToString());
                    SQLiteCommand command = new SQLiteCommand(sql, Connect);
                    Connect.Open(); // открыть соединение
                    command = new SQLiteCommand(sql, Connect);
                    command.ExecuteNonQuery();
                }
            
            }
        }

        public List<Order> getOrdersByDate(int PersonID, DateTime startDate, DateTime endDate)//Получить список заказов в разрезе времени
        {
            Person currentUser = getPersons().First(x => x.id == PersonID);
            if (checkToken(currentUser) == true)
            {
                List<Order> allOrders = getOrders();
                List<Order> resultOrders = allOrders.FindAll(x => x.client.id == PersonID && x.date >= startDate && x.date <= endDate);
                return resultOrders;
            }
            else return null;
        }

        
        private List<ChatMessage> chatMessages = new List<ChatMessage>();
        private List<ChatMessage> getChatMessages()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
            {
                string sql = "select * from ChatMessages";
                SQLiteCommand command = new SQLiteCommand(sql, Connect);
                Connect.Open(); // открыть соединение
                command = new SQLiteCommand(sql, Connect);
                SQLiteDataReader reader = command.ExecuteReader();
                orders.Clear();
                while (reader.Read())
                {
                    ChatMessage chatMessage = new ChatMessage();
                    chatMessage.id = Convert.ToInt32(reader["id"]);
                    persons = getPersons();
                    chatMessage.author = persons.Find(x => x.id == Convert.ToInt32(reader["PersonID"]));
                    orders = getOrders();
                    chatMessage.order = orders.Find(x => x.id == Convert.ToInt32(reader["OrderID"]));
                    chatMessage.message = Convert.ToString(reader["Message"]);
                    chatMessage.date = Convert.ToDateTime(reader["Date"]);
                    chatMessages.Add(chatMessage);
                }
                return chatMessages;
            }
        }

        public void addChatMessage(ChatMessage chatMessage)
        {
            Person currentUser = getPersons().First(x => x.id == chatMessage.author.id);
            if (checkToken(currentUser) == true)
            {
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
                {
                    persons = getPersons();
                    orders = getOrders();
                    int personID = persons.Find(x => x.id == chatMessage.author.id).id;
                    int orderID = orders.Find(x => x.id == chatMessage.order.id).id;
                    string sql = String.Format(@"Insert into ChatMessages (PersonID, OrderID, Message, Date) values ({0}, {1}, ""{2}"", ""{3}"")", personID.ToString(), orderID.ToString(), chatMessage.message.ToString(), chatMessage.date.ToString("yyyy-MM-dd HH:MM"));
                    SQLiteCommand command = new SQLiteCommand(sql, Connect);
                    Connect.Open(); // открыть соединение
                    command = new SQLiteCommand(sql, Connect);
                    command.ExecuteNonQuery();
                }
            }
        }



        private void checkDB()
        {
            if (!File.Exists(@"Typography.db")) // если базы данных нету, то...
            {
                SQLiteConnection.CreateFile(@"Typography.db"); // создать базу данных, по указанному пути содаётся пустой файл базы данных
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=Typography.db; Version=3"))
                {
                    string commandText = @"BEGIN TRANSACTION;
                    CREATE TABLE IF NOT EXISTS ""Persons"" (
	                    ""id""	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                    ""State""	INTEGER,
	                    ""Email""	TEXT,
	                    ""Telephone""	TEXT,
	                    ""BirthdayDate""	TEXT,
	                    ""TokenDate""	TEXT,
	                    ""Login""	TEXT,
	                    ""Password""	TEXT,
	                    ""Token""	TEXT,
	                    ""Name""	TEXT
                    );
                    CREATE TABLE IF NOT EXISTS ""Orders"" (
	                    ""id""	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                    ""PersonID""	INTEGER,
	                    ""ServiceID""	INTEGER,
	                    ""Count""	INTEGER,
	                    ""Date""	DATETIME,
	                    ""State""	TEXT,
	                    FOREIGN KEY(""ServiceID"") REFERENCES ""Services""(""id""),
	                    FOREIGN KEY(""PersonID"") REFERENCES ""Persons""(""id"")
                    );
                    CREATE TABLE IF NOT EXISTS ""Sales"" (
	                    ""id""	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                    ""Name""	TEXT,
	                    ""DateStart""	TEXT,
	                    ""DateEnd""	TEXT,
	                    ""Percent""	INTEGER
                    );
                    CREATE TABLE IF NOT EXISTS ""PrintTypes"" (
	                    ""id""	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                    ""Name""	TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS ""ChatMessages"" (
	                    ""id""	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                    ""PersonID""	INTEGER,
	                    ""OrderID""	INTEGER,
	                    ""Message""	TEXT,
	                    ""Date""	DATETIME,
	                    FOREIGN KEY(""PersonID"") REFERENCES ""Persons""(""id""),
	                    FOREIGN KEY(""OrderID"") REFERENCES ""Orders""(""id"")
                    );
                    CREATE TABLE IF NOT EXISTS ""Services"" (
	                    ""id""	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                    ""PrintTypeID""	INTEGER NOT NULL,
	                    ""Price""	DOUBLE,
	                    ""Name""	TEXT,
	                    FOREIGN KEY(""PrintTypeID"") REFERENCES ""PrintTypes""(""id"")
                    );
                    INSERT INTO ""Persons"" VALUES (1,1,'asd@asd.ru','88005553535','20-10-1997','2020-05-28 20:05','admin','admin','V7vKY2nUk0ejuJ+qTkvrdw==','Ivanov');
                    INSERT INTO ""Persons"" VALUES (2,2,'asd@asd.ru','89005543636','19-11-1997','','test','test','','Petrov');
                    INSERT INTO ""Orders"" VALUES (3,1,1,10,'2020-05-28 18:05','В обработке');
                    INSERT INTO ""Sales"" VALUES (1,'Две кружки по цене одной','11-10-2019','15-10-2019',50);
                    INSERT INTO ""Sales"" VALUES (2,'Две футболки по цене одной','14-10-2019','30-10-2019',50);
                    INSERT INTO ""PrintTypes"" VALUES (1,'Печать на посуде');
                    INSERT INTO ""PrintTypes"" VALUES (2,'Печать на бумаге');
                    INSERT INTO ""PrintTypes"" VALUES (3,'Печать на одежде');
                    INSERT INTO ""ChatMessages"" VALUES (1,1,3,'dfshkjdfshkdfshksdjfh','2020-05-28 19:05');
                    INSERT INTO ""ChatMessages"" VALUES (2,1,3,'qwewqoeiwqoe','2020-05-28 19:05');
                    INSERT INTO ""Services"" VALUES (1,1,50.0,'Печать на кружках');
                    INSERT INTO ""Services"" VALUES (2,1,80.0,'Печать на тарелках');
                    COMMIT;
                    ";
                    SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                    Connect.Open(); // открыть соединение
                    Command.ExecuteNonQuery(); // выполнить запрос
                    Connect.Close(); // закрыть соединение
                }
            }

        }
    }
}

