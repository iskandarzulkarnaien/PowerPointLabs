using Microsoft.VisualStudio.TestTools.UnitTesting;

using Test.Util;

namespace Test.FunctionalTest
{
    [TestClass]
    public class AutoZoomTest : BaseFunctionalTest
    {
        private const int DrillDownActualStartSlideNo = 4;
        private const int DrillDownActualTransitionSlideNo = 5;
        private const int DrillDownActualEndSlideNo = 6;
        private const int DrillDownExpectedSlideNo = 7;
        private const int DrillDownExpectedTransitionSlideNo = 8;
        private const int DrillDownExpectedEndSlideNo = 9;

        private const int DrillDownBackgroundActualStartSlideNo = 10;
        private const int DrillDownBackgroundActualTransitionSlideNo = 11;
        private const int DrillDownBackgroundActualEndSlideNo = 12;
        private const int DrillDownBackgroundExpectedSlideNo = 13;
        private const int DrillDownBackgroundExpectedTransitionSlideNo = 14;
        private const int DrillDownBackgroundExpectedEndSlideNo = 15;

        private const int StepBackActualStartSlideNo = 16;
        private const int StepBackActualTransitionSlideNo = 17;
        private const int StepBackActualEndSlideNo = 18;
        private const int StepBackExpectedSlideNo = 20;
        private const int StepBackExpectedTransitionSlideNo = 21;
        private const int StepBackExpectedEndSlideNo = 22;

        private const int StepBackBackgroundActualStartSlideNo = 23;
        private const int StepBackBackgroundActualTransitionSlideNo = 24;
        private const int StepBackBackgroundActualEndSlideNo = 25;
        private const int StepBackBackgroundExpectedSlideNo = 27;
        private const int StepBackBackgroundExpectedTransitionSlideNo = 28;
        private const int StepBackBackgroundExpectedDestinationSlideNo = 29;

        protected override string GetTestingSlideName()
        {
            return "ZoomLab\\AutoZoom.pptx";
        }

        [TestMethod]
        [TestCategory("FT")]
        public void FT_AutoZoomTest()
        {
            // Do tests in reverse order because added slides change slide numbers lower down.
            TestStepBackBackground();
            TestStepBack();
            TestDrillDownBackground();
            TestDrillDown();
            TestDrillDownUnsuccessful();
            TestStepBackUnsuccessful();
        }

        private void TestDrillDown()
        {
            PplFeatures.SetZoomProperties(true, true);

            PpOperations.SelectSlide(DrillDownActualStartSlideNo);
            PpOperations.SelectShape("Drill Down This Shape");
            PplFeatures.DrillDown();

            AssertIsSame(DrillDownActualStartSlideNo, DrillDownExpectedSlideNo);
            AssertIsSame(DrillDownActualTransitionSlideNo, DrillDownExpectedTransitionSlideNo);
            AssertIsSame(DrillDownActualEndSlideNo, DrillDownExpectedEndSlideNo);
        }

        private void TestDrillDownBackground()
        {
            PplFeatures.SetZoomProperties(false, true);

            PpOperations.SelectSlide(DrillDownBackgroundActualStartSlideNo);
            PpOperations.SelectShape("Drill Down This Shape");
            PplFeatures.DrillDown();

            AssertIsSame(DrillDownBackgroundActualStartSlideNo, DrillDownBackgroundExpectedSlideNo);
            AssertIsSame(DrillDownBackgroundActualTransitionSlideNo, DrillDownBackgroundExpectedTransitionSlideNo);
            AssertIsSame(DrillDownBackgroundActualEndSlideNo, DrillDownBackgroundExpectedEndSlideNo);
        }

        private void TestStepBack()
        {
            PplFeatures.SetZoomProperties(true, true);

            PpOperations.SelectSlide(StepBackActualStartSlideNo);
            PpOperations.SelectShape("Step Back This Shape");
            PplFeatures.StepBack();

            AssertIsSame(StepBackActualStartSlideNo, StepBackExpectedSlideNo);
            AssertIsSame(StepBackActualTransitionSlideNo, StepBackExpectedTransitionSlideNo);
            AssertIsSame(StepBackActualEndSlideNo, StepBackExpectedEndSlideNo);
        }

        private void TestStepBackBackground()
        {
            PplFeatures.SetZoomProperties(false, true);

            PpOperations.SelectSlide(StepBackBackgroundActualStartSlideNo);
            PpOperations.SelectShape("Step Back This Shape");
            PplFeatures.StepBack();

            AssertIsSame(StepBackBackgroundActualStartSlideNo, StepBackBackgroundExpectedSlideNo);
            AssertIsSame(StepBackBackgroundActualTransitionSlideNo, StepBackBackgroundExpectedTransitionSlideNo);
            AssertIsSame(StepBackBackgroundActualEndSlideNo, StepBackBackgroundExpectedEndSlideNo);
        }

        private void TestDrillDownUnsuccessful()
        {
            Microsoft.Office.Interop.PowerPoint.Slide slide = PpOperations.SelectSlide(34);
            slide.MoveTo(35);
            PpOperations.SelectShape("Zoom This Shape");
            MessageBoxUtil.ExpectMessageBoxWillPopUp(
                "Unable to Add Animations",
                "No next slide is found. Please select the correct slide.",
                PplFeatures.DrillDown);
        }

        private void TestStepBackUnsuccessful()
        {
            Microsoft.Office.Interop.PowerPoint.Slide slide = PpOperations.SelectSlide(35);
            slide.MoveTo(1);
            PpOperations.SelectShape("Zoom This Shape");
            MessageBoxUtil.ExpectMessageBoxWillPopUp(
                "Unable to Add Animations",
                "No previous slide is found. Please select the correct slide.",
                PplFeatures.StepBack);
        }


        private void AssertIsSame(int actualSlideIndex, int expectedSlideIndex)
        {
            Microsoft.Office.Interop.PowerPoint.Slide actualSlide = PpOperations.SelectSlide(actualSlideIndex);
            Microsoft.Office.Interop.PowerPoint.Slide expectedSlide = PpOperations.SelectSlide(expectedSlideIndex);
            SlideUtil.IsSameLooking(expectedSlide, actualSlide);
            SlideUtil.IsSameAnimations(expectedSlide, actualSlide);
        }
    }
}
