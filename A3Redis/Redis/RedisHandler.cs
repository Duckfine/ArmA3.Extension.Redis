using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A3Redis.Redis
{
  public class RedisHandler
  {
    NetworkStream m_netstream;
    TcpClient m_tcpClient;


    String m_hostname;
    int m_port;
    string m_password;


    public RedisHandler(string hostname, int port, string password)
    {
      m_hostname = hostname;
      m_port = port;
      m_password = password;
    }

    #region Connecting

    public void Connect()
    {
      m_tcpClient = new TcpClient();
      m_tcpClient.Connect(m_hostname, m_port);

      if (m_tcpClient.Connected)
      {
        Console.WriteLine("Socket nun zum Server verbunden!");
        m_netstream = m_tcpClient.GetStream();
      }
      else
      {
        Console.WriteLine("Verbindung fehlgeschlagen!");
      }
    }

    public void Disconnect()
    {
      m_tcpClient.Close();

      Console.WriteLine("Verbindung wurde getrennt");
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
        string[] toSend = {"INFO"};
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
      m_netstream.Write(bytes, 0, bytes.Length);
    }


    private string GetResponse()
    {
      byte[] bytes = new byte[m_tcpClient.ReceiveBufferSize];

      // Read can return anything from 0 to numBytesToRead. 
      // This method blocks until at least one byte is read.
      m_netstream.Read(bytes, 0, (int) m_tcpClient.ReceiveBufferSize);

      // Returns the data received from the host to the console.
      string returndata = Encoding.UTF8.GetString(bytes);

      return returndata;
    }


    public string HandleResponse()
    {
      //For Simple Strings the first byte of the reply is "+"
      //For Errors the first byte of the reply is "-"
      //For Integers the first byte of the reply is ":"
      //For Bulk Strings the first byte of the reply is "$"
      //For Arrays the first byte of the reply is "*"
      String rawResp = GetResponse().Trim('\0');
      rawResp = rawResp.Replace("\n", "");
      String[] proc = rawResp.Split('\r');


      if (proc[0].StartsWith("+")) // Simple String
      {
        Console.WriteLine("Simple String received");
        proc[0] = proc[0].Substring(1);
        return proc[0];
      }
      else if
        (proc[0].StartsWith("-")) // Error
      {
      }
      else if (proc[0].StartsWith(":")) // Integer
      {
        proc[0] = proc[0].Substring(1);
        return proc[0];
      }
      else if (proc[0].StartsWith("$")) // Bulk String (More than 1 string)
      {
        if (proc[0] == "$-1\r") return null; //Null returned
        return proc[1];
      }
      else if (proc[0].StartsWith("*")) // array
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


          if (tmp.ToLower() == "false") // bool?
          {
            toAdd = "false";
          }
          else if (tmp.ToLower() == "true") // bool?
          {
            toAdd = "true";
          }
          else if (tmp.ToLower().StartsWith("{") && tmp.ToLower().EndsWith("}")) // check if code is sent
          {
            toAdd = tmp;
          }
          else
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
      string[] send = {"SELECT", dbid.ToString()};
      SendBuildCommand(send);
      HandleResponse();
      SendBuildCommand(args);
      return HandleResponse();
    }

    #endregion Core


    #region Setters

    public string SetString(int dbid, String key, String value)
    {
      string[] args = {"SET", key, value};
      return SendCommand(dbid, args);
    }

    public string AddToList(int dbid, String key, String value)
    {
      string[] args = {"RPUSH", key, value};
      return SendCommand(dbid, args);
    }

    public string ListUpdate(int dbid, String key, String index, String value)
    {
      string[] args = {"LSET", key, index, value};
      return SendCommand(dbid, args);
    }

    public string KeyDelete(int dbid, String key)
    {
      string[] args = {"DEL", key};
      return SendCommand(dbid, args);
    }

    #endregion Setters

    #region Getters

    public bool KeyExists(int dbid, string key)
    {
      string[] args = {"EXISTS", key};

      string result = SendCommand(dbid, args);

      if (result == "1")
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    public string DBKeys(int dbid, string regex)
    {
      string[] args = {"KEYS", regex};
      var ret = SendCommand(dbid, args);
      ret = ret.Replace("\"\"", "\"");
      return ret;
    }


    public string DBFlush(int dbid)
    {
      string[] args = {"FLUSHDB"};
      return SendCommand(dbid, args);
    }


    public string DBSize(int dbid)
    {
      string[] args = {"DBSIZE"};

      string result = SendCommand(dbid, args);

      if (int.TryParse(result, out int res) || res < 0)
      {
        return res.ToString();
      }

      return "-1";
    }


    public string GetEntry(int dbid, string key)
    {
      string[] args = {"GET", key};

      return SendCommand(dbid, args);
    }


    public string ListGetEntry(int dbid, string key, string index)
    {
      string[] args = {"LINDEX", key, index.ToString()};

      return SendCommand(dbid, args);
    }

    public string ListGetSize(int dbid, string key)
    {
      string[] args = {"LLEN", key};

      return SendCommand(dbid, args);
    }

    #endregion
  }
}