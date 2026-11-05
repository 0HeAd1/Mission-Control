using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Travelling;
using System;
using System.Collections.Generic;
using System.IO;

namespace RouteEngineUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _mapFilename = "map.txt";
    
    [ObservableProperty]
    private string _travelerName = "";

    [ObservableProperty]
    private string _startLocation = "";

    [ObservableProperty]
    private string _destination = "";

    [ObservableProperty]
    private string _resultMessage = "Welcome Home, Such As It Is. This Squalid Hamlet, These Corrupted Lands, They Are Yours Now, And You Are Bound To Them.";
    //Brigands have run of these lanes. Keep to the side path; the Hamlet is just ahead [another variant]

    [RelayCommand]
    private void CalculateRoute()
    {
        if (string.IsNullOrWhiteSpace(StartLocation) || string.IsNullOrWhiteSpace(Destination) || string.IsNullOrWhiteSpace(TravelerName) || string.IsNullOrWhiteSpace(MapFilename))
        {
            ResultMessage = "Overconfidence is slow and insidious killer. Check the main fields: StartLocation, Destination, etc. You surely missed one of them.";
            return;
        }
        try
        {
            CityGraph map = CityGraph.LoadFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MapFilename));

            Traveler traveler = new Traveler(TravelerName);

            traveler.SetLocation(StartLocation);
            if (traveler.PlanRouteTo(Destination, map))
            {
                List<String> path = new List<String>(map.FindShortestPath(StartLocation, Destination));
                int dist = map.GetPathDistance(path);

                ResultMessage = $"And now the true test... hold fast, or expire. Traveller: {traveler.ToString()}\nMany fall in the face of chaos. But not this one, not today. The total distance: {dist} km";
  
                Logger<string> log = new Logger<string>();
                log.Add($"Traveler {traveler.GetName()} planned route {String.Join(" -> ", traveler.GetRoute())}");
                log.Flush(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"));
            }
            else
            {
                ResultMessage = "No route to find!";
            }
        }
        catch (Exception ex)
        {
            ResultMessage = $"Failure tests the mettle of heart, brain, and body. An error occurred: {ex.Message}";
        }
    }
}
