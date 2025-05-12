using FileTypeChecker.Extensions;
using FileTypeChecker.Types;

namespace MyRecipeBook.Application.Extensions;

public static class StreamImageExtensions
{
    public static (bool IsValidImage, string extension) ValidateAndGetImageExtension(this Stream stream)
    {
        var result = (false, string.Empty);
        
        if (stream.Is<JointPhotographicExpertsGroup>())
            result = (true, NormalizeExtension(JointPhotographicExpertsGroup.TypeExtension));
        else if (stream.Is<PortableNetworkGraphic>())
            result = (true, NormalizeExtension(PortableNetworkGraphic.TypeExtension));
        
        stream.Position = 0;

        return result;
    }

    private static string NormalizeExtension(string extension)
    {
        return extension.StartsWith(".") ? extension: $".{extension}";
    }
}