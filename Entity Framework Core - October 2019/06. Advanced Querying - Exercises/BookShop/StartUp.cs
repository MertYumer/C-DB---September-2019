namespace BookShop
{
    using Models.Enums;
    using Data;
    using System;
    using System.Linq;
    using System.Text;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                var result = RemoveBooks(db);
                Console.WriteLine(result);
            }
        }

        //Problem 1 - Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            var bookTitles = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, bookTitles);

            return result;
        }

        //Problem 2 - Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, bookTitles);

            return result;
        }

        //Problem 3 - Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var result = new StringBuilder();

            var books = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                });

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return result.ToString().TrimEnd();
        }

        //Problem 4 - Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, bookTitles);

            return result;
        }

        //Problem 5 - Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var listOfCategories = input
                .ToLower()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var bookTitles = context.Books
                .Where(b => b.BookCategories
                    .Any(c => listOfCategories
                        .Contains(c.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, bookTitles);

            return result;
        }

        //Problem 6 - Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var result = new StringBuilder();

            var format = "dd-MM-yyyy";
            var provider = CultureInfo.InvariantCulture;
            var parsedDate = DateTime.ParseExact(date, format, provider);

            var books = context.Books
                .Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                });

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return result.ToString().TrimEnd();
        }

        //Problem 7 - Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authorsNames = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => $"{a.FirstName} {a.LastName}");

            var result = string.Join(Environment.NewLine, authorsNames);

            return result;
        }

        //Problem 8 - Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            input = input.ToLower();

            var bookTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(input))
                .OrderBy(b => b.Title)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, bookTitles);

            return result;
        }

        //Problem 9 - Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            input = input.ToLower();

            var booksAndAuthors = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input))
                .OrderBy(b => b.BookId)
                .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})");

            var result = string.Join(Environment.NewLine, booksAndAuthors);

            return result;
        }

        //Problem 10 - Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck);

            return books.Count();
        }

        //Problem 11 - Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var result = new StringBuilder();

            var authors = context.Authors
                .Select(a => new
                {
                    FullName = $"{a.FirstName} {a.LastName}",
                    BooksCount = a.Books.Select(b => b.Copies).Sum()
                })
                .OrderByDescending(a => a.BooksCount);

            foreach (var author in authors)
            {
                result.AppendLine($"{author.FullName} - {author.BooksCount}");
            }

            return result.ToString().TrimEnd();
        }

        //Problem 12 - Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var result = new StringBuilder();

            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                    .Select(cb => cb.Book.Copies * cb.Book.Price).Sum()
                })
                .OrderByDescending(c => c.TotalProfit);

            foreach (var category in categories)
            {
                result.AppendLine($"{category.Name} ${category.TotalProfit:f2}");
            }

            return result.ToString().TrimEnd();
        }

        //Problem 13 - Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var result = new StringBuilder();

            var categories = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks
                            .OrderByDescending(cb => cb.Book.ReleaseDate)
                            .Take(3)
                            .Select(cb => new
                            {
                                cb.Book.Title,
                                cb.Book.ReleaseDate.Value.Year
                            })
                            .ToList()
                });

            foreach (var category in categories)
            {
                result.AppendLine($"--{category.Name}");

                foreach (var book in category.Books)
                {
                    result.AppendLine($"{book.Title} ({book.Year})");
                }
            }

            return result.ToString().TrimEnd();
        }

        //Problem 14 - Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //Problem 15 - Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.Books.RemoveRange(books);
            context.SaveChanges();

            return books.Count;
        }
    }
}
