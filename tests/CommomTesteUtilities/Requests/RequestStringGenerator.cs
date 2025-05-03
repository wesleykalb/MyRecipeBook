using Bogus;

namespace CommomTesteUtilities.Requests
{
    public class RequestStringGenerator
    {
        public static string Paragraphs(int minCharaters)
        {
            var faker = new Faker();

            var longText = faker.Lorem.Paragraphs(7);

            while (longText.Length < minCharaters)
            {
                longText = $"{longText} {faker.Lorem.Paragraph()}";
            }

            return longText;
        }
    }
}