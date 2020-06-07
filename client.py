from zeep import Client
import datetime
from datetime import datetime

client = Client('http://localhost:8080/typography/basic?wsdl')
isAuth = 0
userСhoice = 0
currentUser = ""

def print_order(order):
    print('='*45)
    client = order["client"]
    service = order["service"]
    print("ID:%s \nВладелец:%s \nУслуга:%s \nКоличество:%s \nСтатус:%s \n" %(order["id"], client["name"], service["name"], order["count"], order["state"]))

def print_services(services): 
    print("="*45)
    for service in services:
        print("%s - %s - %s Руб." % (service["id"], service["name"], service["price"]))

def print_sales(sales):
    print('='*45)
    for sale in sales:
        print("%s - %s - %s - %s - %s" % (sale["id"], sale["name"], sale["dateStart"], sale["dateEnd"], sale["percent"]))

def print_orders(orders):
    print('='*45)
    for order in orders:
        print("%s - %s" % (order["id"], order["state"]))  


while(True):
    if (isAuth == 0):
        {
            print("Что будем делать? \n1-Просмотреть список услуг \n2-Просмотреть список акций \n3-Авторизация \n4-Выход из программы")
        }
    userСhoice = int(input())
    if (userСhoice == 1):
        services = client.service.getServices()
        print_services(services)
    if (userСhoice == 2):
        sales = client.service.getSales()
        print_sales(sales)
    if (userСhoice == 3):
        login = input("Введите логин\n")
        pwd = input("Введите пароль\n")
        if (client.service.auth(login, pwd) == True):
            isAuth = 1
            currentUser = client.service.getCurrentUser(login, pwd)
            print("Вы успешно авторизовались!")
        else:
            print("Попытка авторизации провалилась!")
    if (userСhoice == 4):
        exit(0)
    if (userСhoice == 5 and isAuth == 1):
        myOrders = client.service.getOrdersOfCurrentUser(currentUser)
        print_orders(myOrders)
    if (userСhoice == 6 and isAuth == 1):
        order = {}
        services = client.service.getServices()
        print_services(services)
        order["client"] = currentUser
        serviceID = input("Выберите ID услуги\n")
        for service in services:
            if (int(serviceID) == service["id"]):
                order["service"] = service
        count = input("Введите количество\n")
        order["count"] = count
        date = datetime.now()
        order["date"] = date
        order["state"] = "В обработке"
        client.service.addOrder(order)
        print("Заказ был успешно добавлен")
    if (userСhoice == 7 and isAuth == 1):
        myOrders = client.service.getOrdersOfCurrentUser(currentUser)
        print_orders(myOrders)
        orderID = int(input("Выберите ID нужного заказа\n"))
        for order in myOrders:
            if (orderID == order["id"]):
                print_order(order)
    if (userСhoice == 8 and isAuth == 1):
        chatMessage = {}
        chatMessage["author"] = currentUser
        myOrders = client.service.getOrdersOfCurrentUser(currentUser)
        print_orders(myOrders)
        orderID = int(input("Выберите ID нужного заказа\n"))
        for order in myOrders:
            if (orderID == order["id"]):
                chatMessage["order"] = order
        chatMessage["message"] = str(input("Введите текст сообщения\n"))
        date = datetime.now()
        chatMessage["date"] = date
        client.service.addChatMessage(chatMessage)
        print("Сообщение отправлено")
    if (userСhoice == 9 and isAuth == 1):
        myOrders = client.service.getOrdersOfCurrentUser(currentUser)
        print_orders(myOrders)
        orderID = int(input("Выберите ID нужного заказа\n"))
        for order in myOrders:
            if (orderID == order["id"]):
                myChatMessages = client.service.GetChatMessagesOfOrder(order)
        for mes in myChatMessages:
            print("%s - %s" % (mes["id"], mes["message"]))
    if (userСhoice == 10 and isAuth == 1):
        startDate = input("Введите начальную дату в формате yyyy-MM-dd\n")
        endDate = input("Введите начальную дату в формате yyyy-MM-dd\n")
        myOrders = client.service.getOrdersByDate(currentUser["id"], startDate, endDate)
        print_orders(myOrders)
    if (userСhoice == 11 and isAuth == 1):
        pwd = input("Введите новый пароль\n")
        client.service.changePwd(currentUser["id"], pwd)
        print("Пароль успешно изменён")
    if (userСhoice == 12 and isAuth == 1):
        client.service.logout(currentUser["id"])
        isAuth = 0
    if (isAuth == 1):
        print("Что будем делать? \n1 - Просмотреть список услуг \n2 - Просмотреть список акций \n4 - Выход из программы \n5 - Просмотреть мои заказы \n6 - Создать заказ \n7 - Подробная информация о заказе \n8 - Отправить сообщение в чат заказа \n9 - Посмотреть сообщения в чате заказа \n10 - Поиск заказа по датам \n11 - Сменить пароль \n12 - Выйти из профиля")
   
        
