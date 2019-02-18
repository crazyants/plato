using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Localization
{
    //public class LocaleConfiguration : IConfigureOptions<LocaleOptions>
    //{

    //    private readonly IContextFacade _contextFacade;
    //    private readonly IDataProtectionProvider _dataProtectionProvider;
  
    //    public LocaleConfiguration(
    //        IDataProtectionProvider dataProtectionProvider, 
    //        IContextFacade contextFacade)
    //    {
    //        _dataProtectionProvider = dataProtectionProvider;
    //        _contextFacade = contextFacade;
    //    }

    //    public void Configure(LocaleOptions options)
    //    {
            
    //        // We have no settings to configure
    //        if (options == null)
    //        {
    //            throw new ArgumentNullException(nameof(options));
    //        }
            
    //    }


    //    public async Task<string> GetCurrentCultureAsync()
    //    {

    //        // Get users culture
    //        var user = await _contextFacade.GetAuthenticatedUserAsync();
    //        if (user != null)
    //        {
    //            if (!String.IsNullOrEmpty(user.Culture))
    //            {
    //                return user.Culture;
    //            }

    //        }

    //        // Get application culture
    //        var settings = await _contextFacade.GetSiteSettings();
    //        if (settings != null)
    //        {
    //            if (!String.IsNullOrEmpty(settings.Culture))
    //            {
    //                return settings.Culture;
    //            }
    //        }

    //        // Return en-US default culture
    //        return DefaultCulture;

    //    }




    //}

}
