using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InstantMessenger.Core.Base;
using InstantMessenger.Core.UOW;
using NHibernate;
using NHibernate.Linq;

namespace InstantMessenger.Core.Repository
{
    public class Repository<T> : IRepository<T> where T : IBDO
    {
        private ISession Session { get { return UnitOfWork.Current.Session; } }

        /// <summary>
        /// Finds single entity, that satisfies the expression, or null if none was found.
        /// Throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public T FindOne(Expression<Func<T, bool>> expr)
        {
            return FindAll(expr).SingleOrDefault();
        }

        /// <summary>
        /// Finds all entities or empty collection if none was found.
        /// </summary>
        /// <returns></returns>
        public IList<T> FindAll()
        {
            return CreateQuery().ToList();
        }

        /// <summary>
        /// Finds all entities, that satisfy the expression, or empty collection if none was found.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public IList<T> FindAll(Expression<Func<T, bool>> expr)
        {
            return CreateQuery().Where(expr).ToList();
        }

        /// <summary>
        /// Finds entity based on its OID
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public T Get(long oid)
        {
            return Session.Get<T>(oid);
        }

        /// <summary>
        /// Adds entity to the Repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Create(T entity)
        {
            Session.Save(entity);
        }

        /// <summary>
        /// Adds entities to the Repository
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public void Create(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.Created = DateTime.Now;
                Session.Save(item);
            }
        }

        /// <summary>
        /// Updates entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Update(T entity)
        {
            Session.Update(entity);
        }

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            Session.Delete(entity);
        }

        /// <summary>
        /// Delete given entities.
        /// </summary>
        /// <param name="items"></param>
        public void Delete(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Session.Delete(item);
            }
        }

        /// <summary>
        /// Deletes all entites, that satisfy the expression.
        /// </summary>
        /// <param name="expr"></param>
        public void DeleteAll(Expression<Func<T, bool>> expr)
        {
            var items = FindAll(expr);
            foreach (var item in items)
            {
                Session.Delete(item);
            }          
        }

        /// <summary>
        /// Creates IQueryable object, that can be further developed.
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> CreateQuery()
        {
            return Session.Query<T>();
        }

        /// <summary>
        /// Creates IQueryable object based on given entity, that can be further developed.
        /// Typically used with <see cref="Any()"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IQueryable<T> CreateQuery(T entity)
        {
            return CreateQuery().Where(x => x.OID == entity.OID);
        }

        /// <summary>
        /// Determines, wheter exists given entity in Repository.
        /// Can be developed with further queries.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Exists(T entity)
        {
            return CreateQuery().Any(x => x.OID == entity.OID);
        }

        /// <summary>
        /// Returns how many results satisfy condition
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> expr)
        {
            return CreateQuery().Count(expr);
        }

        public ISQLQuery CreateSQL(string query)
        {
            return Session.CreateSQLQuery(query);
        }

        public IQuery CreateHQL(string query)
        {
            return Session.CreateQuery(query);
        }
    }
}
