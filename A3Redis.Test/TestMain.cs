using A3Redis.Test.API;
using Xunit;

namespace A3Redis.Test
{
  public class TestMain
  {


    [Fact]
    public void TestVersion()
    {
      Assert.Equal("1.0", ExtensionCaller.CallExtension("version"));
    }
  }
}