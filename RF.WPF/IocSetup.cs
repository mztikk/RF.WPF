using System;
using System.Collections.Generic;
using System.Reflection;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using RFReborn.Internals;
using RFReborn.RandomR;
using StyletIoC;

namespace RF.WPF
{
    public sealed class IocSetup : StyletIoCModule
    {
        private static readonly string[] s_viewSuffix = new string[] { "View" };
        private static readonly string[] s_viewModelSuffix = new string[] { "ViewModel", "VM" };

        private readonly IEnumerable<Assembly> _assemblies;

        private readonly List<Type> _storageTypes = new List<Type>();

        public IocSetup(IEnumerable<Assembly> assemblies) => _assemblies = assemblies;

        public IocSetup() : this(new HashSet<Assembly> { typeof(IocSetup).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly() }) { }

        protected override void Load()
        {
            Bind<IocSetup>().ToInstance(this);
            Bind<Random>().To<CryptoRandom>().InSingletonScope();
            Bind<INavigationService>().To<NavigationService>().InSingletonScope();

            SetupStorage();
            SetupMVVM();
        }

        public void Configure(IContainer container)
        {
            foreach (Type type in _storageTypes)
            {
                var storage = container.Get(type) as IStorage;
                storage.ILoad();
            }
        }

        private IEnumerable<Type> GetAllTypes() => AssemblyInfo.GetAllTypes(_assemblies);

        private void SetupStorage()
        {
            foreach (Type type in GetAllTypes())
            {
                string baseName = type.BaseType?.FullName;
                if (baseName?.StartsWith("RF.WPF.Storage`1") == true)
                {
                    Bind(type).ToSelf().InSingletonScope();
                    _storageTypes.Add(type);
                }
            }
        }

        private void SetupMVVM()
        {
            Type viewModelType = typeof(ViewModelBase);

            foreach (Type type in GetAllTypes())
            {
                string name = type.FullName;

                if (type.BaseType == viewModelType)
                {
                    foreach (string suffix in s_viewModelSuffix)
                    {
                        if (name.EndsWith(suffix))
                        {
                            // on viewmodel
                            Bind(type).ToSelf();
                            break;
                        }
                    }
                }

                foreach (string suffix in s_viewSuffix)
                {
                    if (name.EndsWith(suffix))
                    {
                        // on view
                        Bind(type).ToSelf();
                        break;
                    }
                }
            }
        }
    }
}
