namespace Test.MusicManager.Domain.UnitTesting
{
    public class AssertUnobservedExceptions
    {
        private Exception? unobservedTaskException;
        
        public void Initialize()
        {
            unobservedTaskException = null;
            TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        }

        public void Cleanup()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            TaskScheduler.UnobservedTaskException -= TaskSchedulerUnobservedTaskException;
            if (unobservedTaskException != null) throw new InvalidOperationException("Detected an unobserved exception.", unobservedTaskException);
        }

        private void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) => unobservedTaskException = e.Exception;
    }
}
