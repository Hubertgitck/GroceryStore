using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class StartProcessingHandler : IRequestHandler<StartProcessing>
{
    private readonly IUnitOfWork _unitOfWork;

    public StartProcessingHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task Handle(StartProcessing request, CancellationToken cancellationToken)
    {
        _unitOfWork.OrderHeader.UpdateStatus(request.Id, Constants.StatusInProcess);
        _unitOfWork.Save();

        return Task.CompletedTask;
    }
}
