using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BCJobClockLib
{
    public abstract class BaseDataExporter
    {
        //interface supported by classes that can be used to export data from the database.

        /// <summary>
        /// retrieves the Name of this DataExporter implementation.
        /// </summary>
        /// <returns> Name of this data Exporter.</returns>
        public abstract String getName();
        /// <summary>
        /// returns a description of this data exporter.
        /// </summary>
        /// <returns></returns>
        public abstract String getDescription();
        /// <summary>
        /// whether this  implementation has a Configuration page that should be called before executing.
        /// </summary>
        /// <returns></returns>
        public abstract bool hasConfigPage();
        public abstract void Configure(IWin32Window ownerwindow);

        public abstract void PerformExport(DataLayer dbObject);




    }
}
