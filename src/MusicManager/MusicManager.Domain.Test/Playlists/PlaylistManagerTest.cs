using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;
using Test.MusicManager.Domain.UnitTesting;

namespace Test.MusicManager.Domain.MusicFiles
{
    [TestClass]
    public class PlaylistManagerTest : DomainTest
    {
        private PlaylistItem[] threeItems = null!;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            threeItems = new[]
            {
                new PlaylistItem(new MockMusicFile(new MusicMetadata(TimeSpan.FromMinutes(2), 256) { Album = "1" }, "")),
                new PlaylistItem(new MockMusicFile(new MusicMetadata(TimeSpan.FromMinutes(2), 256) { Album = "2" }, "")),
                new PlaylistItem(new MockMusicFile(new MusicMetadata(TimeSpan.FromMinutes(2), 256) { Album = "3" }, "")),
            };
        }
        
        [TestMethod]
        public void PreviousAndNext()
        {
            var manager = new PlaylistManager();
            manager.AddAndReplaceItems(threeItems);

            Assert.IsFalse(manager.CanPreviousItem);
            Assert.IsFalse(manager.CanNextItem);

            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.CurrentItem = threeItems[0]);
            Assert.IsFalse(manager.CanPreviousItem);
            Assert.IsTrue(manager.CanNextItem);

