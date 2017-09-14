using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Feature_Inspection
{
    public interface IFeaturesDataSource
    {
        //Job JobInfo { get; }


        //Will bind data directly to view. No need to go through presenter
        DataTable GetFeaturesOnOpKey(string partnumber, string operationNum);

        DataTable AdapterUpdate(DataTable table);
        DataTable GetFeaturesOnPartIndex(int pieceID, int opKey);
    }
}
