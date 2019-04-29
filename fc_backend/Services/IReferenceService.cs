using System.Collections.Generic;
using System.Threading.Tasks;
using fc_backend.DataAccess.Models;

namespace fc_backend.Services {
    public interface IReferenceService {
        Task<ReferenceEntity> GetHardwareReferenceByIndexAsync(int hardware, int version, int index);
        Task<ReferenceEntity> GetHardwareReferenceByNameAsync(int hardware, int version, string name);
        Task<List<ReferenceEntity>> GetHardwareReferenceByOffsetAsync(int hardware, int version, int index, int length);
        Task InitializeVersion(int hardware, int version);
    }
}