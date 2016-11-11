using System;
using System.Collections.Generic;
using TestingDotNet.DAL;

namespace TestingDotNet
{
    public class MyAsset
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public DateTime LastChanged { get; set; }
        public string Owner { get; set; }
        public int UserCount { get; set; }
        public double AverageReviewScore { get; set; }
        public NewType NewType { get; set; }
    }
}