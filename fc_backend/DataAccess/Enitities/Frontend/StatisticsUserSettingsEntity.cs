using System;
using System.Collections.Generic;

namespace Stienen.Backend.DataAccess.Models.Frontend {
    public class StatisticsUserSettingsEntity {
        public GeneralSettingsEntity GeneralSettings { get; set; }
        public FilterSettingsEntity FilterSettings { get; set; }
        public List<RoundEntity> Rounds { get; set; }
    }
}