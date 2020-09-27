using System;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using RFReborn.RandomR;

namespace RF.WPF
{
    public sealed class IocSetup : IocBase
    {
        private static readonly string[] s_viewSuffix = new string[] { "View" };
        private static readonly string[] s_viewModelSuffix = new string[] { "ViewModel", "VM" };

        protected override void Load()
        {
            Bind<IocBase>().ToInstance(this);
            Bind<Random>().To<CryptoRandom>().InSingletonScope();
            Bind<INavigationService>().To<NavigationService>().InSingletonScope();

            SetupMVVM();
        }

        private void SetupMVVM()
        {
            Type viewModelType = typeof(ViewModelBase);

            foreach (Type type in GetAllTypes())
            {
                string? name = type.FullName;

                if (type.BaseType == viewModelType)
                {
                    foreach (string suffix in s_viewModelSuffix)
                    {
                        if (name is { } && name.EndsWith(suffix))
                        {
                            // on viewmodel
                            Bind(type).ToSelf();
                            break;
                        }
                    }
                }

                foreach (string suffix in s_viewSuffix)
                {
                    if (name is { } && name.EndsWith(suffix))
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
