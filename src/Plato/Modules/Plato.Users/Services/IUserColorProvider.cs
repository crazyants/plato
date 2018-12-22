namespace Plato.Users.Services
{
    public interface IUserColorProvider
    {

        UserColor GetColor();

    }

    public class UserColor
    {

        public string ForeColor { get; set; }

        public string BackColor { get; set; }

        public UserColor(string backColor) : this(backColor, "ffffff")
        {
            this.BackColor = backColor;
        }

        public UserColor(string backColor, string foreColor)
        {
            this.ForeColor = foreColor;
        }

    }

}
