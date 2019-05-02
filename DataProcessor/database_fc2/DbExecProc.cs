using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace Stienen.Database {
	public static class DbExecProc {
		public static byte[] MakeGuidStr(ICollection<Guid> guids) {
			if(guids==null) return null;
			int offset=0;
			byte[] result=new byte[guids.Count*16];
			foreach(Guid g in guids) {
				g.ToByteArray().CopyTo(result, offset);
				offset+=16;
			}
			return result;
		}
		public static Guid[] MakeGuidArray(byte[] guids) {
			int offset=0;
			Guid[] result=new Guid[guids.Length/16];
			for(int i=0;i<result.Length;++i, offset+=16) {
				result[i]=new Guid(BitConverter.ToInt32(guids, offset), BitConverter.ToInt16(guids, offset+4), BitConverter.ToInt16(guids, offset+6), guids[offset+8], guids[offset+9], guids[offset+10], guids[offset+11], guids[offset+12], guids[offset+13], guids[offset+14], guids[offset+15]);
			}
			return result;
		}
		public static byte[] HexStr(ICollection<short> coll) {
			byte[] result=new byte[coll.Count*2];
			int length=-1;
			foreach(short item in coll) {
				result[++length]=(byte)(item>>8);
				result[++length]=(byte)(item);
			}
			return result;
		}
		public static byte[] HexStr(ICollection<int> coll) {
			byte[] result=new byte[coll.Count*4];
			int length=-1;
			foreach(int item in coll) {
				result[++length]=(byte)(item>>24);
				result[++length]=(byte)(item>>16);
				result[++length]=(byte)(item>>8);
				result[++length]=(byte)(item);
			}
			return result;
		}
		/*		public static short[] UnHexStr(byte[] shorts) {
					List<short> result=new List<short>(shorts.Length/2);
					for(int i=0;i<shorts.Count;i+=2) result.Add(BitConverter.ToInt16(shorts, i));
					return result;
				}
		*/
		public class ParamLengthComparer: IComparer<ParameterInfo[]> {
			public int Compare(ParameterInfo[] x, ParameterInfo[] y) {
				return y.Length.CompareTo(x.Length);
			}
		}
		private static ParamLengthComparer plc=new ParamLengthComparer();

		public delegate void Action<T1, T2>(T1 t1, T2 t2);
		//Reader
		public static ConstructorInfo FindConstructor<T>(out Type[] types) {
			SortedList<ParameterInfo[], ConstructorInfo> ctors=new SortedList<ParameterInfo[], ConstructorInfo>(plc);
			foreach(ConstructorInfo ctor in typeof(T).GetConstructors()) {
				ctors.Add(ctor.GetParameters(), ctor);
			}
			types=Array.ConvertAll<ParameterInfo, Type>(ctors.Keys[0], (pi) => { return pi.ParameterType; });
			return ctors.Values[0];
		}

		public static void Read<T>(IDataReader reader, Action<T, object[]> action) {
			Type[] types;
			ConstructorInfo ctor=FindConstructor<T>(out types);
			int tcount=types.Length, vcount;
			object[] tvalues, rvalues;

			object[] values=new object[reader.FieldCount];
			while(reader.Read()) {
				vcount=reader.GetValues(values);
				rvalues=new object[vcount-tcount];
				while(--vcount>=tcount) {
					rvalues[vcount-tcount]=DbCreateProc.FromDB(values[vcount], null);
				}
				tvalues=new object[tcount];
				while(--vcount>=0) {
					tvalues[vcount]=DbCreateProc.FromDB(values[vcount], types[vcount]);
				}
				action((T)ctor.Invoke(tvalues), rvalues);
			}
			if(!reader.NextResult()) {
				reader.Close();
			}
		}
		public static IEnumerable<object[]> Read(IDataReader reader) {
			while(reader.Read()) {
				object[] values=new object[reader.FieldCount];
				reader.GetValues(values);
				for(int i=values.Length-1;i>=0;--i) {
					values[i]=DbCreateProc.FromDB(values[i], null);
				}
				yield return values;
			}
			if(!reader.NextResult()) {
				reader.Close();
			}
		}
		public static void Read(IDataReader reader, Action<object[]> action) {
			foreach(object[] values in Read(reader)) {
				action(values);
			}
			if(!reader.NextResult()) {
				reader.Close();
			}
		}
		public static ConstructorInfo FindConstructor<T>(int fieldcount, out Type[] types) {
			Type type=typeof(T);
			if(fieldcount==1 && type.FullName.StartsWith("System", StringComparison.InvariantCulture)) {
				types=new Type[] { type };
				return null;
			}
			ParameterInfo[] pis;
			foreach(ConstructorInfo ci in type.GetConstructors()) {
				pis=ci.GetParameters();
				if(pis.Length==fieldcount) {
					types=Array.ConvertAll<ParameterInfo, Type>(pis, (pi) => { return pi.ParameterType; });
					return ci;
				}
			}
			throw new ArgumentException("no constructor of "+typeof(T).FullName+" has "+fieldcount+" arguments");
		}
		public static IEnumerable<T> Read<T>(IDataReader reader) {
			Type[] types;
//			Trace.TraceInformation("Read {0} {1}", typeof(T).FullName, reader.FieldCount);
			ConstructorInfo ctor=FindConstructor<T>(reader.FieldCount, out types);
//			Trace.TraceInformation("ctor {0}", ctor!=null);
			object[] values=new object[reader.FieldCount];
			if(ctor!=null) {
				while(!reader.IsClosed && reader.Read()) {
					reader.GetValues(values);
					for(int i=values.Length-1;i>=0;--i) {
						values[i]=DbCreateProc.FromDB(values[i], types[i]);
					}
					yield return (T)ctor.Invoke(values);
				}
			} else {
				while(!reader.IsClosed && reader.Read()) {
					reader.GetValues(values);
					yield return (T)DbCreateProc.FromDB(values[0], types[0]);
				}
			}
			if(!reader.IsClosed && !reader.NextResult()) {
				reader.Close();
			}
		}
		public static T ReadFirstRow<T>(IDataReader reader) {
			try {
				foreach(T t in Read<T>(reader)) {
					return t;
				}
				return default(T);
			} finally {
				if(!reader.NextResult()) {
					reader.Close();
				}
			}
		}

        //SYNC
	    //ExecProc
        public static int ExecuteNonQuery(IDbCommand cmd) {
		    Trace.TraceInformation(CmdToString(cmd));
			return cmd.ExecuteNonQuery();
		}
		public static IDataReader ExecuteReader(IDbCommand cmd) {
		    Trace.TraceInformation(CmdToString(cmd));
			return DbCreateProc.CreateReader(cmd);
		}

		public static int ExecProc(string proc, params object[] args) {
			using(IDbConnection conn=DbConn.Open()) {
				return ExecuteNonQuery(DbCreateProc.CreateProc(conn, proc, args));
			}
		}
		public static int ExecProc<T>(string proc, T args) {
			using(IDbConnection conn=DbConn.Open()) {
				return ExecuteNonQuery(DbCreateProc.CreateProc<T>(conn, proc, args));
			}
		}
		public static int ExecProcInout<T>(string proc, T args, params string[] inout) {
			int result=0;
			using(IDbConnection conn=DbConn.Open()) {
				IDbCommand cmd=DbCreateProc.CreateProc<T>(conn, proc, args);
				DbCreateProc.SetInout(cmd, inout);
				result=ExecuteNonQuery(cmd);
				DbCreateProc.GetParams<T>(cmd, args);
			}
			return result;
		}
		public static int ExecProcExtra<T>(string proc, T args, params object[] extra) {
			using(IDbConnection conn=DbConn.Open()) {
				IDbCommand cmd=DbCreateProc.CreateProc<T>(conn, proc, args);
				DbCreateProc.CreateParameters(cmd, extra);
				return ExecuteNonQuery(cmd);
			}
		}
		public static int ExecMultiProc(string proc, string[] names, IEnumerable<object[]> args) {
			return ExecMultiProc(proc, names, names.Length, args);
		}
		public static int ExecMultiProc(string proc, string[] names, int count, IEnumerable<object[]> args) {
			int result=0;
			using(IDbConnection conn=DbConn.Open()) {
				using(IDbTransaction tran=conn.BeginTransaction()) {
					try {
						result=ExecMultiProcEx(conn, tran, proc, names, count, args);
						tran.Commit();
					} catch {
						tran.Rollback();
						throw;
					}
				}
			}
			return result;
		}
		public static int ExecMultiProcEx(IDbConnection conn, IDbTransaction tran, string proc, string[] names, int count, IEnumerable<object[]> args) {
			int result=0;
			IDbCommand cmd=DbCreateProc.InitParameters(DbCreateProc.CreateProc(conn, tran, proc), count, names);
		    Trace.TraceInformation(CmdToString(cmd));
			try {
				foreach(object[] arg in args) {
					DbCreateProc.SetParameters(cmd, arg);
					result+=ExecuteNonQuery(cmd);
				}
			} catch(Exception ex) {
				Trace.TraceWarning(CmdToString(cmd));
				Trace.TraceWarning(ex.ToString());
				throw new Exception(CmdToString(cmd), ex);
			}
			return result;
		}
		public static int ExecMultiProc<T>(string proc, IEnumerable<T> args) {
			int result=0;
			using(IDbConnection conn=DbConn.Open()) {
				using(IDbTransaction tran=conn.BeginTransaction()) {
					try {
						result=ExecMultiProcEx<T>(conn, tran, proc, args);
						tran.Commit();
					} catch {
						tran.Rollback();
						throw;
					}
				}
			}
			return result;
		}
		public static int ExecMultiProcEx<T>(IDbConnection conn, IDbTransaction tran, string proc, IEnumerable<T> args) {
			int result=0;
			IDbCommand cmd=DbCreateProc.InitParameters<T>(DbCreateProc.CreateProc(conn, tran, proc));
			try {
				foreach(T arg in args) {
					DbCreateProc.SetParameters<T>(cmd, arg);
					result+=ExecuteNonQuery(cmd);
					//GetParams<T>(cmd, arg);
				}
			} catch(Exception ex) {
				Trace.TraceWarning(CmdToString(cmd));
				Trace.TraceWarning(ex.ToString());
				throw new Exception(CmdToString(cmd), ex);
			}
			return result;
		}
		public static int ExecMultiProc<T>(string proc, IEnumerable<T> dynargs, params object[] staargs) {
			int result=0;
			using(IDbConnection conn=DbConn.Open()) {
				using(IDbTransaction tran=conn.BeginTransaction()) {
					try {
						result=ExecMultiProcEx<T, T>(conn, tran, proc, dynargs, staargs);
						tran.Commit();
					} catch {
						tran.Rollback();
					}
				}
			}
			return result;
		}
		public static int ExecMultiProcEx<T>(IDbConnection conn, IDbTransaction tran, string proc, IEnumerable<T> dynargs, params object[] staargs) {
			return ExecMultiProcEx<T, T>(conn, tran, proc, dynargs, staargs);
		}
		public static int ExecMultiProcEx<T, U>(IDbConnection conn, IDbTransaction tran, string proc, IEnumerable<U> dynargs, params object[] staargs) where U: T {
			int result=0;
			IDbCommand cmd=DbCreateProc.CreateProc(conn, proc, staargs);
			string[] skip=DbCreateProc.GetParamNames(cmd);
			DbCreateProc.InitParameters<T>(cmd, skip);
			try {
				foreach(U arg in dynargs) {
					DbCreateProc.SetParameters<T>(cmd, arg, skip);
					result+=ExecuteNonQuery(cmd);
					//ReadParams<T>(cmd, arg);
				}
			} catch(Exception ex) {
				Trace.TraceWarning(CmdToString(cmd));
				Trace.TraceWarning(ex.ToString());
				throw new Exception(CmdToString(cmd), ex);
			}
			return result;
		}
		public static T ExecScalarProc<T>(string proc, params object[] args) {
			using(IDbConnection conn=DbConn.Open()) {
				IDbCommand cmd=DbCreateProc.CreateProc(conn, proc, args);
			    Trace.TraceInformation(CmdToString(cmd));
                return (T)DbCreateProc.FromDB(cmd.ExecuteScalar(), typeof(T));
			}
		}
		public static TResult ExecScalarProc<T, TResult>(string proc, T args) {
			using(IDbConnection conn=DbConn.Open()) {
				IDbCommand cmd=DbCreateProc.CreateProc<T>(conn, proc, args);
			    Trace.TraceInformation(CmdToString(cmd));
                TResult result=(TResult)DbCreateProc.FromDB(cmd.ExecuteScalar(), typeof(TResult));
				DbCreateProc.GetParams<T>(cmd, args);
				return result;
			}
		}
		public static T ExecFirstRowProc<T>(string proc, params object[] args) {
			using(IDbConnection conn=DbConn.Open()) {
				IDbCommand cmd=DbCreateProc.CreateProc(conn, proc, args);
				using(IDataReader reader=ExecuteReader(cmd)) {
					foreach(T t in Read<T>(reader)) {
						return t;
					}
				}
			}
			return default(T);
		}
		public static IEnumerable<object[]> ExecReaderProc(string proc, params object[] args) {
			return Read(ExecuteReader(DbCreateProc.CreateProc(DbConn.Open(), proc, args)));
		}
		public static void ExecReaderProc(Action<object[]> action, string proc, params object[] args) {
			using(IDbConnection conn=DbConn.Open()) {
				Read(ExecuteReader(DbCreateProc.CreateProc(conn, proc, args)), action);
			}
		}
		public static IEnumerable<T> ExecReaderProc<T>(string proc, params object[] args) {
			return Read<T>(ExecuteReader(DbCreateProc.CreateProc(DbConn.Open(), proc, args)));
		}

        public static T ExecReaderProcFirst<T>(IDbConnection conn, string proc, params object[] args)
        {
            // Create a reader which does not close the connection
            IDataReader dataReader = DbCreateProc.CreateProc(conn, proc, args).ExecuteReader(CommandBehavior.SequentialAccess);
            return ReadFirstRow<T>(dataReader);
        }

		public static string CmdToString(IDbCommand cmd) {
			string result="DB."+cmd.CommandText+"(";
			IDataParameter param;
			object value;
			if(cmd.Parameters.Count>0) result+='{';
			for(int i=0;i<cmd.Parameters.Count;++i) {
				param=(IDbDataParameter)cmd.Parameters[i];
				if(i!=0) result+=", ";
				result+=param.ParameterName+": ";
				value=param.Value;
				if(value==null || Convert.IsDBNull(value)) {
					result+="<NULL>";
				} else {
					byte[] data=value as byte[];
					if(data!=null) {
						result+=HexStr(data);
					} else {
						result+=value.ToString();
					}
				}
				result+="("+param.DbType.ToString()+")";
			}
			if(cmd.Parameters.Count>0) result+='}';
			result+=")";
			return result;
		}
		private static string HexStr(byte[] data) {
			const string hex="0123456789ABCDEFG";
			string result="0x";
			foreach(byte b in data) {
				result+=hex[b/16];
				result+=hex[b%16];
			}
			return result;
		}
	}
}
