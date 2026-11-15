namespace Travelling
{
    public class Logger<T>
    {
        private List<T> saved_data = new List<T>();

        public void Add(T entry)
        {
            saved_data.Add(entry);
        }

        public void Flush(string filePath)
        {
            for (int i = 0; i < saved_data.Count; ++i)
            {
                File.AppendAllText(filePath, saved_data[i] + "\n");
            }
            saved_data.Clear();
        }
    }
}
/*public class Start
{
    static void Main()
    {
        Logger<string> log = new Logger<string>();

        log.Add("Start");

        log.Flush("log.txt");


        log.Add("Traveler created: Alice");

        log.Add("Route planned: Kyiv -> Lviv -> Warsaw");

        log.Flush("log.txt");
    }
}*/
