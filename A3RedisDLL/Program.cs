using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace A3RedisDLL
{
  public class Program
  {

    [DllExport("RVExtension", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
    public static void RVExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
    {
      output.Append(function);
    }





  }
}
