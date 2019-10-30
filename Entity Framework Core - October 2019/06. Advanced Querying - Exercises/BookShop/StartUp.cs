namespace BookShop
{
    using Models.Enums;
    using Data;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                var result = GetGoldenBooks(db);
                Console.WriteLine(result);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            var bookTitles = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title);

            var result = String.Join(Environment.NewLine, bookTitles);

            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            var result = String.Join(Environment.NewLine, bookTitles);

            return result;
        }
    }
}
