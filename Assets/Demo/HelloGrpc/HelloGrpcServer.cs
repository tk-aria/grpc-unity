using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Google.Protobuf;
using Grpc.Core;

using System;
using System.Collections.Concurrent;


namespace GrpcUnity.Demo
{
    public sealed class HelloGrpcServer : MonoBehaviour
    {
        Server server;

        void Start()
        {
            server = new Server
            {
                Services = { HelloGrpc.BindService(new HelloGrpcServerImplement()) },
                Ports = { new ServerPort("localhost", 50000, ServerCredentials.Insecure) }
            };

            server.Start();

            /*
            while(true)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    await server.ShutdownAsync();
                    Debug.Log("shutdown server");
                }
            }
            */
        }

        void OnDestroy()
        {
            server.ShutdownAsync();
        }
    }

    internal sealed class HelloGrpcServerImplement : HelloGrpc.HelloGrpcBase
    {
        int count = 0;
        ChatRoom _chatroomService;

        public HelloGrpcServerImplement()
        {
            _chatroomService = new ChatRoom();
        }

        public override Task<StreamData> TestResponce(StreamData request, ServerCallContext context)
        {
            //context.

            return Task.FromResult(new StreamData
            {
                Id = $"{request.Id} => response: {count++}",
                Type = request.Type,
                Body = request.Body
            });
        }

        public override async Task TestServerSideStreaming(StreamData request, IServerStreamWriter<StreamData> responseStream, ServerCallContext context)
        {
            await responseStream.WriteAsync(request);
        }

        public override async Task<StreamData> TestClientSideStreaming(IAsyncStreamReader<StreamData> requestStream, ServerCallContext context)
        {

            List<byte> bytes = new List<byte>();
            //await requestStream.ForEachAsync(request =>
            //{
            //    var temp = request.Chunk_.ToByteArray();
            //    bytes.AddRange(temp);
            //    return Task.CompletedTask;
            //});

            while (await requestStream.MoveNext())
            {
                var req = requestStream.Current;
                var temp = req.Body.ToByteArray();
                bytes.AddRange(temp);
            }

            Debug.Log($"size={bytes.Count}");
            // Console.WriteLine(BitConverter.ToString(bytes.ToArray()));

            // 受信完了を返す
            return new StreamData
            {
                Id =  "responce: TestClientSideStreaming",
                Type = 3,
                Body = ByteString.CopyFrom(bytes.ToArray())
            };
        }

        public override async Task TestBidirectionalStreaming(IAsyncStreamReader<StreamData> requestStream, IServerStreamWriter<StreamData> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;

            do
            {
                if (!_chatroomService.HasJoined(requestStream.Current.Id))
                {
                    _chatroomService.Join(requestStream.Current.Id, responseStream);
                }
                await _chatroomService.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());
        }
    }

    public class ChatRoom
    {
        private ConcurrentDictionary<string, IServerStreamWriter<StreamData>> users = new ConcurrentDictionary<string, IServerStreamWriter<StreamData>>();

        public bool HasJoined(string name) => users.ContainsKey(name);
        public void Join(string name, IServerStreamWriter<StreamData> response)
        {
            users.TryAdd(name, response);
            Console.WriteLine($"[INFO] {name} has joined the rooom.");
        }
        public void Remove(string name)
        {
            users.TryRemove(name, out var _);
            Console.WriteLine($"[INFO] {name} has left the room.");
        }

        public async Task BroadcastMessageAsync(StreamData message)
        {
            await BroadcastMessages(message);
            //Console.WriteLine($"[INFO] {message.User} has broadcasted a message '{message.Text}'.");
        }

        private async Task BroadcastMessages(StreamData message)
        {
            foreach (var user in users.Where(x => x.Key != message.Id))
            {
                var item = await SendMessageToSubscriber(user, message);
                if (item != null)
                {
                    Remove(item?.Key);
                };
            }
        }

        private async Task<Nullable<KeyValuePair<string, IServerStreamWriter<StreamData>>>> SendMessageToSubscriber(KeyValuePair<string, IServerStreamWriter<StreamData>> user, StreamData message)
        {
            try
            {
                await user.Value.WriteAsync(message);
                // Console.WriteLine($"[INFO] broadcast message '{message.Text}' from '{message.User}'.");
                return null;
            }
            catch (Exception)
            {
                // Console.WriteLine(ex);
                return user;
            }
        }
    }
}
