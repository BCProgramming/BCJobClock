using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCJobClockLib
{
    public class DateRange : ICloneable
    {

        private DateTime _StartTime, _EndTime;

        public DateTime StartTime { get { return _StartTime; } }
        public DateTime EndTime { get { return _EndTime; } }
      
       
        public TimeSpan Span
        {
            get
            {

                return EndTime - StartTime;
            }
            set
            {
                _EndTime = StartTime + value;

            }

        }
        public List<DateRange> SplitToDays()
        {
            DateTime current = StartTime;
            DateTime useend;
            List<DateRange> result = new List<DateRange>();
            while (current < EndTime)
            {

                useend = current.Add(new TimeSpan(23, 59, 59));
                result.Add(new DateRange( current, useend ));

                current = current.AddDays(1);
            }

            return result;





        }

        public DateRange(DateTime sTime, DateTime eTime)
        {
            if (sTime < eTime) _StartTime = sTime; else _StartTime = eTime;
            if (sTime > eTime) _EndTime = sTime; else _EndTime = eTime;


        }
        public bool Contains(DateTime testdate)
        {

            return (testdate > StartTime && testdate < EndTime);

        }

        public override string ToString()
        {
            return "Range:(" + StartTime.ToString() + ")-(" + EndTime.ToString() + ")" + " (Span:" + Span.ToString() + ")";
        }
        public static DateRange[] Coalesce(DateRange RangeA, DateRange RangeB)
        {
            bool CondA = RangeA.StartTime > RangeB.EndTime;
            bool CondB = RangeA.EndTime < RangeB.StartTime;
            bool overlaps = !(CondA || CondB);


            // bool overlaps = (RangeA.StartTime > RangeB.EndTime) && (RangeA.EndTime < RangeB.StartTime);

            //overlap exists if neither A or B is true.
            if (overlaps)
            {
                //overlap exists.
                //create a new starttime the lower of the given two...
                DateTime newstart, newEnd;

                if (RangeA.StartTime < RangeB.StartTime) newstart = RangeA.StartTime; else newstart = RangeB.StartTime;
                if (RangeA.EndTime > RangeB.EndTime) newEnd = RangeA.EndTime; else newEnd = RangeB.EndTime;

                return new DateRange[] { new DateRange(newstart, newEnd) };

            }
            else
            {
                //there is no overlap, return a daterange array with both the same elements.
                return new DateRange[] { (DateRange)RangeA.Clone(), (DateRange)RangeB.Clone() };

            }


        }

        public DateRange[] Coalesce(DateRange Otherdaterange)
        {

            return DateRange.Coalesce(this, Otherdaterange);

        }




        public static DateRange[] CoalesceRanges(DateRange[] ranges)
        {
            //coalesces/simplifies a given set of date ranges to the simplest form. For example:
            /*   a
             * |----|
             *     b
             *   |----|       c
             *              |----|
             * */
            //will return an array of two Dateranges:

            /*    a
             * |-----|
             *               b
             *             |----|
              
             * */
            //this is the "simplest" reduction of what the previous ranges were.
            List<DateRange> userange = ranges.ToList();
            //userange.Sort((w,p)=>w.StartTime.CompareTo(p.StartTime));
            List<DateRange> workalist = userange;
            List<DateRange> result = new List<DateRange>();


            //algorithm
            //set a flag indicating a coalesce was discovered to true.
            //iterate while said flag is true.

            //set flag to false as loop begins.
            //iterate through every possible pair X and Y in the List. skip iterations where X==Y.

            //for each possible pairing:
            //coalesce the two DateRanges. If the results coalesce (the return value array has a length of 1)
            //mark x and y for removal, set the new Array to be AddRanged() to the List, and break out of both loops, and set the flag saying that
            //a coalesce was discovered.

            bool coalescefound = true;
            while (coalescefound)
            {
                List<DateRange> removalmarked = new List<DateRange>();
                List<DateRange> addmarked = new List<DateRange>();
                bool breakouter = false;
                coalescefound = false;
                foreach (DateRange x in workalist)
                {
                    foreach (DateRange y in workalist)
                    {
                        if (x != y)
                        {
                            DateRange[] coalresult = DateRange.Coalesce(x, y);

                            if (coalresult.Length == 1)
                            {
                                coalescefound = true;
                                //add new array to be added...
                                addmarked.AddRange(coalresult);
                                //remove both x and y. We can't remove them here since we are iterating...
                                removalmarked.Add(x);
                                removalmarked.Add(y);
                                breakouter = true;
                                break;

                            }



                        }


                    }

                    if (breakouter) break; //break out of outer iterator if flag set.

                }
                //add and remove the marked items.
                foreach (var removeme in removalmarked)
                {

                    workalist.Remove(removeme);

                }
                foreach (var addme in addmarked)
                {
                    workalist.Add(addme);

                }





            }



            return workalist.ToArray();
















        }

        public object Clone()
        {
            return new DateRange(StartTime, EndTime);
        }
    }
}
