using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Google.Protobuf;
using Grpc.Core;

namespace GrpcUnity.Demo
{
    public sealed class HelloGrpcClient : MonoBehaviour
    {
        async void Start()
        {
            var channel = this.Connect("127.0.0.1", 50000);

            var client = new HelloGrpc.HelloGrpcClient(channel);

            string message = "hello world";
            var streamData = new StreamData
                {
                    Id = "hoge",
                    Type = 3,
                    Body = ByteString.CopyFrom( Encoding.UTF8.GetBytes(message))
                };

            { // Unary
                var reply = await client.TestResponceAsync(streamData);
                Debug.Log($"responce message : \n{reply.Id}\n{reply.Type}\n{Encoding.UTF8.GetString(reply.Body.ToByteArray())}");
            }

            { // ServerSide Streaming
                //using
                var call = client.TestServerSideStreaming(streamData);

                while (await call.ResponseStream.MoveNext())
                {
                    var res = call.ResponseStream.Current;
                    Debug.Log($"responce serverside message : \n{res.Id}\n{res.Type}\n{Encoding.UTF8.GetString(res.Body.ToByteArray())}");
                }
            }

            { // ClientSide Streaming
                //using
                var call = client.TestClientSideStreaming();

                for (var i = 0; i < 3; i++)
                {
                    await call.RequestStream.WriteAsync(streamData);
                }
                await call.RequestStream.CompleteAsync();

                var res = await call;
                Debug.Log($"responce clientside message : \n{res.Id}\n{res.Type}\n{Encoding.UTF8.GetString(res.Body.ToByteArray())}");
                // Count: 3
            }

            { // Bidirectional Streaming
                //using
                var call = client.TestBidirectionalStreaming();

                Debug.Log("Starting background task to receive messages");
                var readTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var res = call.ResponseStream.Current;
                        Debug.Log($"responce bidirectionalmessage : \n{res.Id}\n{res.Type}\n{Encoding.UTF8.GetString(res.Body.ToByteArray())}");
                        // Echo messages sent to the service
                    }
                });

                Debug.Log("Starting to send messages");
                Debug.Log("Type a message to echo then press enter.");
                //while (true)
                {
                    /*
                    var result = Console.ReadLine();
                    if (string.IsNullOrEmpty(result))
                    {
                        break;
                    }
                    */

                    await call.RequestStream.WriteAsync(streamData);
                }

                //Console.WriteLine("Disconnecting");
                await call.RequestStream.CompleteAsync();
                await readTask;
            }

            await channel.ShutdownAsync();

            Debug.Log("shutdown channel");
        }

        private Channel Connect(string ipAddress, int port) => new Channel($"{ipAddress}:{port}", ChannelCredentials.Insecure);
    }
}
