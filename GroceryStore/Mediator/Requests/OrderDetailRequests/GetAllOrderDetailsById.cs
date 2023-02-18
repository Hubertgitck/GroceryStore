﻿namespace ApplicationWeb.Mediator.Requests.OrderDetailRequests;

public class GetAllOrderDetailsById : IRequest<IEnumerable<OrderDetailDto>>
{
    public int Id;

    public GetAllOrderDetailsById(int id)
    {
        Id = id;
    }
}
