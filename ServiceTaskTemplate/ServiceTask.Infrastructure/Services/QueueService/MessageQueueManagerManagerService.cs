using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using $ext_projectname$.Domain.Models.Messages;
using $ext_projectname$.Domain.Models.WebServices.MessageQueue;
using MailProvisioner.Infrastructure.Infrastructure.EventArgs;
using MailProvisioner.Infrastructure.Interfaces.QueueService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using UNC.Extensions.General;
using UNC.Services;
using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Services.QueueService
{
    public class MessageQueueManagerManagerService : ServiceBase, IMessageQueueManagerService
    {
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }

        private readonly ConnectionFactory _connectionFactory;
        

        public MessageQueueManagerManagerService(ILogger logger, ConnectionFactory connectionFactory) : base(logger)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<IResponse> WriteToQueue(MessageModel message)
        {
            try
            {
                LogBeginRequest();


                var factory = _connectionFactory;

                using var conn = factory.CreateConnection();
                using var channel = conn.CreateModel();


                var queueArgs = new Dictionary<string, object>
                {
                    { "x-queue-type", message.QueueType.ToString().ToLower()}
                };

                channel.ExchangeDeclare(message.ExchangeName, ExchangeType.Direct, true);
                channel.QueueDeclare(message.QueueName, true, false, false, queueArgs);
                channel.QueueBind(message.QueueName, message.ExchangeName, message.QueueName, null);


                var body = Encoding.UTF8.GetBytes(message.Body);
                channel.BasicPublish(message.ExchangeName, message.QueueName, null, body);


                IResponse response = SuccessResponse();
                return Task.FromResult(response);
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

        

        public async Task<IResponse> PurgeMessages(string queueName)
        {
            try
            {
                LogBeginRequest();

                var factory = _connectionFactory;

                return await Task.Run(() =>
                 {
                     using var conn = factory.CreateConnection();
                     using var channel = conn.CreateModel();


                     channel.QueuePurge(queueName);

                     return SuccessResponse();
                 });

            }
            catch (Exception ex)
            {
                return LogException(ex, false);

            }
            finally
            {
                LogEndRequest();
            }
        }

        public async Task<IResponse> GetMessageCount(string queueName)
        {
            try
            {
                LogBeginRequest();

                var factory = _connectionFactory;

                return await Task.Run(() =>
                 {
                     using var conn = factory.CreateConnection();
                     using var channel = conn.CreateModel();

                     var request = channel.MessageCount(queueName);

                     return TypedResponse((int)request);

                 });


            }
            catch (Exception ex)
            {
                return LogException(ex, false);

            }
            finally
            {
                LogEndRequest();
            }
        }

        public async Task<IResponse> GetTopMessage(string queueName, bool ackMessage)
        {
            try
            {
                LogBeginRequest();

                var factory = _connectionFactory;

                return await Task.Run(() =>
                {
                    using var conn = factory.CreateConnection();
                    using var channel = conn.CreateModel();

                    var request = channel.BasicGet(queueName, ackMessage);

                    var messageBody = Encoding.UTF8.GetString(request.Body.ToArray());
                    
                    return TypedResponse(messageBody);

                });


            }
            catch (Exception ex)
            {
                return LogException(ex, false);

            }
            finally
            {
                LogEndRequest();
            }
        }

        public async Task<IResponse> CreateQueue(MessageModel message)
        {
            try
            {
                LogBeginRequest();


                var factory = _connectionFactory;

                return await Task.Run(() =>
                {
                    using var conn = factory.CreateConnection();
                    using var channel = conn.CreateModel();


                    var queueArgs = new Dictionary<string, object>
                    {
                        {"x-queue-type", message.QueueType.ToString().ToLower()}
                    };
                    if (message.QueueSettings != null)
                    {
                        queueArgs = message.QueueSettings;
                    }
                    channel.ExchangeDeclare(message.ExchangeName, ExchangeType.Direct, true);
                    channel.QueueDeclare(message.QueueName, true, false, false, queueArgs);
                    channel.QueueBind(message.QueueName, message.ExchangeName, message.QueueName, null);

                    return SuccessResponse();
                });
                
            }
            catch (Exception ex)
            {
                return  LogException(ex, false);
            }
            finally
            {
                LogEndRequest();
            }
        }

        public Task<IResponse> WriteToRetryQueue(RetryMessageModel retryMessage, MessageModel destinationQueue, BaseMessage message)
        {
            try
            {
                LogBeginRequest();


                var factory = _connectionFactory;

                using var conn = factory.CreateConnection();


                var queueArgs = new Dictionary<string, object>
                {
                    {"x-dead-letter-exchange", destinationQueue.ExchangeName },
                    //{"x-queue-ttl", retryMessage.TimeToLive},
                    {"x-message-ttl",  retryMessage.TimeToLive},
                    { "x-queue-type", retryMessage.QueueType.ToString().ToLower()}
                };
                using var channel = conn.CreateModel();
                channel.ExchangeDeclare(retryMessage.ExchangeName, ExchangeType.Direct, true);
                channel.QueueDeclare(retryMessage.QueueName, true,false, false, queueArgs);
                channel.QueueBind(retryMessage.QueueName, retryMessage.ExchangeName, destinationQueue.QueueName, null);

                

                var body = Encoding.UTF8.GetBytes(message.ToJson(true));
                channel.BasicPublish(retryMessage.ExchangeName, destinationQueue.QueueName, null, body);


                IResponse response = SuccessResponse();
                return Task.FromResult(response);
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

        public Task<IResponse> DeleteQueue(string queueName)
        {
            try
            {
                LogBeginRequest();

                var factory = _connectionFactory;

                using var conn = factory.CreateConnection();
                using var channel = conn.CreateModel();

                channel.QueueDelete(queueName);

                IResponse response = SuccessResponse();
                return Task.FromResult(response);
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

        public Task<IResponse> DeleteExchange(string exchangeName)
        {
            try
            {
                LogBeginRequest();

                var factory = _connectionFactory;

                using var conn = factory.CreateConnection();
                using var channel = conn.CreateModel();

                channel.ExchangeDelete(exchangeName);

                IResponse response = SuccessResponse();
                return Task.FromResult(response);
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


        // ReSharper disable once UnusedMember.Local
        private async Task<IResponse> AttachToQueue(QueueModel queue, CancellationToken cancellationToken)
        {
            try
            {
                LogBeginRequest();

                var factory = _connectionFactory;

                await Task.Run(() =>
                {
                    using var conn = factory.CreateConnection();
                    using var channel = conn.CreateModel();

                    channel.ExchangeDeclare(queue.ExchangeName, ExchangeType.Direct);

                    channel.QueueDeclare(queue.QueueName, true, false, false, null);
                    channel.QueueBind(queue.QueueName, queue.ExchangeName, queue.QueueName, null);


                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {

                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var eventArgs = new MessageReceivedEventArgs
                        {
                            Message = message,
                            AckMessage = () =>
                            {
                                // ReSharper disable once AccessToDisposedClosure
                                channel.BasicAck(ea.DeliveryTag, false);
                            }
                        };
                        MessageReceived?.Invoke(model, eventArgs);


                    };
                    channel.BasicConsume(queue.QueueName, false, consumer);

                    do
                    {
                        Thread.Sleep(1000);
                    } while (channel.IsOpen);
                }, cancellationToken);

                IResponse response = SuccessResponse();
                return response;
            }
            catch (Exception ex)
            {
                return LogException(ex, false);

            }
            finally
            {
                LogEndRequest();
            }

        }
    }
}
