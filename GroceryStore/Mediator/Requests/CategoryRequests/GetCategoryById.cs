﻿namespace ApplicationWeb.Mediator.Requests.CategoryRequests;

public class GetCategoryById : IRequest<CategoryDto>
{
    public int? Id;

    public GetCategoryById(int? id)
    {
        Id = id;
    }
}