using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using SQLModel.Attributes;
using System.Data.Entity;

namespace SQLModel
{
    internal class SetMethodObj
    {
        public static Action<T, object> CreateSetMethodHelper<T, TParam>(MethodInfo method)
        {
            var act = (Action<T, TParam>)Delegate.CreateDelegate(typeof(Action<T, TParam>), method);
            Action<T, object> ret = (T target, object param) => act(target, (TParam)param);
            return ret;
        }

        public static Dictionary<string, Action<A, object>> createSetMethods<A>()
        {
            var properties = typeof(A).GetProperties();
            var r = new Dictionary<string, Action<A, object>>(properties.Length);
            foreach (var p in properties)
            {
                var method = p.GetSetMethod();
                var genericHelper = typeof(SetMethodObj).GetMethod("CreateSetMethodHelper");
                var constructedHelper = genericHelper.MakeGenericMethod(typeof(A), method.GetParameters()[0].ParameterType);
                r[p.Name] = (Action<A, object>)constructedHelper.Invoke(null, new object[] { method });
            }
            return r;
        }
    }

    public class SQLRepository<T> : ISQLRepository<T> where T : class
    {
        protected IDbSet<T> dbSet { get; set; }
        public DbContext dbContext { get; set; }
        public string _connectionString { get; set; }
        public string _errorString { get; set; }
        //public SQLRepository(DbContext context)
        //{
        //    this.dbContext = context;
        //    this.dbSet = this.dbContext.Set<T>();
        //}

