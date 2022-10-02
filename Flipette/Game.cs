namespace Flipette
{
    public class Game
    {
        public enum OperationList
        {
            ADD0,
            ADD1,
            ADD2,
            ADD3,
            ADD4,
            ADD5,
            ADD6,
            ADD7,
            ADD8,
            ADD9,
            ADD10,
            MULT2,
            MULT3,
            MULT4,
            MULT5,
            DIV2,
            FLIP,
            START,
            END
        }

        private List<string> _data;
        private int _l;
        private int _start;
        private long _nbpathes;
        private int _itemid;
        private List<OperationList> _converteddata;

        private static OperationList OpConvert(string s) => s switch
        {
            "0" => OperationList.ADD0,
            "1" => OperationList.ADD1,
            "2" => OperationList.ADD2,
            "3" => OperationList.ADD3,
            "4" => OperationList.ADD4,
            "5" => OperationList.ADD5,
            "6" => OperationList.ADD6,
            "7" => OperationList.ADD7,
            "8" => OperationList.ADD8,
            "9" => OperationList.ADD9,
            "10" => OperationList.ADD10,
            "* 2" => OperationList.MULT2,
            "* 3" => OperationList.MULT3,
            "* 4" => OperationList.MULT4,
            "* 5" => OperationList.MULT5,
            "/2" => OperationList.DIV2,
            "M" => OperationList.FLIP,
            "S" => OperationList.START,
            "F" => OperationList.END,
            _ => throw new ArgumentOutOfRangeException(nameof(s), $"Not expected Operation value: {s}"),
        };

        public Game(List<string> data, int l, int itemid)
        {
            _data = data;
            _l = l;
            _itemid = itemid;
            _converteddata = new List<OperationList>();

            for (int i=0; i<l*l ; i++)
            {
                _converteddata.Add(OpConvert(_data[i]));
                if (_data[i]=="S")
                {
                    _start = i;
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

        public bool CalculateScore(OperationList o, ref long current)
        {
            switch (o)
            {
                case OperationList.ADD0:
                    return false;
                case OperationList.ADD1:
                    current += 1;
                    return false;
                case OperationList.ADD2:
                    current += 2;
                    return false;
                case OperationList.ADD3:
                    current += 3;
                    return false;
                case OperationList.ADD4:
                    current += 4;
                    return false;
                case OperationList.ADD5:
                    current += 5;
                    return false;
                case OperationList.ADD6:
                    current += 6;
                    return false;
                case OperationList.ADD7:
                    current += 7;
                    return false;
                case OperationList.ADD8:
                    current += 8;
                    return false;
                case OperationList.ADD9:
                    current += 9;
                    return false;
                case OperationList.ADD10:
                    current += 10;
                    return false;
                case OperationList.MULT2:
                    current *= 2;
                    return false;
                case OperationList.MULT3:
                    current *= 3;
                    return false;
                case OperationList.MULT4:
                    current *= 4; 
                    return false;
                case OperationList.MULT5:
                    current *= 5;
                    return false;
                case OperationList.DIV2:
                    current /= 2;
                    return false;
                case OperationList.FLIP:
                    long reverse = 0;
                    while (current > 0)
                    {
                        reverse *= 10;
                        reverse += current % 10;
                        current /= 10;
                    }
                    current = reverse;
                    return false;
                case OperationList.END:
                    return true;
                default:
                    throw new Exception($"Not Handled Operation");
            }
        }

        private bool CalculateStep(int pos, long currentScore, bool[] done, ref List<int>? path, ref long newMaxScore)
        {
            List<int>? newPath;
            if (done[pos])
            {
                // On ne peux pas continuer sur cette case, elle est déjà sur le chemin
                return false;
            }

            if (CalculateScore(_converteddata[pos], ref currentScore))
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
