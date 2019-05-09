namespace Stienen.Backend.DataAccess.Models {
    public class RemoteComInfoEntity {
        public int ComAddress { get; set; }
        public int SortOrder { get; set; }

        public bool IsComMaster {
            get { return (this.MasterInfoEntity != null); }
        }

        public ComMasterInfoEntity MasterInfoEntity { get; set; }
    }
}