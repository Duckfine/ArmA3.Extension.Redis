using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3Redis.Redis
{
  public class RedisException : Exception
  {
    public RedisException(string code) : base("Response error")
    {
      this.Code = code;
    }

    public string Code { get; private set; }
  }
}