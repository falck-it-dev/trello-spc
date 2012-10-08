using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace TrelloSpc.Models
{
    public class TrelloNinjectModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind(x => x
                               .FromThisAssembly()
                               .SelectAllClasses()
                               .BindDefaultInterface()
                );
        }
    }
}