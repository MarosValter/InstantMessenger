using System.Reflection;
using Castle.DynamicProxy;
using NHibernate;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace InstantMessenger.Core.UOW
{
    /// <summary>
    /// This interceptor is used to manage transactions.
    /// </summary>
    public class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly ISessionFactory _sessionFactory;

        /// <summary>
        /// Creates a new NhUnitOfWorkInterceptor object.
        /// </summary>
        /// <param name="sessionFactory">Nhibernate session factory.</param>
        public UnitOfWorkInterceptor(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            //If there is a running transaction, just run the method
            if (UnitOfWork.Current != null || !RequiresDbConnection(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            try
            {
                UnitOfWork.Current = new UnitOfWork(_sessionFactory);
                UnitOfWork.Current.BeginTransaction();

                try
                {
                    invocation.Proceed();
                    UnitOfWork.Current.Commit();
                }
                catch
                {
                    try
                    {
                        UnitOfWork.Current.Rollback();
                    }
                    catch
                    {

                    }

                    throw;
                }
            }
            finally
            {
                UnitOfWork.Current = null;
            }
        }

        private static bool RequiresDbConnection(MethodInfo methodInfo)
        {
            return UnitOfWorkHelper.HasAttribute(methodInfo, typeof(UnitOfWorkAttribute)) ||
                   UnitOfWorkHelper.IsRepositoryMethod(methodInfo);
        }
    }
}
