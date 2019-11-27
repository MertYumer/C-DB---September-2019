namespace P02_Composite
{
    using System;
    using System.Collections.Generic;

    public class CompositeGift : GiftBase, IGiftOperations
    {
        private List<GiftBase> gifts;

        public CompositeGift(string name, decimal price)
            : base(name, price)
        {
            this.gifts = new List<GiftBase>();
        }

        public void Add(GiftBase gift)
        {
            this.gifts.Add(gift);
        }

        public void Remove(GiftBase gift)
        {
            this.gifts.Remove(gift);
        }

        public override decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;

            Console.WriteLine($"{this.name} contains the following products with prices");

            this.gifts.ForEach(g => totalPrice += g.CalculateTotalPrice());

            return totalPrice;
        }
    }
}
