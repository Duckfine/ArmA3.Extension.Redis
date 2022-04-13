namespace A3Redis.Redis
{
  public interface RedisConnection
  {
    public void Connect();
    public void Disconnect();
    public void Reconnect();
    public bool IsConnected();
    public string SendCommand(int databaseId, string[] args);

#if DEBUG
    public void SendBuildCommand(string[] args);
    public string HandleResponse();
#endif

    public string SetString(int databaseId, string key, string value);
    public string AddToList(int databaseId, string key, string value);
    public string ListUpdate(int databaseId, string key, string index, string value);
    public string KeyDelete(int databaseId, string key);

    public bool KeyExists(int databaseId, string key);
    public string DBKeys(int databaseId, string regex);
    public string DBFlush(int databaseId);
    public string DBSize(int databaseId);
    public string GetEntry(int databaseId, string key);
    public string GetListEntry(int databaseId, string key, string index);
    public string GetListSize(int databaseId, string key);
  }
}
