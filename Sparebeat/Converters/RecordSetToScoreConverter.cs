using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Sparebeat.Common;

namespace Sparebeat.Converters;

internal sealed class RecordSetToScoreConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RecordSet recordSet)
            return (recordSet.Hard ?? recordSet.Normal ?? recordSet.Easy)?.Score;

        return "000000";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
