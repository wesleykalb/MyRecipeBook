using System.Collections;
using CommomTesteUtilities.Requests;

namespace UsesCases.Test.Recipe.InlineDatas;

public class ImageTypesInlineData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var images = FormFileBuilder.Collection();

        foreach (var image in images)
        {
            yield return new object[] {image};
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}