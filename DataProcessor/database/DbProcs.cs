namespace DataProcessor.database {
    public class DbProcs {
        public static int UpdateValues(Guid did, DateTime stamp, DateTime cStamp, StoreType storeType, IEnumerable<DataPart> data)
        {
            return DbExecexProc.ExecMultiProc<DataPart>("UpdateValues", data, "did", did, "stamp", stamp, "cstamp", cStamp, "storeType", (short)storeType);
        }
    }
}