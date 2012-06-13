namespace Heath.Lister.Infrastructure.ViewModels
{
    public interface IViewModel
    {
        void Activate();
        void Deactivate(bool isNavigationInitiator);
    }
}