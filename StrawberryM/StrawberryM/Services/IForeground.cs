using System;
using System.Collections.Generic;
using System.Text;

namespace StrawberryM.Services
{
    public interface IForeground
    {
        void StartService();
        void StopService();
    }
}
