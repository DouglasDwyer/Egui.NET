using System.ComponentModel;

namespace Egui.Widgets;

/// <summary>
/// Shows a date, and will open a date picker popup when clicked.
/// </summary>
public ref struct DatePicker : IWidget
{
    private ref DateOnly _date;
    private DatePickerInner _inner;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'DatePicker' does not contain a constructor that takes 0 arguments", error: true)]
    public DatePicker() { throw new InvalidOperationException(); }

    public DatePicker(ref DateOnly date)
    {
        _date = ref date;
        _inner.ComboBoxes = true;
        _inner.Arrows = true;
        _inner.Calendar = true;
        _inner.CalendarWeek = true;
        _inner.ShowIcon = true;
        _inner.Format = "%Y-%m-%d";
        _inner.HighlightWeekends = true;
        _inner.ReverseYears = false;
    }

    /// <summary>
    /// Add id source.<br/>
    /// Must be set if multiple date picker buttons are in the same <see cref="Ui"/>.
    /// </summary>
    public readonly DatePicker IdSalt(string idSalt)
    {
        var result = this;
        result._inner.IdSalt = idSalt;
        return result;
    }

    /// <summary>
    /// Show combo boxes in date picker popup. (Default: true)
    /// </summary>
    public readonly DatePicker ComboBoxes(bool comboBoxes)
    {
        var result = this;
        result._inner.ComboBoxes = comboBoxes;
        return result;
    }

    /// <summary>
    /// Show arrows in date picker popup. (Default: true)
    /// </summary>
    public readonly DatePicker Arrows(bool arrows)
    {
        var result = this;
        result._inner.Arrows = arrows;
        return result;
    }

    /// <summary>
    /// Show calendar in date picker popup. (Default: true)
    /// </summary>
    public readonly DatePicker Calendar(bool calendar)
    {
        var result = this;
        result._inner.Calendar = calendar;
        return result;
    }

    /// <summary>
    /// Show calendar week in date picker popup. (Default: true)
    /// </summary>
    public readonly DatePicker CalendarWeek(bool calendarWeek)
    {
        var result = this;
        result._inner.CalendarWeek = calendarWeek;
        return result;
    }

    /// <summary>
    /// Show the calendar icon on the button. (Default: true)
    /// </summary>
    public readonly DatePicker ShowIcon(bool showIcon)
    {
        var result = this;
        result._inner.ShowIcon = showIcon;
        return result;
    }

    /// <summary>
    /// Change the format shown on the button. (Default: %Y-%m-%d)
    /// </summary>
    public readonly DatePicker Format(string format)
    {
        var result = this;
        result._inner.Format = format;
        return result;
    }

    /// <summary>
    /// Highlight weekend days. (Default: true)
    /// </summary>
    public readonly DatePicker HighlightWeekends(bool highlightWeekends)
    {
        var result = this;
        result._inner.HighlightWeekends = highlightWeekends;
        return result;
    }

    /// <summary>
    /// Set the start and end years for the date picker. (Default: today's year - 100 to today's year + 10)<br/>
    /// This will limit the years you can choose from in the dropdown to the specified range.
    /// </summary>
    public readonly DatePicker StartEndYears(short start, short end)
    {
        var result = this;
        result._inner.StartEndYears = (start, end);
        return result;
    }

    /// <summary>
    /// List years in descending order in the year dropdown. (Default: false)
    /// </summary>
    public readonly DatePicker ReverseYears(bool reverseYears)
    {
        var result = this;
        result._inner.ReverseYears = reverseYears;
        return result;
    }

    /// <summary>
    /// Scroll the year dropdown to this year when the picker first opens.<br/>
    /// Defaults to the currently selected year.
    /// </summary>
    public readonly DatePicker YearScrollTo(short year)
    {
        var result = this;
        result._inner.YearScrollTo = year;
        return result;
    }

    /// <inheritdoc/>
    Response IWidget.Ui(Ui ui)
    {
        ui.AssertInitialized();
        var date = ((short)_date.Year, (sbyte)_date.Month, (sbyte)_date.Day);
        var (response, newDate) = EguiMarshal.Call<nuint, DatePickerInner, (short, sbyte, sbyte), (Response, (short, sbyte, sbyte))>(
            EguiFn.egui_extras_datepicker_button_DatePickerButton_ui, ui.Ptr, _inner, date);
        _date = new DateOnly(newDate.Item1, newDate.Item2, newDate.Item3);
        return response;
    }

    private partial struct DatePickerInner
    {
        public string? IdSalt;
        public bool ComboBoxes;
        public bool Arrows;
        public bool Calendar;
        public bool CalendarWeek;
        public bool ShowIcon;
        public string Format;
        public bool HighlightWeekends;
        public (short, short)? StartEndYears;
        public bool ReverseYears;
        public short? YearScrollTo;

        internal static void Serialize(Bincode.BincodeSerializer serializer, DatePickerInner value) => value.Serialize(serializer);

        internal void Serialize(Bincode.BincodeSerializer serializer)
        {
            serializer.increase_container_depth();

            if (IdSalt is not null)
            {
                serializer.serialize_option_tag(true);
                serializer.serialize_str(IdSalt);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }

            serializer.serialize_bool(ComboBoxes);
            serializer.serialize_bool(Arrows);
            serializer.serialize_bool(Calendar);
            serializer.serialize_bool(CalendarWeek);
            serializer.serialize_bool(ShowIcon);
            serializer.serialize_str(Format);
            serializer.serialize_bool(HighlightWeekends);

            if (StartEndYears is (short start, short end))
            {
                serializer.serialize_option_tag(true);
                serializer.serialize_i16(start);
                serializer.serialize_i16(end);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }

            serializer.serialize_bool(ReverseYears);

            if (YearScrollTo is short yearScrollTo)
            {
                serializer.serialize_option_tag(true);
                serializer.serialize_i16(yearScrollTo);
            }
            else
            {
                serializer.serialize_option_tag(false);
            }

            serializer.decrease_container_depth();
        }

        internal static DatePickerInner Deserialize(Bincode.BincodeDeserializer deserializer) => throw new NotImplementedException();
    }
}
