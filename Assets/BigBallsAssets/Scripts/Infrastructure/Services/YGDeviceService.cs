using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class YGDeviceService : IDeviceService
{
    public bool IsDecktop => YG2.envir.isDesktop;
}
