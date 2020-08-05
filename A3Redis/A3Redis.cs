using A3Redis.Redis;
using A3Redis_CS.util;
using Maca134.Arma.DllExport;
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

    private static string hostname = "127.0.0.1";
    private static int port = 6379;
    private static string password = "";

    private static RedisHandler connection;










    [ArmaDllExport]
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
      int intresult, dbid;

      //todo connection check with every call


    String[] parameter = function.Split(':');

      switch (parameter[0])
      {
        case "version":
          ret = "A3Redis by Duckfine V0110";
          break;




        case "redis":

          switch (parameter[1])
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

            case "send": //should not be activated --> security issues disable in release
              String[] args = new string[parameter.Length - 2];
              for (int i = 2; i < parameter.Length; i++)
              {
                args[i - 2] = parameter[i];
              }

              connection.SendBuildCommand(args);
              Console.WriteLine(connection.HandleResponse());
              break;

            case "exists": //done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              bool result = connection.KeyExists(dbid, key);
              if (result)
              {
                return RETURNTRUE;
              } else
              {
                return RETURNFALSE;
              }

            case "set": // done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              value = parameter[4];
              strresult = connection.SetString(dbid, key, value);
              if (strresult == "OK")
              {
                return RETURNTRUE;
              } else
              {
                return RETURNFALSE;
              }


            case "get": // done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.GetEntry(dbid, key);
              return strresult;


            case "dbflush": // done
              dbid = Int32.Parse(parameter[2]);
              return connection.DBFlush(dbid);


            case "dbkeys": // done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.DBKeys(dbid, key);
              return  strresult;


            case "dbsize": // done
              dbid = Int32.Parse(parameter[2]);
              strresult = connection.DBSize(dbid);
              return strresult;


            case "delete": // done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.KeyDelete(dbid, key);
              if(strresult == "1")
              {
                return RETURNTRUE; //erfolgreich gelöscht
              } else
              {
                return RETURNFALSE; //eintrag nicht vorhanden -> bereits gelöscht
              }


            case "listadd": //done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              value = parameter[4];
              strresult = connection.AddToList(dbid, key, value);
              isNumeric = int.TryParse(strresult, out intresult);
              if (isNumeric)
              {
                return RETURNTRUE; //erfolgreich eingefügt
              }
              else
              {
                return RETURNFALSE; //nicht erfolgreich eingefügt unnötig?
              }


            case "listget": //done
              dbid = Int32.Parse(parameter[2]);
              index = parameter[3];
              key = parameter[4];
              strresult = connection.ListGetEntry(dbid, index, key);
              return strresult;

            case "listsize": //done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.ListGetSize(dbid, key);
              isNumeric = int.TryParse(strresult, out intresult);
              if (isNumeric)
              {
                return intresult.ToString(); //erfolgreich eingefügt
              }
              else
              {
                return RETURNFALSE; //nicht erfolgreich eingefügt unnötig?
              }


            case "listupdate": //done
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              index = parameter[4];
              value = parameter[5];
              strresult = connection.ListUpdate(dbid, key, index, value);
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




  }
}