            AssertHelper.PropertyChangedEvent(manager, x => x.CanPreviousItem, () => manager.NextItem());
            Assert.AreEqual(threeItems[1], manager.CurrentItem);
            Assert.IsTrue(manager.CanPreviousItem);
            Assert.IsTrue(manager.CanNextItem);

            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.NextItem());
            Assert.AreEqual(threeItems[2], manager.CurrentItem);
            Assert.IsTrue(manager.CanPreviousItem);
            Assert.IsFalse(manager.CanNextItem);

            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.PreviousItem());
            Assert.AreEqual(threeItems[1], manager.CurrentItem);
            Assert.IsTrue(manager.CanPreviousItem);
            Assert.IsTrue(manager.CanNextItem);
        }

        [TestMethod]
        public void PreviousAndNextWithShuffle()
        {
            var randomService = new MockRandomService();
            int randomNumber = 0;
            int lastMaxValue = -1;
            randomService.NextRandomNumberStub = maxValue =>
            {
                lastMaxValue = maxValue;
                return randomNumber;
            };

            var manager = new PlaylistManager(3, randomService);
            manager.AddAndReplaceItems(threeItems);
            manager.Shuffle = true;

            Assert.IsFalse(manager.CanPreviousItem);
            Assert.IsFalse(manager.CanNextItem);

            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.CurrentItem = threeItems[0]);
            Assert.IsFalse(manager.CanPreviousItem);
            Assert.IsTrue(manager.CanNextItem);

            randomNumber = 1;
            AssertHelper.PropertyChangedEvent(manager, x => x.CanPreviousItem, () => manager.NextItem());
            Assert.AreEqual(1, lastMaxValue);
            Assert.AreEqual(threeItems[2], manager.CurrentItem);
            Assert.IsTrue(manager.CanPreviousItem);
            Assert.IsTrue(manager.CanNextItem);

            randomNumber = 0;
            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.NextItem());
            Assert.AreEqual(0, lastMaxValue);
            Assert.AreEqual(threeItems[1], manager.CurrentItem);
            Assert.IsTrue(manager.CanPreviousItem);
            Assert.IsFalse(manager.CanNextItem);

            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.PreviousItem());
            Assert.AreEqual(threeItems[2], manager.CurrentItem);
            Assert.IsTrue(manager.CanPreviousItem);
            Assert.IsTrue(manager.CanNextItem);

            AssertHelper.PropertyChangedEvent(manager, x => x.CanPreviousItem, () => manager.PreviousItem());
            Assert.AreEqual(threeItems[0], manager.CurrentItem);
            Assert.IsFalse(manager.CanPreviousItem);
            Assert.IsTrue(manager.CanNextItem);
        }

        [TestMethod]
        public void ResetNextWithShuffle()
        {
            var manager = new PlaylistManager();
            manager.AddAndReplaceItems(threeItems);
            manager.Shuffle = true;
            manager.CurrentItem = threeItems[0];

            for (int i = 0; i < 2; i++)
            {
                Assert.IsTrue(manager.CanNextItem);
                manager.NextItem();
            }

            Assert.IsFalse(manager.CanNextItem);

            manager.Reset();

            for (int i = 0; i < 2; i++)
            {
                Assert.IsTrue(manager.CanNextItem);
                manager.NextItem();
            }

            Assert.IsFalse(manager.CanNextItem);
        }

        [TestMethod]
        public void PreviousAndNextWithRepeat()
        {
            var manager = new PlaylistManager();
            manager.AddAndReplaceItems(threeItems);
            manager.Repeat = true;
            manager.CurrentItem = threeItems[0];

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(threeItems[i % 3], manager.CurrentItem);
                Assert.IsTrue(manager.CanNextItem);
                manager.NextItem();
            }

            manager.Shuffle = true;

            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(manager.CanNextItem);
                manager.NextItem();
            }
        }
        
        [TestMethod]
        public void PreviousAndNextWithClear()
        {
            var manager = new PlaylistManager();
            manager.AddAndReplaceItems(threeItems);
            manager.CurrentItem = threeItems[1];

            Assert.IsTrue(manager.CanPreviousItem);
            AssertHelper.PropertyChangedEvent(manager, x => x.CanPreviousItem, () => manager.ClearItems());
            Assert.IsFalse(manager.CanPreviousItem);
            AssertHelper.ExpectedException<InvalidOperationException>(() => manager.PreviousItem());

            manager.AddAndReplaceItems(threeItems);
            manager.CurrentItem = threeItems[1];

            Assert.IsTrue(manager.CanNextItem);
            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.ClearItems());
            Assert.IsFalse(manager.CanNextItem);
            AssertHelper.ExpectedException<InvalidOperationException>(() => manager.NextItem());
        }

        [TestMethod]
        public void NextWithShuffleAndRepeatAndOneItem()
        {
            var manager = new PlaylistManager();
            manager.AddAndReplaceItems(threeItems.Take(1));
            manager.Shuffle = true;
            manager.Repeat = true;
            manager.CurrentItem = threeItems[0];

            manager.NextItem();
            Assert.AreEqual(threeItems[0], manager.CurrentItem);
        }

        [TestMethod]
        public void AddRemoveAndMoveItems()
        {
            var manager = new PlaylistManager();
            Assert.AreEqual(0, manager.Items.Count);

            manager.AddItems(threeItems);
            Assert.AreEqual(3, manager.Items.Count);

            manager.RemoveItems(new[] { threeItems[2] });
            Assert.AreEqual(2, manager.Items.Count);

            manager.AddAndReplaceItems(threeItems);
            Assert.AreEqual(3, manager.Items.Count);

            manager.MoveItems(2, new[] { manager.Items[0] });
            AssertHelper.SequenceEqual(new[] { threeItems[1], threeItems[2], threeItems[0] }, manager.Items);
        }

        [TestMethod]
        public void PreviousAndNextDuringAddRemoveItems()
        {
            var manager = new PlaylistManager();
            manager.AddAndReplaceItems(threeItems);
            
            manager.CurrentItem = threeItems[0];
            manager.NextItem();

            Assert.IsTrue(manager.CanNextItem);
            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.RemoveItems(new[] { threeItems[2] }));
            Assert.IsFalse(manager.CanNextItem);
            AssertHelper.PropertyChangedEvent(manager, x => x.CanNextItem, () => manager.AddItems(new[] { threeItems[2] }));
            Assert.IsTrue(manager.CanNextItem);

            manager.NextItem();
            Assert.AreEqual(threeItems[2], manager.CurrentItem);
            manager.PreviousItem();
            Assert.AreEqual(threeItems[1], manager.CurrentItem);

            Assert.IsTrue(manager.CanPreviousItem);
            AssertHelper.PropertyChangedEvent(manager, x => x.CanPreviousItem, () => manager.RemoveItems(new[] { threeItems[0] }));
            Assert.IsFalse(manager.CanPreviousItem);
        }

        [TestMethod]
        public void TotalDurationWithMetadataLoading()
        {
            var manager = new PlaylistManager();
            Assert.AreEqual(TimeSpan.Zero, manager.TotalDuration);
            var firstFile = new MockMusicFile(new MusicMetadata(TimeSpan.FromSeconds(10), 0), "");
            manager.AddAndReplaceItems(new[] { new PlaylistItem(firstFile) });
            
            var secondMetadata = new MusicMetadata(TimeSpan.FromSeconds(20), 0);
            var secondFile = new MusicFile(async x =>
            {
                await Task.Delay(10);
                return secondMetadata;
            }, "");

            manager.AddItems(new[] { new PlaylistItem(secondFile) });
            Assert.IsTrue(manager.IsTotalDurationEstimated);
            Assert.AreEqual(TimeSpan.FromSeconds(20), manager.TotalDuration);
            AssertHelper.PropertyChangedEvent(manager, x => x.TotalDuration, () => secondFile.GetMetadataAsync().GetResult());
            Assert.IsFalse(manager.IsTotalDurationEstimated);
            Assert.AreEqual(TimeSpan.FromSeconds(30), manager.TotalDuration);

            var thirdFile = new MockMusicFile(new MusicMetadata(TimeSpan.FromSeconds(30), 0), "");
            AssertHelper.PropertyChangedEvent(manager, x => x.TotalDuration, () => manager.AddItems(new[] { new PlaylistItem(thirdFile) }));
            Assert.AreEqual(TimeSpan.FromSeconds(60), manager.TotalDuration);
        }
    }
}
