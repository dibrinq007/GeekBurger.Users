using GeekBurger.Users.Model;
using System.Threading.Tasks;

namespace GeekBurger.Users.Services
{
    public interface IServiceBusService
    {      
        Task<bool> SendMessagesAsync<T>(T objResponse) where T : class;
    }
}
