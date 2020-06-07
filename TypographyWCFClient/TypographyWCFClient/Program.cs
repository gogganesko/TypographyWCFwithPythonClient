using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using TypographyWCFClient.WorkClassersRef;

namespace TypographyWCFClient
{   
    class Program
    {
        static void Main(string[] args)
        {            
            ClassesClient classesClient = new ClassesClient("BasicHttpBinding_IClasses");
            int isAuth = 0;
            Person currentUser = new Person();
            int userChoice = 0;
            while (true)
            {
                if (isAuth == 0)
                {
                    Console.WriteLine("Что будем делать? \n1-Просмотреть список услуг \n2-Просмотреть список акций \n3-Авторизация \n4-Выход из программы");
                }
                userChoice = Convert.ToInt32(Console.ReadLine());

                if (userChoice == 1)
                {
                    Service[] services = classesClient.getServices();
                    Console.WriteLine("ID||Тип печати||Цена||Название");
                    foreach (Service service in services)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}", Convert.ToString(service.id), Convert.ToString(service.printType.name), Convert.ToString(service.price), Convert.ToString(service.name));
                    }
                }
                if (userChoice == 2)
                {
                    Sale[] sales = classesClient.getSales();
                    Console.WriteLine("ID||Название||Дата начала акции||Дата конца акции||Процент");
                    foreach (Sale sale in sales)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}||{4}", Convert.ToString(sale.id), Convert.ToString(sale.name), Convert.ToString(sale.dateStart), Convert.ToString(sale.dateEnd), Convert.ToString(sale.percent));
                    }
                }
                if (userChoice == 3 && isAuth == 0)
                {

                    Console.WriteLine("Введите логин");
                    string login = Convert.ToString(Console.ReadLine());
                    Console.WriteLine("Введите пароль");
                    string password = Convert.ToString(Console.ReadLine());
                    if (classesClient.auth(login, password) == 1)
                    {
                        isAuth = 1;
                        currentUser = classesClient.getCurrentUser(login, password);                            
                        Console.WriteLine("Вы успешно авторизовались!");
                    }
                    else
                    {
                        Console.WriteLine("Попытка авторизации провалилась!");
                    }
                }

                if (userChoice == 4)
                {
                    Environment.Exit(0);
                }

                //getordersByUser на стороне сервера
                if (userChoice == 5 && isAuth == 1)
                {

                    Order[] myOrders = classesClient.getOrdersOfCurrentUser(currentUser);                        
                    Console.WriteLine("ID||Клиент||Услуга||Количество||Дата||Статус");
                    foreach (Order order in myOrders)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Convert.ToString(order.id), order.client.name, order.service.name, Convert.ToString(order.count), Convert.ToString(order.date), order.state);
                    }
                    
                }
                if (userChoice == 6 && isAuth == 1)
                {

                    Order order = new Order();
                    order.client = currentUser;
                    Service[] services = classesClient.getServices();
                    Console.WriteLine("ID||Тип печати||Цена||Название");
                    foreach (Service service in services)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}", Convert.ToString(service.id), Convert.ToString(service.printType.name), Convert.ToString(service.price), Convert.ToString(service.name));
                    }
                    Console.WriteLine("Выберите ID услуги");
                    int serviceID = Convert.ToInt32(Console.ReadLine());
                    services = classesClient.getServices();
                    order.service = services.First(x => x.id == serviceID);
                    Console.WriteLine("Введите количество");
                    int count = Convert.ToInt32(Console.ReadLine());
                    order.count = count;
                    order.date = DateTime.Now;
                    order.state = "В обработке";
                    classesClient.addOrder(order);
                    Console.WriteLine("Заказ был успешно добавлен!");
                    
                }

                if (userChoice == 7 && isAuth == 1)
                {

                    Order[] myOrders = classesClient.getOrdersOfCurrentUser(currentUser);
                    Console.WriteLine("ID||Клиент||Услуга||Количество||Дата||Статус");
                    foreach (Order order in myOrders)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Convert.ToString(order.id), order.client.name, order.service.name, Convert.ToString(order.count), Convert.ToString(order.date), order.state);
                    }
                    Console.WriteLine("Выберите ID заказа для показа подробной информации");
                    int orderID = Convert.ToInt32(Console.ReadLine());
                    Order o = myOrders.First(x => x.id == orderID);
                    Console.WriteLine("Подробная информация о заказе:\nID:{0}\nВладелец:{1}\nУслуга:{2}\nКоличество:{3}\nДата:{4}\nСтатус:{5}", Convert.ToString(o.id), o.client.name, o.service.name, Convert.ToString(o.count), Convert.ToString(o.date), o.state);
                    
                }

                if (userChoice == 8 && isAuth == 1)
                {

                    ChatMessage chatMessage = new ChatMessage();
                    chatMessage.author = currentUser;
                    Order[] myOrders = classesClient.getOrdersOfCurrentUser(currentUser);
                    Console.WriteLine("ID||Клиент||Услуга||Количество||Дата||Статус");
                    foreach (Order order in myOrders)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Convert.ToString(order.id), order.client.name, order.service.name, Convert.ToString(order.count), Convert.ToString(order.date), order.state);
                    }
                    Console.WriteLine("Выберите заказ в который нужно отправить сообщение");
                    int orderID = Convert.ToInt32(Console.ReadLine());
                    chatMessage.order = myOrders.First(x => x.id == orderID);
                    Console.WriteLine("Введите текст сообщения");
                    chatMessage.message = Convert.ToString(Console.ReadLine());
                    chatMessage.date = DateTime.Now;
                    classesClient.addChatMessage(chatMessage);
                    Console.WriteLine("Сообщение отправлено!");
                    
                }

                if (userChoice == 9 && isAuth == 1)
                {

                    Order[] myOrders = classesClient.getOrdersOfCurrentUser(currentUser);
                    Console.WriteLine("ID||Клиент||Услуга||Количество||Дата||Статус");
                    foreach (Order order in myOrders)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Convert.ToString(order.id), order.client.name, order.service.name, Convert.ToString(order.count), Convert.ToString(order.date), order.state);
                    }
                    Console.WriteLine("Выберите нужный заказ");
                    int orderID = Convert.ToInt32(Console.ReadLine());
                    Order o = myOrders.First(x => x.id == orderID);
                    ChatMessage[] myChatMessages = classesClient.GetChatMessagesOfOrder(o);
                    Console.WriteLine("ID||Автор||ID Заказа||Текст||Дата");
                    foreach (ChatMessage cm in myChatMessages)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}||{4}", Convert.ToString(cm.id), cm.author.name, Convert.ToString(cm.order.id), cm.message, Convert.ToString(cm.date));
                    }
                    
                }

                if (userChoice == 10 && isAuth == 1)
                {

                    Console.WriteLine("Введите начальную дату в формате yyyy-MM-dd");
                    DateTime startDate = Convert.ToDateTime(Console.ReadLine());
                    Console.WriteLine("Введите конечную дату в формате yyyy-MM-dd");
                    DateTime endDate = Convert.ToDateTime(Console.ReadLine());
                    Order[] myOrders = classesClient.getOrdersByDate(currentUser.id, startDate, endDate);
                    foreach (Order order in myOrders)
                    {
                        Console.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Convert.ToString(order.id), order.client.name, order.service.name, Convert.ToString(order.count), Convert.ToString(order.date), order.state);
                    }
                    
                }

                if (userChoice == 11 && isAuth == 1)
                {

                    Console.WriteLine("Введите новый пароль");
                    string pwd = Console.ReadLine();
                    classesClient.changePwd(currentUser.id, pwd);
                    Console.WriteLine("Пароль успешно изменён");
                    
                }

                if (userChoice == 12 && isAuth == 1)
                {

                    classesClient.logout(currentUser.id);
                    isAuth = 0;
                    
                }

                if (isAuth == 1)
                {
                    Console.WriteLine("Что будем делать? \n1 - Просмотреть список услуг \n2 - Просмотреть список акций \n4 - Выход из программы \n5 - Просмотреть мои заказы \n6 - Создать заказ \n7 - Подробная информация о заказе \n8 - Отправить сообщение в чат заказа \n9 - Посмотреть сообщения в чате заказа \n10 - Поиск заказа по датам \n11 - Сменить пароль \n12 - Выйти из профиля");
                }
                
            }
            
        }
    }
}
