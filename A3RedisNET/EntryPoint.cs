using System.Runtime.InteropServices;
using System.Text;

namespace A3RedisNET
{
  public class EntryPoint
  {
    [UnmanagedCallersOnly(EntryPoint = "RVExtension")]
    public unsafe static void RVExtension(char* output, int outputSize, char* function)
    {
      string parameter = Marshal.PtrToStringAnsi((IntPtr) function);


      string result = A3Redis.A3Redis.DUCExtension(parameter, outputSize);
      byte[] resultBytes = Encoding.ASCII.GetBytes(result);

      Marshal.Copy(resultBytes, 0, (IntPtr)output, resultBytes.Length);
    }
  }
}