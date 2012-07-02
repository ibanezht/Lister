#region usings

using Ninject;
using Ninject.Modules;

#endregion

namespace Heath.Lister.Infrastructure
{
    public static class DependencyContainer
    {
        private static IKernel _instance;

        public static IKernel Instance
        {
            get { return _instance; }
        }

        public static void Configure(INinjectModule module)
        {
            _instance = new StandardKernel(module);
        }
    }
}