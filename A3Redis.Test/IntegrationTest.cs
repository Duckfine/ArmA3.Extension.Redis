using A3Redis.Test.API;
using Xunit;

namespace A3Redis.Test
{
  public class IntegrationTest
  {


    [Fact]
    public void TestVersion()
    {
      Assert.Equal("1.0", ExtensionCaller.CallExtension("version"));
    }

    [Fact]
    public void TestAuthor()
    {
      Assert.Equal("Daniel Gran", ExtensionCaller.CallExtension("author"));
    }

    [Fact]
    public void TestConnect()
    {
      Assert.Equal("success", ExtensionCaller.CallExtension("connect"));
    }

    [Fact]
    public void TestConnection()
    {
      Assert.Equal("success", ExtensionCaller.CallExtension("connect"));
      Assert.Equal("error_already_connected", ExtensionCaller.CallExtension("connect"));
      //Assert.Equal("error_already_connected", ExtensionCaller.CallExtension("connect"));
      //Assert.Equal("success", ExtensionCaller.CallExtension("disconnect"));
      //Assert.Equal("success", ExtensionCaller.CallExtension("connect"));
      //Assert.Equal("success", ExtensionCaller.CallExtension("reconnect"));
      //Assert.Equal("success", ExtensionCaller.CallExtension("reconnect"));
    }


  }
}