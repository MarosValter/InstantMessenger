using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Mapping;

namespace InstantMessenger.Core.Base
{
    /// <summary>
    /// Basic repository for storing entities.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IBDO
    {
        /// <summary>
        /// Finds single entity, that satisfies the expression, or null if none was found.
        /// Throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        T FindOne(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Finds all entities or empty collection if none was found.
        /// </summary>
        /// <returns></returns>
        IList<T> FindAll();

        /// <summary>
        /// Finds all entities, that satisfy the expression, or empty collection if none was found.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        IList<T> FindAll(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Finds entity based on its OID
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        T Get(long oid);

        /// <summary>
        /// Adds entity to the Repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Create(T entity);

        /// <summary>
        /// Adds entities to the Repository
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        void Create(IEnumerable<T> items);

        /// <summary>
        /// Updates entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Update(T entity);

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Delete given entities.
        /// </summary>
        /// <param name="items"></param>
        void Delete(IEnumerable<T> items);

        /// <summary>
        /// Deletes all entites, that satisfy the expression.
        /// </summary>
        /// <param name="expr"></param>
        void DeleteAll(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Creates an IQueryable object, that can be further extended.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> CreateQuery();

        /// <summary>
        /// Creates an IQueryable object based on given entity, that can be further extended.
        /// Typically used with <see cref="Any"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IQueryable<T> CreateQuery(T entity);

        /// <summary>
        /// Determines, wheter exists given entity in Repository.
        /// Can be extended with further queries.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Exists(T entity);

        /// <summary>
        /// Returns how many results satisfy condition.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Creates a SQL query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ISQLQuery CreateSQL(string query);

        /// <summary>
        /// Creates a HQL query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IQuery CreateHQL(string query);
    }
}