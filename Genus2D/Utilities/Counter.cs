namespace Genus2D.Utililities
{
    /// <summary>
    /// Given a min/max and step, counts between them wrapping.
    /// </summary>
    /// <typeparam name="T">Minimum and maxmimum type (also returned type</typeparam>
    /// <typeparam name="Q"></typeparam>
    public class Counter
    {
        private double _minimum;
        private double _maximum;
        private double _step;
        private double _value;

        public Counter(double pStarting, double pMinium, double pMaximum, double pStep)
        {
            _minimum = pMinium;
            _maximum = pMaximum;
            _step = pStep;
            _value = pStarting;
        }

        public double GetValue()
        {
            return _value;
        }

        public void Increment()
        {
            double result = _value + _step;

            if (result > _maximum)
            {
                result = _maximum;
            }

            _value = result;
        }

        public void Decrement()
        {
            double result = _value - _step;

            if (result < _minimum)
            {
                result = _minimum;
            }
            _value = result;
        }
    }
}