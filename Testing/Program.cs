using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using A3Redis;

namespace Testing
{
  class Program
  {

    static void Main(string[] args)
    {

      

      

      bool isRunning = true;
      while(isRunning)
      {
        Console.Write("> ");
        string input = Console.ReadLine();

        switch(input)
        {
          case "exit":
            Environment.Exit(0);
            break;
        }

        Console.WriteLine(A3Redis.A3Redis.DUCExtension(input, 1024));



      }








    }

  }
}
