using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace A3Redis.Test
{
  public unsafe class TestMain
  {


    [Fact]
    public unsafe void TestBehaviour()
    {
      const int OUTPUTSIZE = 4096;

      char* output = (char*)NativeMemory.Alloc(OUTPUTSIZE);

      delegate* unmanaged<char*, int, char*, void> bb = &Main.RVExtension;

      fixed (char* str = "version")
      {
        char* input = (char*)NativeMemory.Alloc(OUTPUTSIZE);


        //bb(output, (Int32)OUTPUTSIZE, "asd");


      }







    }
  }
}