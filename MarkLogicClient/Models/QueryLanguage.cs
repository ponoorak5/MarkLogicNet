using System.ComponentModel;

namespace MarkLogicClient.Models
{
    public enum QueryLanguage
    {
        [Description("xquery")]
        XQuery,
        [Description("javascript")]
        JavaScript
    };
}
