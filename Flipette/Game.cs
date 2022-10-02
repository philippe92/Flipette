namespace Flipette
{
    public class Game
    {
        private List<string> _data;
        private int _l;
        private int _start;
        private long _nbpathes;
        private int _itemid;

        public Game(List<string> data, int l, int itemid)
        {
            _data = data;
            _l = l;
            _itemid = itemid;
            
            for(int i=0; i<l*l ; i++)
            {
                if (_data[i]=="S")
                {
                    _start = i;
                    break;
                }
            }
        }

        public (int,int) DecodeXY(int i)
        {
            (int, int) result;
            result.Item2 = i / _l;
            result.Item1 = i % _l;
            return result;
        }

        public int EncodeXY(int x, int y)
        {
            return y * _l + x;
        }

        public long BestScore { get; private set; }

        public List<int>? BestPath { get; private set; }

        public bool Run()
        {
            Console.WriteLine($"[{_itemid:000}] Démarrage");
            var startTime = DateTime.Now;
            var done = new bool[_l * _l];
            long score = 0;
            if (!RecursiveLoop(_start, ref score, done, out var chemin))
            {
                Console.WriteLine("Erreur : pas de chemin trouvé!!!");
                return false;
            }

            var diff = DateTime.Now.Subtract(startTime);
            Console.WriteLine($"[{_itemid:000}] Meilleur score trouvé : {score:##,#}");
            Console.WriteLine($"[{_itemid:000}] Nb de chemins parcourus : {_nbpathes:##,#}");

            if (chemin == null)
            {
                Console.WriteLine("Erreur : le chemin retourné est null!!");
                return false;
            }

            chemin.Add(_start);
            chemin.Reverse();
            /*int[] table = new int[_l*_l];
            bool[] iswalked = new bool[_l * _l];
            for (int i=0; i<chemin.Count; i++)
            {
                table[chemin[i]] = i;
                iswalked[chemin[i]] = true;
            }

            for (int x = 0; x < _l; x++)
            {
                for (int y = 0; y < _l; y++)
                {
                    var pos = EncodeXY(x, y);
                    if (iswalked[pos])
                    {
                        Console.Write($"| {table[pos]:00} ");
                    }
                    else 
                    {
                        Console.Write($"|    ");
                    }
                }
                Console.WriteLine();
            }*/

            Console.WriteLine($"[{_itemid:000}] Temps de calcul : {diff}");

            BestScore = score;
            BestPath = chemin;
            return true;
        }

        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public bool CalculateScore(string s, ref long current)
        {
            if (s.Length == 1)
            {
                var c = s[0];
                if (c>='0' && c<='9')
                {
                    current += c - '0';
                    return false;
                }
            }

            if (s == "10")
            {
                current += 10;
                return false;
            }

            switch (s)
            {
                case "* 2":
                    current *= 2;
                    return false;
                case "* 3":
                    current *= 3;
                    return false;
                case "* 4":
                    current *= 4; 
                    return false;
                case "* 5":
                    current *= 5;
                    return false;
                case "/2":
                    current /= 2;
                    return false;
                case "M":
                    var str = current.ToString();
                    string str2 = Reverse(str);
                    if (!long.TryParse(str2, out current))
                    {
                        throw new Exception($"Flip impossible!!!!! {str} - {str2}");
                    }
                    return false;
                case "F":
                    return true;

            }
            throw new Exception($"Valeur inconnue!!!!! {s}");
        }

        private bool CalculateStep(int pos, long currentScore, bool[] done, ref List<int>? path, ref long newMaxScore)
        {
            List<int>? newPath;
            if (done[pos])
            {
                // On ne peux pas continuer sur cette case, elle est déjà sur le chemin
                return false;
            }

            if (CalculateScore(_data[pos], ref currentScore))
            {
                // Fin trouvé, condition d'arret de la boucle récursive
                newPath = new List<int> { pos };
                _nbpathes++;
                if (_nbpathes % 10000000 == 0)
                {
                    Console.WriteLine($"[{_itemid:000}] {_nbpathes:##,#} fait, environ {_nbpathes*100/275000000} %");
                }
            }
            else
            {
                bool[] copy = new bool[_l * _l];
                done.CopyTo(copy, 0);
                // Appel récursif
                if (!RecursiveLoop(pos, ref currentScore, copy, out newPath))
                {
                    // Le chemin trouvé n'est pas valide, pas besoin d'aller plus loin
                    return false;
                }
                if (newPath != null)
                {
                    newPath.Add(pos);
                }
            }

            // On considere le chemin OK uniquement si il a un score supérieur aux précédents
            if (currentScore > newMaxScore)
            {
                path = newPath;
                newMaxScore = currentScore;
                return true;
            }
            return false;
        }

        public bool RecursiveLoop(int pos, ref long score, bool[] done, out List<int>? path)
        {
            done[pos] = true;
            long newMaxScore = 0;
            bool IsnextStepDone = false;
            path = null;

            (var x, var y) = DecodeXY(pos);

            if (x > 0)
            {
                IsnextStepDone |= CalculateStep(pos - 1, score, done, ref path, ref newMaxScore);
            }

            if (x < _l-1)
            {
                IsnextStepDone |= CalculateStep(pos + 1, score, done, ref path, ref newMaxScore);
            }

            if (y > 0)
            {
                IsnextStepDone |= CalculateStep(pos - _l, score, done, ref path, ref newMaxScore);
            }

            if (y < _l-1)
            {
                IsnextStepDone |= CalculateStep(pos + _l, score, done, ref path, ref newMaxScore);
            }
            score = newMaxScore;
            return IsnextStepDone;
        }
    }
}
