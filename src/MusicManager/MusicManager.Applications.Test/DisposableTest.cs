using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.MusicManager.Applications;

namespace Test.MusicManager.Applications;

[TestClass]
public class DisposableTest
{
    [TestMethod]
    public void DisposeCalled()
    {
        var disposable = new MockDisposable();
        Assert.AreEqual(0, disposable.DisposeCalled);
        disposable.Dispose();
        Assert.AreEqual(1, disposable.DisposeCalled);
        disposable.Dispose();
        Assert.AreEqual(1, disposable.DisposeCalled);
    }


    private class MockDisposable : Disposable
    {
        public int DisposeCalled { get; set; }
            
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing) DisposeCalled++; 
            base.Dispose(isDisposing);
        }
    }
}
