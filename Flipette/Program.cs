using Flipette;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Program
{
    static public void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Erreur : Argument nom de fichier attendu.");
            return;
        }
        var filename = args[0];
        if (!File.Exists(args[0]))
        {
            Console.WriteLine($"Erreur : Le fichier {args[0]} n'existe pas.");
            return;
        }

        string jsonString = File.ReadAllText(filename);
        var json = JsonConvert.DeserializeObject<dynamic[]>(jsonString);
        if (json == null)
        {
            Console.WriteLine("Impossible de deserialiser le fichier!");
            return;
        }
        var total = json.Length;
        int current = 0;
        var startTime = DateTime.Now;

        var options = new ParallelOptions();
        options.MaxDegreeOfParallelism = 20;

        Parallel.ForEach(json, options, item =>
        {
            var itemid = Interlocked.Increment(ref current);
            var gridlist = JsonConvert.DeserializeObject<object[]>(item.grid_json.ToString());
            var data = new List<string>();

            foreach (var g in gridlist)
            {
                data.Add(g.ToString());
            }

            var game = new Game(data, (int)item.grid_size, itemid);
            if (game.Run())
            {
                item.grid_bestscore = game.BestScore;
                if (game.BestPath != null)
                {
                    item.grid_bestpath = JArray.FromObject(game.BestPath);
                }
            }
        });

        string outputfilename = $"{Path.GetDirectoryName(filename)}\\{Path.GetFileNameWithoutExtension(filename)}-res{Path.GetExtension(filename)}";
        if (File.Exists(outputfilename))
        {
            File.Delete(outputfilename);
        }
        File.WriteAllText(outputfilename, JsonConvert.SerializeObject(json));

        var diff = DateTime.Now.Subtract(startTime);
        Console.WriteLine($"Temps de calcul total : {diff}");
    }
}
