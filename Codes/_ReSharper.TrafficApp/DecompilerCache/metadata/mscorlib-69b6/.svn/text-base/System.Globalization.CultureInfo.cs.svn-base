// Type: System.Globalization.CultureInfo
// Assembly: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll

using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
    [ComVisible(true)]
    [Serializable]
    public class CultureInfo : ICloneable, IFormatProvider
    {
        public CultureInfo(string name);
        public CultureInfo(string name, bool useUserOverride);
        public CultureInfo(int culture);
        public CultureInfo(int culture, bool useUserOverride);
        public static CultureInfo CurrentCulture { get; }
        public static CultureInfo CurrentUICulture { get; }
        public static CultureInfo InstalledUICulture { get; }
        public static CultureInfo InvariantCulture { get; }
        public virtual CultureInfo Parent { get; }
        public virtual int LCID { get; }

        [ComVisible(false)]
        public virtual int KeyboardLayoutId { get; }

        public virtual string Name { get; }

        [ComVisible(false)]
        public string IetfLanguageTag { get; }

        public virtual string DisplayName { get; }
        public virtual string NativeName { get; }
        public virtual string EnglishName { get; }
        public virtual string TwoLetterISOLanguageName { get; }
        public virtual string ThreeLetterISOLanguageName { get; }
        public virtual string ThreeLetterWindowsLanguageName { get; }
        public virtual CompareInfo CompareInfo { get; }
        public virtual TextInfo TextInfo { get; }
        public virtual bool IsNeutralCulture { get; }

        [ComVisible(false)]
        public CultureTypes CultureTypes { get; }

        public virtual NumberFormatInfo NumberFormat { get; set; }
        public virtual DateTimeFormatInfo DateTimeFormat { get; set; }
        public virtual Calendar Calendar { get; }
        public virtual Calendar[] OptionalCalendars { get; }
        public bool UseUserOverride { get; }
        public bool IsReadOnly { get; }

        #region ICloneable Members

        public virtual object Clone();

        #endregion

        #region IFormatProvider Members

        public virtual object GetFormat(Type formatType);

        #endregion

        public static CultureInfo CreateSpecificCulture(string name);
        public static CultureInfo[] GetCultures(CultureTypes types);
        public override bool Equals(object value);
        public override int GetHashCode();
        public override string ToString();
        public void ClearCachedData();

        [ComVisible(false)]
        public CultureInfo GetConsoleFallbackUICulture();

        public static CultureInfo ReadOnly(CultureInfo ci);
        public static CultureInfo GetCultureInfo(int culture);
        public static CultureInfo GetCultureInfo(string name);
        public static CultureInfo GetCultureInfo(string name, string altName);
        public static CultureInfo GetCultureInfoByIetfLanguageTag(string name);
    }
}
