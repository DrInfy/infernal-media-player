namespace Imp.Controls
{
    /// <summary>
    /// Button class that allows button to be pushed down
    /// </summary>
    public class PushDownButton : ImpButton
    {
        #region Properties

        public override int CheckStates
        {
            get { return 2; }
            set { sCheckStates = 2; }
        }

        #endregion

        public PushDownButton()
        {
            sContent = new object[1];
        }

        /// <summary>
        /// Always returns first content
        /// </summary>
        /// <returns></returns>
        protected override object GetCurrentContent()
        {
            return sContent[0];
        }

        protected override double SetupTranslation(double x, ref double y)
        {
            if (CurrentState != 1) return base.SetupTranslation(x, ref y);

            x = sStyle.PressedTranslation.X;
            y = sStyle.PressedTranslation.Y;

            return x;
        }

        protected override void AdjustRenderColors()
        {
            base.AdjustRenderColors();

            if (CurrentState != 1) return;
            if (!IsEnabled) { return; }

            if (Pressed & MouseOver)
            {
                renderData.BackBrush = sStyle.BackPressedBrush;
                renderData.BorderBrush = sStyle.BorderPressedBrush;
                renderData.FrontBrush = sStyle.PressedBrush;
                renderData.SetTranslate(sStyle.PressedTranslation);
            }
            else if (MouseOver | Pressed)
            {
                renderData.BackBrush = sStyle.BackPressedBrush;
                renderData.BorderBrush = sStyle.BorderPressedBrush;
                renderData.FrontBrush = sStyle.PressedBrush;
                renderData.SetTranslate(sStyle.PressedTranslation);
            }
            else
            {
                renderData.BackBrush = sStyle.BackPressedBrush;
                renderData.BorderBrush = sStyle.BorderPressedBrush;
                renderData.FrontBrush = sStyle.PressedBrush;
                renderData.SetTranslate(sStyle.PressedTranslation);
            }
        }
    }
}