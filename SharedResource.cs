using System.Globalization;
using System.Resources;

namespace BIT240049_Mid;

/// <summary>
/// Marker class: IStringLocalizer&lt;SharedResource&gt; se doc file
/// Resources/SharedResource.resx (English) va Resources/SharedResource.vi.resx (Tieng Viet).
/// </summary>
public class SharedResource
{
}

/// <summary>
/// Truy cap chuoi da dich o noi khong co IStringLocalizer (vd: thong bao loi model binding).
/// </summary>
public static class Res
{
    private static readonly ResourceManager Manager =
        new("BIT240049_Mid.Resources.SharedResource", typeof(SharedResource).Assembly);

    public static string Get(string key, params object[] args)
    {
        var text = Manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        return args.Length == 0 ? text : string.Format(text, args);
    }
}
