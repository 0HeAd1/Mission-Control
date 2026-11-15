namespace Travelling
{
    public class CityGraph
    {
        public struct Way
        {
            public string destination;
            public int distance;
            public Way(string destination, int distance)
            {
                this.destination = destination;
                this.distance = distance;
            }
        }
        private Dictionary<String, List<Way>>? adjacencyList;

        private string? initial_input;

        public int GetPathDistance(List<string> path)
        {
            if (path == null)
                return 0;
            List<String> path1 = new List<String>();
            foreach (var elem in path)
            {
                if (string.IsNullOrEmpty(elem))
                    continue;
                path1.Add(elem[0..1].ToUpper() + elem[1..]);
            }
            int Distance = 0;
            String temp_city = "";
            List<Way> out_list = new List<Way>();
            int ctr = 0;
            bool flag = false;
            if (path1.Count() == 0 || path1.Count() == 1)
                return 0;
            while (ctr < (path1.Count() - 1))
            {
                temp_city = path1[ctr];
                if (adjacencyList.TryGetValue(temp_city, out out_list))
                {
                    foreach (var elem in out_list)
                    {
                        if (elem.destination == path1[ctr + 1])
                        {
                            Distance += elem.distance;
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    ++ctr;
                    flag = false;
                }
                else
                {
                    return 0;
                }
            }
            return Distance;
        }
        public List<string> FindShortestPath(string from, string to)
        {
            List<string> res_lst = new List<string>();
            string from1 = from[0..1].ToUpper() + from[1..];
            string to1 = to[0..1].ToUpper() + to[1..];
            if (!(adjacencyList.ContainsKey(from1) && adjacencyList.ContainsKey(to1)))
            {
                return res_lst;
            }
            if (from1 == to1)
            {
                res_lst.Add(from1);
                return res_lst;
            }
            int Distance = 0;
            int temp_num = int.MaxValue;
            string temp_city = from1;
            List<String> path = new List<String>();
            List<String> unvisited = new List<String>(adjacencyList.Keys);
            List<Way>? corners = new List<Way>();
            Dictionary<String, int>? distance_dict = new Dictionary<String, int>();
            Dictionary<String, String>? cities_dict = new Dictionary<String, String>();
            foreach (String elem in unvisited)
            {
                if (elem == from1)
                {
                    distance_dict.Add(from1, 0);
                    continue;
                }
                distance_dict.Add(elem, int.MaxValue);
            }
            while (unvisited.Count() != 0)
            {
                foreach (string elem in unvisited)
                {
                    if (distance_dict[elem] < temp_num)
                    {
                        temp_num = distance_dict[elem];
                        temp_city = elem;
                    }
                }
                if (distance_dict[temp_city] == int.MaxValue)
                    break;
                if (temp_city == to1)
                {
                    break;
                }
                adjacencyList.TryGetValue(temp_city, out corners);
                foreach (var elem in corners)
                {
                    Distance = distance_dict[temp_city] + elem.distance;
                    if (Distance < distance_dict[elem.destination])
                    {
                        distance_dict[elem.destination] = Distance;
                        cities_dict[elem.destination] = temp_city;
                    }
                    Distance = 0;
                }
                unvisited.Remove(temp_city);
                temp_num = int.MaxValue;
            }
            if (!cities_dict.ContainsKey(to1))
                return res_lst;
            res_lst.Add(to1);
            temp_city = to1;
            while (temp_city != from1)
            {
                res_lst.Add(cities_dict[temp_city]);
                temp_city = (cities_dict[temp_city]);
            }
            res_lst.Reverse();
            return res_lst;
        }

        // Loads a bidirectional graph from a file and returns an instance.
        public static CityGraph LoadFromFile(string filePath)
        {
            CityGraph new_graph = new CityGraph();
            new_graph.adjacencyList = new Dictionary<String, List<Way>>();
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Count(); ++i)
            {
                if (String.IsNullOrWhiteSpace(lines[i]))
                    continue;
                int cool_dash = lines[i].IndexOf("-");
                int cool_comma = lines[i].IndexOf(",");

                string word1 = lines[i][0..1].ToUpper() + lines[i][1..cool_dash];

                string word2 = lines[i][(cool_dash + 1)..(cool_dash + 2)].ToUpper() + lines[i][(cool_dash + 2)..cool_comma];

                string number = lines[i][(cool_comma + 1)..];

                Way new_Way = new Way(word2, int.Parse(number));
                List<Way>? new_list = new List<Way>();
                if (new_graph.adjacencyList.TryGetValue(word1, out new_list))
                {
                    new_list.Add(new_Way);
                    new_graph.initial_input += word1 + "-" + word2 + "," + number + "\n";
                }
                else
                {
                    List<Way>? new_list1 = new List<Way>();
                    new_list1.Add(new_Way);
                    new_graph.adjacencyList.Add(word1, new_list1);
                    new_graph.initial_input += word1 + "-" + word2 + "," + number + "\n";
                }

                Way other_Way = new Way(word1, int.Parse(number));
                List<Way>? other_list = new List<Way>();
                if (new_graph.adjacencyList.TryGetValue(word2, out other_list))
                {

                    other_list.Add(other_Way);
                    new_graph.initial_input += word2 + "-" + word1 + "," + number;
                    if (i != (lines.Count() - 1))
                        new_graph.initial_input += "\n";
                }
                else
                {
                    List<Way>? other_list1 = new List<Way>();
                    other_list1.Add(other_Way);
                    new_graph.adjacencyList.Add(word2, other_list1);
                    new_graph.initial_input += word2 + "-" + word1 + "," + number;
                    if (i != (lines.Count() - 1))
                        new_graph.initial_input += "\n";
                }
            }
            return new_graph;
        }
        public override string ToString()
        {
            return initial_input;
        }
        /*static void Main()
        {
            CityGraph g = CityGraph.LoadFromFile("map.txt");

            var path1 = new List<string> { "kyiv", "lviv", "larsaw" };

            int d1 = g.GetPathDistance(path1);

            //Output: Distance: 930

            Console.WriteLine($"Distance: {d1}");
        }*/
    }
}
