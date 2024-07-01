using MediatR;

namespace lms.buildingblocks.CQRS
{
    public interface IQuery : IQuery<Unit>
    {

    }
    public interface IQuery <out TResponse> : IRequest<TResponse>
    {
    }
}
