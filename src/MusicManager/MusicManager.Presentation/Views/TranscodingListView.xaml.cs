using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Data;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;
using Waf.MusicManager.Presentation.Controls;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(ITranscodingListView))]
    public partial class TranscodingListView : ITranscodingListView
    {
        private readonly Lazy<TranscodingListViewModel> viewModel;
        private readonly ListBoxDragDropHelper<TranscodeItem> listBoxDragDropHelper;
        private ListCollectionView transcodeItemsCollectionView = null!;

        public TranscodingListView()
        {
            InitializeComponent();
            viewModel = new Lazy<TranscodingListViewModel>(() => this.GetViewModel<TranscodingListViewModel>()!);
            listBoxDragDropHelper = new ListBoxDragDropHelper<TranscodeItem>(transcodingListBox, null, TryGetInsertItems, InsertItems);
            Loaded += FirstTimeLoadedHandler;
        }

        private TranscodingListViewModel ViewModel => viewModel.Value;

        private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
        {
            Loaded -= FirstTimeLoadedHandler;
            transcodeItemsCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(ViewModel.TranscodingManager.TranscodeItems);

            transcodeItemsCollectionView.IsLiveSorting = true;
            transcodeItemsCollectionView.LiveSortingProperties.Add(nameof(TranscodeItem.TranscodeStatus));
            var statusSortDescription = new SortDescription(nameof(TranscodeItem.TranscodeStatus), ListSortDirection.Ascending);
            transcodeItemsCollectionView.SortDescriptions.Add(statusSortDescription);

            transcodeItemsCollectionView.IsLiveGrouping = true;
            transcodeItemsCollectionView.LiveGroupingProperties.Add(nameof(TranscodeItem.TranscodeStatus));
            var statusGroupDescription = new PropertyGroupDescription(nameof(TranscodeItem.TranscodeStatus));
            transcodeItemsCollectionView.GroupDescriptions.Add(statusGroupDescription);

            foreach (var x in ViewModel.TranscodingManager.TranscodeItems) PropertyChangedEventManager.AddHandler(x, TranscodeItemPropertyChanged, "");
            CollectionChangedEventManager.AddHandler(ViewModel.TranscodingManager.TranscodeItems, TranscodeItemsCollectionChanged);
        }

        private static IEnumerable? TryGetInsertItems(DragEventArgs e) => e.Data.GetData(DataFormats.FileDrop) as IEnumerable ?? e.Data.GetData(typeof(MusicFile[])) as IEnumerable;

        private void InsertItems(int index, IEnumerable itemsToInsert)
        {
            if (itemsToInsert is IEnumerable<string> fileNames)
            {
                ViewModel.InsertFilesAction(index, fileNames);
            }
            else if (itemsToInsert is IEnumerable<MusicFile> musicFiles)
            {
                ViewModel.InsertMusicFilesAction(index, musicFiles);
            }
        }

        private void TranscodeItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var x in e.OldItems!.Cast<TranscodeItem>()) PropertyChangedEventManager.RemoveHandler(x, TranscodeItemPropertyChanged, "");
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var x in e.NewItems!.Cast<TranscodeItem>()) PropertyChangedEventManager.AddHandler(x, TranscodeItemPropertyChanged, "");
                transcodingListBox.ScrollIntoView(ViewModel.TranscodingManager.TranscodeItems[^1]);
            }
            else throw new NotSupportedException("This action type is not supported: " + e.Action);
            transcodeItemsCollectionView.Refresh();  // Workaround because live shaping does not support to sort groupings.
        }
        
        private void TranscodeItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TranscodeItem.TranscodeStatus)) transcodeItemsCollectionView.Refresh();  // Workaround because live shaping does not support to sort groupings.
        }

        private void ListBoxItemContextMenuOpening(object sender, RoutedEventArgs e) => ((FrameworkElement)sender).ContextMenu.DataContext = ViewModel;

        private void StatusBarButtonClick(object sender, RoutedEventArgs e)
        {
            menuPopup.Width = statusBarButton.ActualWidth;
            menuPopup.IsOpen = true;
        }
    }
}
