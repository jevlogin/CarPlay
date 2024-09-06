namespace WORLDGAMDEVELOPMENT
{
    internal static class Pause
    {
        private static Random _random = new Random();

        #region Pause

        public static async Task Wait(int? valueMax = null, int? valueMin = null)
        {
            int minV = 500;
            int maxV = 5000;
            int result = minV;

            if (valueMin.HasValue && valueMax.HasValue)
            {
                if (valueMin > valueMax)
                {
                    var temp = valueMin;
                    valueMin = valueMax;
                    valueMax = temp;
                }
                result = _random.Next(valueMin.Value, valueMax.Value);
            }
            else if (valueMin.HasValue)
            {
                result = _random.Next(valueMin.Value, maxV);
            }
            else if (valueMax.HasValue)
            {
                result = _random.Next(minV, valueMax.Value);
            }
            else
            {
                result = _random.Next(minV, maxV);
            }

            await Task.Delay(result);
        }

        #endregion
    }
}
