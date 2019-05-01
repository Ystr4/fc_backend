using System;
using System.Data;

namespace Stienen.Database {
	public interface IAsyncDbCommand: IDbCommand {
		IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object state);
		int EndExecuteNonQuery(IAsyncResult result);

		IAsyncResult BeginExecuteReader(AsyncCallback callback, object state);
		IAsyncResult BeginExecuteReader(CommandBehavior behaviour, AsyncCallback callback, object state);
		IDataReader EndExecuteReader(IAsyncResult result);

		IAsyncResult BeginExecuteScalar(AsyncCallback callback, object state);
		object EndExecuteScalar(IAsyncResult result);
	}

	public class NonAsyncDbCommand: IDbCommand, IDisposable {
		protected IDbCommand Cmd;
		private NonAsyncDbCommand() { }
		public NonAsyncDbCommand(IDbCommand cmd) {
			Cmd=cmd;
		}

        ~NonAsyncDbCommand()
        {
            Dispose(false);
        }

		public void Cancel() {
			Cmd.Cancel();
		}

		public string CommandText {
			get { return Cmd.CommandText; }
			set { Cmd.CommandText=value; }
		}

		public int CommandTimeout {
			get { return Cmd.CommandTimeout; }
			set { Cmd.CommandTimeout=value; }
		}

		public CommandType CommandType {
			get { return Cmd.CommandType; }
			set { Cmd.CommandType=value; }
		}

		public IDbConnection Connection {
			get { return Cmd.Connection; }
			set { Cmd.Connection=value; }
		}

		public IDbDataParameter CreateParameter() {
			return Cmd.CreateParameter();
		}

		public int ExecuteNonQuery() {
			return Cmd.ExecuteNonQuery();
		}

		public IDataReader ExecuteReader(CommandBehavior behavior) {
			return Cmd.ExecuteReader(behavior);
		}

		public IDataReader ExecuteReader() {
			return Cmd.ExecuteReader();
		}

		public object ExecuteScalar() {
			return Cmd.ExecuteScalar();
		}

		public IDataParameterCollection Parameters {
			get { return Cmd.Parameters; }
		}

		public void Prepare() {
			Cmd.Prepare();
		}

		public IDbTransaction Transaction {
			get { return Cmd.Transaction; }
			set { Cmd.Transaction=value; }
		}

		public UpdateRowSource UpdatedRowSource {
			get { return Cmd.UpdatedRowSource; }
			set { Cmd.UpdatedRowSource=value; }
		}

		public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
		}

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cmd.Dispose();
                Cmd = null;
            }
        }
	}

	public class AsyncDbCommand: NonAsyncDbCommand, IAsyncDbCommand {
		private delegate int ExecuteNonQueryDef(IDbCommand cmd);
		private delegate IDataReader ExecuteReaderDef(IDbCommand cmd, CommandBehavior behaviour);
		private delegate object ExecuteScalarDef(IDbCommand cmd);

		private static ExecuteNonQueryDef ExecuteNonQueryImpl;
		private static ExecuteReaderDef ExecuteReaderImpl;
		private static ExecuteScalarDef ExecuteScalarImpl;

		private delegate IAsyncResult BeginExecuteNonQueryDef(IDbCommand cmd, AsyncCallback callback, object state);
		private delegate int EndExecuteNonQueryDef(IDbCommand cmd, IAsyncResult result);
		private delegate IAsyncResult BeginExecuteReaderDef(IDbCommand cmd, CommandBehavior behaviour, AsyncCallback callback, object state);
		private delegate IDataReader EndExecuteReaderDef(IDbCommand cmd, IAsyncResult result);
		private delegate IAsyncResult BeginExecuteScalarDef(IDbCommand cmd, AsyncCallback callback, object state);
		private delegate object EndExecuteScalarDef(IDbCommand cmd, IAsyncResult result);

		private static BeginExecuteNonQueryDef BeginExecuteNonQueryImpl;
		private static EndExecuteNonQueryDef EndExecuteNonQueryImpl;
		private static BeginExecuteReaderDef BeginExecuteReaderImpl;
		private static EndExecuteReaderDef EndExecuteReaderImpl;
		private static BeginExecuteScalarDef BeginExecuteScalarImpl;
		private static EndExecuteScalarDef EndExecuteScalarImpl;

		private static T CreateDelegate<T>(Type target, string name, bool throwonfail) {
			return (T)(object)Delegate.CreateDelegate(typeof(T), target, name, false, throwonfail);
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

		public static void Init(Type target) {
			BeginExecuteNonQueryImpl=CreateDelegate<BeginExecuteNonQueryDef>(target, "BeginExecuteNonQuery", false);
			if(BeginExecuteNonQueryImpl!=null) {
				EndExecuteNonQueryImpl=CreateDelegate<EndExecuteNonQueryDef>(target, "EndExecuteNonQuery", true);
			} else {
				ExecuteNonQueryImpl=CreateDelegate<ExecuteNonQueryDef>(target, "ExecuteNonQuery", true);
			}
			BeginExecuteReaderImpl=CreateDelegate<BeginExecuteReaderDef>(target, "BeginExecuteReader", false);
			if(BeginExecuteReaderImpl!=null) {
				EndExecuteReaderImpl=CreateDelegate<EndExecuteReaderDef>(target, "EndExecuteReader", true);
			} else {
				ExecuteReaderImpl=CreateDelegate<ExecuteReaderDef>(target, "ExecuteReader", true);
			}
			BeginExecuteScalarImpl=CreateDelegate<BeginExecuteScalarDef>(target, "BeginExecuteScalar", false);
			if(BeginExecuteScalarImpl!=null) {
				EndExecuteScalarImpl=CreateDelegate<EndExecuteScalarDef>(target, "EndExecuteScalar", true);
			} else {
				ExecuteScalarImpl=CreateDelegate<ExecuteScalarDef>(target, "ExecuteScalar", true);
			}
		}

		public AsyncDbCommand(IDbCommand cmd) : base(cmd) {
			if(BeginExecuteNonQueryImpl==null && ExecuteNonQueryImpl==null) {
				Init(cmd.GetType());
			}
		}

		public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object state) {
			if(BeginExecuteNonQueryImpl!=null) {
				return BeginExecuteNonQueryImpl(Cmd, callback, state);
			} else {
				return ExecuteNonQueryImpl.BeginInvoke(Cmd, callback, state);
			}
		}

		public int EndExecuteNonQuery(IAsyncResult result) {
			if(EndExecuteNonQueryImpl!=null) {
				return EndExecuteNonQueryImpl(Cmd, result);
			} else {
				return ExecuteNonQueryImpl.EndInvoke(result);
			}
		}

		public IAsyncResult BeginExecuteReader(AsyncCallback callback, object state) {
			return BeginExecuteReader(CommandBehavior.Default, callback, state);
		}

		public IAsyncResult BeginExecuteReader(CommandBehavior behaviour, AsyncCallback callback, object state) {
			if(BeginExecuteReaderImpl!=null) {
				return BeginExecuteReaderImpl(Cmd, behaviour, callback, state);
			} else {
				return ExecuteReaderImpl.BeginInvoke(Cmd, behaviour, callback, state);
			}
		}

		public IDataReader EndExecuteReader(IAsyncResult result) {
			if(EndExecuteReaderImpl!=null) {
				return EndExecuteReaderImpl(Cmd, result);
			} else {
				return ExecuteReaderImpl.EndInvoke(result);
			}
		}

		public IAsyncResult BeginExecuteScalar(AsyncCallback callback, object state) {
			if(BeginExecuteScalarImpl!=null) {
				return BeginExecuteScalarImpl(Cmd, callback, state);
			} else {
				return ExecuteScalarImpl.BeginInvoke(Cmd, callback, state);
			}
		}

		public object EndExecuteScalar(IAsyncResult result) {
			if(EndExecuteScalarImpl!=null) {
				return EndExecuteScalarImpl(Cmd, result);
			} else {
				return ExecuteScalarImpl.EndInvoke(result);
			}
		}
	}


}
