using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Stienen.Backend.DataAccess;
using Stienen.Backend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Stienen.Backend.Services {
    public class DefaultReferenceService : IReferenceService {
        private AppDataContext _context;

        public DefaultReferenceService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<ReferenceEx> GetHardwareReferenceByIndexAsync(int hardware, int version, int index)
        {
            //            IQueryable<ReferenceEx> query = _context.References;
            //
            //            var result = await query.Where(p => p.Hardware == hardware 
            //                                                && p.Version == version
            //                                                && p.Index == index).SingleAsync();
            //            if (result == null) {
            //                throw new Exception($"Could not find reference, hardware: {hardware}, version: {version}, index: {index}");
            //            }
            //            return result;
            throw new NotImplementedException();
        }

        public async Task<ReferenceEx> GetHardwareReferenceByNameAsync(int hardware, int version, string name)
        {
            //            IQueryable<ReferenceEx> query = _context.References;
            //
            //            var result = await query.Where(p => p.Hardware == hardware
            //                                                && p.Version == version
            //                                                && p.Name == name).SingleAsync();
            //            if (result == null)
            //            {
            //                throw new Exception($"Could not find reference, hardware: {hardware}, version: {version}, name: {name}");
            //            }
            //            return result;
            throw new NotImplementedException();
        }

        public async Task<List<ReferenceEx>> GetHardwareReferenceByOffsetAsync(int hardware, int version, int index, int length)
        {
            //            IQueryable<ReferenceEx> query = _context.References;
            //            List<ReferenceEx> refs = new List<ReferenceEx>();
            //            int count = 0;
            //
            //            while (count < length) {
            //                var item = await GetHardwareReferenceByIndexAsync(hardware, version, index + count);
            //                refs.Add(item);
            //                count += item.Length;
            //            }
            //            
            //            if (refs.Count == 0)
            //            {
            //                throw new Exception($"Could not find reference, hardware: {hardware}, version: {version}");
            //            }
            //            return refs;
            throw new NotImplementedException();

        }

        public async Task InitializeVersion(int hardware, int version)
        {
            //            using (HttpClient client = new HttpClient())
            //            {
            //                try
            //                {
            //                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("FetchReferencesUser,zlvihAFRmFvPS3rS2y6tZOMJBbUqCQq30S9tRJOPZiQ=,9a503270-54da-4841-8a84-9b0a238e9688");
            //                    const string uri = "https://fc215.farmconnect.eu/webservice/Config.svc/GetReferences?hardware={version}&version={version}";
            //                    string responseBody = await client.GetStringAsync(uri);
            //                    var refs = JsonConvert.DeserializeObject<List<ReferenceEx>>(responseBody);
            //
            //                    var hardwareVersion = new HardwareVersionEntity(hardware, version);
            ////                    foreach (ReferenceEx reference in refs) {
            ////                        reference.Version = hardwareVersion;
            ////                        _context.References.Add(reference);
            ////                    }
            //
            //                    var created = await _context.SaveChangesAsync();
            //                    if (created < 1) throw new InvalidOperationException("Could not save changes to ReferencesEntity.");
            //                }
            //                catch (HttpRequestException e)
            //                {
            //                    Console.WriteLine("\nException Caught!");
            //                    Console.WriteLine("Message :{0} ", e.Message);
            //                }
            //            }
            throw new NotImplementedException();
        }
    }
}