using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Waf.UnitTesting;
using Waf.MusicManager.Domain;
using Test.MusicManager.Domain.UnitTesting;

namespace Test.MusicManager.Domain
{
    [TestClass]
    public class TaskUtilityTest : DomainTest
    {
        private readonly AssertUnobservedExceptions assertUnobservedExceptions = new AssertUnobservedExceptions();

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
            Task task1 = Task.FromResult(new object());
            Task task2 = Task.FromResult(new object());

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
            TaskUtility.WhenAllFast(new[] { task1, task2 }).Wait();
            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);

            // Check tasks that not completed before calling WhenAllFast. Both are completed afterwards.
            task1 = Task.Delay(25);
            task2 = Task.Delay(50);

            Assert.IsFalse(task1.IsCompleted);
            Assert.IsFalse(task2.IsCompleted);
            TaskUtility.WhenAllFast(new[] { task1, task2 }).Wait();
            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);

            // Check tasks that will be cancelled. WhenAllFast waits just for the first one.
            task1 = Task.Run(() =>
            {
                Task.Delay(25).Wait();
                throw new TaskCanceledException();
            });
            task2 = Task.Run(() =>
            {
                Task.Delay(50).Wait();
                throw new TaskCanceledException();
            });

            Assert.IsFalse(task1.IsCompleted);
            Assert.IsFalse(task2.IsCompleted);
            AssertHelper.ExpectedException<TaskCanceledException>(
                () => TaskUtility.WhenAllFast(new[] { task1, task2 }).GetAwaiter().GetResult());
            Assert.IsTrue(task1.IsCompleted);
            Assert.IsFalse(task2.IsCompleted);

            // Check tasks that will fail. WhenAllFast waits just for the first one.
            task1 = Task.Run(() =>
            {
                Task.Delay(50).Wait();
                throw new InvalidOperationException();
            });
            task2 = Task.Run(() =>
            {
                Task.Delay(25).Wait();
                throw new ArgumentException();
            });

            Assert.IsFalse(task1.IsCompleted);
            Assert.IsFalse(task2.IsCompleted);
            AssertHelper.ExpectedException<ArgumentException>(
                () => TaskUtility.WhenAllFast(new[] { task1, task2 }).GetAwaiter().GetResult());
            Assert.IsFalse(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);

            // Argument check test
            AssertHelper.ExpectedException<ArgumentNullException>(() => TaskUtility.WhenAllFast(null));

            // Wait until task1 has finished so that we can check for unobserved task exceptions.
            Task.Delay(50).Wait();
        }
    }
}
