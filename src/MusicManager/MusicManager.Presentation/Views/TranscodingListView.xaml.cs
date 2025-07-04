﻿using System.Collections;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Data;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;
using Waf.MusicManager.Presentation.Controls;

namespace Waf.MusicManager.Presentation.Views;

public partial class TranscodingListView : ITranscodingListView
{
    private readonly Lazy<TranscodingListViewModel> viewModel;
    private readonly ListBoxDragDropHelper<TranscodeItem> listBoxDragDropHelper;
    private ListCollectionView transcodeItemsCollectionView = null!;

    public TranscodingListView()
    {
        InitializeComponent();
        viewModel = new(() => this.GetViewModel<TranscodingListViewModel>()!);
        listBoxDragDropHelper = new(transcodingListBox, null, TryGetInsertItems, InsertItems);
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

        ViewModel.TranscodingManager.TranscodeItems.CollectionChanged += TranscodeItemsCollectionChanged;
        ViewModel.TranscodingManager.TranscodeItems.CollectionItemChanged += TranscodeItemPropertyChanged;
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
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            transcodingListBox.ScrollIntoView(ViewModel.TranscodingManager.TranscodeItems[^1]);
        }
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
