using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Layout.Adaptors;
using Plato.Users.ViewModels;

namespace Plato.Users.Adaptors
{
    public class UserListAdaptor : BaseAdaptorProvider
    {

        public IStringLocalizer T { get; set; }
        
        public UserListAdaptor(
            IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        /// <summary>
        /// An example implementation of view adaptors
        /// </summary>
        /// <returns></returns>
        public override async Task<IViewAdaptorResult> ConfigureAsync()
        {

            // Adapt the default UserList view
            return await Adapt("UserList",
                builder =>
                {
                    builder
                        .AdaptView("UserList2") // here we can change the default view file
                        .AdaptModel<UsersPaged>(model =>
                        {
                            // here we can alter the model if needed
                            model.PagerOpts.Page = 10;
                            return model;
                        })
                        .AdaptOutput(output =>
                        {
                            // here we can modify the compiled view output directly
                            return output;

                        });
                });
        }

    }
}
