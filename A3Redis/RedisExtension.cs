using A3Redis.Redis;


namespace A3Redis
{
  public class RedisExtension
  {
    public const string RETURNTRUE = "TRUE";
    public const string RETURNFALSE = "FALSE";


    public const string RETURN_SUCCESS = "success";
    public const string RETURN_ALREADY_CONNECTED = "error_already_connected";

    private static bool FirstStart = true;

    private static string Hostname = "localhost";
    private static int Port = 6379;
    private static string Password = "";
    private RedisConnection RedisConnection;


    public RedisExtension(RedisConnection connection)
    {
      RedisConnection = connection;
      HandleConnectCommand();
    }

    public static string Version()
    {
      return "1.0";
    }

    public string HandleCommand(string command)
    {
      string ret = "";
      String key, value, index, strresult = "";
      bool isNumeric;
      int intresult, dbid;

      String[] parameter = command.Split(':');

      switch (parameter[0])
      {
        case "version":
          ret = "1.0";
          break;

        case "author":
          ret = "Daniel Gran";
          break;

        case "connect":
          return HandleConnectCommand();
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
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          bool result = RedisConnection.KeyExists(dbid, key);
          return result ? RETURNTRUE : RETURNFALSE;

        case "set":
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          value = parameter[3];
          strresult = RedisConnection.SetString(dbid, key, value);
          return strresult == "OK" ? RETURNTRUE : RETURNFALSE;


        case "get":
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          strresult = RedisConnection.GetEntry(dbid, key);
          return strresult;


        case "dbflush":
          dbid = Int32.Parse(parameter[1]);
          return RedisConnection.DBFlush(dbid);


        case "dbkeys":
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          strresult = RedisConnection.DBKeys(dbid, key);
          return strresult;


        case "dbsize":
          dbid = Int32.Parse(parameter[1]);
          strresult = RedisConnection.DBSize(dbid);
          return strresult;


        case "delete":
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          strresult = RedisConnection.KeyDelete(dbid, key);
          return strresult == "1" ? RETURNTRUE : RETURNFALSE;

        case "listadd":
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          value = parameter[3];
          strresult = RedisConnection.AddToList(dbid, key, value);
          isNumeric = int.TryParse(strresult, out intresult);
          return isNumeric ? RETURNTRUE : RETURNFALSE;

        case "listget":
          dbid = Int32.Parse(parameter[1]);
          index = parameter[2];
          key = parameter[3];
          strresult = RedisConnection.GetListEntry(dbid, index, key);
          return strresult;

        case "listsize":
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          strresult = RedisConnection.GetListSize(dbid, key);
          isNumeric = int.TryParse(strresult, out intresult);
          if (isNumeric)
            return intresult.ToString();
          else
            return RETURNFALSE;

        case "listupdate":
          dbid = Int32.Parse(parameter[1]);
          key = parameter[2];
          index = parameter[3];
          value = parameter[4];
          strresult = RedisConnection.ListUpdate(dbid, key, index, value);

          return strresult == "OK" ? RETURNTRUE : RETURNFALSE;
      }
      return ret;
    }

    private string HandleConnectCommand()
    {
      if (RedisConnection.IsConnected())
        return RETURN_ALREADY_CONNECTED;
      try
      {
        RedisConnection.Connect();
        return RETURN_SUCCESS;
      }
      catch (Exception ex)
      {
        return "Connection error: " + ex.Message;
      }
    }
  }
}