namespace ClaraMundi
{
    public static class ItemInstanceFactory
    {
        public static ulong counter;

        public static ulong NewId()
        {
            // start at 1, leaving 0 to be invalid in all cases
            counter += 1;
            return counter;
        }
    }
}