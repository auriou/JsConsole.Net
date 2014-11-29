using System.Collections.Generic;
using System.Linq;

namespace JsConsole.Services
{
    public static class Extends
    {
        public static bool Exists(this IEnumerable<ClientConnected> list, string id)
        {
            return list.Any(p => p.Id == id);
        }
        public static ClientConnected GetById(this IEnumerable<ClientConnected> list, string id)
        {
            return list.FirstOrDefault(p => p.Id == id);
        }
        public static List<ClientConnected> Same(this IEnumerable<ClientConnected> list, ClientConnected clientConnected)
        {
            //var returnList = new List<ClientConnected>();
            //foreach (var connected in list.ToList())
            //{
            //    if(connected.IsSame(clientConnected))
            //        returnList.Add(connected);
            //    else
            //    {
            //        Debug.WriteLine(clientConnected + "\n" + connected);
            //    }
            //}
            //return returnList;
            return list.Where(p => p.IsSame(clientConnected)).ToList();
        }
    }
}