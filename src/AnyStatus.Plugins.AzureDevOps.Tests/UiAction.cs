using AnyStatus.API.Common.Services;
using System;
using System.Windows.Threading;

namespace AnyStatus.Plugins.AzureDevOps.Tests
{
    public class UiAction : IUiAction
    {
        public void Invoke(Action callback)
        {
            Dispatcher.CurrentDispatcher.Invoke(callback);
        }
    }
}
