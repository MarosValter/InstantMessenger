using FluentNHibernate.Mapping;

namespace InstantMessenger.Core.Base.Implementation
{
    public abstract class ClassMapBase<TBDO> : ClassMap<TBDO> where TBDO : IBDO
    {
        protected ClassMapBase(string tablePrefix, string tableName)
        {
            Table(tableName);
            Id(x => x.OID).Column(tablePrefix + "OID").GeneratedBy.Identity();
            Map(x => x.Created).Column(tablePrefix + "Created").Not.Nullable();
        }
    }
}
