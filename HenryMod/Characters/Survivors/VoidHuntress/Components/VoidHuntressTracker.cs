using RoR2;

namespace Henry2Mod.Characters.Survivors.VoidHuntress.Components
{
    public class VoidHuntressTracker : HuntressTracker
    {
        public float customMaxTrackingDistance = 35f; // Set your desired max tracking distance here
        public float customMaxTrackingAngle = 20f;
        public float customTrackerUpdateFrequency = 10f;
        protected Indicator indicatorRef;

        // Override the Start method to change the maxTrackingDistance value
        private new void Start()
        {
            base.Start(); // Call the base class Start method
            indicatorRef = indicator;
            maxTrackingDistance = customMaxTrackingDistance; // Override the maxTrackingDistance
            maxTrackingAngle = customMaxTrackingAngle;
            trackerUpdateFrequency = customTrackerUpdateFrequency;

        }
    }
}
