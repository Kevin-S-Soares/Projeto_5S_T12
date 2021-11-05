using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOdontologista.Models.Interfaces
{
    public interface ITimeZoneService
    {
        DateTime CurrentTime();
    }
}
