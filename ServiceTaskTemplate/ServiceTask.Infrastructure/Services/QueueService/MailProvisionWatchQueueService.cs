using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailProvisioner.Domain.Constants;
using MailProvisioner.Domain.Models.Messages;
using MailProvisioner.Domain.Models.WebServices.MessageQueue;
using MailProvisioner.Infrastructure.Infrastructure.Attributes;
using MailProvisioner.Infrastructure.Interfaces.QueueService;
using MailProvisioner.Infrastructure.Pocos.QueueService;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;
using UNC.Extensions.General;
using UNC.Services.Interfaces.Response;
using UNC.Services.Responses;

namespace $ext_projectname$.Infrastructure.Services.QueueService
{
    public class MailProvisionWatchQueueService : BackgroundWatchQueueService, IMailProvisionWatchQueueService
{

        private readonly AsyncRetryPolicy _retryConnection;
        public MailProvisionWatchQueueService(ILogger logger,
            IServiceProvider serviceProvider,
            ConnectionFactory connectionFactory,
            QueueModel queue) : base(logger, serviceProvider, connectionFactory, queue)
        {
            
            _retryConnection = Policy.Handle<BrokerUnreachableException>()
                .WaitAndRetryAsync(10, c => TimeSpan.FromSeconds(10));


          
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            try
            {
                LogBeginRequest();

                stoppingToken.ThrowIfCancellationRequested();

                await _retryConnection.ExecuteAsync(() =>
                {
                    Connection = ConnectionFactory.CreateConnection();

                    Channel = Connection.CreateModel();



                    Channel.ExchangeDeclare(Que.ExchangeName, ExchangeType.Direct, true, false, null);
                    var queueArgs = new Dictionary<string, object>
                {
                    { "x-queue-type", Que.QueueType.ToString().ToLower()}
                };
                    Channel.QueueDeclare(Que.QueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: queueArgs);
                    Channel.QueueBind(Que.QueueName, Que.ExchangeName, Que.QueueName, null);
                    Channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(Channel);


                    consumer.Received += (sender, args) =>
                    {

                        var channel = ((EventingBasicConsumer)sender).Model;
                        var messageBody = Encoding.UTF8.GetString(args.Body.ToArray());

                        Console.WriteLine(new string('*', 60));
                        Console.WriteLine("Message Received");
                        Console.WriteLine(messageBody);
                        Console.WriteLine(new string('*', 60));

                        var request = MessageReceived(messageBody);
                        request.Wait(stoppingToken);

                        if (request.Result is IErrorResponse errorResponse)
                        {
                            LogError(errorResponse.Message);
                            channel.BasicAck(args.DeliveryTag, false);
                        }
                        else if (request.Result is MessageReceivedResponse messageResponse && messageResponse.AcknowledgeMessage)
                        {
                            channel.BasicAck(args.DeliveryTag, false);
                        }

                    };

                    Channel.BasicConsume(Que.QueueName, false, consumer);

                    return Task.CompletedTask;
                });





            }
            catch (Exception ex)
            {
                LogError($"Broken RabbitMq connection. Exception type thrown {ex.GetType()}.");
                Console.WriteLine(ex);

                LogException(ex, true);

            }
        }

        public Task<IResponse> MessageReceived(string messageBody)
        {
            try
            {
                LogBeginRequest();

                if (!messageBody.IsJson())
                {
                    IResponse response = new ErrorResponse($"A message was received but was not in JSON format. Message contents:{messageBody}");
                    return Task.FromResult(response);
                }
                var message = messageBody.FromJson<ProvisionMailboxMessage>();

                //forget the task property bdarley
                //if (message.Task.IsEmpty())
                //{
                //    IResponse response = new ErrorResponse($"A message was received but the destination task was unknown. Message contents:{messageBody}");
                //    return Task.FromResult(response);
                //}

                LogInfo($"Message received. Message contents: {message.ToJson()}");



                //Find the class that will be responsible for handling this message
                var handlerType = GetMessageHandlerForQueue(QueueConstants.MAIL_PROVISION_TASK);
                if (handlerType is null)
                {
                    IResponse response = new ErrorResponse($"Failed to locate message handler, did you forget to decorate a message handler class and provide a queue name to the attributes? Missing handler {QueueConstants.MAIL_PROVISION_TASK}");
                    return Task.FromResult(response);
                }

                var handlerInterface = handlerType.GetInterfaces().SingleOrDefault(c => c.Name.EndsWith("MessageHandler", StringComparison.CurrentCultureIgnoreCase));

                if (handlerInterface is null)
                {
                    IResponse response = new ErrorResponse("Failed to locate message handler, did you forget to implement an interface with a name 'Handler'?");
                    return Task.FromResult(response);
                }

                //we won't use tasks here anymore, just get the one method 'HandleProvisionRequest'
                //var method = handlerInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance).SingleOrDefault(c =>
                //    c.GetCustomAttribute<TaskAttribute>() != null
                //    && c.GetCustomAttribute<TaskAttribute>().Task.EqualsIgnoreCase(message.Task));
                var method = handlerInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance).SingleOrDefault(c => c.Name.Equals("HandleProvisionRequest"));

                if (method is null)
                {
                    IResponse response = new ErrorResponse($"Failed to locate method with task {message.Task}, did you forget to decorate method with the given task attribute?");
                    return Task.FromResult(response);
                }

                //we retrieve the service from DI because there are dependencies we don't want to have to bother with, like logging etc.
                var service = ServiceProvider.GetService(handlerInterface);

                if (service is null)
                {
                    IResponse response = new ErrorResponse("Failed to locate message handler, did you forget to implement an interface with a name 'Handler'?");
                    return Task.FromResult(response);
                }


                var ackMessage = false;

                void AcknowledgeMessage()
                {
                    ackMessage = true;
                }


                var task = (Task)method.Invoke(service, new object[] { message, (Action)AcknowledgeMessage });
                task.Wait();
                var messageReceived = new MessageReceivedResponse
                {
                    AcknowledgeMessage = ackMessage
                };

                return Task.FromResult((IResponse)messageReceived);



            }
            catch (Exception ex)
            {
                IResponse response = LogException(ex, false);
                return Task.FromResult(response);
            }
            finally
            {
                LogEndRequest();
            }
        }
    }
}
