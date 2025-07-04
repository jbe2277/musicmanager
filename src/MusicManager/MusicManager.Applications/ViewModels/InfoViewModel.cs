﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Applications.ViewModels;

public class InfoViewModel : ViewModel<IInfoView>
{
    public InfoViewModel(IInfoView view) : base(view)
    {
        ShowWebsiteCommand = new DelegateCommand(ShowWebsite);
    }

    public ICommand ShowWebsiteCommand { get; }

    public string ProductName => ApplicationInfo.ProductName;

    public string Version => ApplicationInfo.Version;

    public string OSVersion => Environment.OSVersion.ToString();

    public string NetVersion => Environment.Version.ToString();

    public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;

    public void ShowDialog(object owner) => ViewCore.ShowDialog(owner);

    private void ShowWebsite(object? parameter)
    {
        var url = (string)parameter!;
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception e)
        {
            Log.Default.Error(e, "An exception occured when trying to show the url '{0}'.", url);
        }
    }
}
