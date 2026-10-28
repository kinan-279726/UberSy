using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Tags;

public static class TagErrors
{
    public static Error NameRequired =>
        Error.Validation("Tag_Name_Required", "Tag Name Is Required");
}
