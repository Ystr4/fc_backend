using System;
using System.Threading.Tasks;
using Data.Postgres.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stienen.Backend.Services;

namespace Stienen.Backend.Controllers {

    [Route("/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoricDataController : ControllerBase {
        private IDeviceDataService _dataService;

        public HistoricDataController(IDeviceDataService dataService)
        {
            _dataService = dataService;
        }

        // GET /ChartData
        [HttpGet(Name = nameof(GetHistoricData))]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<HistoricDataSet>> GetHistoricData(
                [FromQuery] Guid did,
                [FromQuery] string name,
                [FromQuery] DateTime begin,
                [FromQuery] DateTime end)
        {
            var data = await _dataService.GetHistoricDeviceData(did, name, begin, end);
            if (data == null) {
                return StatusCode(500);
            }

            return data;
        }
    }
}