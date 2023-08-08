using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlookReminder
{

    using Microsoft.VisualBasic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Drawing.Drawing2D;
    using System.Drawing.Design;
    using System.Xml.Serialization;

    /// <summary>
    /// Customizable TrackBar Control
    /// </summary>
    /// <remarks>v1.9.0</remarks>
    [ToolboxItem(true)]
    [DefaultEvent("ValueChanged")]
    [System.Diagnostics.DebuggerStepThrough()]
    public partial class ProgressBar: UserControl
    {
        private Timer withEventsField_MouseTimer = new Timer();
        private Timer MouseTimer
        {
            get { return withEventsField_MouseTimer; }
            set
            {
                if (withEventsField_MouseTimer != null)
                {
                    withEventsField_MouseTimer.Tick -= MouseTimer_Tick;
                }
                withEventsField_MouseTimer = value;
                if (withEventsField_MouseTimer != null)
                {
                    withEventsField_MouseTimer.Tick += MouseTimer_Tick;
                }
            }
        }
        public event ValueChangedEventHandler ValueChanged;
        public delegate void ValueChangedEventHandler(object sender, EventArgs e);
        public new event ScrollEventHandler Scroll;
        public new delegate void ScrollEventHandler(object sender, ScrollEventArgs e);

        #region "Initiate"

        private eMouseState MouseState = eMouseState.Up;
        private bool IsOverSlider;
        private bool IsOverDownButton;
        private bool IsOverUpButton;
        private readonly GraphicsPath gpSlider = new GraphicsPath();
        private int intSlideIndent = 13;
        private float sngSliderPos = 35;
        private Rectangle rectValueBox;
        private Rectangle rectSlider;
        private Rectangle rectDownButton;
        private Rectangle rectUpButton;
        private Rectangle rectLabel;

        private bool Init = true;

        private readonly StringFormat sf = new StringFormat();

        public ProgressBar()
        {
            GotFocus += gTrackBar_LostFocus;
            LostFocus += gTrackBar_LostFocus;
            Resize += TBSlider_Resize;
            KeyUp += gTrackBar_KeyUp;
            MouseWheel += TBSlider_MouseWheel;
            MouseUp += TBSlider_MouseUp;
            MouseMove += TBSlider_MouseMove;
            MouseLeave += gTrackBar_MouseLeave;
            MouseDown += TBSlider_MouseDown;
            Load += TBSlider_Load;

            rectValueBox = new Rectangle(0, 0, 30, 20);
            rectSlider = new Rectangle(1, 1, 250, 19);
            rectDownButton = new Rectangle(0, 2, 15, 26);
            rectUpButton = new Rectangle(235, 2, 15, 26);
            rectLabel = new Rectangle(0, 0, Width, 20);

            CurrSliderColor = _ColorUp.Face;
            CurrSliderBorderColor = _ColorUp.Border;
            CurrSliderHiLtColor = _ColorUp.Highlight;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            
        }

        private void TBSlider_Load(object sender, EventArgs e)
        {
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            UpdateRects();
            Init = false;

        }

        #endregion

        #region "Enum"

        public enum eTickType
        {
            None,
            Up_Right,
            Down_Left,
            Both,
            Middle
        }

        public enum eMouseState
        {
            Up,
            Down
        }

        public enum eShape
        {
            None,
            Ellipse,
            Rectangle,
            ArrowUp,
            ArrowDown,
            ArrowRight,
            ArrowLeft
        }

        public enum eValueBox
        {
            None,
            Left,
            Right
        }

        public enum eBrushStyle
        {
            Image,
            Linear,
            Linear2,
            Path
        }

        #endregion

        #region "Properties"

        #region "Hidden"

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool BorderStyle
        {
            //always false 
            get { return false; }
            //empty 
            set { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Font Font
        {
            //always false 
            get { return null; }
            //empty 
            set { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color ForeColor
        {
            //always false 
            get { return Color.Empty; }
            //empty 
            set { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Padding Padding
        {
            get { return _LabelPadding; }
            set { base.Padding = value; }
        }
        #endregion

        #region "Value"

        private int _Value;
        [Category("Appearance gTrackBar")]
        [Description("Current Value for the Slider")]
        [Bindable(true)]
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    if (value < _MinValue)
                    {
                        _Value = _MinValue;
                    }
                    else
                    {
                        if (value > _MaxValue)
                        {
                            _Value = _MaxValue;
                        }
                        else
                        {
                            _Value = value;
                        }
                    }
                    UpdateRects();
                    Invalidate();
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        [Category("Appearance gTrackBar")]
        [Description("Current Value Adjusted by the Divisor")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public float ValueAdjusted
        {
            get { return Convert.ToSingle(_Value / (int)_valueDivisor); }

            set { value = Convert.ToInt32(value * (int)_valueDivisor); }
        }

        public enum eValueDivisor
        {
            e1 = 1,
            e10 = 10,
            e100 = 100,
            e1000 = 1000
        }

        private eValueDivisor _valueDivisor = eValueDivisor.e1;
        [Category("Appearance gTrackBar")]
        [Description("Divisor to adjust the Value by")]
        public eValueDivisor ValueDivisor
        {
            get { return _valueDivisor; }
            set { _valueDivisor = value; }
        }

        private string _valueStrFormat;
        [Category("Appearance gTrackBar")]
        [Description("Format to display the Value")]
        public string ValueStrFormat
        {
            get { return _valueStrFormat; }
            set
            {
                _valueStrFormat = value;
                Invalidate();
            }
        }
        #endregion

        #region "Control"

        private eBrushStyle _BrushStyle = eBrushStyle.Path;
        [Category("Appearance Slider")]
        [Description("Use a Linear or Path type Brush on the Slider")]
        [DefaultValue(typeof(eBrushStyle), "Path")]
        public eBrushStyle BrushStyle
        {
            get { return _BrushStyle; }
            set
            {
                _BrushStyle = value;
                Invalidate();
            }
        }

        private LinearGradientMode _BrushDirection = LinearGradientMode.Horizontal;
        [Category("Appearance Slider")]
        [Description("The LinearGradientMode for the Linear Fill Type Brush")]
        [DefaultValue(typeof(LinearGradientMode), "Horizontal")]
        public LinearGradientMode BrushDirection
        {
            get { return _BrushDirection; }
            set
            {
                _BrushDirection = value;
                Invalidate();
            }
        }

        private Orientation _Orientation = Orientation.Horizontal;
        [Category("Appearance gTrackBar")]
        [Description("Horizontal or Vertical Orientation")]
        [DefaultValue(typeof(Orientation), "Horizontal")]
        public Orientation Orientation
        {
            get { return _Orientation; }
            set
            {
                _Orientation = value;
                Size = new Size(Height, Width);
                SliderSize = new Size(_SliderSize.Height, _SliderSize.Width);
                UpdateRects();
                Invalidate();
            }
        }

        private int _MinValue;
        [Category("Appearance gTrackBar")]
        [Description("Minimum Value allowed for the Slider")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(0)]
        public int MinValue
        {
            get { return _MinValue; }
            set
            {
                if (!Init)
                {
                    if (value >= _MaxValue)
                        value = _MaxValue - 10;
                    if (_Value < value)
                        _Value = value;
                }
                _MinValue = value;
                UpdateRects();
                Invalidate();
            }
        }

        private int _MaxValue = 50;
        [Category("Appearance gTrackBar")]
        [Description("Maximum Value allowed for the Slider")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(50)]
        public int MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (!Init)
                {
                    if (value <= _MinValue)
                        value = _MinValue + 10;
                    if (_Value > value)
                        _Value = value;
                }
                _MaxValue = value;
                UpdateRects();
                Invalidate();
            }
        }

        private int _ChangeLarge = 10;
        [Category("Appearance gTrackBar")]
        [Description("How far to adjust the value when clicking to the right or left of the slider or when the Arrow Keys are pressed while holding the Shift Key too.")]
        [DefaultValue(10)]
        public int ChangeLarge
        {
            get { return _ChangeLarge; }
            set { _ChangeLarge = Math.Abs(value); }
        }

        private int _ChangeSmall = 1;
        [Category("Appearance gTrackBar")]
        [Description("How far to adjust the value when clicking the Arrow buttons or when the Arrow Keys are pressed")]
        [DefaultValue(1)]
        public int ChangeSmall
        {
            get { return _ChangeSmall; }
            set { _ChangeSmall = Math.Abs(value); }
        }

        private bool _BorderShow;
        [Category("Appearance gTrackBar")]
        [Description("Show or not show the border around the control")]
        [DefaultValue(false)]
        public bool BorderShow
        {
            get { return _BorderShow; }
            set
            {
                _BorderShow = value;
                Invalidate();
            }
        }

        private bool _ShowFocus = true;
        [Category("Appearance gTrackBar")]
        [Description("Show or not show when the control has focus")]
        [DefaultValue(true)]
        public bool ShowFocus
        {
            get { return _ShowFocus; }
            set { _ShowFocus = value; }
        }

        private bool _JumpToMouse;
        [Category("Behavior Slider")]
        [Description("Get or Set if the Slider Jumps to the mouse position or increments to it")]
        [DefaultValue(false)]
        public bool JumpToMouse
        {
            get { return _JumpToMouse; }
            set { _JumpToMouse = value; }
        }

        private bool _SnapToValue = true;
        [Category("Behavior Slider")]
        [Description("Get or Set if the Slider Jumps to the Value position when the mouse is up")]
        [DefaultValue(true)]
        public bool SnapToValue
        {
            get { return _SnapToValue; }
            set { _SnapToValue = value; }
        }

        #endregion

        #region "Ticks"

        private eTickType _TickType = eTickType.None;
        [Category("Appearance gTrackBar")]
        [Description("Where to draw the Tick Marks")]
        [DefaultValue(typeof(eTickType), "None")]
        public eTickType TickType
        {
            get { return _TickType; }
            set
            {
                _TickType = value;
                Invalidate();
            }
        }

        private int _TickInterval = 10;
        [Category("Appearance gTrackBar")]
        [Description("The Interval between the Tick Marks")]
        [DefaultValue(10)]
        public int TickInterval
        {
            get { return _TickInterval; }
            set
            {
                _TickInterval = value;
                Invalidate();
            }
        }

        private int _TickWidth = 5;
        [Category("Appearance gTrackBar")]
        [Description("How long to draw the Tick Marks")]
        [DefaultValue(5)]
        public int TickWidth
        {
            get { return _TickWidth; }
            set
            {
                _TickWidth = value;
                Invalidate();
            }
        }

        private float _TickThickness = 1;
        [Category("Appearance gTrackBar")]
        [Description("How Thick to draw the Tick Marks")]
        [DefaultValue(1)]
        public float TickThickness
        {
            get { return _TickThickness; }
            set
            {
                _TickThickness = value;
                Invalidate();
            }
        }

        private int _TickOffset = 10;
        [Category("Appearance gTrackBar")]
        [Description("How far to offset the Tick Marks")]
        [DefaultValue(10)]
        public int TickOffset
        {
            get { return _TickOffset; }
            set
            {
                _TickOffset = value;
                Invalidate();
            }
        }

        #endregion

        #region "FloatValue"

        private bool _FloatValue = true;
        [Category("Appearance FloatValue")]
        [Description("Show or not show the value above the slider while dragging it back and forth")]
        [DefaultValue(true)]
        public bool FloatValue
        {
            get { return _FloatValue; }
            set { _FloatValue = value; }
        }

        private Font _FloatValueFont = new Font("Arial", 8, FontStyle.Bold);
        [Category("Appearance FloatValue")]
        [Description("Font to use for the value above the slider ")]
        [DefaultValue(typeof(Font), "Arial, 8pt, style=Bold")]
        public Font FloatValueFont
        {
            get { return _FloatValueFont; }
            set
            {
                _FloatValueFont = value;
                Invalidate();
            }
        }

        #endregion

        #region "Label"

        private string _Label;
        [Category("Appearance Label")]
        [Description("Text to appear as a label on the control")]
        public string Label
        {
            get { return _Label; }
            set
            {
                _Label = value;
                Invalidate();
            }
        }

        private Font _LabelFont = new Font("Arial", 12, FontStyle.Bold);
        [Category("Appearance Label")]
        [Description("Font to use for the Label Text")]
        [DefaultValue(typeof(Font), "Arial, 12pt, style=Bold")]
        public Font LabelFont
        {
            get { return _LabelFont; }
            set
            {
                _LabelFont = value;
                UpdateRects();
                Invalidate();
            }
        }

        private readonly StringFormat _Labelsf = new StringFormat();
        private StringAlignment _LabelAlighnment = StringAlignment.Near;
        [Category("Appearance Label")]
        [Description("Alignment for the Label Text")]
        [DefaultValue(typeof(StringAlignment), "Near")]
        public StringAlignment LabelAlighnment
        {
            get { return _LabelAlighnment; }
            set
            {
                _LabelAlighnment = value;
                _Labelsf.Alignment = value;
                _Labelsf.Trimming = StringTrimming.EllipsisCharacter;
                Invalidate();
            }
        }

        private Padding _LabelPadding = new Padding(3);
        [Category("Appearance Label")]
        [Description("Pad the Label Text from the edge of the Control")]
        [DefaultValue(typeof(Padding), "3, 3, 3, 3")]
        public Padding LabelPadding
        {
            get { return _LabelPadding; }
            set
            {
                _LabelPadding = value;
                Padding = value;
                UpdateRects();
                Invalidate();
            }
        }

        private bool _LabelShow;
        [Category("Appearance Label")]
        [Description("Show or not show the Label Text")]
        [DefaultValue(false)]
        public bool LabelShow
        {
            get { return _LabelShow; }
            set
            {
                _LabelShow = value;
                UpdateRects();

                Invalidate();
            }
        }

        #endregion

        #region "Slider"

        private float _SliderWidthHigh = 1;
        [Category("Appearance Slider")]
        [Description("How wide to make the High side of the Slider Line")]
        [DefaultValue(1)]
        public float SliderWidthHigh
        {
            get { return _SliderWidthHigh; }
            set
            {
                _SliderWidthHigh = value;
                Invalidate();
            }
        }

        private float _SliderWidthLow = 1;
        [Category("Appearance Slider")]
        [Description("How wide to make Low side of the Slider Line")]
        [DefaultValue(1)]
        public float SliderWidthLow
        {
            get { return _SliderWidthLow; }
            set
            {
                _SliderWidthLow = value;
                Invalidate();
            }
        }

        private Bitmap _SliderImage;
        [Category("Appearance Slider")]
        [Description("Slider Image")]
        [DefaultValue(typeof(Bitmap), "none")]
        public Bitmap SliderImage
        {
            get { return _SliderImage; }
            set
            {
                _SliderImage = value;
                Invalidate();
            }
        }

        private LineCap _SliderCapStart = LineCap.Round;
        [Category("Appearance Slider")]
        [Description("Cap style to use for the start of the Slider Line")]
        [DefaultValue(typeof(LineCap), "Round")]
        public LineCap SliderCapStart
        {
            get { return _SliderCapStart; }
            set
            {
                _SliderCapStart = value;
                Invalidate();
            }
        }

        private LineCap _SliderCapEnd = LineCap.Round;
        [Category("Appearance Slider")]
        [Description("Cap style to use for the end of the Slider Line")]
        [DefaultValue(typeof(LineCap), "Round")]
        public LineCap SliderCapEnd
        {
            get { return _SliderCapEnd; }
            set
            {
                _SliderCapEnd = value;
                Invalidate();
            }
        }

        private Size _SliderSize = new Size(20, 10);
        [Category("Appearance Slider")]
        [Description("Size of the Slider")]
        [DefaultValue(typeof(Size), "20, 10")]
        public Size SliderSize
        {
            get { return _SliderSize; }
            set
            {
                _SliderSize = value;
                if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    intSlideIndent = Convert.ToInt32(value.Width / 2f) + 5;
                }
                else
                {
                    intSlideIndent = Convert.ToInt32(value.Height / 2f) + 5;
                }
                UpdateRects();
                Invalidate();
            }
        }

        private eShape _SliderShape = eShape.Ellipse;
        [Category("Appearance Slider")]
        [Description("Shape for the Slider")]
        [DefaultValue(typeof(eShape), "Ellipse")]
        public eShape SliderShape
        {
            get { return _SliderShape; }
            set
            {
                _SliderShape = value;
                SetSliderPath();
                Invalidate();
            }
        }

        private PointF _SliderHighlightPt = new PointF(-5f, -2.5f);
        [Category("Appearance Slider")]
        [Description("Point on the Slider for the Highlight Color")]
        [TypeConverter(typeof(PointFConverter))]
        public PointF SliderHighlightPt
        {
            get { return _SliderHighlightPt; }
            set
            {
                _SliderHighlightPt = value;
                Invalidate();
            }
        }

        #region "SliderHighlightPt Default Value"

        public void ResetSliderHighlightPt()
        {
            SliderHighlightPt = new PointF(-5f, -2.5f);
        }

        private bool ShouldSerializeSliderHighlightPt()
        {
            return !(_SliderHighlightPt.Equals(new PointF(-5f, -2.5f)));
        }
        #endregion

        private PointF _SliderFocalPt = new PointF(0f, 0f);
        [Category("Appearance Slider")]
        [Description("Focus of the Center Point")]
        [TypeConverter(typeof(PointFConverter))]
        public PointF SliderFocalPt
        {
            get { return _SliderFocalPt; }
            set
            {
                _SliderFocalPt = value;
                Invalidate();
            }
        }

        #region "SliderFocalPt Default Value"

        public void ResetSliderFocalPt()
        {
            SliderFocalPt = new PointF(0f, 0f);
        }

        private bool ShouldSerializeSliderFocalPt()
        {
            return !(_SliderFocalPt.Equals(new PointF(0f, 0f)));
        }
        #endregion

        #endregion

        #region "ValueBox"

        private eValueBox _ValueBox = eValueBox.None;
        [Category("Appearance ValueBox")]
        [Description("Where to draw the Value Box")]
        [DefaultValue(typeof(eValueBox), "None")]
        public eValueBox ValueBox
        {
            get { return _ValueBox; }
            set
            {
                _ValueBox = value;
                SetSliderRect();
                Invalidate();
            }
        }

        private Size _ValueBoxSize = new Size(30, 20);
        [Category("Appearance ValueBox")]
        [Description("What size to draw the Value Box")]
        [DefaultValue(typeof(Size), "30, 20")]
        public Size ValueBoxSize
        {
            get { return _ValueBoxSize; }
            set
            {
                _ValueBoxSize = value;
                rectValueBox.Width = value.Width;
                rectValueBox.Height = value.Height;
                SetSliderRect();
                Invalidate();
            }
        }

        private Font _ValueBoxFont = new Font("Arial", (float)8.25);
        [Category("Appearance ValueBox")]
        [Description("What font to use in the Value Box")]
        [DefaultValue(typeof(Font), "Arial, 8.25pt")]
        public Font ValueBoxFont
        {
            get { return _ValueBoxFont; }
            set
            {
                _ValueBoxFont = value;
                Invalidate();
            }
        }

        private eShape _ValueBoxShape = eShape.Rectangle;
        [Category("Appearance ValueBox")]
        [Description("What Shape to draw the Value Box")]
        [DefaultValue(typeof(eShape), "Rectangle")]
        public eShape ValueBoxShape
        {
            get { return _ValueBoxShape; }
            set
            {
                _ValueBoxShape = value;
                Invalidate();
            }
        }

        #endregion

        #region "UpDownButtons"

        private int _UpDownWidth = 30;
        [Category("Appearance UpDownButtons")]
        [Description("Width to draw the Up and Down Buttons if not set to Auto")]
        [DefaultValue(30)]
        public int UpDownWidth
        {
            get { return _UpDownWidth; }
            set
            {
                if (value < 10)
                    value = 10;
                _UpDownWidth = value;
                SetUpDnButtonsRect();
                Invalidate();
            }
        }

        private bool _UpDownAutoWidth = true;
        [Category("Appearance UpDownButtons")]
        [Description("Auto Size the Buttons to the Control")]
        [DefaultValue(true)]
        public bool UpDownAutoWidth
        {
            get { return _UpDownAutoWidth; }
            set
            {
                _UpDownAutoWidth = value;
                SetUpDnButtonsRect();
                Invalidate();
            }
        }

        private bool _UpDownShow = true;
        [Category("Appearance UpDownButtons")]
        [Description("Get or Set if the Up and Down buttons are shown")]
        [DefaultValue(true)]
        public bool UpDownShow
        {
            get { return _UpDownShow; }
            set
            {
                _UpDownShow = value;
                SetSliderRect();
                Invalidate();
            }
        }

        #endregion

        #region "Colors"

        private ColorPack _ColorUp = new ColorPack();
        [Category("Appearance Slider")]
        [Description("Main Color of the Slider when State is Up")]
        [Editor(typeof(ColorPackEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorPackConverter))]
        public ColorPack ColorUp
        {
            get { return _ColorUp; }
            set
            {
                _ColorUp = value;
                CurrSliderColor = _ColorUp.Face;
                CurrSliderBorderColor = _ColorUp.Border;
                CurrSliderHiLtColor = _ColorUp.Highlight;
                Invalidate();
            }
        }

        #region "ColorUp Default Value"

        public void ResetColorUp()
        {
            ColorUp = new ColorPack();
        }

        private bool ShouldSerializeColorUp()
        {
            return !(_ColorUp.Equals(new ColorPack()));
        }
        #endregion

        private ColorPack _ColorDown = new ColorPack(Color.CornflowerBlue, Color.DarkSlateBlue, Color.AliceBlue);
        [Category("Appearance Slider")]
        [Description("Main Color of the Slider when State is Down")]
        [Editor(typeof(ColorPackEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorPackConverter))]
        public ColorPack ColorDown
        {
            get { return _ColorDown; }
            set
            {
                _ColorDown = value;
                Invalidate();
            }
        }

        #region "ColorDown Default Value"

        public void ResetColorDown()
        {
            ColorDown = new ColorPack(Color.CornflowerBlue, Color.DarkSlateBlue, Color.AliceBlue);
        }

        private bool ShouldSerializeColorDown()
        {
            return !(_ColorDown.Equals(new ColorPack(Color.CornflowerBlue, Color.DarkSlateBlue, Color.AliceBlue)));
        }
        #endregion

        private ColorPack _ColorHover = new ColorPack(Color.Blue, Color.RoyalBlue, Color.White);
        [Category("Appearance Slider")]
        [Description("Main Color of the Slider when State is Hovering")]
        [Editor(typeof(ColorPackEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorPackConverter))]
        public ColorPack ColorHover
        {
            get { return _ColorHover; }
            set
            {
                _ColorHover = value;
                Invalidate();
            }
        }

        #region "ColorHover Default Value"

        public void ResetColorHover()
        {
            ColorHover = new ColorPack(Color.Blue, Color.RoyalBlue, Color.White);
        }

        private bool ShouldSerializeColorHover()
        {
            return !(_ColorHover.Equals(new ColorPack(Color.Blue, Color.RoyalBlue, Color.White)));
        }
        #endregion

        private Color _BorderColor = Color.Black;
        [Category("Appearance gTrackBar")]
        [Description("The Color of the Border around the Control")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                _BorderColor = value;
                Invalidate();
            }
        }

        private ColorLinearGradient _SliderColorLow = new ColorLinearGradient(Color.Red, Color.Red);
        [Category("Appearance Slider")]
        [Description("The Color of the Slider Line on the Low Value Side")]
        [Editor(typeof(ColorLinearGradientEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorLinearGradientConverter))]
        public ColorLinearGradient SliderColorLow
        {
            get { return _SliderColorLow; }
            set
            {
                _SliderColorLow = value;
                Invalidate();
            }
        }

        #region "SliderColorLow Default Value"

        public void ResetSliderColorLow()
        {
            SliderColorLow = new ColorLinearGradient(Color.Red, Color.Red);
        }

        private bool ShouldSerializeSliderColorLow()
        {
            return !(_SliderColorLow.Equals(new ColorLinearGradient(Color.Red, Color.Red)));
        }
        #endregion

        private ColorLinearGradient _SliderColorHigh = new ColorLinearGradient(Color.DarkGray, Color.DarkGray);
        [Category("Appearance Slider")]
        [Description("The Color of the Slider Line on the High Value Side")]
        [Editor(typeof(ColorLinearGradientEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorLinearGradientConverter))]
        public ColorLinearGradient SliderColorHigh
        {
            get { return _SliderColorHigh; }
            set
            {
                _SliderColorHigh = value;
                Invalidate();
            }
        }

        #region "SliderColorHigh Default Value"

        public void ResetSliderColorHigh()
        {
            SliderColorHigh = new ColorLinearGradient(Color.DarkGray, Color.DarkGray);
        }

        private bool ShouldSerializeSliderColorHigh()
        {
            return !(_SliderColorHigh.Equals(new ColorLinearGradient(Color.DarkGray, Color.DarkGray)));
        }
        #endregion

        private Color _ArrowColorUp = Color.LightSteelBlue;
        [Category("Appearance UpDownButtons")]
        [Description("Color of the Button Arrow when the State is Up")]
        [DefaultValue(typeof(Color), "LightSteelBlue")]
        public Color ArrowColorUp
        {
            get { return _ArrowColorUp; }
            set
            {
                _ArrowColorUp = value;
                Invalidate();
            }
        }

        private Color _ArrowColorDown = Color.GhostWhite;
        [Category("Appearance UpDownButtons")]
        [Description("Color of the Button Arrow when the State is Down")]
        [DefaultValue(typeof(Color), "GhostWhite")]
        public Color ArrowColorDown
        {
            get { return _ArrowColorDown; }
            set
            {
                _ArrowColorDown = value;
                Invalidate();
            }
        }

        private Color _ArrowColorHover = Color.DarkBlue;
        [Category("Appearance UpDownButtons")]
        [Description("Color of the Button Arrow when the State is Hovering")]
        [DefaultValue(typeof(Color), "DarkBlue")]
        public Color ArrowColorHover
        {
            get { return _ArrowColorHover; }
            set
            {
                _ArrowColorHover = value;
                Invalidate();
            }
        }

        private ColorPack _AButColor = new ColorPack(Color.SteelBlue, Color.CornflowerBlue, Color.Lavender);
        [Category("Appearance UpDownButtons")]
        [Description("Color of the Up Down Button")]
        [Editor(typeof(ColorPackEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorPackConverter))]
        public ColorPack AButColor
        {
            get { return _AButColor; }
            set
            {
                _AButColor = value;
                Invalidate();
            }
        }

        #region "AButColor Default Value"

        public void ResetAButColor()
        {
            AButColor = new ColorPack(Color.SteelBlue, Color.CornflowerBlue, Color.Lavender);
        }

        private bool ShouldSerializeAButColor()
        {
            return !(_AButColor.Equals(new ColorPack(Color.SteelBlue, Color.CornflowerBlue, Color.Lavender)));
        }
        #endregion

        private Color _ValueBoxBackColor = Color.White;
        [Category("Appearance ValueBox")]
        [Description("Background Color for the Value Box")]
        [DefaultValue(typeof(Color), "White")]
        public Color ValueBoxBackColor
        {
            get { return _ValueBoxBackColor; }
            set
            {
                _ValueBoxBackColor = value;
                Invalidate();
            }
        }

        private Color _ValueBoxBorder = Color.MediumBlue;
        [Category("Appearance ValueBox")]
        [Description("Color of the Border for the Value Box")]
        [DefaultValue(typeof(Color), "MediumBlue")]
        public Color ValueBoxBorder
        {
            get { return _ValueBoxBorder; }
            set
            {
                _ValueBoxBorder = value;
                Invalidate();
            }
        }

        private Color _ValueBoxFontColor = Color.MediumBlue;
        [Category("Appearance ValueBox")]
        [Description("Color of the Font for the Value Box")]
        [DefaultValue(typeof(Color), "MediumBlue")]
        public Color ValueBoxFontColor
        {
            get { return _ValueBoxFontColor; }
            set
            {
                _ValueBoxFontColor = value;
                Invalidate();
            }
        }

        private Color _LabelColor = Color.MediumBlue;
        [Category("Appearance Label")]
        [Description("Color of the Label Text")]
        [DefaultValue(typeof(Color), "MediumBlue")]
        public Color LabelColor
        {
            get { return _LabelColor; }
            set
            {
                _LabelColor = value;
                Invalidate();
            }
        }

        private Color _FloatValueFontColor = Color.MediumBlue;
        [Category("Appearance FloatValue")]
        [Description("Color of the Value floating above the Slider")]
        [DefaultValue(typeof(Color), "MediumBlue")]
        public Color FloatValueFontColor
        {
            get { return _FloatValueFontColor; }
            set
            {
                _FloatValueFontColor = value;
                Invalidate();
            }
        }

        private Color _TickColor = Color.DarkGray;
        [Category("Appearance Slider")]
        [Description("Color of the Tick Marks")]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color TickColor
        {
            get { return _TickColor; }
            set
            {
                _TickColor = value;
                Invalidate();
            }
        }

        #endregion

        #endregion

        #region "Painting"

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //Setup the Graphics
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //Draw a Border around the control if requested
            if (_BorderShow)
            {
                g.DrawRectangle(new Pen(_BorderColor), 0, 0, Width - 1, Height - 1);
            }

            //Add the value increment buttons if requested
            if (_UpDownShow)
                DrawUpDnButtons(ref g);

            //Add the Line and Tick Marks
            DrawSliderLine(ref g);

            //Draw the Label Text if requested
            if (_LabelShow)
            {
                DrawLabel(ref g);
                //g.DrawRectangle(Pens.Gray, rectLabel)
            }

            //Add the Slider button
            DrawSlider(ref g);

            //Draw the Value above the Slider if requested
            if (_FloatValue && IsOverSlider && MouseState == eMouseState.Down)
            {
                DrawFloatValue(ref g);
            }

            //Draw the Box displating the value if requested
            if (!(_ValueBox == eValueBox.None))
            {
                DrawValueBox(ref g);
            }

            //Draw Focus Rectangle around control if requested 
            if (_ShowFocus && Focused)
            {
                ControlPaint.DrawFocusRectangle(g, new Rectangle(2 + Convert.ToInt32(!_BorderShow), 2 + Convert.ToInt32(!_BorderShow), Width - ((2 + Convert.ToInt32(!_BorderShow)) * 2), Height - ((2 + Convert.ToInt32(!_BorderShow)) * 2)), Color.Black, BackColor);
            }


        }

        private void DrawLabel(ref Graphics g)
        {
            if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                _Labelsf.FormatFlags = StringFormatFlags.DisplayFormatControl;
            }
            else
            {
                _Labelsf.FormatFlags = StringFormatFlags.DirectionVertical;
            }
            g.DrawString(_Label, _LabelFont, new SolidBrush(_LabelColor), rectLabel, _Labelsf);
        }

        private void DrawSlider(ref Graphics g)
        {
            switch (_BrushStyle)
            {
                case eBrushStyle.Linear:

                    using (LinearGradientBrush br = new LinearGradientBrush(gpSlider.GetBounds(), CurrSliderHiLtColor, CurrSliderColor, _BrushDirection))
                    {

                        g.FillPath(br, gpSlider);

                    }


                    break;
                case eBrushStyle.Linear2:
                    ColorBlend blend = new ColorBlend();
                    Color[] bColors = new Color[] {
                    CurrSliderColor,
                    CurrSliderColor,
                    CurrSliderHiLtColor,
                    CurrSliderColor,
                    CurrSliderColor
                };
                    blend.Colors = bColors;

                    float[] bPts = new float[] {
                    0,
                    _SliderFocalPt.X,
                    (float) 0.5,
                    _SliderFocalPt.Y,
                    1
                };
                    blend.Positions = bPts;

                    using (LinearGradientBrush br = new LinearGradientBrush(gpSlider.GetBounds(), CurrSliderColor, CurrSliderHiLtColor, _BrushDirection))
                    {
                        br.InterpolationColors = blend;
                        g.FillPath(br, gpSlider);

                    }


                    break;
                case eBrushStyle.Path:

                    using (PathGradientBrush br = new PathGradientBrush(gpSlider))
                    {
                        br.SurroundColors = new Color[] { CurrSliderColor };
                        br.CenterColor = CurrSliderHiLtColor;
                        br.CenterPoint = new PointF(br.CenterPoint.X + SliderHighlightPt.X, br.CenterPoint.Y + SliderHighlightPt.Y);
                        br.FocusScales = _SliderFocalPt;
                        g.FillPath(br, gpSlider);
                    }


                    break;
                case eBrushStyle.Image:

                    break;

            }

            if (_BrushStyle == eBrushStyle.Image)
            {
                if (_SliderImage == null)
                {
                    g.DrawRectangle(new Pen(CurrSliderBorderColor), Rectangle.Round(gpSlider.GetBounds()));
                }
                else
                {
                    g.DrawImage(_SliderImage, gpSlider.GetBounds());
                }
            }
            else
            {
                g.DrawPath(new Pen(CurrSliderBorderColor), gpSlider);
            }

        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore()]
        public Func<string> StringValue { get; set; }

        private void DrawFloatValue(ref Graphics g)
        {
            dynamic text = StringValue.Invoke();
            SizeF sz = g.MeasureString(text, _FloatValueFont, new PointF(0, 0), StringFormat.GenericDefault);
            Rectangle rect = default(Rectangle);
            PathGradientBrush pbr = default(PathGradientBrush);
            GraphicsPath gp = new GraphicsPath();
            if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                rect = new Rectangle(Convert.ToInt32(sngSliderPos - (sz.Width / 2f)), Convert.ToInt32((rectSlider.Height / 2f) + rectSlider.Y - (_SliderSize.Height / 2f) - 1 - sz.Height), Convert.ToInt32(sz.Width) + 1, Convert.ToInt32(sz.Height));
            }
            else
            {
                rect = new Rectangle(Convert.ToInt32((rectSlider.Width / 2f) - (sz.Width / 2f)), Convert.ToInt32(sngSliderPos - sz.Height - (_SliderSize.Height / 2f) - 3), Convert.ToInt32(sz.Width + 1), Convert.ToInt32(sz.Height + 2));
            }
            gp.AddRectangle(rect);
            pbr = new PathGradientBrush(gp);
            pbr.SurroundColors = new Color[] { Color.Transparent };
            if (BackColor == Color.Transparent)
            {
                pbr.CenterColor = Parent.BackColor;
            }
            else
            {
                pbr.CenterColor = BackColor;
            }
            g.FillRectangle(pbr, rect);
            rect.Y += 2;
            g.DrawString(text, _FloatValueFont, new SolidBrush(_FloatValueFontColor), rect, sf);
            pbr.Dispose();
            gp.Dispose();
        }


        private void DrawValueBox(ref Graphics g)
        {
            using (Brush bbr = new SolidBrush(_ValueBoxBackColor))
            {
                using (Pen pn = new Pen(_ValueBoxBorder))
                {
                    using (Brush fbr = new SolidBrush(_ValueBoxFontColor))
                    {
                        Rectangle rect = new Rectangle(rectValueBox.X, rectValueBox.Y, rectValueBox.Width, rectValueBox.Height);
                        if (ValueBoxShape == eShape.Rectangle)
                        {
                            g.FillRectangle(bbr, rect);
                            g.DrawRectangle(pn, rect.X, rect.Y, rect.Width, rect.Height);
                        }
                        else
                        {
                            g.FillEllipse(bbr, rect);
                            g.DrawEllipse(pn, rect.X, rect.Y, rect.Width, rect.Height);
                        }

                        g.DrawString(ValueAdjusted.ToString(_valueStrFormat), _ValueBoxFont, fbr, new Rectangle(rect.X, rect.Y + 1, rect.Width + 1, rect.Height + 1), sf);
                    }
                }
            }

        }

        private void DrawUpDnButtons(ref Graphics g)
        {
            using (Pen pn = new Pen(_ArrowColorUp, 2))
            {
                pn.EndCap = LineCap.Round;
                pn.StartCap = LineCap.Round;
                pn.LineJoin = LineJoin.Round;
                GraphicsPath gp = new GraphicsPath();
                Point[] pts = null;
                Matrix mx = new Matrix();
                pts = new Point[] {
                new Point(5, 0),
                new Point(0, 5),
                new Point(5, 10)
            };
                gp.AddLines(pts);


                if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    if (IsOverDownButton)
                    {
                        g.FillRectangle(new LinearGradientBrush(rectDownButton, _AButColor.Highlight, _AButColor.Face, LinearGradientMode.Horizontal), rectDownButton);
                        if (MouseState == eMouseState.Down)
                        {
                            pn.Color = _ArrowColorDown;
                        }
                        else
                        {
                            pn.Color = _ArrowColorHover;
                        }
                        g.DrawRectangle(new Pen(_AButColor.Border), new Rectangle(rectDownButton.X + 1, rectDownButton.Y, rectDownButton.Width - 1, rectDownButton.Height - 1));
                    }
                    var _with1 = rectDownButton;
                    mx.Translate(5, Convert.ToSingle((rectDownButton.Y + (rectDownButton.Height / 2f)) - 6));
                    gp.Transform(mx);
                    g.DrawPath(pn, gp);

                    pn.Color = _ArrowColorUp;
                    if (IsOverUpButton)
                    {
                        g.FillRectangle(new LinearGradientBrush(rectUpButton, _AButColor.Face, _AButColor.Highlight, LinearGradientMode.Horizontal), rectUpButton);
                        if (MouseState == eMouseState.Down)
                        {
                            pn.Color = _ArrowColorDown;
                        }
                        else
                        {
                            pn.Color = _ArrowColorHover;
                        }
                        g.DrawRectangle(new Pen(_AButColor.Border), new Rectangle(rectUpButton.X, rectUpButton.Y, rectUpButton.Width - 1, rectUpButton.Height - 1));
                    }
                    var _with2 = rectUpButton;
                    mx = new Matrix(-1, 0, 0, 1, 5, 0);
                    mx.Translate(_with2.X + 9, 0, MatrixOrder.Append);
                    gp.Transform(mx);
                    g.DrawPath(pn, gp);

                }
                else
                {
                    if (IsOverUpButton)
                    {
                        g.FillRectangle(new LinearGradientBrush(rectUpButton, _AButColor.Face, _AButColor.Highlight, LinearGradientMode.Vertical), rectUpButton);
                        g.DrawRectangle(new Pen(_AButColor.Border), new Rectangle(rectUpButton.X, rectUpButton.Y, rectUpButton.Width - 1, rectUpButton.Height - 1));
                        if (MouseState == eMouseState.Down)
                        {
                            pn.Color = _ArrowColorDown;
                        }
                        else
                        {
                            pn.Color = _ArrowColorHover;
                        }
                    }
                    var _with3 = rectUpButton;
                    var bounds = gp.GetBounds();
                    mx.RotateAt(90, new PointF(bounds.Width / 2f, bounds.Height / 2f));
                    mx.Translate(Convert.ToSingle((rectDownButton.X + (rectDownButton.Width / 2f)) - 3), 4, MatrixOrder.Append);
                    gp.Transform(mx);
                    g.DrawPath(pn, gp);

                    pn.Color = _ArrowColorUp;
                    if (IsOverDownButton)
                    {
                        g.FillRectangle(new LinearGradientBrush(rectDownButton, _AButColor.Highlight, _AButColor.Face, LinearGradientMode.Vertical), rectDownButton);
                        g.DrawRectangle(new Pen(_AButColor.Border), new Rectangle(rectDownButton.X, rectDownButton.Y, rectDownButton.Width - 1, rectDownButton.Height - 1));
                        if (MouseState == eMouseState.Down)
                        {
                            pn.Color = _ArrowColorDown;
                        }
                        else
                        {
                            pn.Color = _ArrowColorHover;
                        }
                    }
                    var _with4 = rectDownButton;
                    mx = new Matrix(1, 0, 0, -1, 0, 10);
                    mx.Translate(0, _with4.Y + 6, MatrixOrder.Append);
                    gp.Transform(mx);
                    g.DrawPath(pn, gp);
                }
                mx.Dispose();
                gp.Dispose();
            }

        }

        private void DrawSliderLine(ref Graphics g)
        {
            using (Pen pn = new Pen(_SliderColorLow.ColorA, _SliderWidthLow))
            {
                using (Pen tpn = new Pen(_TickColor, _TickThickness))
                {
                    int @switch = Convert.ToInt32((_Orientation == System.Windows.Forms.Orientation.Horizontal ? 1 : -1));
                    float t1 = 0;
                    float t2 = 0;
                    int lAdj = 0;

                    switch (_TickType)
                    {
                        case eTickType.Middle:
                            t1 = Convert.ToSingle(-_TickWidth / 2f);
                            t2 = Convert.ToSingle(_TickWidth / 2f);

                            break;
                        case eTickType.Up_Right:
                            t1 = (-5 - _TickOffset - _TickWidth) * @switch;
                            t2 = (-5 - _TickOffset) * @switch;

                            break;
                        case eTickType.Down_Left:
                        case eTickType.Both:
                            t1 = (5 + _TickOffset + _TickWidth) * @switch;
                            t2 = (5 + _TickOffset) * @switch;

                            break;
                    }

                    if (_LabelShow)
                    {
                        lAdj += rectLabel.Height + _LabelPadding.Vertical - 4;
                    }

                    int Tickpos = 0;

                    if (Orientation == System.Windows.Forms.Orientation.Horizontal)
                    {
                        if (_TickType != eTickType.None)
                        {
                            for (int i = 0; i <= _MaxValue - _MinValue; i += _TickInterval)
                            {
                                Tickpos = Convert.ToInt32(rectSlider.X + (rectSlider.Width * (i / (float)(_MaxValue - _MinValue))));
                                g.DrawLine(tpn, Tickpos, Convert.ToSingle(rectSlider.Height / 2f) + t1 + lAdj, Tickpos, Convert.ToSingle(rectSlider.Height / 2f) + t2 + lAdj);
                                if (_TickType == eTickType.Both)
                                {
                                    g.DrawLine(tpn, Tickpos, Convert.ToSingle(rectSlider.Height / 2f) - t1 + lAdj, Tickpos, Convert.ToSingle(rectSlider.Height / 2f) - t2 + lAdj);
                                }
                            }
                        }

                        pn.StartCap = _SliderCapStart;
                        if (_Value == _MaxValue)
                        {
                            pn.EndCap = _SliderCapEnd;
                        }
                        else
                        {
                            pn.EndCap = LineCap.Flat;
                        }
                        pn.Brush = new LinearGradientBrush(new PointF(Convert.ToSingle(rectSlider.X - _SliderWidthLow), Convert.ToSingle(rectSlider.Height / 2f + rectSlider.Y)), new PointF(sngSliderPos + _SliderWidthLow, Convert.ToSingle(rectSlider.Height / 2f + rectSlider.Y)), _SliderColorLow.ColorA, _SliderColorLow.ColorB);
                        g.DrawLine(pn, Convert.ToSingle(rectSlider.X), Convert.ToSingle(rectSlider.Height / 2f + rectSlider.Y), sngSliderPos + 1, Convert.ToSingle(rectSlider.Height / 2f+ rectSlider.Y));

                        if (_Value == _MinValue)
                        {
                            pn.StartCap = _SliderCapStart;
                        }
                        else
                        {
                            pn.StartCap = LineCap.Flat;
                        }
                        pn.EndCap = _SliderCapEnd;
                        pn.Brush = new LinearGradientBrush(new PointF(sngSliderPos - _SliderWidthHigh, Convert.ToSingle(rectSlider.Height / 2f + rectSlider.Y)), new PointF(Convert.ToSingle(rectSlider.X + rectSlider.Width + _SliderWidthHigh + 1), Convert.ToSingle(rectSlider.Height / 2 + rectSlider.Y)), _SliderColorHigh.ColorA, _SliderColorHigh.ColorB);
                        pn.Width = _SliderWidthHigh;
                        g.DrawLine(pn, sngSliderPos, Convert.ToSingle(rectSlider.Height / 2f+ rectSlider.Y), Convert.ToSingle(rectSlider.X + rectSlider.Width), Convert.ToSingle(rectSlider.Height / 2f + rectSlider.Y));

                    }
                    else
                    {
                        if (_TickType != eTickType.None)
                        {
                            for (int i = 0; i <= _MaxValue - _MinValue; i += _TickInterval)
                            {
                                Tickpos = Convert.ToInt32(rectSlider.Y + (rectSlider.Height * (i / (float)(_MaxValue - _MinValue))));
                                g.DrawLine(tpn, Convert.ToSingle(rectSlider.Width / 2f) + t1, Tickpos, Convert.ToSingle(rectSlider.Width / 2f) + t2, Tickpos);
                                if (_TickType == eTickType.Both)
                                {
                                    g.DrawLine(tpn, Convert.ToSingle(rectSlider.Width / 2f) - t1, Tickpos, Convert.ToSingle(rectSlider.Width / 2f) - t2, Tickpos);
                                }
                            }
                        }

                        //Bottom
                        pn.StartCap = _SliderCapStart;
                        if (_Value == _MaxValue)
                        {
                            pn.EndCap = _SliderCapEnd;
                        }
                        else
                        {
                            pn.EndCap = LineCap.Flat;
                        }
                        pn.Brush = new LinearGradientBrush(new PointF(Convert.ToSingle(rectSlider.Width / 2f), sngSliderPos - _SliderWidthLow), new PointF(Convert.ToSingle(rectSlider.Width / 2f), Convert.ToSingle(rectSlider.Y + rectSlider.Height + _SliderWidthLow + 1)), _SliderColorLow.ColorA, _SliderColorLow.ColorB);

                        pn.Width = _SliderWidthLow;
                        g.DrawLine(pn, Convert.ToSingle(rectSlider.Width / 2f), Convert.ToSingle(rectSlider.Y + rectSlider.Height), Convert.ToSingle(rectSlider.Width / 2f), sngSliderPos);

                        //top
                        if (_Value == _MinValue)
                        {
                            pn.StartCap = _SliderCapStart;
                        }
                        else
                        {
                            pn.StartCap = LineCap.Flat;
                        }
                        pn.EndCap = _SliderCapEnd;
                        pn.Color = _SliderColorHigh.ColorA;
                        pn.Width = _SliderWidthHigh;
                        pn.Brush = new LinearGradientBrush(new PointF(Convert.ToSingle(rectSlider.Width / 2f), Convert.ToSingle(rectSlider.Y - _SliderWidthHigh - 1)), new PointF(Convert.ToSingle(rectSlider.Width / 2f), sngSliderPos + _SliderWidthHigh), _SliderColorHigh.ColorA, _SliderColorHigh.ColorB);

                        pn.Width = _SliderWidthHigh;

                        g.DrawLine(pn, Convert.ToSingle(rectSlider.Width / 2f), sngSliderPos, Convert.ToSingle(rectSlider.Width / 2f), Convert.ToSingle(rectSlider.Y));
                    }
                }
            }

        }

        #endregion

        #region "Building"

        private void SetSliderPath()
        {
            gpSlider.Reset();
            RectangleF rect = default(RectangleF);
            if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                rect = new RectangleF(Convert.ToSingle(sngSliderPos - (_SliderSize.Width / 2f)), Convert.ToSingle(rectSlider.Y + (rectSlider.Height / 2f) - (_SliderSize.Height) / 2f), _SliderSize.Width, _SliderSize.Height);
            }
            else
            {
                rect = new RectangleF(Convert.ToSingle((rectSlider.Width - _SliderSize.Width) / 2f), Convert.ToSingle(sngSliderPos - (_SliderSize.Height / 2f)), _SliderSize.Width, _SliderSize.Height);
            }

            switch (_SliderShape)
            {

                case eShape.Rectangle:
                    gpSlider.AddRectangle(rect);

                    break;
                case eShape.Ellipse:
                    gpSlider.AddEllipse(rect);

                    break;
                case eShape.ArrowUp:
                    gpSlider.AddPolygon(new PointF[] {
                    new PointF(rect.X, rect.Bottom),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.X + (rect.Width / 2f), rect.Top)
                });

                    break;
                case eShape.ArrowDown:
                    gpSlider.AddPolygon(new PointF[] {
                    new PointF(rect.X, rect.Top),
                    new PointF(rect.Right, rect.Top),
                    new PointF(rect.X + (rect.Width / 2f), rect.Bottom)
                });

                    break;
                case eShape.ArrowRight:
                    gpSlider.AddPolygon(new PointF[] {
                    new PointF(rect.X, rect.Bottom),
                    new PointF(rect.Right, rect.Top + (rect.Height / 2f)),
                    new PointF(rect.X, rect.Top)
                });

                    break;
                case eShape.ArrowLeft:
                    gpSlider.AddPolygon(new PointF[] {
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.X, rect.Top + (rect.Height / 2f)),
                    new PointF(rect.Right, rect.Top)
                });

                    break;

            }

            InvRect = Rectangle.Round(gpSlider.GetBounds());
            InvRect.Inflate(2, 2);
        }

        private void UpdateSlider(int xPos)
        {
            RectangleF rect = gpSlider.GetBounds();
            rect.Inflate(20, 20);
            rect.Offset(-10, -10);
            Invalidate(Rectangle.Round(rect));
            sngSliderPos = xPos;
            if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (sngSliderPos - rectSlider.X < 0)
                    sngSliderPos = rectSlider.X;
                if (sngSliderPos > rectSlider.X + rectSlider.Width)
                    sngSliderPos = rectSlider.X + rectSlider.Width;
            }
            else
            {
                if (sngSliderPos - rectSlider.Y < 0)
                    sngSliderPos = rectSlider.Y;
                if (sngSliderPos > rectSlider.Y + rectSlider.Height)
                    sngSliderPos = rectSlider.Y + rectSlider.Height;
            }
            SetSliderPath();
            Invalidate(Rectangle.Round(rect));
        }

        private void SetUpDnButtonsRect()
        {
            int UDWidth = 0;
            int UDY = 0;

            if (Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                if (_UpDownAutoWidth)
                {
                    UDWidth = rectSlider.Height - 4;
                    UDY = 3;
                }
                else
                {
                    UDWidth = _UpDownWidth;
                    UDY = Convert.ToInt32((rectSlider.Height - UDWidth) / 2f);
                }

                if (_LabelShow)
                    UDY += rectLabel.Height + _LabelPadding.Vertical - 4;

                rectDownButton = new Rectangle(1, UDY, 15, UDWidth);
                rectUpButton = new Rectangle(Width - 17, UDY, 15, UDWidth);
            }
            else
            {
                if (_UpDownAutoWidth)
                {
                    UDWidth = rectSlider.Width - 4;
                    UDY = 2;
                }
                else
                {
                    UDWidth = _UpDownWidth;
                    UDY = Convert.ToInt32((rectSlider.Width - UDWidth) / 2f);
                }

                rectUpButton = new Rectangle(UDY, 2, UDWidth, 15);
                rectDownButton = new Rectangle(UDY, Height - 17, UDWidth, 15);
            }
        }

        private void SetLabelRect()
        {
            if (Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                rectLabel = new Rectangle(_LabelPadding.Left, _LabelPadding.Top, Width - _LabelPadding.Horizontal - 1, LabelFont.Height);
            }
            else
            {
                rectLabel = new Rectangle(Width - LabelFont.Height - _LabelPadding.Top, _LabelPadding.Left, LabelFont.Height, Height - _LabelPadding.Horizontal - 1);
            }
        }

        private void SetSliderRect()
        {
            try
            {
                int ButtonOffset = 17;
                if (!_UpDownShow)
                    ButtonOffset = 0;
                var _with5 = rectSlider;
                if (Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    float _SliderWidth = Math.Max(_SliderWidthLow, _SliderWidthHigh);

                    if (_LabelShow)
                    {
                        _with5.Height = Height - rectLabel.Height - _LabelPadding.Top;
                    }
                    else
                    {
                        _with5.Height = Height - 1;
                    }

                    switch (_ValueBox)
                    {
                        case eValueBox.None:
                            _with5.X = ButtonOffset + intSlideIndent;
                            _with5.Width = Width - ((ButtonOffset * 2) + 1) - (intSlideIndent * 2);
                            break;
                        case eValueBox.Left:
                            rectValueBox.X = ButtonOffset + 1;
                            rectValueBox.Y = Convert.ToInt32(((rectSlider.Height - rectValueBox.Height) / 2f));
                            _with5.Width = Convert.ToInt32(Width - ((ButtonOffset * 2) + 1) - rectValueBox.Width - (intSlideIndent * 2) - (_SliderWidth / 2f));
                            _with5.X = Convert.ToInt32(rectValueBox.Width + ButtonOffset + intSlideIndent + (_SliderWidth / 2f));
                            break;
                        case eValueBox.Right:
                            rectValueBox.X = Width - ButtonOffset - 2 - rectValueBox.Width;
                            rectValueBox.Y = Convert.ToInt32(((rectSlider.Height - rectValueBox.Height) / 2f));
                            _with5.Width = Convert.ToInt32(Width - ((ButtonOffset * 2) + 1) - rectValueBox.Width - (intSlideIndent * 2) - (_SliderWidth / 2f));
                            _with5.X = ButtonOffset + intSlideIndent;
                            break;
                    }

                    if (_LabelShow)
                    {
                        _with5.Y = rectLabel.Height + _LabelPadding.Vertical - 4;
                        rectValueBox.Y += rectLabel.Height + _LabelPadding.Vertical - 4;
                    }
                    else
                    {
                        _with5.Y = 0;
                    }
                    UpdateSlider(Convert.ToInt32(rectSlider.X + (rectSlider.Width * ((_Value - _MinValue) / (float)(_MaxValue - _MinValue)))));

                }
                else
                {
                    switch (_ValueBox)
                    {
                        case eValueBox.None:
                            _with5.Y = ButtonOffset + intSlideIndent;
                            _with5.Height = Height - ((ButtonOffset * 2) + 1) - (intSlideIndent * 2);
                            break;
                        case eValueBox.Left:
                            rectValueBox.X = Convert.ToInt32(((rectSlider.Width - rectValueBox.Width) / 2f));
                            rectValueBox.Y = ButtonOffset + 1;
                            _with5.Height = Convert.ToInt32(Height - ((ButtonOffset * 2) + 1) - rectValueBox.Height - (intSlideIndent * 2));
                            _with5.Y = Convert.ToInt32(rectValueBox.Height + ButtonOffset + intSlideIndent);
                            break;
                        case eValueBox.Right:
                            rectValueBox.X = Convert.ToInt32(((rectSlider.Width - rectValueBox.Width) / 2f));
                            rectValueBox.Y = Height - ButtonOffset - 2 - rectValueBox.Height;
                            _with5.Height = Convert.ToInt32(Height - ((ButtonOffset * 2) + 1) - rectValueBox.Height - (intSlideIndent * 2));
                            _with5.Y = ButtonOffset + intSlideIndent;
                            break;
                    }
                    if (_LabelShow)
                    {
                        _with5.X = 0;
                        _with5.Width = Width - rectLabel.Width - _LabelPadding.Top;
                    }
                    else
                    {
                        _with5.X = 0;
                        _with5.Width = Width - 1;
                    }
                    int adj = 0;
                    if (_MinValue < 0)
                        adj = Math.Abs(_MinValue);
                    UpdateSlider(Convert.ToInt32(rectSlider.Y + (rectSlider.Height * (((_MaxValue + adj) - _Value - adj) / (float)((_MaxValue + adj) - (_MinValue + adj))))));

                }

            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateRects()
        {
            SetLabelRect();
            SetSliderRect();
            SetSliderPath();
            SetUpDnButtonsRect();
        }

        #endregion

        #region "Mouse"

        private Rectangle InvRect;
        private Color CurrSliderColor;
        private Color CurrSliderBorderColor;
        private Color CurrSliderHiLtColor;
        //   Private Orient As Integer = 1
        private int MouseHoldDownTicker;
        private int MouseHoldDownChange;
        private int OldValue;

        private ScrollEventType ScrollType;
        private void TBSlider_MouseDown(object sender, MouseEventArgs e)
        {
            OldValue = _Value;
            MouseState = eMouseState.Down;
            MouseHoldDownTicker = 0;
            MouseTimer.Interval = 100;
            if (_UpDownShow)
            {
                if (IsOverDownButton)
                {
                    MouseHoldDownChange = -_ChangeSmall;
                    OldValue = _Value;
                    ScrollType = ScrollEventType.SmallDecrement;
                    Value += MouseHoldDownChange;
                    if (Scroll != null)
                    {
                        Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
                    }
                    MouseTimer.Start();
                }
                else if (IsOverUpButton)
                {
                    MouseHoldDownChange = _ChangeSmall;
                    OldValue = _Value;
                    ScrollType = ScrollEventType.SmallIncrement;
                    Value += MouseHoldDownChange;
                    if (Scroll != null)
                    {
                        Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
                    }
                    MouseTimer.Start();
                }
            }
            IsOverSlider = gpSlider.IsVisible(e.X, e.Y);
            int pos = 0;
            if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                pos = e.X;
            }
            else
            {
                pos = e.Y;
            }
            if (IsOverSlider)
            {
                CurrSliderColor = _ColorDown.Face;
                CurrSliderBorderColor = _ColorDown.Border;
                CurrSliderHiLtColor = _ColorDown.Highlight;
            }
            else if (rectSlider.Contains(e.Location))
            {
                if (_JumpToMouse)
                {
                    sngSliderPos = pos;
                    IsOverSlider = true;
                    OldValue = _Value;
                    SetSliderValue(new Point(e.X, e.Y));
                    ScrollType = ScrollEventType.ThumbPosition;
                    if (Scroll != null)
                    {
                        Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
                    }

                }
                else
                {
                    if (pos < sngSliderPos)
                    {
                        MouseHoldDownChange = _ChangeLarge * Convert.ToInt32((Orientation == System.Windows.Forms.Orientation.Horizontal ? -1 : 1));
                        OldValue = _Value;
                        ScrollType = ScrollEventType.LargeIncrement;
                        Value += MouseHoldDownChange;
                        if (Scroll != null)
                        {
                            Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
                        }
                    }
                    else
                    {
                        MouseHoldDownChange = -(_ChangeLarge * Convert.ToInt32((Orientation == System.Windows.Forms.Orientation.Horizontal ? -1 : 1)));
                        OldValue = _Value;
                        ScrollType = ScrollEventType.LargeDecrement;
                        Value += MouseHoldDownChange;
                        if (Scroll != null)
                        {
                            Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
                        }
                    }
                    MouseTimer.Start();
                }
            }
            Invalidate();
        }

        private void gTrackBar_MouseLeave(object sender, EventArgs e)
        {
            IsOverDownButton = false;
            IsOverUpButton = false;
            CurrSliderColor = _ColorUp.Face;
            CurrSliderBorderColor = _ColorUp.Border;
            CurrSliderHiLtColor = _ColorUp.Highlight;
            Invalidate();
        }

        private void TBSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsOverSlider)
            {
                IsOverDownButton = rectDownButton.Contains(e.Location);
                IsOverUpButton = rectUpButton.Contains(e.Location);
            }
            Rectangle rect = rectDownButton;
            rect.Inflate(1, 1);
            Invalidate(rect);
            rect = rectUpButton;
            rect.Inflate(1, 1);
            Invalidate(rect);

            if (MouseState == eMouseState.Up)
                IsOverSlider = gpSlider.IsVisible(e.X, e.Y);

            if (IsOverSlider & MouseState == eMouseState.Down)
            {
                OldValue = _Value;
                SetSliderValue(new Point(e.X, e.Y));
                if (Scroll != null)
                {
                    Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, OldValue, _Value, (ScrollOrientation)this.Orientation));
                }

            }
            else if (IsOverSlider & MouseState == eMouseState.Up)
            {
                CurrSliderColor = _ColorHover.Face;
                CurrSliderBorderColor = _ColorHover.Border;
                CurrSliderHiLtColor = _ColorHover.Highlight;
                Invalidate(InvRect);
            }
            else
            {
                CurrSliderColor = _ColorUp.Face;
                CurrSliderBorderColor = _ColorUp.Border;
                CurrSliderHiLtColor = _ColorUp.Highlight;
                Invalidate(InvRect);

            }
            Update();
        }

        private void SetSliderValue(Point pt)
        {
            if (_Orientation == System.Windows.Forms.Orientation.Horizontal)
            {
                Value = Convert.ToInt32(((sngSliderPos - rectSlider.X) / (rectSlider.Width / (_MaxValue - _MinValue))) + _MinValue);
                UpdateSlider(pt.X);
            }
            else
            {
                int adj = 0;
                if (_MinValue < 0)
                    adj = Math.Abs(_MinValue);
                Value = ((_MaxValue + adj) - Convert.ToInt32(((sngSliderPos - rectSlider.Y) / (rectSlider.Height / ((_MaxValue + adj) - (_MinValue + adj)))))) - adj;
                UpdateSlider(pt.Y);
            }

        }

        private void TBSlider_MouseUp(object sender, MouseEventArgs e)
        {
            MouseTimer.Stop();
            MouseState = eMouseState.Up;
            IsOverDownButton = rectDownButton.Contains(e.Location);
            IsOverUpButton = rectUpButton.Contains(e.Location);

            if (_SnapToValue)
            {
                OldValue = _Value;
                SetSliderRect();

            }
            Invalidate();
        }

        private void TBSlider_MouseWheel(object sender, MouseEventArgs e)
        {
            OldValue = _Value;
            if (e.Delta > 0)
            {
                ScrollType = ScrollEventType.SmallIncrement;
                Value += _ChangeSmall;
            }
            else
            {
                ScrollType = ScrollEventType.SmallDecrement;
                Value -= _ChangeSmall;
            }

            if (Scroll != null)
            {
                Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
            }

        }

        #endregion

        #region "KeyDown"

        protected override bool IsInputKey(Keys keyData)
        {
            //Because a Usercontrol ignores the arrows in the KeyDown Event
            //and changes focus no matter what in the KeyUp Event
            //This is needed to fix the KeyDown problem
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Left:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }


        private void gTrackBar_KeyUp(object sender, KeyEventArgs e)
        {
            OldValue = _Value;

            int adjust = 0;
            if (e.Shift)
            {
                adjust = _ChangeLarge;
            }
            else
            {
                adjust = _ChangeSmall;
            }

            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Right:
                    Value += adjust;
                    if (e.Shift)
                    {
                        ScrollType = ScrollEventType.LargeIncrement;
                    }
                    else
                    {
                        ScrollType = ScrollEventType.SmallIncrement;
                    }

                    break;
                case Keys.Down:
                case Keys.Left:
                    Value -= adjust;
                    if (e.Shift)
                    {
                        ScrollType = ScrollEventType.LargeDecrement;
                    }
                    else
                    {
                        ScrollType = ScrollEventType.SmallDecrement;
                    }
                    break;
            }
            if (Scroll != null)
            {
                Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
            }
        }

        #endregion

        #region "Resize"

        private void TBSlider_Resize(object sender, EventArgs e)
        {
            UpdateRects();
            Refresh();
        }

        #endregion

        #region "Focus"

        private void gTrackBar_LostFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        #endregion

        #region "Mouse Hold Down Timer"

        private void MouseTimer_Tick(object sender, EventArgs e)
        {
            //Check if mouse was just clicked
            if (MouseHoldDownTicker < 5)
            {
                MouseHoldDownTicker += 1;
                //Interval was set to 100 on MouseDown
                //Tick off 5 times and then reset the Timer Interval
                //  based on the Min/Max span
                if (MouseHoldDownTicker == 5)
                {
                    MouseTimer.Interval = Convert.ToInt32(Math.Max(10, 100 - ((_MaxValue - _MinValue) / 10)));
                }
            }
            else
            {
                //Change the value until the mouse is released
                OldValue = _Value;
                Value += MouseHoldDownChange;
                if (Scroll != null)
                {
                    Scroll(this, new ScrollEventArgs(ScrollType, OldValue, _Value, (ScrollOrientation)this.Orientation));
                }
            }
        }

        #endregion

    }

    #region "PointFConverter"

    internal class PointFConverter : ExpandableObjectConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if ((object.ReferenceEquals(sourceType, typeof(string))))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);

        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {

            if (value is string)
            {
                try
                {
                    string s = Convert.ToString(value);
                    string[] ConverterParts = new string[3];
                    ConverterParts = s.Split(',');
                    if (ConverterParts != null)
                    {
                        if (ConverterParts[0] == null)
                            ConverterParts[0] = "-5";
                        if (ConverterParts[1] == null)
                            ConverterParts[1] = "-2.5";
                        return new PointF(Convert.ToSingle(ConverterParts[0].Trim()), Convert.ToSingle(ConverterParts[1].Trim()));
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("Can not convert '{0}' to type Corners", Convert.ToString(value)));
                }
            }
            else
            {
                return new PointF(-5f, -2.5f);
            }

            return base.ConvertFrom(context, culture, value);

        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {


            if ((object.ReferenceEquals(destinationType, typeof(string)) && value is PointF))
            {
                PointF ConverterProperty = (PointF)value;
                // build the string representation 
                return string.Format("{0}, {1}", ConverterProperty.X, ConverterProperty.Y);
            }
            return base.ConvertTo(context, culture, value, destinationType);

        }

    }
    //PointFConverter Class

    #endregion

    #region "ColorPack"

    #region "ColorPack Class"

    public class ColorPack
    {

        public ColorPack()
        {
            _border = Color.DarkBlue;
            _face = Color.Blue;
            _highlight = Color.AliceBlue;
        }
        public ColorPack(Color Border, Color Face, Color Highlight)
        {
            _border = Border;
            _face = Face;
            _highlight = Highlight;
        }

        private Color _border = Color.Blue;
        public Color Border
        {
            get { return _border; }
            set { _border = value; }
        }

        private Color _face = Color.Blue;
        public Color Face
        {
            get { return _face; }
            set { _face = value; }
        }

        private Color _highlight = Color.AliceBlue;
        public Color Highlight
        {
            get { return _highlight; }
            set { _highlight = value; }
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2}", getColorString(_border), getColorString(_face), getColorString(_highlight));
        }

        private string getColorString(Color bcolor)
        {
            if (bcolor.IsNamedColor)
            {
                return bcolor.Name;
            }
            else
            {
                return string.Format("{0},{1},{2},{3}", bcolor.A, bcolor.R, bcolor.G, bcolor.B);
            }
        }

        public override bool Equals(object obj)
        {
            return this.ToString() == ((ColorPack)obj).ToString();
        }

    }
    #endregion

    #region "ColorPackConverter"

    internal class ColorPackConverter : ExpandableObjectConverter
    {

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            ColorPack Item = new ColorPack();
            Item.Border = (Color)propertyValues["Border"];
            Item.Face = (Color)propertyValues["Face"];
            Item.Highlight = (Color)propertyValues["Highlight"];
            return Item;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if ((object.ReferenceEquals(sourceType, typeof(string))))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);

        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {


            if (value is string)
            {
                try
                {
                    List<Color> bColors = new List<Color>();

                    foreach (string cstring in Convert.ToString(value).Split(';'))
                    {
                        bColors.Add((Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(cstring));
                    }

                    if ((bColors != null) && bColors.Count != 3)
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        return new ColorPack(bColors[0], bColors[1], bColors[2]);
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("Can not convert '{0}' to type ColorPack", Convert.ToString(value)));
                }

            }
            else
            {
                return new ColorPack();
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {

            if ((object.ReferenceEquals(destinationType, typeof(string)) && value is ColorPack))
            {
                return ((ColorPack)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);

        }

    }

    #endregion

    #region "ColorPackEditor"

    public class ColorPackEditor : UITypeEditor
    {

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {

            return true;

        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            // Erase the area.
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            ColorPack cPack = null;
            if ((e.Context == null))
            {
                cPack = new ColorPack();
            }
            else
            {
                cPack = (ColorPack)e.Value;
            }
            // Draw the sample.
            using (Pen border_pen = new Pen(cPack.Border, 2))
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRectangle(e.Bounds);
                    if (e.Context.PropertyDescriptor.DisplayName == "AButColor" || (((ProgressBar)e.Context.Instance).BrushStyle == ProgressBar.eBrushStyle.Linear || ((ProgressBar)e.Context.Instance).BrushStyle == ProgressBar.eBrushStyle.Linear2))
                    {
                        using (LinearGradientBrush br = new LinearGradientBrush(gp.GetBounds(), cPack.Highlight, cPack.Face, LinearGradientMode.Horizontal))
                        {

                            e.Graphics.FillPath(br, gp);

                        }
                    }
                    else
                    {
                        using (PathGradientBrush br = new PathGradientBrush(gp))
                        {
                            br.SurroundColors = new Color[] { cPack.Face };
                            br.CenterColor = cPack.Highlight;
                            br.CenterPoint = new PointF(br.CenterPoint.X - 5, Convert.ToSingle(br.CenterPoint.Y - 2.5));
                            br.FocusScales = new PointF(0, 0);
                            e.Graphics.FillPath(br, gp);
                        }
                    }

                    e.Graphics.DrawRectangle(border_pen, 2, 2, e.Bounds.Width - 2, e.Bounds.Height - 2);
                }
            }

        }
    }

    #endregion
    #endregion

    #region "ColorLinearGradient"

    #region "ColorLinearGradient Class"

    public class ColorLinearGradient
    {

        public ColorLinearGradient()
        {
            _ColorA = Color.Blue;
            _ColorB = Color.Black;
        }
        public ColorLinearGradient(Color ColorA, Color ColorB)
        {
            _ColorA = ColorA;
            _ColorB = ColorB;
        }

        private Color _ColorA = Color.Blue;
        public Color ColorA
        {
            get { return _ColorA; }
            set { _ColorA = value; }
        }

        private Color _ColorB = Color.Black;
        public Color ColorB
        {
            get { return _ColorB; }
            set { _ColorB = value; }
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", getColorString(_ColorA), getColorString(_ColorB));
        }

        private string getColorString(Color scolor)
        {
            if (scolor.IsNamedColor)
            {
                return scolor.Name;
            }
            else
            {
                return string.Format("{0},{1},{2},{3}", scolor.A, scolor.R, scolor.G, scolor.B);
            }
        }

        public override bool Equals(object obj)
        {
            return this.ToString() == ((ColorLinearGradient)obj).ToString();
        }

    }

    #endregion

    #region "ColorLinearGradientConverter"

    internal class ColorLinearGradientConverter : ExpandableObjectConverter
    {

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            ColorLinearGradient Item = new ColorLinearGradient();
            Item.ColorA = (Color)propertyValues["ColorA"];
            Item.ColorB = (Color)propertyValues["ColorB"];
            return Item;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if ((object.ReferenceEquals(sourceType, typeof(string))))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);

        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {


            if (value is string)
            {
                try
                {
                    List<Color> bColors = new List<Color>();

                    foreach (string cstring in Convert.ToString(value).Split(';'))
                    {
                        bColors.Add((Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(cstring));
                    }

                    if ((bColors != null) && bColors.Count != 2)
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        return new ColorLinearGradient(bColors[0], bColors[1]);
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("Can not convert '{0}' to type ColorLinearGradient", Convert.ToString(value)));
                }

            }
            else
            {
                return new ColorLinearGradient();
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {

            if ((object.ReferenceEquals(destinationType, typeof(string)) && value is ColorLinearGradient))
            {
                return ((ColorLinearGradient)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);

        }

    }

    #endregion

    #region "ColorLinearGradientEditor"

    public class ColorLinearGradientEditor : UITypeEditor
    {

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {

            return true;

        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            // Erase the area.
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            ColorLinearGradient cLinearGradient = null;
            if ((e.Context == null))
            {
                cLinearGradient = new ColorLinearGradient();
            }
            else
            {
                cLinearGradient = (ColorLinearGradient)e.Value;
            }
            // Draw the sample.
            using (Pen border_pen = new Pen(Color.Black, 1))
            {
                using (LinearGradientBrush br = new LinearGradientBrush(e.Bounds, cLinearGradient.ColorA, cLinearGradient.ColorB, LinearGradientMode.Horizontal))
                {

                    e.Graphics.FillRectangle(br, e.Bounds);

                }

                e.Graphics.DrawRectangle(border_pen, 1, 1, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

        }
    }

    #endregion

    #endregion

    //=======================================================
    //Service provided by Telerik (www.telerik.com)
    //Conversion powered by NRefactory.
    //Twitter: @telerik
    //Facebook: facebook.com/telerik
    //=======================================================

}