        #region SQLRepository
        public SQLRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        #region GetByWhere
        /// <summary>
        /// 取得全部的 T
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetByWhere(string where, object[] paramenter, string column = "*")
        {
            IList<T> _list = new List<T>();
            string sql = "";
            var cname = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + column + " FROM " + typeof(T).Name + " where " + where;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    if (paramenter != null)
                    {
                        foreach (var obj in paramenter)
                        {
                            cmd.Parameters.AddWithValue("@" + idx, obj);
                            idx++;
                        }
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
                    while (reader.Read())
                    {

                        T obj = (T)System.Activator.CreateInstance(typeof(T));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                cname = reader.GetName(i);
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<T>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        #region GetByWhere
        /// <summary>
        /// 取得全部的 T
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetByWhere(T entity, string column = "*")
        {
            IList<T> _list = new List<T>();
            var sql = "";
            var cname = "";
            try
            {
                var Properties = entity.GetType().GetProperties();
                var sb = new System.Text.StringBuilder(); ;

                var itemarr = new List<string>();
                var keyarr = new List<string>();
                var keyvalue = new List<object>();
                var valuearr = new List<object>();
                var idx = 1;
                foreach (var p in Properties)
                {
                    if (p.GetValue(entity) != null)
                    {
                        if (p.PropertyType.Name == "String" && p.GetValue(entity).ToString() == "")
                        {
                            continue;
                        }
                        keyarr.Add(p.Name);
                        keyvalue.Add(p.GetValue(entity));
                    }

                }

                sb.Append("Select " + column + " From " + typeof(T).Name + " Where ");
                sb.Append(string.Join(",", itemarr));
                bool firstkey = true;
                for (var keyidx = 0; keyidx < keyarr.Count(); keyidx++)
                {
                    if (keyvalue.Count == keyidx)
                    {
                        continue;
                    }
                    if (keyvalue[keyidx] != null)
                    {
                        if (firstkey)
                        {
                            sb.Append(keyarr[keyidx] + "=@" + idx);
                            firstkey = false;
                        }
                        else
                        {
                            sb.Append(" And " + keyarr[keyidx] + "=@" + idx);
                        }
                        idx += 1;
                    }
                }
                sql = sb.ToString();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sb.ToString(), conn);
                    idx = 1;
                    foreach (var obj in keyvalue)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
                    while (reader.Read())
                    {

                        T obj = (T)System.Activator.CreateInstance(typeof(T));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                cname = reader.GetName(i);
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<T>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        #region GetByWhere
        /// <summary>
        /// 取得全部的 T
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetByWhere(string where, string tablename, object[] paramenter, string column = "*")
        {
            IList<T> _list = new List<T>();
            string sql = "";
            var cname = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + column + " FROM " + tablename + " where " + where;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    if (paramenter != null)
                    {
                        foreach (var obj in paramenter)
                        {
                            cmd.Parameters.AddWithValue("@" + idx, obj);
                            idx++;
                        }
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
                    while (reader.Read())
                    {

                        T obj = (T)System.Activator.CreateInstance(typeof(T));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                cname = reader.GetName(i);
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<T>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        #region GetByWhere
        /// <summary>
        /// 取得全部的 T
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<D> GetJoinByWhere<D>(string where, object[] paramenter, string joinstr, string column, string ctablename = "", string jointype = "left")
        {
            IList<D> _list = new List<D>();
            string sql = "";
            var cname = "";
            try
            {
                var tablename = "";
                if (ctablename == "")
                {
                    SetTableNameAttribute tablenameAttr =
      (SetTableNameAttribute)Attribute.GetCustomAttribute(typeof(D), typeof(SetTableNameAttribute));
                    if (tablenameAttr != null)
                    {
                        var tlist = tablenameAttr.GetTableName().Split(',');
                        if (tlist.Length > 1)
                        {
                            for (var i = 1; i < tlist.Length; i++)
                            {
                                tablename += string.Format(" {0} {1} Join {2} On {3}", new string[] { tlist[i - 1], jointype, tlist[i], joinstr });
                            }
                        }
                        else
                        {
                            tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                        }
                    }
                    else
                    {
                        tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                    }
                }
                else
                {
                    var tlist = ctablename.Split(',');
                    tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { tlist[0], jointype, tlist[1], joinstr });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + column + " FROM " + tablename + " where " + where;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    if (paramenter != null)
                    {
                        foreach (var obj in paramenter)
                        {
                            cmd.Parameters.AddWithValue("@" + idx, obj);
                            idx++;
                        }
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<D, object>> SetMethods = SetMethodObj.createSetMethods<D>();
                    while (reader.Read())
                    {

                        D obj = (D)System.Activator.CreateInstance(typeof(D));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                cname = reader.GetName(i);
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<D>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 取得全部的 T,使用Where In 語法時候使用
        /// </summary>
        /// <returns></returns>
        /// 
        #region GetByWhereIn
        public virtual IQueryable<T> GetByWhereIn(string where, object[] paramenter)
        {
            IList<T> _list = new List<T>();
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT * FROM " + typeof(T).Name + " where " + where;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;

                    foreach (var obj in paramenter)
                    {
                        if (idx == 1)
                        {
                            cmd.Parameters.Add(new SqlParameter("@" + idx, SqlDbType.Xml) { Value = obj });
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + idx, obj);
                        }

                        idx++;
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
                    while (reader.Read())
                    {

                        T obj = (T)System.Activator.CreateInstance(typeof(T));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<T>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 建立 T
        /// </summary>
        /// <param name="entity"></param>
        /// 
        #region Create
        public virtual int Create(T entity)
        {
            var r = 0;
            var sb = new System.Text.StringBuilder(); ;
            try
            {
                var Properties = entity.GetType().GetProperties();


                var keyarr = new List<string>();
                var parameterarr = new List<string>();
                var valuearr = new List<object>();
                var idx = 1;
                PropertyInfo seqPropertyInfo = null;
                foreach (var p in Properties)
                {
                    IsSequenceAttribute isattribute =
                     (IsSequenceAttribute)Attribute.GetCustomAttribute(p, typeof(IsSequenceAttribute));
                    if (isattribute == null)
                    {
                        var value = p.GetValue(entity);
                        if (value != null)
                        {
                            keyarr.Add(p.Name);
                            valuearr.Add(p.GetValue(entity));
                            parameterarr.Add("@" + idx);
                            idx += 1;
                        }
                        else
                        {
                            SetDefaultAttribute setDefault =
                            (SetDefaultAttribute)Attribute.GetCustomAttribute(p, typeof(SetDefaultAttribute));
                            if (setDefault != null)
                            {
                                var dvalue = setDefault.GetDefault();
                                keyarr.Add(p.Name);
                                valuearr.Add(dvalue);
                                parameterarr.Add("@" + idx);
                                idx += 1;
                            }
                        }
                    }
                    else
                    {
                        seqPropertyInfo = p;
                    }
                }

                sb.Append("Insert into " + typeof(T).Name + "(");
                sb.Append(string.Join(",", keyarr) + ")values(");
                sb.Append(string.Join(",", parameterarr) + ")");
                if (seqPropertyInfo != null)
                {
                    sb.Append(";set @myid= SCOPE_IDENTITY();");
                }
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sb.ToString(), conn);

                    idx = 1;
                    foreach (var obj in valuearr)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    SqlParameter myid = null;
                    if (seqPropertyInfo != null)
                    {
                        myid = new SqlParameter
                        {
                            ParameterName = "@myid",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Direction = System.Data.ParameterDirection.Output
                        };
                        cmd.Parameters.Add(myid);
                    }
                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                    if (seqPropertyInfo != null)
                    {
                        if (myid.Value != null) { seqPropertyInfo.SetValue(entity, myid.Value); }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sb.ToString());
                _errorString = ex.Message;
                return -1;
            }

            return r;
        }

        #region Create
        public int Create(string insertstr, object[] paramenter)
        {
            var r = 0;
            var sql = "";
            try
            {
                var sb = new System.Text.StringBuilder(); ;
                sb.Append("Insert Into " + typeof(T).Name + "(" + insertstr + ") Values(");
                for (var idx = 0; idx < paramenter.Count(); idx++)
                {
                    sb.Append("@" + idx + ",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                sql = sb.ToString();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sb.ToString(), conn);
                    var idx = 0;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                throw ex;
            }

            return r;
        }
        #endregion
        #endregion

        /// <summary>
        /// 取得全部的 T
        /// </summary>
        /// <returns></returns>
        /// 

        #region GetAll
        public virtual IQueryable<T> GetAll(string column = "*", string orderby = "")
        {
            IList<T> _list = new List<T>();
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + column + " FROM " + typeof(T).Name + orderby;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
                    while (reader.Read())
                    {

                        T obj = (T)System.Activator.CreateInstance(typeof(T));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<T>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 取得全部的 T
        /// </summary>
        /// <returns></returns>
        /// 
        #region GetJoinAll
        public virtual IQueryable<D> GetJoinAll<D>(string joinstr, string jointype = "left", string orderby = "", string ctablename = "")
        {
            IList<D> _list = new List<D>();
            string sql = "";
            try
            {
                var tablename = "";
                SetTableNameAttribute tablenameAttr =
                  (SetTableNameAttribute)Attribute.GetCustomAttribute(typeof(D), typeof(SetTableNameAttribute));
                if (ctablename == "")
                {
                    if (tablenameAttr != null)
                    {
                        var tlist = tablenameAttr.GetTableName().Split(',');
                        if (tlist.Length > 1)
                        {
                            for (var i = 1; i < tlist.Length; i++)
                            {
                                tablename += string.Format(" {0} {1} Join {2} On {3}", new string[] { tlist[i - 1], jointype, tlist[i], joinstr });
                            }
                        }
                        else
                        {
                            tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                        }
                    }
                    else
                    {
                        tablename = typeof(T).Name + "," + typeof(D).Name;
                    }
                }
                else
                {
                    var tlist = ctablename.Split(',');
                    tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { tlist[0], jointype, tlist[1], joinstr });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT * FROM " + tablename + " " + orderby;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<D, object>> SetMethods = SetMethodObj.createSetMethods<D>();
                    while (reader.Read())
                    {

                        D obj = (D)System.Activator.CreateInstance(typeof(D));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<D>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        #region GetAllDataRange
        //for sql server 2008 舊版
        //public virtual List<T> GetAllDataRange(int sidx, int eidx, string order)
        //{
        //    List<T> _list = new List<T>();
        //    string sql = "";
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(_connectionString))
        //        {
        //            sql = "SELECT * FROM (SELECT  ROW_NUMBER() OVER (ORDER BY " + order + ") As RowNum,* From " + typeof(T).Name + ") as N Where" +
        //                " RowNum >= " + sidx + " and RowNum<=" + eidx;
        //            SqlCommand cmd = new SqlCommand(sql, conn);
        //            conn.Open();
        //            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //            var st = new SetMethodObj();
        //            Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
        //            while (reader.Read())
        //            {

        //                T obj = (T)System.Activator.CreateInstance(typeof(T));
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
        //                    {
        //                        SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
        //                    }

        //                }
        //                _list.Add(obj);
        //            }
        //            reader.Close();
        //        }
        //        return _list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Utilities.NLogManagement.DBLogInfo(ex.Message);
        //        Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
        //        return _list;
        //    }
        //}

        public virtual List<T> GetAllDataRange(int sidx, int offset, string order)
        {
            List<T> _list = new List<T>();
            string sql = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT *  From " + typeof(T).Name + " Order by " + order +
                        " Offset " + sidx + " Row Fetch Next " + offset + " Rows Only";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
                    while (reader.Read())
                    {

                        T obj = (T)System.Activator.CreateInstance(typeof(T));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }

                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list;
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return _list;
            }
        }
        #endregion

        /// <summary>
        /// 取得全部的 T
        /// </summary>
        /// <returns></returns>
        /// 
        #region GetJoinAll
        public virtual IQueryable<D> GetJoinAllDataRange<D>(string joinstr, int sidx, int offset, string orderby, string column, string jointype = "left", string ctablename = "")
        {
            IList<D> _list = new List<D>();
            string sql = "";
            try
            {
                var tablename = "";
                SetTableNameAttribute tablenameAttr =
                  (SetTableNameAttribute)Attribute.GetCustomAttribute(typeof(D), typeof(SetTableNameAttribute));
                if (ctablename == "")
                {
                    if (tablenameAttr != null)
                    {
                        var tlist = tablenameAttr.GetTableName().Split(',');
                        if (tlist.Length > 1)
                        {
                            for (var i = 1; i < tlist.Length; i++)
                            {
                                tablename += string.Format(" {0} {1} Join {2} On {3}", new string[] { tlist[i - 1], jointype, tlist[i], joinstr });
                            }
                        }
                        else
                        {
                            tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                        }
                    }
                    else
                    {
                        tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                    }
                }
                else
                {
                    var tlist = ctablename.Split(',');
                    tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { tlist[0], jointype, tlist[1], joinstr });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + column + "  From " + tablename + " Order by " + orderby +
                     " Offset " + sidx + " Row Fetch Next " + offset + " Rows Only";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<D, object>> SetMethods = SetMethodObj.createSetMethods<D>();
                    while (reader.Read())
                    {

                        D obj = (D)System.Activator.CreateInstance(typeof(D));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list.AsQueryable<D>();
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return null;
            }
        }
        #endregion

        #region Update
        public int Update(T entity, bool allPropertiesUpdate = false)
        {
            var r = 0;
            var sql = "";
            try
            {
                var Properties = entity.GetType().GetProperties();
                var sb = new System.Text.StringBuilder(); ;
                var itemarr = new List<string>();
                var keyarr = new List<string>();
                var keyvalue = new List<object>();
                var valuearr = new List<object>();
                var idx = 1;
                foreach (var p in Properties)
                {
                    KeyAttribute iskey =
                    (KeyAttribute)Attribute.GetCustomAttribute(p, typeof(KeyAttribute));
                    IsSequenceAttribute isseq =
                    (IsSequenceAttribute)Attribute.GetCustomAttribute(p, typeof(IsSequenceAttribute));
                    EmptyNull emptyNull = (EmptyNull)Attribute.GetCustomAttribute(p, typeof(EmptyNull));
                    if (iskey == null)
                    {
                        if (isseq == null)
                        {
                            var value = p.GetValue(entity);
                            if (allPropertiesUpdate == false)
                            {
                                if (value != null)
                                {
                                    itemarr.Add(p.Name + "=@" + idx);
                                    valuearr.Add(p.GetValue(entity));
                                    idx += 1;
                                }
                                else
                                {
                                    if (emptyNull != null)
                                    {
                                        itemarr.Add(p.Name + "=@" + idx);
                                        valuearr.Add(DBNull.Value);
                                        idx += 1;
                                    }

                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    itemarr.Add(p.Name + "=@" + idx);
                                    valuearr.Add(p.GetValue(entity));
                                    idx += 1;
                                }
                                else
                                {
                                    itemarr.Add(p.Name + "=@" + idx);
                                    valuearr.Add(DBNull.Value);
                                    idx += 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        keyarr.Add(p.Name);
                        if (p.GetValue(entity) != null)
                        {
                            keyvalue.Add(p.GetValue(entity));
                        }
                    }
                }

                sb.Append("Update " + typeof(T).Name + " Set ");
                sb.Append(string.Join(",", itemarr));
                if (itemarr.Count() == 0)
                {
                    return 0;
                }
                if (keyarr.Count() > 0)
                {
                    sb.Append(" Where ");
                }
                bool firstkey = true;
                for (var keyidx = 0; keyidx < keyarr.Count(); keyidx++)
                {
                    if (keyvalue.Count == keyidx)
                    {
                        continue;
                    }
                    if (keyvalue[keyidx] != null)
                    {
                        if (firstkey)
                        {
                            sb.Append(keyarr[keyidx] + "=@" + idx);
                            firstkey = false;
                        }
                        else
                        {
                            sb.Append(" And " + keyarr[keyidx] + "=@" + idx);
                        }
                        idx += 1;
                    }
                }
                sql = sb.ToString();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sb.ToString(), conn);

                    idx = 1;
                    foreach (var obj in valuearr)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    foreach (var obj in keyvalue)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                throw ex;
            }

            return r;
        }
        #endregion

        #region Update
        public int Update(string updatestr, string where, object[] paramenter)
        {
            var r = 0;
            var sql = "";
            try
            {
                var sb = new System.Text.StringBuilder(); ;
                sb.Append("Update " + typeof(T).Name + " Set " + updatestr);
                if (string.IsNullOrEmpty(where) == false)
                {
                    sb.Append(" Where " + where);
                }
                sql = sb.ToString();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sb.ToString(), conn);
                    var idx = 1;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                throw ex;
            }

            return r;
        }
        #endregion

        #region GetAllDataCount
        public virtual int GetAllDataCount()
        {
            List<T> _list = new List<T>();
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT Count(*) FROM " + typeof(T).Name;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    var cnt = cmd.ExecuteScalar();
                    return (int)cnt;

                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return -1;
            }
        }
        #endregion

        #region GetDataCaculate
        public virtual int GetDataCaculate(string cstr)
        {
            List<T> _list = new List<T>();
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + cstr + " FROM " + typeof(T).Name;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    var cnt = cmd.ExecuteScalar();
                    return (int)cnt;

                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return 0;
            }
        }
        #endregion

        #region GetDataCaculate
        public virtual int GetDataCaculate(string cstr, string where, object[] paramenter)
        {
            List<T> _list = new List<T>();
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + cstr + " FROM " + typeof(T).Name + " where " + where;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    var cnt = cmd.ExecuteScalar();
                    return (int)cnt;
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return 0;
            }
        }
        #endregion

        #region GetCountUseWhere
        public virtual int GetCountUseWhere(string where, object[] paramenter)
        {
            List<T> _list = new List<T>();
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT count(*) FROM " + typeof(T).Name + " where " + where;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    var cnt = cmd.ExecuteScalar();
                    return (int)cnt;
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return -1;
            }
        }
        #endregion

        #region GetJoinDataUseWhereCount
        public virtual int GetJoinDataUseWhereCount<D>(string joinstr, string where, object[] paramenter, string column = "*", string ctablename = "", string jointype = "left")
        {
            List<T> _list = new List<T>();
            string sql = "";
            try
            {
                var tablename = "";
                SetTableNameAttribute tablenameAttr =
                  (SetTableNameAttribute)Attribute.GetCustomAttribute(typeof(D), typeof(SetTableNameAttribute));
                if (ctablename == "")
                {
                    if (tablenameAttr != null)
                    {
                        var tlist = tablenameAttr.GetTableName().Split(',');
                        if (tlist.Length > 1)
                        {
                            for (var i = 1; i < tlist.Length; i++)
                            {
                                tablename += string.Format(" {0} {1} Join {2} On {3}", new string[] { tlist[i - 1], jointype, tlist[i], joinstr });
                            }
                        }
                        else
                        {
                            tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                        }
                    }
                    else
                    {
                        tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                    }
                }
                else
                {
                    var tlist = ctablename.Split(',');
                    tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { tlist[0], jointype, tlist[1], joinstr });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "Select count(" + column + ") From " + tablename + " Where " + where;
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    var cnt = cmd.ExecuteScalar();
                    return (int)cnt;
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return -1;
            }
        }
        #endregion

        #region GetDataUseWhereRange
        //for sql server 2008
        //public virtual List<T> GetDataUseWhereRange(string where, object[] paramenter, int sidx, int eidx, string order)
        //{
        //    List<T> _list = new List<T>();
        //    string sql = "";
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(_connectionString))
        //        {
        //            sql = "SELECT * FROM (SELECT  ROW_NUMBER() OVER (ORDER BY " + order + ") As RowNum,* From " + typeof(T).Name + " where " + where + ") as N Where " +
        //             " RowNum >= " + sidx + " and RowNum<=" + eidx;

        //            SqlCommand cmd = new SqlCommand(sql, conn);
        //            var idx = 1;
        //            foreach (var obj in paramenter)
        //            {
        //                cmd.Parameters.AddWithValue("@" + idx, obj);
        //                idx++;
        //            }
        //            conn.Open();
        //            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //            var st = new SetMethodObj();
        //            Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
        //            while (reader.Read())
        //            {
        //                T obj = (T)System.Activator.CreateInstance(typeof(T));
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
        //                    {
        //                        SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
        //                    }
        //                }
        //                _list.Add(obj);
        //            }
        //            reader.Close();
        //        }
        //        return _list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Utilities.NLogManagement.DBLogInfo(ex.Message);
        //        Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
        //        return _list;
        //    }
        //}

        public virtual List<T> GetDataUseWhereRange(string where, object[] paramenter, int sidx, int offset, string order)
        {
            List<T> _list = new List<T>();
            string sql = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT *  From " + typeof(T).Name + " where " + where + " Order by " + order +
                    " Offset " + sidx + " Row Fetch Next " + offset + " Rows Only";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<T, object>> SetMethods = SetMethodObj.createSetMethods<T>();
                    while (reader.Read())
                    {
                        T obj = (T)System.Activator.CreateInstance(typeof(T));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list;
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return _list;
            }
        }
        #endregion

        #region GetDataUseWhereRange
        //public virtual List<D> GetJoinDataUseWhereRange<D>(string joinstr,string where, object[] paramenter, int sidx, int eidx, string order, string column = "*")
        //{
        //    List<D> _list = new List<D>();
        //    string sql = "";
        //    try
        //    {
        //        var tablename = "";
        //        SetTableNameAttribute tablenameAttr =
        //          (SetTableNameAttribute)Attribute.GetCustomAttribute(typeof(D), typeof(SetTableNameAttribute));
        //        if (tablenameAttr != null)
        //        {
        //            tablename = tablenameAttr.GetTableName();
        //        }
        //        else
        //        {
        //            tablename = typeof(T).Name + "," + typeof(D).Name;
        //        }
        //        using (SqlConnection conn = new SqlConnection(_connectionString))
        //        {
        //            sql = "SELECT * FROM (SELECT  ROW_NUMBER() OVER (ORDER BY " + order + ") As RowNum,"+ column+" From " + tablename + " where " + joinstr+"  And "+ where + ") as N Where " +
        //             " RowNum >= " + sidx + " and RowNum<=" + eidx;

        //            SqlCommand cmd = new SqlCommand(sql, conn);
        //            var idx = 1;
        //            foreach (var obj in paramenter)
        //            {
        //                cmd.Parameters.AddWithValue("@" + idx, obj);
        //                idx++;
        //            }
        //            conn.Open();
        //            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //            var st = new SetMethodObj();
        //            Dictionary<string, Action<D, object>> SetMethods = SetMethodObj.createSetMethods<D>();
        //            while (reader.Read())
        //            {
        //                D obj = (D)System.Activator.CreateInstance(typeof(D));
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
        //                    {
        //                        SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
        //                    }
        //                }
        //                _list.Add(obj);
        //            }
        //            reader.Close();
        //        }
        //        return _list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Utilities.NLogManagement.DBLogInfo(ex.Message);
        //        Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
        //        return _list;
        //    }
        //}

        public virtual List<D> GetJoinDataUseWhereRange<D>(string joinstr, string where, object[] paramenter, int sidx, int offset, string order, string column, string ctablename = "", string jointype = "left")
        {
            List<D> _list = new List<D>();
            string sql = "";
            try
            {
                SetTableNameAttribute tablenameAttr =
                  (SetTableNameAttribute)Attribute.GetCustomAttribute(typeof(D), typeof(SetTableNameAttribute));
                var tablename = "";
                if (ctablename == "")
                {
                    if (tablenameAttr != null)
                    {
                        var tlist = tablenameAttr.GetTableName().Split(',');
                        if (tlist.Length > 1)
                        {
                            for (var i = 1; i < tlist.Length; i++)
                            {
                                tablename += string.Format(" {0} {1} Join {2} On {3}", new string[] { tlist[i - 1], jointype, tlist[i], joinstr });
                            }
                        }
                        else
                        {
                            tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                        }
                    }
                    else
                    {
                        tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { typeof(T).Name, jointype, typeof(D).Name, joinstr });
                    }
                }
                else
                {
                    var tlist = ctablename.Split(',');
                    tablename = string.Format("{0} {1} Join {2} On {3}", new string[] { tlist[0], jointype, tlist[1], joinstr });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    sql = "SELECT " + column + "  From " + tablename + " where " + where + " Order by " + order +
                " Offset " + sidx + " Row Fetch Next " + offset + " Rows Only";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var st = new SetMethodObj();
                    Dictionary<string, Action<D, object>> SetMethods = SetMethodObj.createSetMethods<D>();
                    while (reader.Read())
                    {
                        D obj = (D)System.Activator.CreateInstance(typeof(D));
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            if (SetMethods.ContainsKey(reader.GetName(i)) && reader.IsDBNull(i) == false)
                            {
                                SetMethods[reader.GetName(i)](obj, reader.GetValue(i));
                            }
                        }
                        _list.Add(obj);
                    }
                    reader.Close();
                }
                return _list;
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return _list;
            }
        }
        #endregion

        #region DelDataUseWhere
        public virtual int DelDataUseWhere(string where, object[] paramenter)
        {
            string sql = "";
            try
            {
                int r = 0;
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    if (where == "")
                    {
                        sql = "Delete  FROM " + typeof(T).Name;
                    }
                    else
                    {
                        sql = "Delete  FROM " + typeof(T).Name + " where " + where;
                    }

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    var idx = 1;
                    foreach (var obj in paramenter)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                }
                return r;
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return -1;
            }
        }
        #endregion

        #region Delete
        public int Delete(T entity)
        {
            var r = 0;
            var sql = "";
            try
            {
                var Properties = entity.GetType().GetProperties();
                var sb = new System.Text.StringBuilder(); ;

                var itemarr = new List<string>();
                var keyarr = new List<string>();
                var keyvalue = new List<object>();
                var valuearr = new List<object>();
                var idx = 1;
                foreach (var p in Properties)
                {
                    KeyAttribute iskey =
                    (KeyAttribute)Attribute.GetCustomAttribute(p, typeof(KeyAttribute));
                    if (iskey != null)
                    {
                        keyarr.Add(p.Name);
                        if (p.GetValue(entity) != null)
                        {
                            keyvalue.Add(p.GetValue(entity));
                        }
                    }

                }

                sb.Append("Delete " + typeof(T).Name + " Where ");
                sb.Append(string.Join(",", itemarr));
                bool firstkey = true;
                for (var keyidx = 0; keyidx < keyarr.Count(); keyidx++)
                {
                    if (keyvalue.Count == keyidx)
                    {
                        continue;
                    }
                    if (keyvalue[keyidx] != null)
                    {
                        if (firstkey)
                        {
                            sb.Append(keyarr[keyidx] + "=@" + idx);
                            firstkey = false;
                        }
                        else
                        {
                            sb.Append(" And " + keyarr[keyidx] + "=@" + idx);
                        }
                        idx += 1;
                    }
                }
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sb.ToString(), conn);
                    sql = sb.ToString();
                    idx = 1;
                    foreach (var obj in keyvalue)
                    {
                        cmd.Parameters.AddWithValue("@" + idx, obj);
                        idx++;
                    }
                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Utilities.NLogManagement.DBLogInfo(ex.Message);
                Utilities.NLogManagement.DBLogInfo("SQL =" + sql);
                _errorString = ex.Message;
                return -1;
            }

            return r;
        }
        #endregion

        #region Mapping
        public D Mapping<S, D>(S s, D d)
        {
            var PropertiesSource = s.GetType().GetProperties();
            var PropertiesDestination = d.GetType().GetProperties();
            foreach (var p in PropertiesDestination)
            {
                var dp = PropertiesSource.Where(v => v.Name == p.Name);
                SetDefaultAttribute defaultvalue =
                  (SetDefaultAttribute)Attribute.GetCustomAttribute
                  (p, typeof(SetDefaultAttribute));
                if (dp.Count() > 0)
                {
                    var value = dp.First().GetValue(s, null);
                    if (defaultvalue != null && value == null) { value = defaultvalue.GetDefault(); }
                    p.SetValue(d, value);
                }
                else
                {
                    if (defaultvalue != null) { p.SetValue(d, defaultvalue.GetDefault()); }
                }
            }
            return d;
        }
        #endregion
    }
}
