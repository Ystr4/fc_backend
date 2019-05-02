using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Stienen.Database {
	public static class DbConn {
		public static IDbConnection CreateConnection(string name) {
			ConnectionStringSettings css;
			try {
				css=ConfigurationManager.ConnectionStrings[name];
			} catch(Exception e) {
				Trace.TraceError("Exception loading: {0}", name);
				Trace.TraceError(e.ToString());
				Trace.TraceInformation("Available ConnectionStrings");
				foreach(string[] cs in EnumerateConnectionStrings()) {
					Trace.TraceError("Name: {0}, Provider: {1}", cs[0], cs[2]);
				}
				throw;
			}
			DbProviderFactory pf;
			try {
				pf=DbProviderFactories.GetFactory(css.ProviderName);
			} catch(Exception e) {
				Trace.TraceError("Exception loading: {0}", css.ProviderName);
				Trace.TraceError(e.ToString());
				Trace.TraceInformation("Available Providers");
				foreach(string[] prov in EnumerateProviders()) {
					Trace.TraceInformation(string.Join(",\t", prov));
				}
				throw;
			}
			IDbConnection conn=pf.CreateConnection();
			conn.ConnectionString=css.ConnectionString;
			return conn;
		}
		public static IDbConnection Open() {
			if(ConfigurationManager.ConnectionStrings.Count==0) {
				throw new DataException("no ConnectionStrings defined");
			}
			return Open(ConfigurationManager.ConnectionStrings[0].Name);
		}
		public static IDbConnection Open(string name) {
            IDbConnection conn=CreateConnection(name);
            try {
				if(conn.State!=ConnectionState.Open) conn.Close();
            } catch(ObjectDisposedException) {
				conn=CreateConnection(name);
			}
			if(conn.State!=ConnectionState.Open) conn.Open();
            return conn;
		}

		public static IEnumerable<string[]> EnumerateProviders() {
			DataTable table=DbProviderFactories.GetFactoryClasses();
			string[] header=new string[table.Columns.Count];
			for(int i=header.Length-1;i>=0;--i) {
				header[i]=table.Columns[i].ColumnName;
			}
			yield return header;
			foreach(DataRow row in table.Rows) {
				yield return Array.ConvertAll<object, string>(row.ItemArray, (o) => { return o.ToString(); });
			}
		}

		public static IEnumerable<string[]> EnumerateConnectionStrings() {
			yield return new string[] { "Name", "ConnectionString", "ProviderName" };
			foreach(ConnectionStringSettings css in ConfigurationManager.ConnectionStrings) {
				yield return new string[] { css.Name, css.ConnectionString, css.ProviderName };
			}
		}
	}
}