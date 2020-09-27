using System;
using System.Collections.Generic;
using System.Reflection;
using RFReborn.Internals;
using StyletIoC;

namespace RF.WPF
{
    public abstract class IocBase : StyletIoCModule
    {
        protected readonly IEnumerable<Assembly> _assemblies;

        public IocBase(IEnumerable<Assembly> assemblies) => _assemblies = assemblies;

        public IocBase() : this(new HashSet<Assembly> { typeof(IocSetup).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly() }) { }
        protected IEnumerable<Type> GetAllTypes() => AssemblyInfo.GetAllTypes(_assemblies);

        public virtual void Configure(IContainer container) { }

        protected override void Load()
        {
            Bind<IocBase>().ToInstance(this);

            Setup();
        }

        protected abstract void Setup();
    }
}
