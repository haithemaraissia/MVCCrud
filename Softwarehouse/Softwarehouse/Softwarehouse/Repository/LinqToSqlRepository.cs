//Basic Linq to Sql abstract repository to be inherited and used for the generic controller
//Developed by Matthew Hood - 2009

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace Softwarehouse.MvcCrud.Repository
{
    public abstract class LinqToSqlRepository<TKeyType, TModel> : IMvcCrudRepository<TKeyType, TModel> where TModel : class, new() where TKeyType : IComparable
    {
        public DataContext db;
        public string primaryKey { get; set; }
        public string orderBy { get; set; }

        public Dictionary<int, string> exceptionText = new Dictionary<int, string>();

        protected LinqToSqlRepository(DataContext db, string primaryKey, string orderBy)
        {
            SetupExceptionsList();
            this.db = db;
            this.primaryKey = primaryKey;
            this.orderBy = orderBy;
        }

        protected void SetupExceptionsList(){
            exceptionText.Add(541, "Cannot delete this item - it is in use.");
            exceptionText.Add(547, "Cannot delete this item - it is in use.");
            exceptionText.Add(2627, "Cannot create/update this item, it will create a duplicate which is not allowed");
            exceptionText.Add(2601, "Cannot create/update this item, it will create a duplicate which is not allowed");
            exceptionText.Add(515, "Cannot create/update this item, blank values on one or more of the fields are not allowed");
        }

        public virtual IQueryable<TModel> All()
        {
            return db.GetTable<TModel>();
        }

        public virtual IQueryable<TModel> AllOrdered()
        {
            return All().OrderBy(orderBy);
        }

        public virtual TModel Single(TKeyType id)
        {
            //TODO: change to something along the lines of Single(t => PrimaryKey(t) == id), and avoid dynamic linq
            return db.GetTable<TModel>().Where( primaryKey +  " = @0", id).First();
        }

        public virtual void Create(TModel t)
        {
            try
            {
                db.GetTable<TModel>().InsertOnSubmit(t);
                db.SubmitChanges();
            }
            catch (SqlException e)
            {
                throw new Exception(exceptionText.ContainsKey(e.Number) ? exceptionText[e.Number] : "SQL Error #" + e.Number + ": " + e.Message);
            }
            catch (ExternalException e)
            {
                throw new Exception(exceptionText.ContainsKey(e.ErrorCode) ? exceptionText[e.ErrorCode] : e.ErrorCode + ": " + e.Message);
            }
        }

        private TKeyType PrimaryKey(TModel t)
        {
            return (TKeyType)t.GetType().GetProperty(primaryKey).GetValue(t, null);
        }

        public virtual void Save(TModel t)
        {
            try
            {
                try
                {
                    TModel old = Single(PrimaryKey(t));
                    DataContext secondContextToAttach = new DataContext(db.Connection);
                    secondContextToAttach.GetTable<TModel>().Attach(t, old);
                    secondContextToAttach.SubmitChanges();
                }
                catch (NotSupportedException)
                {
                    //user is trying to save an object that is already in the datacontext.
                    db.SubmitChanges();
                }
            }
            catch (SqlException e)
            {
                throw new Exception(exceptionText.ContainsKey(e.Number) ? exceptionText[e.Number] : "SQL Error #" + e.Number + ": " + e.Message);
            }
            catch (ExternalException e)
            {
                throw new Exception(exceptionText.ContainsKey(e.ErrorCode) ? exceptionText[e.ErrorCode] : e.ErrorCode + ": " + e.Message);
            }
        }

        public virtual void Delete(TKeyType id)
        {
            try
            {
                db.GetTable<TModel>().DeleteOnSubmit(Single(id));
                db.SubmitChanges();
            }
            catch (SqlException e)
            {
                throw new Exception(exceptionText.ContainsKey(e.Number) ? exceptionText[e.Number] : "SQL Error #" + e.Number + ": " + e.Message);
            }
            catch (ExternalException e)
            {
                throw new Exception(exceptionText.ContainsKey(e.ErrorCode) ? exceptionText[e.ErrorCode] : e.ErrorCode + ": " + e.Message);
            }
        }
    }
}