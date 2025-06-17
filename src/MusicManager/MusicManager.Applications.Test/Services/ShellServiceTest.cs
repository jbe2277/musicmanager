using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.MusicManager.Applications.Views;
using Waf.MusicManager.Applications.Services;

namespace Test.MusicManager.Applications.Services;

[TestClass]
public class ShellServiceTest : ApplicationsTest
{
    [TestMethod]
    public void ClosingEventTest()
    {
        var service = Get<ShellService>();
        var shellView = Get<MockShellView>();

        Assert.AreEqual(shellView, service.ShellView);

        bool closingCalled = false;
        void EventHandler(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            closingCalled = true;
        }
        service.Closing += EventHandler;

        Assert.IsFalse(closingCalled);
        var eventArgs = new CancelEventArgs();
        shellView.RaiseClosing(eventArgs);
        Assert.IsTrue(closingCalled);
        Assert.IsTrue(eventArgs.Cancel);

        closingCalled = false;
        service.Closing -= EventHandler;
        Assert.IsFalse(closingCalled);
        shellView.RaiseClosing(new CancelEventArgs());
        Assert.IsFalse(closingCalled);
    }

    [TestMethod]
    public void TasksToCompleteBeforeShutdownTest()
    {
        var service = Get<ShellService>();

        Assert.IsFalse(service.TasksToCompleteBeforeShutdown.Any());
        var task = Task.CompletedTask;
        service.AddTaskToCompleteBeforeShutdown(task);
        Assert.AreEqual(task, service.TasksToCompleteBeforeShutdown.Single());
    }

    [TestMethod]
    public void IsApplicationBusyTest()
    {
        var service = Get<ShellService>();

        Assert.IsFalse(service.IsApplicationBusy);
            
        var busyContext1 = service.SetApplicationBusy();
        Assert.IsTrue(service.IsApplicationBusy);

        var busyContext2 = service.SetApplicationBusy();
        Assert.IsTrue(service.IsApplicationBusy);

        busyContext1.Dispose();
        Assert.IsTrue(service.IsApplicationBusy);

        busyContext2.Dispose();
        Assert.IsFalse(service.IsApplicationBusy);
    }

    [TestMethod]
    public void ShowErrorTest()
    {
        var service = Get<ShellService>();

        Exception? exception = null;
        string? message = null;
        service.ShowErrorAction = (ex, msg) => 
        {
            exception = ex;
            message = msg;
        };

        var testException = new InvalidOperationException();
        string testMessage = "Test";
        service.ShowError(testException, testMessage);
        Assert.AreEqual(testException, exception);
        Assert.AreEqual(testMessage, message);

        exception = null;
        message = null;
        service.ShowError(testException, "err: {0}", testMessage);
        Assert.AreEqual(testException, exception);
        Assert.AreEqual("err: " + testMessage, message);
    }
}
