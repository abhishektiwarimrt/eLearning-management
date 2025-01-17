﻿using MediatR;

namespace lms.buildingblocks.CQRS
{
    public interface ICommand : ICommand<Unit>
    {

    }
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
