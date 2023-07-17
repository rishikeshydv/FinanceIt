
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace gRPCServer
{
    [Authorize]
    public class MessageService : MessageProvider.MessageProviderBase
    {
        public override Task<Empty> PrintMessage(Empty request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;

            Console.WriteLine(user.Identity.Name);

            return Task.FromResult<Empty>(new Empty());
​
        }
    }
}