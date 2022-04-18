using System;
using System.Runtime.InteropServices;
using System.Text;

namespace A3Redis.Test.API
{
  public static unsafe class ExtensionCaller
  {
    //static delegate* unmanaged<char*, int, char*, void> CALLEXT = &Main.RVExtension;

    [DllImport("A3Redis_x64", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RVExtension(char* output, int outputSize, char* function);
    
    [DllImport("A3Redis_x64", CallingConvention = CallingConvention.Cdecl)]
    public static extern void RVExtensionVersion(char* output, int outputSize);


    public static string CallExtension(string param)
    {
      try
      {
        const int OUTPUTSIZE = 20000;
        char* output = (char*)NativeMemory.Alloc(OUTPUTSIZE);
        char* argument = (char*)NativeMemory.Alloc(OUTPUTSIZE);
        byte[] paramBytes = Encoding.ASCII.GetBytes(param);


        Marshal.Copy(paramBytes, 0, (IntPtr)argument, paramBytes.Length);
        RVExtension(output, (Int32)OUTPUTSIZE, argument);


        string callExtensionResult = Marshal.PtrToStringAnsi((IntPtr)output);
        return callExtensionResult;
      }
      catch (Exception ex)
      {
        throw ex;
      }

    }


    public static string RVExtensionVersion()
    {
      try
      {
        const int OUTPUTSIZE = 20000;
        char* output = (char*)NativeMemory.Alloc(OUTPUTSIZE);
        RVExtensionVersion(output, (Int32)OUTPUTSIZE);
        string callExtensionResult = Marshal.PtrToStringAnsi((IntPtr)output);
        return callExtensionResult;
      }
      catch (Exception ex)
      {
        throw ex;
      }

    }




  }
}
