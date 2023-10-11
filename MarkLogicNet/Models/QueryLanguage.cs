using System.ComponentModel;

namespace MarkLogicNet.Models
{
    public enum QueryLanguage
    {
        [Description("xquery")]
        XQuery,
        [Description("javascript")]
        JavaScript
    };
}
