using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using VContainer;

public class InputServiceAdapter
{
    private readonly IDeviceService _deviceService;
    private readonly InputService _inputService;
    private readonly MobileInputService _mobileInputService;

    public InputServiceAdapter (IDeviceService deviceService, InputService inputService, MobileInputService mobileInputService)
    {
        _deviceService = deviceService;
        _inputService = inputService;
        _mobileInputService = mobileInputService;
    }

    public IInputService Get()
    {
        if (_deviceService.IsDecktop)
            return _inputService;
        else
            return _mobileInputService;
    }
}
