using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A3Redis.Redis.Handler
{
  public class RedisHandler
  {
    public const int BUFFERSIZE = 1024;


    Socket m_socket;
    NetworkStream m_netstream;
    BufferedStream m_buffStream;






    String m_hostname;
    int m_port;
    string m_password;




    public RedisHandler(string hostname, int port, string password)
    {
      m_hostname = hostname;
      m_port = port;
      m_password = password;
    }


    public void Connect()
    {
      this.m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this.m_socket.NoDelay = true;
      this.m_socket.Connect(this.m_hostname, this.m_port);

      if (m_socket.Connected)
      {
        Console.WriteLine("Socket nun zum Server verbunden!");
        this.m_buffStream = new BufferedStream((Stream)new NetworkStream(this.m_socket), 16384);
      }
      else
      {
        this.m_socket.Close();
        this.m_socket = (Socket)null;
        Console.WriteLine("Cant connect to Socket");
      }
    }

    public void Disconnect()
    {
      m_socket.Shutdown(SocketShutdown.Both);
      m_socket.Disconnect(true);
      Console.WriteLine("Verbindung wurde getrennt");
    }

    public void Reconnect()
    {
      Disconnect();
      Connect();
    }


    public void CheckConnection()
    {

    }


    public void SendSimpleCommand(string command)
    {
      StringBuilder output = new StringBuilder("*");

      string[] args = command.Split(' ');
      int len = args.Length;
      output.Append(len + "\r\n");

      foreach (String arg in args)
      {
        len = arg.Length;
        output.Append("$" + len + "\r\n" + arg + "\r\n");
      }


      m_socket.Send(Encoding.UTF8.GetBytes(output.ToString()));

    }


    public string ReadSingleLineFromStream()
    {

      StringBuilder stringBuilder = new StringBuilder();
      int num = m_buffStream.ReadByte();
      while (num != -1)
      {
        num = m_buffStream.ReadByte();
        switch (num)
        {
          case 10: //newline
            stringBuilder.Append("\n");
            break;
          default:
            stringBuilder.Append((char)num);
            continue;
        }
      }
      return stringBuilder.ToString();
    }
  }


















}