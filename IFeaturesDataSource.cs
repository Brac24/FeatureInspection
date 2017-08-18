﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    public interface IFeaturesDataSource
    {
        //Job JobInfo { get; }

        IList<Feature> GetFeaturesOnOpKey(int opKey);



    }
}
