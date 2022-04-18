using A3Redis.Redis;
using Xunit;

namespace A3Redis.Test.UnitTests
{
  public class KeyExists
  {
    [Fact]
    public void TestExists()
    {
      var str = "exists:0:auth#76561199259412469";

      var re = new RedisExtension(new RedisConnectionTcpClientAdapter());
      var return_str = re.HandleCommand(str);


    }

  }
}
