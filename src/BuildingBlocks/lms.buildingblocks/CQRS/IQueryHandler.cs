using MediatR;

namespace lms.buildingblocks.CQRS
{
    public interface IQueryHandler<in IQuery, TResponse> 
        : IRequestHandler<IQuery, TResponse>
        where TResponse : notnull
        where IQuery : IQuery<TResponse>
    {
    }
}
