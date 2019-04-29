using System.Collections.Generic;
using System.Threading.Tasks;
using fc_backend.DataAccess.Models;

namespace fc_backend.Services {
    public interface IReferenceService {
        Task<ReferenceEx> GetHardwareReferenceByIndexAsync(int hardware, int version, int index);
        Task<ReferenceEx> GetHardwareReferenceByNameAsync(int hardware, int version, string name);
        Task<List<ReferenceEx>> GetHardwareReferenceByOffsetAsync(int hardware, int version, int index, int length);
        Task InitializeVersion(int hardware, int version);
    }
}