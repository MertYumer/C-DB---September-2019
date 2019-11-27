namespace P03_TemplatePattern
{
    public class StartUp
    {
        public static void Main()
        {
            var sourdough = new Sourdough();
            var twelveGrain = new TwelveGrain();
            var wholeWheat = new WholeWheat();

            sourdough.Make();
            twelveGrain.Make();
            wholeWheat.Make();
        }
    }
}
