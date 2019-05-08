using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stienen.Backend.Infrastructure
{
    public interface IEtaggable
    {
        string GetEtag();
    }
}
