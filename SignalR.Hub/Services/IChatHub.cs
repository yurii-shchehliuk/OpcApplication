namespace WebApplication1.Services
{
    public interface IChatHub
    {
        Task Enter(string username, string groupName);
        Task Send(object message);
        Task Send(object message, string group);
    }
}