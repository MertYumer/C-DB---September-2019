namespace P01_Prototype
{
    public class Sandwich : SandwichPrototype
    {
        private string bread;
        private string meat;
        private string cheese;
        private string veggie;

        public Sandwich(string bread, string meat, string cheese, string veggie)
        {
            this.bread = bread;
            this.meat = meat;
            this.cheese = cheese;
            this.veggie = veggie;
        }

        public override SandwichPrototype Clone()
        {
            var ingredientsList = this.GetIngredientsList();
            System.Console.WriteLine($"Clonning sandwich with ingredients: " + ingredientsList);

            return MemberwiseClone() as SandwichPrototype;
        }

        private string GetIngredientsList()
        {
            return $"{this.bread}, {this.meat}, {this.cheese}, {this.veggie}";
        }
    }
}
