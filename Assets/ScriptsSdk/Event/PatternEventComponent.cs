using Naki3D.Common.Protocol;
using System.Text.RegularExpressions;
using UnityEngine;

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

    // TODO: Initialize the regex object!
    
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
