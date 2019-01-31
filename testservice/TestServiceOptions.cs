using System;

namespace testservice
{
    public class TestServiceOptions
    {
        public string Name { get; set; }

        public TimeSpan Interval { get; set; }
        public int DieCount { get; set; }
    }
}
