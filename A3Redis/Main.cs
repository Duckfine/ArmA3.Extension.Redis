using System.Runtime.InteropServices;
using System.Text;
using A3Redis.Redis;

namespace A3Redis
{
  public unsafe class Main
  {
    public static bool notInitialized = true;
    public static RedisExtension RedisExtension;


    [UnmanagedCallersOnly(EntryPoint = "RVExtension")]
    public static unsafe void RVExtension(char* output, int outputSize, char* function)
    {
      if (notInitialized)
        RedisExtension = new RedisExtension(new RedisConnectionTcpClientAdapter("172.18.0.2"));

      string parameter = Marshal.PtrToStringAnsi((IntPtr) function);


      string result = RedisExtension.HandleCommand(parameter);
      byte[] resultBytes = Encoding.ASCII.GetBytes(result + "\0");

      Marshal.Copy(resultBytes, 0, (IntPtr)output, resultBytes.Length);
    }

[UnmanagedCallersOnly(EntryPoint = "RVExtensionVersion")]
    public static unsafe void RVExtensionVersion(char* output, int outputSize)
    {
      string result = RedisExtension.Version();
      byte[] resultBytes = Encoding.ASCII.GetBytes(result + "\0");

      Marshal.Copy(resultBytes, 0, (IntPtr)output, resultBytes.Length);
    }


  }
}