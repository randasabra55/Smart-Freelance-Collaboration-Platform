using MediatR;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core.Features.Messages.Dto;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Core.Features.Messages.Queries.GetMessages
{
    //this end point to get chat history for inial laod
    public record GetMessagesQuery(long roomId) : IRequest<Result<List<MessageDto>>>;

    public class GetMessageQueryHandler(Context context)
        : IRequestHandler<GetMessagesQuery, Result<List<MessageDto>>>
    {
        public Task<Result<List<MessageDto>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = context.Messages.Where(m => m.RoomId == request.roomId)
                .Include(m => m.Sender)
                .Select(m => MessageDto.FromEntity(m))
                .ToList();
            return Task.FromResult(Result.Success(messages));
        }
    }
}
