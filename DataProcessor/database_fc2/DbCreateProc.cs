using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace Stienen.Database {
	public static class DbCreateProc {
		//Misc
		public static DbType GetDbType(object value) {
			if(value is Guid) return DbType.Guid;
			if(value is TimeSpan) return DbType.Time;
			if(value is byte[]) return DbType.Binary;
			switch(Convert.GetTypeCode(value)) {
				//case TypeCode.Empty:
				//case TypeCode.Object:
				//case TypeCode.DBNull:
				case TypeCode.Boolean: return DbType.Boolean;
				case TypeCode.Char: return DbType.StringFixedLength;
				case TypeCode.SByte: return DbType.SByte;
				case TypeCode.Byte: return DbType.Byte;
				case TypeCode.Int16: return DbType.Int16;
				case TypeCode.UInt16: return DbType.UInt16;
				case TypeCode.Int32: return DbType.Int32;
				case TypeCode.UInt32: return DbType.UInt32;
				case TypeCode.Int64: return DbType.Int64;
				case TypeCode.UInt64: return DbType.UInt64;
				case TypeCode.Single: return DbType.Single;
				case TypeCode.Double: return DbType.Double;
				case TypeCode.Decimal: return DbType.Decimal;
				case TypeCode.DateTime: return DbType.DateTime;
				case TypeCode.String: return DbType.String;
			}
			return DbType.Object;
		}

		public static object ToDB(object value) {
			if(value==null) return DBNull.Value;
			return value;
		}
		public static object FromDB(object value, Type type) {
			if(value==null || Convert.IsDBNull(value)) {
				if(type==typeof(DateTime)) return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc); // WCF 3.5 DataContractJsonSerializer bug
				return null;
			}
			if(value is DateTime) {
				value=DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
			}
			if(type==null) {
				if(value is ulong) return Convert.ToBoolean(value);
				return value;
			}
			if(type==null || type==typeof(object)) return value;
			type=Nullable.GetUnderlyingType(type)??type;
			if(type!=null && type.Equals(value.GetType())) return value;
			if(type.IsAssignableFrom(value.GetType())) return value;
			try {
				return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
			} catch(Exception) {
				Trace.TraceWarning("change "+value.ToString()+" to "+type.ToString());
				throw;
			}
		}

		public static IDataParameter InitParameter(IDbCommand cmd, object name, bool read, bool write) {
			IDataParameter param=cmd.CreateParameter();
			param.ParameterName=name.ToString();
			if(read && write) param.Direction=ParameterDirection.InputOutput;
			else {
				if(read) param.Direction=ParameterDirection.Input;
				if(write) param.Direction=ParameterDirection.Output;
			}
			return param;
		}
		public static void SetParam(IDataParameter param, object value) {
			param.Value=ToDB(value);
			param.DbType=GetDbType(value);
		}

		public static IDataParameter CreateParameter(IDbCommand cmd, object name, object value, bool read, bool write) {
			IDataParameter param=InitParameter(cmd, name, read, write);
			SetParam(param, value);
			return param;
		}

		public static bool UseParam(string name, string[] skip) {
			return skip==null || (skip!=null && Array.IndexOf(skip, name)<0);
		}

		public static void SetParam(IDbCommand cmd, int index, object value) {
			SetParam((IDataParameter)cmd.Parameters[index], value);
		}
		public static void SetParam(IDbCommand cmd, string name, object value) {
			SetParam(cmd, name, value, null);
		}
		public static void SetParam(IDbCommand cmd, string name, object value, string[] skip) {
			if(UseParam(name, skip)) {
				SetParam((IDataParameter)cmd.Parameters[name], value);
			}
		}
		public static object GetParam(IDbCommand cmd, int index) {
			return FromDB(((IDataParameter)cmd.Parameters[index]).Value, null);
		}
		public static object GetParam(IDbCommand cmd, string name) {
			return FromDB(((IDataParameter)cmd.Parameters[name]).Value, null);
		}
		public static string[] GetParamNames(IDbCommand cmd) {
			int count=cmd.Parameters.Count;
			string[] names=new string[count];
			while(--count>=0) names[count]=((IDataParameter)cmd.Parameters[count]).ParameterName;
			return names;
		}
		public static IDbCommand InitParameters(IDbCommand cmd, int count, string[] names) {
			foreach(string name in names) {
				cmd.Parameters.Add(InitParameter(cmd, name.ToLowerInvariant(), true, false));
				if(--count<=0) break;
			}
			return cmd;
		}
		public static IDbCommand CreateParameters(IDbCommand cmd, params object[] args) {
			for(int i=0;i<args.Length;++i) {
				cmd.Parameters.Add(CreateParameter(cmd, args[i], args[++i], true, false));
			}
			return cmd;
		}
		private static IDbCommand InitOrCreateParameters<T>(IDbCommand cmd, T args, string[] skip) {
			Type t=typeof(T);
			IDataParameter param;
			foreach(PropertyInfo pi in t.GetProperties()) {
				param=InitParameter(cmd, pi.Name.ToLowerInvariant(), pi.CanRead, !pi.CanRead&pi.CanWrite);
				if(UseParam(param.ParameterName, skip)) {
					if(args!=null) {
						if(pi.CanRead) {
							SetParam(param, pi.GetValue(args, null));
						} else {
							SetParam(param, null);
						}
					}
					cmd.Parameters.Add(param);
				}
			}
			foreach(FieldInfo fi in t.GetFields()) {
				param=InitParameter(cmd, fi.Name.ToLowerInvariant(), true, false);
				if(UseParam(param.ParameterName, skip)) {
				if(args!=null) {
					SetParam(param, fi.GetValue(args));
				}
				cmd.Parameters.Add(param);
				}
			}
			return cmd;
		}
		public static IDbCommand InitParameters<T>(IDbCommand cmd) {
			return InitParameters<T>(cmd, null);
		}
		public static IDbCommand InitParameters<T>(IDbCommand cmd, params string[] skip) {
			return InitOrCreateParameters<T>(cmd, default(T), skip);
		}
		public static IDbCommand CreateParameters<T>(IDbCommand cmd, T args) {
			if(args==null) throw new ArgumentNullException("args");
			return InitOrCreateParameters<T>(cmd, args, null);
		}
		public static IDbCommand SetParameters(IDbCommand cmd, params object[] args) {
			for(int i=0;i<args.Length;++i) SetParam(cmd, i, args[i]);
			return cmd;
		}
		public static IDbCommand SetParameters<T>(IDbCommand cmd, T args) {
			return SetParameters<T>(cmd, args, null);
		}
		public static IDbCommand SetParameters<T>(IDbCommand cmd, T args, params string[] skip) {
			if(args==null) return cmd;
			Type t=typeof(T);
			foreach(PropertyInfo pi in t.GetProperties()) {
				if(pi.CanRead) {
					SetParam(cmd, pi.Name.ToLowerInvariant(), pi.GetValue(args, null), skip);
				}
			}
			foreach(FieldInfo fi in t.GetFields()) {
				SetParam(cmd, fi.Name.ToLowerInvariant(), fi.GetValue(args), skip);
			}
			return cmd;
		}
		public static void GetParams<T>(IDbCommand cmd, T args) {
			if(args==null) return;
			Type t=typeof(T);
			foreach(PropertyInfo pi in t.GetProperties()) {
				if(pi.CanWrite && cmd.Parameters.Contains(pi.Name.ToLowerInvariant())) {
					pi.SetValue(args, GetParam(cmd, pi.Name.ToLowerInvariant()), null);
				}
			}
		}
		//Command
		public static IDbCommand CreateProc(IDbConnection conn, string proc) {
			IDbCommand cmd=conn.CreateCommand();
			cmd.CommandText=proc;
			cmd.CommandTimeout=0;
			cmd.CommandType=CommandType.StoredProcedure;
			return cmd;
		}
		public static IDbCommand CreateProc(IDbConnection conn, IDbTransaction tran, string proc) {
			IDbCommand cmd=CreateProc(conn, proc);
			cmd.Transaction=tran;
			return cmd;
		}
		public static IDbCommand CreateProc(IDbConnection conn, string proc, params object[] args) {
			return CreateParameters(CreateProc(conn, proc), args);
		}
		public static IDbCommand CreateProc(IDbConnection conn, IDbTransaction tran, string proc, params object[] args) {
			return CreateParameters(CreateProc(conn, tran, proc), args);
		}
		public static IDbCommand CreateProc<T>(IDbConnection conn, string proc, T args) {
			return CreateParameters<T>(CreateProc(conn, proc), args);
		}
		public static IDbCommand CreateProc<T>(IDbConnection conn, IDbTransaction tran, string proc, T args) {
			return CreateParameters<T>(CreateProc(conn, tran, proc), args);
		}
		public static IDataReader CreateReader(IDbCommand cmd) {
			return cmd.ExecuteReader(CommandBehavior.CloseConnection|CommandBehavior.SequentialAccess);
		}
		public static void SetInout(IDbCommand cmd, params string[] inout) {
			foreach(string s in inout) {
				if(cmd.Parameters.Contains(s)) {
					((IDataParameter)cmd.Parameters[s]).Direction=ParameterDirection.InputOutput;
				} else {
					cmd.Parameters.Add(InitParameter(cmd, s, false, true)); // confirmation test.
				}
			}
		}
	}
}