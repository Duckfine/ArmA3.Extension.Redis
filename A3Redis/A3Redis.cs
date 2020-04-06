using A3Redis.Redis;
using A3Redis.Redis.Handler;
using A3Redis_CS.util;
//using Maca134.Arma.DllExport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



/*

    Format: X:Callytype:arg0:arg1:arg2:...

    Example:
    (ignore case)
    0:VERSION:asdf --> return: "v1.0"

    todo make the console in another thread because the entire game stops when copying something frome the console WTF

*/

namespace A3Redis
{
  public class A3Redis
  {
    private static bool FirstStart = true;

    private static string ret;

    private static string hostname = "192.168.178.50";
    private static int port = 6379;
    private static string password = "";

    private static RedisHandler connection;







    //[ArmaDllExport]
    public static string DUCExtension(string function, int outputSize)
    {
      if (FirstStart)
      {
        FirstStart = false;
        Initialize();
      }

      ret = "";


      String[] parameter = function.Split(':');

      switch (parameter[0])
      {
        case "version":
          ret = "A3Redis by Duckfine V0100";
          break;




        case "redis":

          switch(parameter[1])
          {
            case "connect":
              connection.Connect();
              return "true";
            case "disconnect":
              connection.Disconnect();
              return "true";
            case "reconnect":
              connection.Reconnect();
              return "true";

            case "send":
              connection.SendSimpleCommand(parameter[2]);

              Console.WriteLine(connection.ReadSingleLineFromStream());
              break;

            case "sl":
              Console.WriteLine(connection.ReadSingleLineFromStream());
              break;

          }




          break;

         
      }

      
      return ret;
    }


    public static void Initialize()
    {
      connection = new RedisHandler(hostname, port, password);
      //connection.Connect();

    }
    public static void Main(string[] args)
    {
      while (true)
      {
        Console.Write(">");
        string input = Console.ReadLine();
        DUCExtension(input, 1024);
      }
    }


  }
}