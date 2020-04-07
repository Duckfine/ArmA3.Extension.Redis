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

    todo make checks if parameter is set becaus then the whole server crashes


  todo sorted set
  todo implement logging for redis that every query is logged down to a file

*/

namespace A3Redis
{
  public class A3Redis
  {
    public const string RETURNTRUE = "TRUE";
    public const string RETURNFALSE = "FALSE";









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
      String key, value, index, strresult = "";
      bool isNumeric;

      //todo connection check with every call



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
              connection.Connect(); //todo checks
              return "true";
            case "disconnect":
              connection.Disconnect(); //todo checks
              return "true";
            case "reconnect":
              connection.Reconnect(); //todo checks
              return "true";

            case "send": //should not be activated --> security issues
              String[] args = new string[parameter.Length - 2];
              for (int i = 2; i < parameter.Length; i++)
              {
                args[i - 2] = parameter[i];
              }

              connection.SendBuildCommand(args);
              Console.WriteLine(connection.HandleResponse());
              break;

            case "exists": //done
              key = parameter[2];
              bool result = connection.KeyExists(key);
              if (result)
              {
                return RETURNTRUE;
              } else
              {
                return RETURNFALSE;
              }

            case "set": //done

              key = parameter[2];
              value = parameter[3];
              connection.SetString(key, value);
              strresult = connection.HandleResponse();
              if(strresult == "OK")
              {
                return RETURNTRUE;
              } else
              {
                return RETURNFALSE;
              }
            case "get": //done

              key = parameter[2];
              connection.GetEntry(key);
              strresult = connection.HandleResponse();
              return strresult;



            case "delete": //done
              connection.KeyDelete(parameter[2]);
              strresult = connection.HandleResponse();
              if(strresult == "1")
              {
                return RETURNTRUE; //erfolgreich gelöscht
              } else
              {
                return RETURNFALSE; //eintrag nicht vorhanden -> bereits gelöscht
              }


            case "listadd": //done

              key = parameter[2];
              value = parameter[3];
              connection.AddToList(key, value);
              strresult = connection.HandleResponse();
              isNumeric = int.TryParse(strresult, out int intresult);
              if (isNumeric)
              {
                return RETURNTRUE; //erfolgreich eingefügt
              }
              else
              {
                return RETURNFALSE; //nicht erfolgreich eingefügt unnötig?
              }


            case "listget": //done
              index = parameter[2];
              key = parameter[3];
              connection.ListGetEntry(index, key);
              strresult = connection.HandleResponse();
              return strresult;


            case "listupdate":
              key = parameter[2];
              index = parameter[3];
              value = parameter[4];
              connection.ListUpdate(key, index, value);
              strresult = connection.HandleResponse();
              if(strresult == "OK")
              {
                return RETURNTRUE;
              } else
              {
                return RETURNFALSE;
              }


          }




          break;

         
      }

      
      return ret;
    }


    public static void Initialize()
    {
      connection = new RedisHandler(hostname, port, password);
      connection.Connect();


    }
    public static void Main(string[] args)
    {
      while (true)
      {
        Console.Write(">");
        string input = Console.ReadLine();
        Console.WriteLine(DUCExtension(input, 1024));
      }
    }


  }
}