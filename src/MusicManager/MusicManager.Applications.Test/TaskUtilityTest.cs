using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices.ActiveDirectory;
using System.Waf.UnitTesting;
using Test.MusicManager.Domain;
using Waf.MusicManager.Applications;

namespace Test.MusicManager.Applications;

[TestClass]
public class TaskUtilityTest : DomainTest
{
    private readonly AssertUnobservedExceptions assertUnobservedExceptions = new();

    protected override void OnInitialize()
    {
        base.OnInitialize();
        assertUnobservedExceptions.Initialize();
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();
        assertUnobservedExceptions.Cleanup();
    }        

    [TestMethod]
    public void WhenAllFastTest()
    {
        // Check tasks that are completed before calling WhenAllFast.
        (Task task1, Task task2) = (Task.CompletedTask, Task.FromResult(new object()));

        Assert.IsTrue(task1.IsCompleted);
        Assert.IsTrue(task2.IsCompleted);
        TaskUtility.WhenAllFast([task1, task2]).Wait();

        // Check tasks that not completed before calling WhenAllFast. Both are completed afterwards.
        var (tcs1, tcs2) = (new TaskCompletionSource(), new TaskCompletionSource());
        (task1, task2) = (tcs1.Task, tcs2.Task);
        var whenAll = TaskUtility.WhenAllFast([task1, task2]);
        Assert.AreEqual((false, false), (task1.IsCompleted, task2.IsCompleted));
        Assert.IsFalse(whenAll.IsCompleted);
        tcs2.SetResult();
        Assert.AreEqual((false, true), (task1.IsCompleted, task2.IsCompleted));
        Assert.IsFalse(whenAll.IsCompleted);
        tcs1.SetResult();
        Assert.AreEqual((true, true), (task1.IsCompleted, task2.IsCompleted));
        Assert.IsTrue(whenAll.IsCompleted);
        whenAll.GetAwaiter().GetResult();

        // Check tasks that will be cancelled. WhenAllFast waits just for the first one.
        (tcs1, tcs2) = (new TaskCompletionSource(), new TaskCompletionSource());
        (task1, task2) = (tcs1.Task, tcs2.Task);
        whenAll = TaskUtility.WhenAllFast([task1, task2]);
        Assert.AreEqual((false, false), (task1.IsCompleted, task2.IsCompleted));
        Assert.IsFalse(whenAll.IsCompleted);
        tcs1.SetCanceled();
        Assert.AreEqual((true, false), (task1.IsCompleted, task2.IsCompleted));
        Assert.IsTrue(whenAll.IsCompleted);
        AssertHelper.ExpectedException<TaskCanceledException>(() => whenAll.GetAwaiter().GetResult());

        // Check tasks that will fail. WhenAllFast waits just for the first one.
        (tcs1, tcs2) = (new TaskCompletionSource(), new TaskCompletionSource());
        (task1, task2) = (tcs1.Task, tcs2.Task);
        whenAll = TaskUtility.WhenAllFast([task1, task2]);
        Assert.AreEqual((false, false), (task1.IsCompleted, task2.IsCompleted));
        Assert.IsFalse(whenAll.IsCompleted);
        tcs2.SetException(new InvalidOperationException());
        Assert.AreEqual((false, true), (task1.IsCompleted, task2.IsCompleted));
        Assert.IsTrue(whenAll.IsCompleted);
        AssertHelper.ExpectedException<InvalidOperationException>(() => whenAll.GetAwaiter().GetResult());


        // Argument check test
        Assert.IsTrue(TaskUtility.WhenAllFast([]).IsCompleted);
        Assert.IsTrue(TaskUtility.WhenAllFast(null!).IsCompleted);
    }
}
