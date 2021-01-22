using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using WEDEPX_DB.Models;

namespace WEDEPX_DB.Dao
{
    public class DaoDb 
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private WEDEPXEntities db { get; set; }
    private DbContextTransaction trans { get; set; }

    public DaoDb()
    {
        GetConnection();
    }

    protected WEDEPXEntities GetConnection()
    {
        if (db == null)
            db = new WEDEPXEntities();
        return db;
    }

    protected bool IsDebugStatement
    {
        get
        {
            return db.Database.Log.Target != null;
        }

        set
        {
            if (value)
                db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            else
                db.Database.Log = null;
        }
    }

    protected void BeginTrans()
    {
        trans = db.Database.BeginTransaction();
    }
    protected void CommitTrans()
    {
        trans.Commit();
    }
    protected void RollbackTrans()
    {
        trans.Rollback();
    }

    protected DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
    {
        return db.Database.SqlQuery<TElement>(sql, parameters);
    }

    protected DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql)
    {
        return db.Database.SqlQuery<TElement>(sql);
    }

    public int ExecuteSqlCommand(string sql, params object[] parameters)
    {
        return db.Database.ExecuteSqlCommand(sql, parameters);
    }


    public DataTable ExecuteQuery(string sqlQuery)
    {
        WEDEPXEntities db = GetConnection();
        DbProviderFactory dbFactory = DbProviderFactories.GetFactory(db.Database.Connection);

        using (var cmd = dbFactory.CreateCommand())
        {
            cmd.Connection = db.Database.Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlQuery;
            using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
            {
                adapter.SelectCommand = cmd;

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }
    }


    private static List<T> ConvertDataTable<T>(DataTable dt)
    {
        List<T> data = new List<T>();
        foreach (DataRow row in dt.Rows)
        {
            T item = GetItem<T>(row);
            data.Add(item);
        }
        return data;
    }
    private static T GetItem<T>(DataRow dr)
    {
        Type temp = typeof(T);
        T obj = Activator.CreateInstance<T>();

        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                if (pro.Name == column.ColumnName)
                    if (dr[column.ColumnName] == DBNull.Value)
                        pro.SetValue(obj, null, null);
                    else
                        pro.SetValue(obj, dr[column.ColumnName], null);
                else
                    continue;
            }
        }
        return obj;
    }

       
    }

}