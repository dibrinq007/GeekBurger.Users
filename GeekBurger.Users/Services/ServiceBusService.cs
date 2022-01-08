using GeekBurger.Users.Extensions;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.Users.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private const string TopicName = "userretrieved";        
        private const string SubscriptionName = "sub_users";

        IServiceBusNamespace _namespace;
        IConfiguration _configuration;        
        ServiceBusConfiguration _serviceBusConfiguration;
        CancellationTokenSource _cancelMessages;      

        public ServiceBusService(IConfiguration configuration)
        {
            _configuration = configuration;
            _serviceBusConfiguration = _configuration.GetSection("servicebus").Get<ServiceBusConfiguration>();
            _namespace = _configuration.GetServiceBusNamespace();
            _cancelMessages = new CancellationTokenSource();            
        }

        public bool CreateTopic()
        {
            try
            {
                if (!_namespace.Topics.List().Any(t => t.Name.Equals(TopicName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _namespace.Topics
                        .Define(TopicName)
                        .WithNewSubscription(SubscriptionName)
                        .WithSizeInMB(1024)
                        .Create();
                }

                var topic = _namespace.Topics.GetByName(TopicName);
                                
                topic.Subscriptions.DeleteByName(SubscriptionName);

                if (!topic.Subscriptions.List()
                       .Any(subscription => subscription.Name
                           .Equals(SubscriptionName, StringComparison.InvariantCultureIgnoreCase)))
                    topic.Subscriptions
                        .Define(SubscriptionName)
                        .Create();                

                return true;
            }
            catch
            {
                return false;
            }
        }    

        public async Task<bool> SendMessagesAsync<T>(T objResponse) where T : class 
        {
            try
            {    
                var _topicClient = new TopicClient(_serviceBusConfiguration.ConnectionString, TopicName);

                string messageBody = JsonSerializer.Serialize(objResponse);
                Message message = new Message(Encoding.UTF8.GetBytes(messageBody));

                await _topicClient.SendAsync(message);

                var closeTask = _topicClient.CloseAsync();
                await closeTask;                

                return true;                              
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
