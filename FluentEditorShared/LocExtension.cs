using System;
using Avalonia.Markup.Xaml;

namespace FluentEditorShared;

public class LocExtension : MarkupExtension
{
    private readonly string _key;

    public LocExtension(string key)
    {
        _key = key;
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _key;
    }
}