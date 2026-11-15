using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Travelling;

namespace RouteEngineUI.Views;

public partial class MainWindow : Window
{
    private CityGraph? _map;
    private Traveler _traveler;

    public MainWindow()
    {
        InitializeComponent();

        _traveler = new Traveler("");
    }

    private async void OnLoadMapClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var file = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Choose the route file",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Text Document")
                    {
                        Patterns = new [] {"*.txt"}
                    }
                }

            });
            if (file.Count == 1)
            {
                string? path = file[0].TryGetLocalPath();
                if (path != null)
                {
                    _map = CityGraph.LoadFromFile(path);
                }
            }

        }
        catch (System.Exception ex)
        {
            AfflictionText.Text = ex.Message;
            AfflictionBanner.IsVisible = true;
        }
    }

    private void OnDismissErrorClick(object? sender, RoutedEventArgs e)
    {
        AfflictionBanner.IsVisible = false;
    }

    private void OnPlanRouteClick(object? sender, RoutedEventArgs e)
    {
        string? travelerName = TravellerName.Text;
        string? place = TravellerPlace.Text;
        string? destination = DestName.Text;


        var missingFields = new System.Collections.Generic.List<string>();

        if (string.IsNullOrWhiteSpace(travelerName)) missingFields.Add("Dungeoner name");
        if (string.IsNullOrWhiteSpace(place)) missingFields.Add("Dungeoner native city");
        if (string.IsNullOrWhiteSpace(destination)) missingFields.Add("Destination city");

        if (missingFields.Count != 0)
        {
            string errorDetails = string.Join(", ", missingFields);
            if (missingFields.Count > 1)
            {
                AfflictionText.Text = $"Missing required fields ({errorDetails})";
            }
            else
            {
                AfflictionText.Text = $"Missing required field ({errorDetails})";
            }

            AfflictionBanner.IsVisible = true;
            return;
        }

        if (_map == null)
        {
            AfflictionText.Text = "You must load a Realm Map (.txt) first!";
            AfflictionBanner.IsVisible = true;
            return;
        }

        AfflictionBanner.IsVisible = false;

        _traveler.Name = travelerName;
        _traveler.SetLocation(place);

        bool success = _traveler.PlanRouteTo(destination, _map);

        if (success)
        {
            RouteList.ItemsSource = _traveler.Route;

            int totalDist = _map.GetPathDistance(_traveler.Route);
            DistanceText.Text = $"{totalDist} km";
        }
        else
        {
            RouteList.ItemsSource = null;
            DistanceText.Text = "0 km";

            AfflictionText.Text = "Path obscured. No valid route found in the current Realm.";
            AfflictionBanner.IsVisible = true;
        }
    }

    private async void OnSaveJsonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Choose where to save traveler state",
                SuggestedFileName = "traveler.json",
                DefaultExtension = "json",
                FileTypeChoices = new[]
                 {
                    new FilePickerFileType("JSON File")
                    {
                        Patterns = new [] {"*.json"}
                    }
                }
            });
            var path = file?.TryGetLocalPath();
            if (path != null)
            {
                _traveler.SaveToFile(path);
            }
        }
        catch (System.Exception ex)
        {
            AfflictionText.Text = ex.Message;
            AfflictionBanner.IsVisible = true;
        }
    }

    private async void OnLoadJsonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_map == null)
            {
                AfflictionText.Text = "Enter the required map first!";
                AfflictionBanner.IsVisible = true;
                return;
            }

            AfflictionBanner.IsVisible = false;

            var files = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Load traveler state",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                new FilePickerFileType("JSON File")
                    {
                        Patterns = new [] {"*.json"}
                    }
                }
            });

            if (files.Count == 1)
            {
                string? path = files[0].TryGetLocalPath();
                if (path != null)
                {
                    _traveler = Traveler.LoadFromFile(path);
                    TravellerName.Text = _traveler.Name;
                    TravellerPlace.Text = _traveler.CurrentLocation;
                    RouteList.ItemsSource = _traveler.Route;
                    if (_map != null && _traveler.Route != null && _traveler.Route.Count > 0)
                    {
                        int totalDist = _map.GetPathDistance(_traveler.Route);
                        DistanceText.Text = $"{totalDist} km";
                    }
                    else
                    {
                        DistanceText.Text = "0 km";
                    }

                    AfflictionBanner.IsVisible = false;
                }
            }
        }
        catch (System.Exception ex)
        {
            AfflictionText.Text = $"Failed to load: {ex.Message}";
            AfflictionBanner.IsVisible = true;
        }
    }

    private void OnClearRouteClick(object? sender, RoutedEventArgs e)
    {
        TravellerName.Text = System.String.Empty;
        TravellerPlace.Text = System.String.Empty;
        RouteList.ItemsSource = null;
        DistanceText.Text = "0 km";

        if (_traveler?.Route != null)
        {
            _traveler.Route.Clear();
        }

        AfflictionBanner.IsVisible = false;
    }

    private void OnAbandonClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
