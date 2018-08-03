using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.ViewAdaptors;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewAdaptors
{
    public class UserListAdaptor : BaseAdaptorProvider
    {

        public IStringLocalizer T { get; set; }
        
        public UserListAdaptor(
            IStringLocalizer<UserListAdaptor> localizer)
        {
            T = localizer;
        }
        
        /// <summary>
        /// An example implementation of view adaptors
        /// </summary>
        /// <returns></returns>
        public override Task<IViewAdaptorResult> ConfigureAsync()
        {

            return Task.FromResult(default(IViewAdaptorResult));

            // Adapt the default UserList view
            //return await Adapt("User.List",
            //    builder =>
            //    {
            //        builder
            //            //.AdaptView("UserList2") // here we can change the default view file
            //            .AdaptModel<UsersPagedViewModel>(model =>
            //            {
            //                // here we can alter the model if needed
            //                model.PagerOpts.Page = 10;
            //                return model;
            //            })
            //            .AdaptOutput(output =>
            //            {
            //                // here we can modify the compiled view output directly
            //                return output;

            //            });
            //    });
        }

    }
}
