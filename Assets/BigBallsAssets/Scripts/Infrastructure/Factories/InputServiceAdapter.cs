using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using VContainer;

public class InputServiceAdapter
{
    private readonly IDeviceService _deviceService;
    private readonly IObjectResolverProvider _resolver;

    public InputServiceAdapter (IDeviceService deviceService, IObjectResolverProvider resolver)
    {
        _deviceService = deviceService;
        _resolver = resolver;
    }

    public IInputService Get()
    {
        if (_deviceService.IsDecktop)
            return _resolver.CurrentResolver.Resolve<InputService>();
        else
            return _resolver.CurrentResolver.Resolve<MobileInputService>();
    }
}
