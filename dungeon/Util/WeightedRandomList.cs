using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dungeon.Util
{
    class WeightedRandomList<T>
    {
        List<T> contents = new List<T>();
        List<double> weights = new List<double>();
        double sum = 0;

        public void Add(T t, double weight)
        {
            sum += weight;
            weights.Add(weight);
            contents.Add(t);
        }

        public bool Any()
        {
            return contents.Any();
        }

        public int Count()
        {
            return contents.Count();
        }

        /// <summary>
        /// Gets a random element from the list with proper weighting.
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            double rand = Dungeon.RAND.NextDouble() * sum;
            for (int i = 0; i < contents.Count(); i++)
            {
                if (rand < weights[i])
                    return contents[i];
                rand -= weights[i];
            }
            throw new Exception("Get didn't work for randlist");
        }

        /// <summary>
        /// Removes a random element and returns it.
        /// </summary>
        /// <returns></returns>
        public T Remove()
        {
            double rand = Dungeon.RAND.NextDouble() * sum;
            for (int i = 0; i < contents.Count(); i++)
            {
                if (rand < weights[i])
                {
                    T save = contents[i];
                    contents.RemoveAt(i);
                    sum -= weights[i];
                    weights.RemoveAt(i);
                    return save;
                }
                rand -= weights[i];
            }
            throw new Exception("Get didn't work for randlist");
        }
    }
}
