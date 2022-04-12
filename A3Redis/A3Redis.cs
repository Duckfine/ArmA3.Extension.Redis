using A3Redis.Redis;

/*

    Format: X:Callytype:arg0:arg1:arg2:...

    Example:
    (ignore case)
    0:VERSION:asdf --> return: "v1.0"

*/

namespace A3Redis
{
  public class A3Redis
  {
    public const string RETURNTRUE = "TRUE";
    public const string RETURNFALSE = "FALSE";


    private static bool FirstStart = true;

    private static string hostname = "172.18.0.2";
    private static int port = 6379;
    private static string password = "";

    private static RedisHandler connection;


    public static string Extension(string function, int outputSize)
    {
      if (FirstStart)
      {
        FirstStart = false;
        Initialize();
      }

      string ret = "";
      String key, value, index, strresult = "";
      bool isNumeric;
      int intresult, dbid;

      //todo connection check with every call


      String[] parameter = function.Split(':');

      switch (parameter[0])
      {
        case "version":
          ret = "1.0";
          break;


        case "redis":

          switch (parameter[1])
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
#if DEBUG
            case "send":
              String[] args = new string[parameter.Length - 2];
              for (int i = 2; i < parameter.Length; i++)
                args[i - 2] = parameter[i];

              connection.SendBuildCommand(args);
              Console.WriteLine(connection.HandleResponse());
              break;
#endif
            case "exists":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              bool result = connection.KeyExists(dbid, key);
              if (result)
              {
                return RETURNTRUE;
              }
              else
              {
                return RETURNFALSE;
              }

            case "set":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              value = parameter[4];
              strresult = connection.SetString(dbid, key, value);
              if (strresult == "OK")
              {
                return RETURNTRUE;
              }
              else
              {
                return RETURNFALSE;
              }


            case "get":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.GetEntry(dbid, key);
              return strresult;


            case "dbflush":
              dbid = Int32.Parse(parameter[2]);
              return connection.DBFlush(dbid);


            case "dbkeys":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.DBKeys(dbid, key);
              return strresult;


            case "dbsize":
              dbid = Int32.Parse(parameter[2]);
              strresult = connection.DBSize(dbid);
              return strresult;


            case "delete":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.KeyDelete(dbid, key);
              if (strresult == "1")
              {
                return RETURNTRUE;
              }
              else
              {
                return RETURNFALSE;
              }


            case "listadd":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              value = parameter[4];
              strresult = connection.AddToList(dbid, key, value);
              isNumeric = int.TryParse(strresult, out intresult);
              if (isNumeric)
              {
                return RETURNTRUE;
              }
              else
              {
                return RETURNFALSE;
              }


            case "listget":
              dbid = Int32.Parse(parameter[2]);
              index = parameter[3];
              key = parameter[4];
              strresult = connection.ListGetEntry(dbid, index, key);
              return strresult;

            case "listsize":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = connection.ListGetSize(dbid, key);
              isNumeric = int.TryParse(strresult, out intresult);
              if (isNumeric)
              {
                return intresult.ToString();
              }
              else
              {
                return RETURNFALSE;
              }


            case "listupdate":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              index = parameter[4];
              value = parameter[5];
              strresult = connection.ListUpdate(dbid, key, index, value);
              if (strresult == "OK")
              {
                return RETURNTRUE;
              }
              else
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
    }
  }
}