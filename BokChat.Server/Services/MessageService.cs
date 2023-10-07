using BokChat.Server.Protos;
using Grpc.Core;

namespace BokChat.Server.Services;

public class MessageService : ChatMessage.ChatMessageBase
{
    private readonly ILogger<MessageService> _logger;

    public MessageService(ILogger<MessageService> logger)
    {
        _logger = logger;
    }

    public override Task<ChatMessageResponse> GetChatMessage(ChatMessageRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ChatMessageResponse { Content = "Nice" });
    }

    public override async Task GetChatMessages(ChatMessagesRequest request, IServerStreamWriter<ChatMessageResponse> responseStream, ServerCallContext context)
    {
        var messages = new List<ChatMessageResponse>
        {
            new ChatMessageResponse
            {
                Content = "1"
            },
            new ChatMessageResponse
            {
                Content= "2"
            }
        };

        foreach (var message in messages)
        {
            await responseStream.WriteAsync(message);
        }
    }
}
