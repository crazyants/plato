using System;
using System.Collections.Generic;
using System.Text;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Badges.Tasks
{
    
    public class VisitsAwarder
    {

        private readonly IBadgesManager<Badge> _badgesManager;
        private readonly IBackgroundTaskManager _backgroundTaskManager;

        public VisitsAwarder(IBackgroundTaskManager backgroundTaskManager, IBadgesManager<Badge> badgesManager)
        {
            _backgroundTaskManager = backgroundTaskManager;
            _badgesManager = badgesManager;
        }

        public void Init()
        {

            _backgroundTaskManager.Start((sender, args) =>
            {

            }, 1000);

        }




    }
}
