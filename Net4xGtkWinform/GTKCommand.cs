using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Net4xGtkWinform.Properties;
using Process = System.Diagnostics.Process;

namespace Net4xGtkWinform
{
    internal sealed class GtkCommand
    {
        public const string guidGTKCommandSet = "93334a89-b8cb-4fcb-a75c-64661180b671";
        public const int cmdidOpenToCreateForm = 768 /*0x0300*/;
        public const int cmdidFixFormDesigner = 816;
        public const int cmdidLinkToRepository = 912;
        public const int cmdidLinkToSite = 913;

        public static readonly Guid CommandSet = new Guid(guidGTKCommandSet);
        private readonly AsyncPackage package;

        private GtkCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var commandOpenToCreateForm = new MenuCommand(OpenToCreateForm,
                new CommandID(CommandSet, cmdidOpenToCreateForm /*0x0300*/));
            commandService.AddCommand(commandOpenToCreateForm);
            var commandFixFormDesignerConfig = new MenuCommand(FixFormDesignerConfig,
                new CommandID(CommandSet, cmdidFixFormDesigner));
            commandService.AddCommand(commandFixFormDesignerConfig);
            var commandLinkToGitee = new MenuCommand(LinkToGitee,
                new CommandID(CommandSet, cmdidLinkToRepository));
            commandService.AddCommand(commandLinkToGitee);
            var commandLinkToGtkSite = new MenuCommand(LinkToGTKAPP,
                new CommandID(CommandSet, cmdidLinkToSite));
            commandService.AddCommand(commandLinkToGtkSite);
        }

        public static GtkCommand Instance { get; private set; }

        private IAsyncServiceProvider ServiceProvider => package;

        public bool Execute()
        {
            var stringBuilder = new StringBuilder();
            try
            {
                var project =
                    ((Package.GetGlobalService(typeof(DTE)) as DTE2).ToolWindows.SolutionExplorer.SelectedItems as
                        UIHierarchyItem[])[0].Object as Project;
                var name = project.Name;
                var debug = Path.GetDirectoryName(project.FullName) + "\\obj\\Debug";
                if (Directory.Exists(debug))
                {
                    foreach (var directory in Directory.GetDirectories(debug))
                    {
                        var fileName = Path.GetFileName(directory);
                        var newValue = Regex.Match(fileName, "[\\d\\.]+", RegexOptions.IgnoreCase).Value + ".0";
                        var runtimeConfigJson = $"{directory}\\{name}.runtimeconfig.json";
                        if (File.Exists(runtimeConfigJson))
                        {
                            var input = File.ReadAllText(runtimeConfigJson);
                            var pattern = "\"name\"\\:[\\s]*\"Microsoft\\.NETCore\\.App\"";
                            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                            {
                                var contents = Regex.Replace(input, pattern,
                                    "\"name\":\"Microsoft.WindowsDesktop.App\"", RegexOptions.IgnoreCase);
                                File.WriteAllText(runtimeConfigJson, contents);
                            }
                        }
                        else
                        {
                            var str =
                                Resources.GtkCommand_RuntimeConfigJson;
                            File.WriteAllText(runtimeConfigJson, str.Replace("[0]", fileName).Replace("[1]", newValue));
                        }

                        var designerPath = $"{directory}\\{name}.designer.runtimeconfig.json";
                        if (File.Exists(designerPath))
                        {
                            var input = File.ReadAllText(designerPath);
                            var pattern = "\"name\"\\:[\\s]*\"Microsoft\\.NETCore\\.App\"";
                            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                            {
                                var contents = Regex.Replace(input, pattern,
                                    "\"name\":\"Microsoft.WindowsDesktop.App\"", RegexOptions.IgnoreCase);
                                File.WriteAllText(designerPath, contents);
                            }
                        }
                        else
                        {
                            var str =
                                Net4xGtkWinform.Properties.Resources.GtkCommand_DesignerRuntimeConfigJson;
                            File.WriteAllText(designerPath,
                                str.Replace("[0]", fileName).Replace("[1]", newValue)
                                    .Replace("[2]", Environment.UserName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stringBuilder.Append(ex.Message);
            }

            if (stringBuilder.Length == 0)
            {
                stringBuilder.Append("Completed successfully. It is recommended to restart Visual Studio.\n");
            }

            VsShellUtilities.ShowMessageBox(package, stringBuilder.ToString(), "Fixing the form designer...",
                OLEMSGICON.OLEMSGICON_INFO, 0, 0);
            return true;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var obj = await package.GetServiceAsync(typeof(IMenuCommandService));
            var commandService = obj as OleMenuCommandService;
            obj = null;
            Instance = new GtkCommand(package, commandService);
            commandService = null;
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            VsShellUtilities.ShowMessageBox(package,
                string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()",
                    GetType().FullName), nameof(GtkCommand), OLEMSGICON.OLEMSGICON_INFO, 0,
                0);
        }

        private void FixFormDesignerConfig(object sender, EventArgs e)
        {
            Execute();
        }

        private void LinkToGitee(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://bitbucket.org/piero-viano/net4x.gtk.windows.forms");
        }

        private void LinkToGTKAPP(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://bitbucket.org/piero-viano/net4x.gtk.windows.forms");
        }

        private void OpenToCreateForm(object sender, EventArgs e)
        {
            SendKeys.Send("^+A");
        }
    }
}