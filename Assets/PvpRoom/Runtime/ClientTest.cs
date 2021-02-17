using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Scribble.Runtime.Model.Generated;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Core.Utils;
using UnityEngine;

namespace PvpRoom.Runtime
{
    /// <summary>
	///
	/// </summary>
    internal sealed class ClientTest : MonoBehaviour
    {
        Channel channel;
        Client client = null;

        [SerializeField] string userId = "hoge";
        [SerializeField] string roomId = "";

        [SerializeField] RequestType requestType;
        StreamData streamData;

        [SerializeField, Multiline(4)] string message = "";
        [SerializeField] string recvMsg= "";

        private AsyncDuplexStreamingCall<StreamData, StreamData> msgContext = null;

        private async void Start()
        {

            string msg = "hello world";
            streamData = new StreamData
                {
                    Id = roomId,
                    Type = requestType,
                    Body = ByteString.CopyFrom( Encoding.UTF8.GetBytes(msg))
                };

            // channel = new Channel("127.0.0.1:50000", ChannelCredentials.Insecure);
            channel = new Channel("140.238.63.161:50000", ChannelCredentials.Insecure);
            client = new Client(channel);

            var res = client.Context.CreateRoom(new UserId{ Id = userId });

            Debug.Log($"CreateRoom Responce: {res.Id}, {res.UserCount}");
            roomId = res.Id;

            msgContext = client.Context.JoinRoom();

            Debug.Log("Starting background task to receive messages");
            var readTask = Task.Run(async () =>
            {
                recvMsg = "initialize gprc";

                /*while(true)
                {
                    await msgContext.ResponseStream.ForEachAsync(x => {

                        recvMsg = $"JoinRoom Responce : \n{x.Id}\n{x.Type}\n{Encoding.UTF8.GetString(x.Body.ToByteArray())}";
                        //Debug.Log($"JoinRoom Responce : \n{x.Id}\n{x.Type}\n{Encoding.UTF8.GetString(x.Body.ToByteArray())}");
                        //return null;
                        return msgContext.ResponseStream.;
                    });
                }*/

                while (await msgContext.ResponseStream.MoveNext())
                {
                    var x = msgContext.ResponseStream.Current;
                    recvMsg = $"JoinRoom Responce : \n{x.Id}\n{x.Type}\n{Encoding.UTF8.GetString(x.Body.ToByteArray())}";

                    //Debug.Log($"JoinRoom Responce : \n{res_.Id}\n{res_.Type}\n{Encoding.UTF8.GetString(res_.Body.ToByteArray())}");
                    // Echo messages sent to the service
                }


                //return msgContext.ResponseStream;
                //while (await msgContext.ResponseStream.MoveNext())
                //{
                //    var res_ = msgContext.ResponseStream.Current;
                //    Debug.Log($"JoinRoom Responce : \n{res_.Id}\n{res_.Type}\n{Encoding.UTF8.GetString(res_.Body.ToByteArray())}");
                //    // Echo messages sent to the service
                //}
            });

            //Debug.Log("Starting to send messages");
            //Debug.Log("Type a message to echo then press enter.");

            /*Task.Run(async () => {

                while(true)
                {


                }

            });
            //while (true)
            {

                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    break;
                }


                await msgContext.RequestStream.WriteAsync(streamData);
            }*/

            await readTask;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.LogWarning("shutdown client");
                channel.ShutdownAsync().Wait();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                var res = client.Context.GetRoomList(new Null());
                Debug.Log($"GetRoomList Responce: {res.Id}, {res.UserCount}");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Task.Run(async() =>
                {
                    string message_ = "hello world";
                    var _streamData = new StreamData
                    {
                        Id = userId,
                        Type = requestType,
                        Body = ByteString.CopyFrom( Encoding.UTF8.GetBytes(roomId))
                    };

                    Debug.Log("sample");
                    await msgContext.RequestStream.WriteAsync(_streamData);
                    Debug.Log("sample2");

                    //await msgContext.RequestStream.CompleteAsync();
                    Debug.Log("sample3");
                });
            }
        }
    }
}
