using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
namespace Typography_WCF
{
    [ServiceContract]
    public interface IClasses
    {
        [OperationContract]
        List<ChatMessage> GetChatMessagesOfOrder(Order order);

        [OperationContract]
        Person getCurrentUser(string login, string password);

        [OperationContract]
        List<Order> getOrdersOfCurrentUser(Person user);

       // [OperationContract]
       // bool checkToken(Person user);

        [OperationContract]
        void changePwd(int personID, string password);

      //  [OperationContract]
        //void checkDB();

        [OperationContract]
        void addChatMessage(ChatMessage chatMessage);
       // [OperationContract]
       // void delChatMessage(int id);
        //[OperationContract]
       // List<ChatMessage> getChatMessages();

        [OperationContract]
        void addOrder(Order order);

        [OperationContract]
        List<Order> getOrdersByDate(int PersonID, DateTime startDate, DateTime endDate);

       // [OperationContract]
       // List<Order> getOrders();

       // [OperationContract]
       // List<Person> getPersons();

        //[OperationContract]
       // void addPerson(Person person);

        [OperationContract]
        int auth(String login, string password);

        [OperationContract]
        int logout(int personID);

        //[OperationContract]
       // List<PrintType> getPrintTypes();

        [OperationContract]
        List<Sale> getSales();

        [OperationContract]
        List<Service> getServices();
    }
}
