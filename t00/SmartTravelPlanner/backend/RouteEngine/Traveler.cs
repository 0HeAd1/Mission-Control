using System.Text.Json;

namespace Travelling
{
    public class Traveler : ICloneable
    {
        private string name;
        private string? currentLocation;
        private List<string> route = new List<string>();

        public bool PlanRouteTo(string destination, CityGraph map)
        {
            List<string> try_list = new List<string>(map.FindShortestPath(currentLocation, destination));
            if (try_list.Count() == 0)
            {
                return false;
            }
            if (String.IsNullOrEmpty(currentLocation))
            {
                if (route.Count() == 0)
                {
                    return false;
                }
                List<String> temp_route1 = map.FindShortestPath(route[0], destination);
                if (temp_route1.Count() == 0)
                {
                    return false;
                }
                route = map.FindShortestPath(route[0], destination);
                return true;
            }
            List<String> temp_route2 = map.FindShortestPath(currentLocation, destination);
            if (temp_route2.Count() == 0)
            {
                return false;
            }
            route = map.FindShortestPath(currentLocation, destination);
            return true;
        }
        public static Traveler LoadFromFile(string filePath)
        {
            JsonSerializerOptions json_options = new JsonSerializerOptions();
            json_options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            json_options.WriteIndented = true;
            string jsonString = File.ReadAllText(filePath);
            try
            {
                Traveler traveler = JsonSerializer.Deserialize<Traveler>(jsonString, json_options)!;

            }
            catch (JsonException)
            {
                throw new FileLoadException("Invalid travel data");

            }
            return JsonSerializer.Deserialize<Traveler>(jsonString, json_options)!;
        }

        public void SaveToFile(string filePath)
        {
            JsonSerializerOptions json_options = new JsonSerializerOptions();
            json_options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            json_options.WriteIndented = true;
            string JSON_name = JsonSerializer.Serialize(this, json_options);
            int fun_num = JSON_name.IndexOf("[");
            string nw_JSON = JSON_name[0..fun_num];
            if (route.Count == 0)
            {
                File.WriteAllText(filePath, nw_JSON + "[" + "]" + "\n" + "}");
                return;
            }
            if (route[0] is null)
            {
                nw_JSON += "[" + "]" + "\n" + "}";
                File.WriteAllText(filePath, nw_JSON);
                return;
            }
            nw_JSON += "[" + "\"" + string.Join("\", \"", route) + "\"]" + "\n" + "}";
            File.WriteAllText(filePath, nw_JSON);
        }


        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                name = value;
            }
        }

        public string CurrentLocation
        {
            get
            {
                return currentLocation;
            }
            set
            {
                currentLocation = value;
            }
        }

        public List<string> Route
        {
            get
            {
                return route;
            }
            set
            {
                route = value;
            }
        }

        public virtual string GetTravelSummary()
        {
            List<string> cool_lst = [$"Traveler: {name}", $"Location: {currentLocation}", $"Route: "];
            return string.Join(" | ", cool_lst) + string.Join(" -> ", route);
        }

        public object Clone()
        {
            Traveler clone = new Traveler(this.name!);
            clone.currentLocation = this.currentLocation;
            clone.route = new List<string>(this.route!);
            return clone;
        }

        public static bool operator ==(Traveler a, Traveler b)
        {
            if (a is null || b is null)
                return false;
            return a.name == b.name && a.currentLocation == b.currentLocation;
        }


        public string this[int index]
        {
            get
            {
                return route[index];
            }
        }

        public static bool operator !=(Traveler a, Traveler b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Traveler other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, currentLocation);
        }

        public string? GetNextStop()
        {
            if (GetStopCount() != 0)
            {
                return route[0];
            }
            return null;
        }
        public bool RemoveCity(string city)
        {
            return route.Remove(city[0..1].ToUpper() + city[1..]);
        }
        public void SortRoute()
        {
            route.Sort();
        }
        public bool HasCity(string city)
        {

            return route.Contains(city[0..1].ToUpper() + city[1..]);
        }

        public int GetStopCount()
        {
            return route.Count();
        }
        public void ClearRoute()
        {
            route.Clear();
        }
        public override string ToString()
        {
            List<string> cool_lst = [$"Traveler: {name}", $"Location: {currentLocation}", $"Route: "];
            return string.Join(" | ", cool_lst) + string.Join(" -> ", route);
        }
        public void AddCity(string city)
        {
            if (city == "" || city is null)
            {
                route.Add(null);
                throw new Exception();
            }
            route.Add(city[0..1].ToUpper() + city[1..]);
        }

        public string GetRoute()
        {
            return string.Join(" -> ", route);
        }

        public Traveler(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetLocation(string location)
        {
            if (location == "")
            {
                currentLocation = null;
                return;
            }
            currentLocation = location[0..1].ToUpper() + location[1..];
        }

        public string? GetLocation()
        {
            return currentLocation;
        }
        /*static void Main()
        {
            Traveler traveler = new Traveler("Alice");

            traveler.SetLocation("Kyiv");

            CityGraph graph = CityGraph.LoadFromFile("map.txt");

            traveler.PlanRouteTo("Warsaw", graph);

            // Output: Traveler: Alice | Location: Kyiv | Route: Kyiv -> Lviv -> Warsaw

            Console.WriteLine(traveler);
        }*/
    }
}