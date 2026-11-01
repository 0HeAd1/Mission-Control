namespace Travelling 
{  
    class Program
    {
        static void Main()
        {
            Console.Write("Enter map filename: ");
            string input = Console.ReadLine();
            CityGraph map = CityGraph.LoadFromFile(input);
            // Initialization of the map

            Console.Write("Enter traveler name: ");
            string name = Console.ReadLine();
            Traveler traveler = new Traveler(name);

            Console.Write("Enter starting location: ");
            string strt_loc = Console.ReadLine();

            traveler.SetLocation(strt_loc);
            // Initialization of the traveler

            Console.Write("Enter destination: ");
            string destination = Console.ReadLine();
            // Initialization of the destination city

            if (traveler.PlanRouteTo(destination, map))
            {
                List<String> path = new List<String>(map.FindShortestPath(traveler.GetLocation(), destination));
                int dist = map.GetPathDistance(path);
                // find the shortest path from the traveler’s location to the destination
                Console.WriteLine(traveler);
                Console.WriteLine($"Total distance: {dist} km");
                Logger<string> log = new Logger<string>();
                log.Add($"Traveler {traveler.GetName()} planned route {String.Join(" -> ", traveler.GetRoute())}");
                log.Flush("log.txt");
            }
            else
            {
                Console.WriteLine("No route to find!");
            }
        }
    }
}
