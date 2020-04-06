using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A3Redis.Redis
{
  public class RedisConnection : IDisposable
  {

    private Socket socket;
    private BufferedStream bstream;

    private static byte[] end_data = new byte[2]
    {
      (byte) 13,
      (byte) 10
    };

    public string m_host;
    public int m_port;
    public string m_password;


    private int db;
    private const long UnixEpoch = 621355968000000000;

    public int m_retryTimeout;
    public int m_retryCount;
    public int m_sendTimeout;


    public RedisConnection(String hostname, int port, String password)
    {
      SetDefaults(hostname, port, password);

    }
    public RedisConnection(String hostname, int port, String password, int retryTimeout, int retrys, int sendTimeout)
    {
      this.m_retryTimeout = retryTimeout;
      this.m_retryCount = retrys;
      this.m_sendTimeout = sendTimeout;
      SetDefaults(hostname, port, password);
    }


    private void SetDefaults(string hostname, int port, String password)
    {
      this.m_host = hostname;
      this.m_port = port;
      this.m_password = password;

    }




    public void Connect()
    {
      this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this.socket.NoDelay = true;
      this.socket.SendTimeout = this.m_sendTimeout;
      this.socket.Connect(this.m_host, this.m_port);
      if (!this.socket.Connected)
      {
        this.socket.Close();
        this.socket = (Socket)null;
        Console.WriteLine("Cant connect to Socket");
      }
      else
      {
        Console.WriteLine("Connected to Socket");
        this.bstream = new BufferedStream((Stream)new NetworkStream(this.socket), 16384);
        if (this.m_password == null)
          return;
        //this.SendExpectSuccess("AUTH", (object)this.m_password);
      }
    }


    public void Reconnect()
    {
      
      if(socket.Connected)
      {
        Disconnect();
      }

      Connect();




    }



    public void Disconnect()
    {
      socket.Shutdown(SocketShutdown.Both);
      socket.Disconnect(true);

    }




    public bool CheckConnection()
    {
      SendCommand("CLIENT", "ID");


      string result = ReadLine();
      Console.WriteLine(result);

      //Integer received
      if(result.StartsWith(":"))
      {
        return true;
      }
      else
      {
        return false;
      }
    }














































    #region Sends

    public bool SendCommand(string cmd, params object[] args)
    {
      if (this.socket == null)
        return false;
      int index = 1 + args.Length;
      string s1 = "*" + index.ToString() + "\r\n" + "$" + (object)cmd.Length + "\r\n" + cmd + "\r\n";
      object[] objArray = args;
      for (index = 0; index < objArray.Length; ++index)
      {
        string s2 = objArray[index].ToString();
        int byteCount = Encoding.UTF8.GetByteCount(s2);
        s1 = s1 + "$" + (object)byteCount + "\r\n" + s2 + "\r\n";
      }
      byte[] bytes = Encoding.UTF8.GetBytes(s1);
      try
      {
        this.socket.Send(bytes);
      }
      catch (SocketException ex)
      {
        this.socket.Close();
        this.socket = (Socket)null;
        return false;
      }
      return true;
    }

    public string sendExpectString(string cmd, params object[] args)
    {
      if (!this.SendCommand(cmd, args))
        throw new Exception("Unable to connect");
      int num = this.bstream.ReadByte();

      Console.WriteLine(num);
      if (num == -1)
      {
        throw new RedisException("No more data");
      }

      string str = this.ReadLine();
      if (num == 45)
        throw new RedisException(str.StartsWith("ERR ") ? str.Substring(4) : str);
      if (num == 43)
      {
        return str;
      }


      throw new RedisException("Unknown reply on integer request: " + (object)num + str);
    }


    private int SendExpectInt(string cmd, params object[] args)
    {
      if (!this.SendCommand(cmd, args))
        throw new Exception("Unable to connect");
      int num = this.bstream.ReadByte();
      if (num == -1)
        throw new RedisException("No more data");
      string s = this.ReadLine();
      if (num == 45)
        throw new RedisException(s.StartsWith("ERR ") ? s.Substring(4) : s);
      int result;
      if (num == 58 && int.TryParse(s, out result))
        return result;
      throw new RedisException("Unknown reply on integer request: " + (object)num + s);
    }

    private byte[] SendExpectData(string cmd, params object[] args)
    {
      if (!this.SendCommand(cmd, args))
        throw new Exception("Unable to connect");
      return this.ReadData();
    }



    #endregion Sends




    #region Reads
    public byte[] ReadData()
    {
      string str = this.ReadLine();
      if (str.Length == 0)
        throw new RedisException("Zero length respose");

      switch (str[0])
      {



        case '$':
          if (str == "$-1")
            return (byte[])null;
          int result;
          if (!int.TryParse(str.Substring(1), out result))
            throw new RedisException("Invalid length");
          byte[] buffer = new byte[result];
          int offset = 0;
          do
          {
            int num = this.bstream.Read(buffer, offset, result - offset);
            if (num < 1)
              throw new RedisException("Invalid termination mid stream");
            offset += num;
          }
          while (offset < result);
          if (this.bstream.ReadByte() != 13 || this.bstream.ReadByte() != 10)
            throw new RedisException("Invalid termination");
          return buffer;



        case '-':
          throw new RedisException(str.StartsWith("-ERR ") ? str.Substring(5) : str.Substring(1));
        default:
          throw new RedisException("Unexpected reply: " + str);
      }
    }


    public string ReadLine()
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num;
      while ((num = this.bstream.ReadByte()) != -1)
      {
        switch (num)
        {
          case 10:
            goto label_4;
          case 13:
            continue;
          default:
            stringBuilder.Append((char)num);
            continue;
        }
      }
    label_4:
      return stringBuilder.ToString();
    }

    #endregion  Reads

    #region Setters

    public void SetString(string key, String value)
    {

      this.SetString(key, Encoding.UTF8.GetBytes(value));
    }

    public void SetString(string key, byte[] value)
    {
      this.SendDataCommand("SET", value, (object)key);

      //this.ExpectSuccess();
    }




    public void SetList(string key, int index, string value)
    {
      this.SendCommand("LSET", (object)key, (object)index, (object)value);
    }


    public void RemoveList(string key, int index, string value)
    {
      this.SendCommand("LREM", (object)key, (object)index, (object)value);
    }




    public void RightPush(string key, string value)
    {
      this.RightPush(key, Encoding.UTF8.GetBytes(value));
    }

    public void RightPush(string key, byte[] value)
    {
      this.SendDataCommand("RPUSH", value, (object)key);

    }

    #endregion Setters

    #region Getters



    public string GetString(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));
      return Encoding.UTF8.GetString(this.Get(key));
    }

    public byte[] Get(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));
      return this.SendExpectData("GET", (object)key);
    }

    public byte[] ListIndex(string key, int index)
    {
      SendCommand("LINDEX", (object)key, (object)index);
      return this.ReadData();
    }




    #endregion Getters


    public bool ContainsKey(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));
      return this.SendExpectInt("EXISTS", (object)key) == 1;
    }







    private bool SendDataCommand(string cmd, byte[] data, params object[] args)
    {
      string str = "*" + (1 + args.Length + 1).ToString() + "\r\n" + "$" + (object)cmd.Length + "\r\n" + cmd + "\r\n";

      //*2\r\n$3\r\n$
      foreach (object obj in args)
      {
        string s = obj.ToString();
        int byteCount = Encoding.UTF8.GetByteCount(s);
        str = str + "$" + (object)byteCount + "\r\n" + s + "\r\n";
      }
      string resp = str + "$" + (object)data.Length + "\r\n";
      return this.SendDataResponse(data, resp);
    }

    private bool SendDataResponse(byte[] data, string resp)
    {
      if (this.socket == null)
        this.Connect();
      if (this.socket == null)
        return false;
      byte[] bytes = Encoding.UTF8.GetBytes(resp);
      try
      {
        this.socket.Send(bytes);
        if (data != null)
        {
          this.socket.Send(data);
          this.socket.Send(end_data);
        }
      }
      catch (SocketException ex)
      {
        this.socket.Close();
        this.socket = (Socket)null;
        return false;
      }
      return true;
    }




    public bool Remove(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));
      return this.SendExpectInt("DEL", (object)key) == 1;
    }

    public int Remove(params string[] args)
    {
      if (args == null)
        throw new ArgumentNullException(nameof(args));
      return this.SendExpectInt("DEL", (object[])args);
    }




    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}