using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Net4xGtkWinform
{

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class GtkWinformVSIXProjectPackage : AsyncPackage
    {
        public const string PackageGuidString = "678f9761-406c-4d51-ac24-cb669cfa8387";

        protected override async Task InitializeAsync(
          CancellationToken cancellationToken,
          IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await GtkCommand.InitializeAsync((AsyncPackage)this);
        }
    }
}
