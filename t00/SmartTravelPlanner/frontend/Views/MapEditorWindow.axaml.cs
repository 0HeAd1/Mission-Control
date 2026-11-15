using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Travelling;

namespace RouteEngineUI.Views;

public partial class MapEditorWindow : Window
{

    private CityGraph _map;
    public bool HasChanges { get; private set; }

    public MapEditorWindow(CityGraph map)
    {
        InitializeComponent();

        _map = map;
        MapList.ItemsSource = _map.GetMap();
    }

    private void OnAddConnectionClick(object? sender, RoutedEventArgs e)
    {
        ErrorText.IsVisible = false;

        try
        {
            if (!int.TryParse(DistInput.Text, out int dist))
            {
                throw new Exception("Distance must be an integer");
            }

            _map.AddConnection(FromCity.Text ?? "", ToCity.Text ?? "", dist);

            HasChanges = true;
            MapList.ItemsSource = _map.GetMap();

            FromCity.Text = "";
            ToCity.Text = "";
            DistInput.Text = "";
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            ErrorText.IsVisible = true;
        }
    }

    private void OnRemoveConnectionClick(object? sender, RoutedEventArgs e)
    {
        ErrorText.IsVisible = false;

        try
        {
            bool removed = _map.RemoveConnection(FromCity.Text ?? "", ToCity.Text ?? "");

            if (!removed)
            {
                throw new Exception("Connection not found");
            }

            HasChanges = true;
            MapList.ItemsSource = _map.GetMap();
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            ErrorText.IsVisible = true;
        }
    }

    private void OnDoneClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnMapSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        string? road = MapList.SelectedItem as string;

        if (road == null)
        {
            return;
        }

        int arrow = road.IndexOf(" -> ");
        int comma = road.LastIndexOf(", ");

        if (arrow < 0 || comma < arrow + 4)
            return;

        FromCity.Text = road[..arrow];
        ToCity.Text = road[(arrow + 4)..comma];
    }
}