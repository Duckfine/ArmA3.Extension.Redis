using System;
using System.Runtime.InteropServices;
using System.Text;

namespace A3Redis.Test.API
{
  public unsafe static class ExtensionCaller
  {
    public static string CallExtension(string param)
    {
      try
      {
        const int OUTPUTSIZE = 20000;
        char* output = (char*)NativeMemory.Alloc(OUTPUTSIZE);
        char* argument = (char*)NativeMemory.Alloc(OUTPUTSIZE);
        byte[] paramBytes = Encoding.ASCII.GetBytes(param);


        Marshal.Copy(paramBytes, 0, (IntPtr)argument, paramBytes.Length);
        delegate* unmanaged<char*, int, char*, void> bb = &Main.RVExtension;
        bb(output, (Int32)OUTPUTSIZE, argument);


        string callExtensionResult = Marshal.PtrToStringAnsi((IntPtr)output);
        return callExtensionResult;
      }
      catch (Exception ex)
      {
        throw new CallExtensionException(ex.Message);
      }

    }
  }

  public class CallExtensionException : Exception
  {
    public CallExtensionException(string message)
    {
    }
  }
}
