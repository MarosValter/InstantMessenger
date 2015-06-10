using System;
using System.Collections.Generic;
using System.Linq;

namespace InstantMessenger.Core.Base.Implementation
{
    public class BROGeneric<TBDO> where TBDO : IBDO
    {
        protected readonly IRepository<TBDO> Repository;

        protected BROGeneric(IRepository<TBDO> repository)
        {
            Repository = repository;
        }

        public IList<TBDO> GetAll()
        {
            return Repository.FindAll();
        }

        public TBDO GetByOid(long oid)
        {
            return Repository.Get(oid);
        }

        public IList<TBDO> GetByOids(IList<long> oids)
        {
            return Repository.FindAll(x => oids.Contains(x.OID));
        }

        public void Create(TBDO aBDO)
        {
            aBDO.Created = DateTime.Now;
            if (ValidateCreate(aBDO))
                Repository.Create(aBDO);
        }

        public void Update(TBDO aBDO)
        {
            if (ValidateUpdate(aBDO))
                Repository.Update(aBDO);
        }

        public void Delete(TBDO aBDO)
        {
            if (ValidateDelete(aBDO))
                Repository.Delete(aBDO);
        }

        public IQueryable<TBDO> CreateQuery()
        {
            return Repository.CreateQuery();
        }

        #region Validation

        protected virtual bool ValidateCreate(IBDO aBDO)
        {
            return true;
        }

        protected virtual bool ValidateUpdate(IBDO aBDO)
        {
            return true;
        }

        protected virtual bool ValidateDelete(IBDO aBDO)
        {
            return true;
        }

        #endregion
    }
}
