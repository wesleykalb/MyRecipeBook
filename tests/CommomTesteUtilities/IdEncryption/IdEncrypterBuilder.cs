using Sqids;

namespace CommomTesteUtilities.IdEncryption;

public class IdEncrypterBuilder
{
    public static SqidsEncoder<long> Build()
    {
        return new SqidsEncoder<long>(new()
        {
            Alphabet = "4V8AXduWMqoRQOYNxFHcgyjzS2aITksv7C0nUBDp9KZmEwrifPJ3hbL5el16Gt",
            MinLength = 3,
        });
    }
}