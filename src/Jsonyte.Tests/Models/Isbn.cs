namespace JsonApi.Tests.Models
{
    public struct Isbn
    {
        public string Part1;

        public string Part2;

        public string Part3;

        public string Part4;

        public Isbn(string part1, string part2, string part3, string part4)
        {
            Part1 = part1;
            Part2 = part2;
            Part3 = part3;
            Part4 = part4;
        }
    }
}
