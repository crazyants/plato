
namespace Plato.Internal.Layout
{
    public class LayoutZones
    {

        public const string HeaderZoneName = "header";
        public const string ToolsZoneName = "tools";
        public const string MetaZoneName = "meta";
        public const string ContentZoneName = "content";
        public const string SideBarZoneName = "sidebar";
        public const string FooterZoneName = "footer";
        public const string ActionsZoneName = "actions";
        public const string AsidesZoneName = "asides";

        public static string[] SupportedZones => new string[]
        {
            HeaderZoneName,
            ToolsZoneName,
            MetaZoneName,
            ContentZoneName,
            SideBarZoneName,
            FooterZoneName,
            ActionsZoneName,
            AsidesZoneName
        };
    }
}
