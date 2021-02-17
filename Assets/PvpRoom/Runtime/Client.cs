using System;
using System.Collections;
using System.Collections.Generic;
using Scribble.Runtime.Model.Generated;
//using grpc = global::Grpc.Core;
using Grpc.Core;

namespace PvpRoom.Runtime
{
    /// <summary>
	///
	/// </summary>
    internal sealed class Client
    {
        public Scribble.Runtime.Model.Generated.PvpRoom.PvpRoomClient Context;

        public Client(Channel channel)
        {
            Context = new Scribble.Runtime.Model.Generated.PvpRoom.PvpRoomClient(channel);
        }
    }
}
