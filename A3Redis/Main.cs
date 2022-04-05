using System.Runtime.InteropServices;
using System.Text;

namespace A3Redis
{
  public class Main
  {
    [UnmanagedCallersOnly(EntryPoint = "RVExtension")]
    public static unsafe void RVExtension(char* output, int outputSize, char* function)
    {
      string parameter = Marshal.PtrToStringAnsi((IntPtr) function);


      string result = A3Redis.Extension(parameter, outputSize);
      byte[] resultBytes = Encoding.ASCII.GetBytes(result);

      Marshal.Copy(resultBytes, 0, (IntPtr)output, resultBytes.Length);
    }
  }
}