namespace Heath.Lister.Infrastructure.ViewModels
{
    public interface IPageViewModel
    {
        void Activate();
        void Deactivate(bool isNavigationInitiator);
        void ViewReady(); // TODO: Maybe can delete...
    }
}