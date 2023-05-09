using Naki3D.Common.Protocol;
using System.Text.RegularExpressions;

public class PatternEventComponent : EventComponent
{
    public enum PatternMatchTypeEnum
    {
        Exact,
        Contains,
        StartsWith,
        EndsWith,
        Regex
    }

    public string Pattern;
    public SensorDataMessage.DataOneofCase DataType;
    public PatternMatchTypeEnum PatternMatchType;

    private Regex _regex;

    void Start()
    {
        if (PatternMatchType == PatternMatchTypeEnum.Regex) _regex = new Regex(Pattern, RegexOptions.Compiled);
    }

    protected override void OnEventReceived(SensorDataMessage e)
    {
        var matchResult = PatternMatchType switch
        {
            PatternMatchTypeEnum.Exact => e.Path == Pattern,
            PatternMatchTypeEnum.Contains => e.Path.Contains(Pattern),
            PatternMatchTypeEnum.StartsWith => e.Path.StartsWith(Pattern),
            PatternMatchTypeEnum.EndsWith => e.Path.EndsWith(Pattern),
            PatternMatchTypeEnum.Regex => _regex.IsMatch(e.Path),
            _ => false
        };

        if (!matchResult) return;
        if (e.DataCase != DataType) return;

        base.OnEventReceived(e);
    }
}
