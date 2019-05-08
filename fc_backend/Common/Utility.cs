namespace fc_backend.Common {
    public class Utility {
//        public void ClearCache(string key, string value)
//        {
//            if (key == null || key == "Users") Users.ClearCache(value);
//        }
//
//        public string ShowCacheTime(string stamp)
//        {
//            return CustomCache.Debug(stamp);
//        }
//        public enum CacheGroup
//        {
//            STATIC,
//            USERS,
//            SCREENS,
//            DEVICES,
//            DEVICE_DATA,
//            DEVICE_LOGDATA,
//            nCacheGroups
//        }
//
//        public static class CustomCache
//        {
//            static Dictionary<CacheGroup, DateTime> CacheGroupStamp = new Dictionary<CacheGroup, DateTime>();
//
//            static CustomCache()
//            {
//                ClearAll();
//            }
//
//            public static void ClearAll()
//            {
//                int group = (int)CacheGroup.nCacheGroups;
//                while (--group >= 0)
//                {
//                    Modified((CacheGroup)group);
//                }
//            }
//
//            public static void Modified(CacheGroup group)
//            {
//                DateTime stamp = DateTime.UtcNow;
//                CacheGroupStamp[group] = new DateTime(stamp.Ticks - (stamp.Ticks % TimeSpan.TicksPerSecond), stamp.Kind);
//            }
//
//            public static bool Cached(CacheGroup group)
//            {
//                string imc = null;
//                WebOperationContext woc = WebOperationContext.Current;
//                HttpContext hc = HttpContext.Current;
//                if (woc != null)
//                {
//                    imc = woc.IncomingRequest.Headers[HttpRequestHeader.IfModifiedSince];
//                }
//                else if (hc != null)
//                {
//                    imc = hc.Request.Headers["If-Modified-Since"];
//                }
//                if (!string.IsNullOrEmpty(imc))
//                {
//                    Trace.TraceInformation("CustomCache.Check \"{0}\" {1} <= {2} = {3}", imc, CacheGroupStamp[group].ToString("O"), ParseHttpDate(imc).ToString("O"), CacheGroupStamp[group] <= ParseHttpDate(imc));
//                }
//                if (!string.IsNullOrEmpty(imc) && CacheGroupStamp[group] <= ParseHttpDate(imc))
//                {
//                    if (woc != null)
//                    {
//                        woc.OutgoingResponse.StatusCode = HttpStatusCode.NotModified;
//                    }
//                    else if (hc != null)
//                    {
//                        hc.Response.StatusCode = (int)HttpStatusCode.NotModified;
//                    }
//                    return true;
//                    //					throw new WebException();
//                }
//                if (woc != null)
//                {
//                    WebOperationContext.Current.OutgoingResponse.LastModified = CacheGroupStamp[group];
//                }
//                else if (hc != null)
//                {
//                    hc.Response.AppendHeader("Last-Modified", CacheGroupStamp[group].ToString("R"));
//                }
//                return false;
//            }
//
//            private static DateTime ParseHttpDate(string stamp)
//            {
//                return DateTime.ParseExact(stamp, "R", null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
//            }
//
//            internal static string Debug(string httpdate)
//            {
//                System.Text.StringBuilder sb = new System.Text.StringBuilder();
//                DateTime stamp, check = ParseHttpDate(httpdate);
//                sb.AppendLine(check.ToString("O"));
//                int group = (int)CacheGroup.nCacheGroups;
//                while (--group >= 0)
//                {
//                    stamp = CacheGroupStamp[(CacheGroup)group];
//                    if (stamp <= check)
//                    {
//                        sb.Append("=");
//                    }
//                    else
//                    {
//                        sb.Append(" ");
//                    }
//                    sb.Append(((CacheGroup)group).ToString());
//                    sb.Append(":=");
//                    sb.AppendLine(stamp.ToString("O"));
//                }
//                return sb.ToString();
//            }
//        }
    }
}