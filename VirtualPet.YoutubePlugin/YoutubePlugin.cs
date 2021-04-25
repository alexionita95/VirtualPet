using System;
using VirtualPet.Core;
namespace VirtualPet.YoutubePlugin
{
    public class YoutubePlugin : IPlugin
    {
        IPetInteractor Interactor;
        public void Initialize()
        {

                YoutubeSearchForm form = new YoutubeSearchForm(Interactor);
                form.Show();
        }

        public void SetInteractor(IPetInteractor interactor)
        {
            Interactor = interactor;
        }
    }
}
