namespace Portafolio.Helpers
{
    public static class RandomGenerator
    {
        private static readonly Random random = new();

        public static string SetCode(int longitud)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] code = new char[longitud];

            for (int i = 0; i < longitud; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            return new string(code);
        }
    }
}
