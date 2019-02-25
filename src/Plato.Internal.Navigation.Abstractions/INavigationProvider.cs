namespace Plato.Internal.Navigation.Abstractions
{
    public interface INavigationProvider
    {
        void BuildNavigation(string name, INavigationBuilder builder);

    }
}
