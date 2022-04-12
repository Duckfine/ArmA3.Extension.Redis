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

    private static string Hostname = "172.18.0.2";
    private static int Port = 6379;
    private static string Password = "";
    private static RedisHandler RedisConnection;

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

      String[] parameter = function.Split(':');

      switch (parameter[0])
      {
        case "version":
          ret = "1.0";
          break;

        default:
          switch (parameter[1])
          {
            case "connect":
              RedisConnection.Connect();
              return RETURNTRUE;
            case "disconnect":
              RedisConnection.Disconnect();
              return RETURNTRUE;
            case "reconnect":
              RedisConnection.Reconnect();
              return RETURNTRUE;

#if DEBUG
            case "send":
              String[] args = new string[parameter.Length - 2];
              for (int i = 2; i < parameter.Length; i++)
                args[i - 2] = parameter[i];

              RedisConnection.SendBuildCommand(args);
              Console.WriteLine(RedisConnection.HandleResponse());
              break;
#endif
            case "exists":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              bool result = RedisConnection.KeyExists(dbid, key);
              return result ? RETURNTRUE : RETURNFALSE;

            case "set":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              value = parameter[4];
              strresult = RedisConnection.SetString(dbid, key, value);
              return strresult == "OK" ? RETURNTRUE : RETURNFALSE;


            case "get":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = RedisConnection.GetEntry(dbid, key);
              return strresult;


            case "dbflush":
              dbid = Int32.Parse(parameter[2]);
              return RedisConnection.DBFlush(dbid);


            case "dbkeys":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = RedisConnection.DBKeys(dbid, key);
              return strresult;


            case "dbsize":
              dbid = Int32.Parse(parameter[2]);
              strresult = RedisConnection.DBSize(dbid);
              return strresult;


            case "delete":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = RedisConnection.KeyDelete(dbid, key);
              return strresult == "1" ? RETURNTRUE : RETURNFALSE;

            case "listadd":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              value = parameter[4];
              strresult = RedisConnection.AddToList(dbid, key, value);
              isNumeric = int.TryParse(strresult, out intresult);
              return isNumeric ? RETURNTRUE : RETURNFALSE;

            case "listget":
              dbid = Int32.Parse(parameter[2]);
              index = parameter[3];
              key = parameter[4];
              strresult = RedisConnection.ListGetEntry(dbid, index, key);
              return strresult;

            case "listsize":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              strresult = RedisConnection.ListGetSize(dbid, key);
              isNumeric = int.TryParse(strresult, out intresult);
              if (isNumeric)
                return intresult.ToString();
              else
                return RETURNFALSE;

            case "listupdate":
              dbid = Int32.Parse(parameter[2]);
              key = parameter[3];
              index = parameter[4];
              value = parameter[5];
              strresult = RedisConnection.ListUpdate(dbid, key, index, value);

              return strresult == "OK" ? RETURNTRUE : RETURNFALSE;
          }
          break;
      }
      return ret;
    }


    public static void Initialize()
    {
      RedisConnection = new RedisHandler(Hostname, Port, Password);
    }
  }
}