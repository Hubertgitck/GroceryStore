using Microsoft.AspNetCore.Mvc;

namespace Application.Utility;

public static class TempDataWriter
{
    public static void Write(Controller controller, string key, string message)
    {
        controller.TempData[key] = message;
    }
}
