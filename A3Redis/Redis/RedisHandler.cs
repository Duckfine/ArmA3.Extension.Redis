using System.Net.Sockets;
using System.Text;

namespace A3Redis.Redis
{
  public class RedisHandler
  {
    private const string STANDARD_HOSTNAME = "localhost";
    private const int STANDARD_PORT = 6379;
    private const string STANDARD_PASSWORD = "";

    TcpClient TcpClient;
    private NetworkStream NetworkStream;

    private readonly string Hostname;
    private readonly int Port;
    string Password;


    public RedisHandler(string hostname=STANDARD_HOSTNAME, int port = STANDARD_PORT, string password = STANDARD_PASSWORD)
    {
      Hostname = hostname;
      Port = port;
      Password = password;
    }

    #region Connecting
    public void Connect()
    {
      TcpClient = new TcpClient();
      TcpClient.Connect(Hostname, Port);
      NetworkStream = TcpClient.GetStream();
    }

    public void Disconnect()
    {
      TcpClient.Close();
    }

    public void Reconnect()
    {
      Disconnect();
      Connect();
    }

    public bool CheckConnection()
    {
      try
      {
        string[] toSend = { "INFO" };
        SendBuildCommand(toSend);
        //todo handle response
        return true;
      }
      catch
      {
        return false;
      }
    }
    #endregion Connecting


    #region Core
    public void SendBuildCommand(String[] args)
    {
      StringBuilder output = new StringBuilder("*");
      output.Append(args.Length + "\r\n");

      foreach (string item in args)
      {
        output.Append("$" + item.Length + "\r\n" + item + "\r\n");
      }

      String toSend = output.ToString();
      byte[] bytes = Encoding.UTF8.GetBytes(toSend);
      NetworkStream.Write(bytes, 0, bytes.Length);
    }

    private string GetResponse()
    {
      byte[] bytes = new byte[TcpClient.ReceiveBufferSize];
      NetworkStream.Read(bytes, 0, (int)TcpClient.ReceiveBufferSize);
      string returndata = Encoding.UTF8.GetString(bytes);
      return returndata;
    }

    public string HandleResponse()
    {
      String rawResp = GetResponse().Trim('\0');
      rawResp = rawResp.Replace("\n", "");
      String[] proc = rawResp.Split('\r');

      if (proc[0].StartsWith("+")) // Simple String received
      {
        proc[0] = proc[0].Substring(1);
        return proc[0];
      }
      else if (proc[0].StartsWith("-")) { } // Error received
      else if (proc[0].StartsWith(":")) // Integer received
      {
        proc[0] = proc[0].Substring(1);
        return proc[0];
      }
      else if (proc[0].StartsWith("$")) // Bulk String (More than 1 string) received
      {
        if (proc[0] == "$-1\r") return null; // Null received 
        return proc[1];
      }
      else if (proc[0].StartsWith("*")) // Array received
      {
        int arrSize;
        if (!Int32.TryParse(proc[0].Substring(1), out arrSize)) return rawResp; // Failed
        string[] arr = String.Join("", proc).Split('$');
        if (!int.TryParse(arr[0].Replace("*", ""), out arrSize)) return rawResp; // Failed

        if (arrSize <= 0) return "[]";

        string result = "["; // like [1,2,4] in an compilable a3 array
        for (int i = 1; i < arrSize + 1; i++)
        {
          int sub;
          string tmp;
          string toAdd;
          if (arr[i].Length < 12) // delete not needed redis chars
          {
            sub = 1;
          }
          else
          {
            sub = 2;
          }

          tmp = arr[i].Substring(sub);

          if (tmp.ToLower() == "false")
          {
            toAdd = "false";
          }
          else if (tmp.ToLower() == "true")
          {
            toAdd = "true";
          }
          else if (tmp.ToLower().StartsWith("{") && tmp.ToLower().EndsWith("}")) // A3 Datatype Code?
          {
            toAdd = tmp;
          }
          else // Its a string, so put parentheses around it.
          {
            toAdd = "\"\"" + tmp + "\"\"";
          }
          result = result + toAdd + ", ";
        }
        result = result.Substring(0, result.Length - 2);
        result += "]";
        return result;
      }
      return rawResp;
    }

    private string SendCommand(int dbid, string[] args)
    {
      string[] send = { "SELECT", dbid.ToString() };
      SendBuildCommand(send);
      HandleResponse();
      SendBuildCommand(args);
      return HandleResponse();
    }
    #endregion Core


    #region Setters
    public string SetString(int dbid, String key, String value)
    {
      string[] args = { "SET", key, value };
      return SendCommand(dbid, args);
    }

    public string AddToList(int dbid, String key, String value)
    {
      string[] args = { "RPUSH", key, value };
      return SendCommand(dbid, args);
    }

    public string ListUpdate(int dbid, String key, String index, String value)
    {
      string[] args = { "LSET", key, index, value };
      return SendCommand(dbid, args);
    }

    public string KeyDelete(int dbid, String key)
    {
      string[] args = { "DEL", key };
      return SendCommand(dbid, args);
    }

    #endregion Setters


    #region Getters

    public bool KeyExists(int dbid, string key)
    {
      string[] args = { "EXISTS", key };
      string result = SendCommand(dbid, args);
      return result == "1" ? true : false;
    }

    public string DBKeys(int dbid, string regex)
    {
      string[] args = { "KEYS", regex };
      var ret = SendCommand(dbid, args);
      ret = ret.Replace("\"\"", "\"");
      return ret;
    }

    public string DBFlush(int dbid)
    {
      string[] args = { "FLUSHDB" };
      return SendCommand(dbid, args);
    }

    public string DBSize(int dbid)
    {
      string[] args = { "DBSIZE" };
      string result = SendCommand(dbid, args);
      if (int.TryParse(result, out int res) || res < 0)
        return res.ToString();
      return "-1";
    }

    public string GetEntry(int dbid, string key)
    {
      string[] args = { "GET", key };
      return SendCommand(dbid, args);
    }

    public string ListGetEntry(int dbid, string key, string index)
    {
      string[] args = { "LINDEX", key, index.ToString() };
      return SendCommand(dbid, args);
    }

    public string ListGetSize(int dbid, string key)
    {
      string[] args = { "LLEN", key };
      return SendCommand(dbid, args);
    }
    #endregion
  }
}