using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feature_Inspection
{
    public class FeatureCreationModelMock : IFeaturesDataSource
    {

        public FeatureCreationModelMock()
        {

        }


        public IList<Feature> GetFeaturesOnOpKey(int opKey)
        {
            throw new NotImplementedException();
        }
    }
}
