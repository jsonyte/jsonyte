using System.Collections.Generic;

namespace Jsonyte.Serialization
{
    internal struct FrugalList<T>
    {
        private const int ItemsCount = 8;

        private const int ArrayItemsCount = 64;

        private const int TotalBufferCount = ItemsCount + ArrayItemsCount;

        private T item0;

        private T item1;

        private T item2;

        private T item3;

        private T item4;

        private T item5;

        private T item6;

        private T item7;

        private T[]? arrayItems;

        private List<T>? listItems;

        private int count;

        public int Count => count;

        public T this[int index]
        {
            get
            {
                return index switch
                {
                    0 => item0,
                    1 => item1,
                    2 => item2,
                    3 => item3,
                    4 => item4,
                    5 => item5,
                    6 => item6,
                    7 => item7,
                    _ => GetOverflow(index)
                };
            }
        }

        public void Add(T item)
        {
            if (count == 0)
            {
                item0 = item;
            }
            else if (count == 1)
            {
                item1 = item;
            }
            else if (count == 2)
            {
                item2 = item;
            }
            else if (count == 3)
            {
                item3 = item;
            }
            else if (count == 4)
            {
                item4 = item;
            }
            else if (count == 5)
            {
                item5 = item;
            }
            else if (count == 6)
            {
                item6 = item;
            }
            else if (count == 7)
            {
                item7 = item;
            }
            else if (count < TotalBufferCount)
            {
                arrayItems ??= new T[ArrayItemsCount];

                arrayItems[count - ItemsCount] = item;
            }
            else
            {
                listItems ??= new List<T>(ArrayItemsCount * 2);

                listItems.Add(item);
            }

            count++;
        }

        private T GetOverflow(int index)
        {
            if (index < TotalBufferCount)
            {
                return arrayItems![index - ItemsCount];
            }

            return listItems![index - TotalBufferCount];
        }
    }
}
