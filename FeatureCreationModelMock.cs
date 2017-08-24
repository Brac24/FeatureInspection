using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Feature_Inspection
{
    public class FeatureCreationModelMock : IFeaturesDataSource
    {
        private readonly string connection_string = "DSN=unipointDB;UID=jbread;PWD=Cloudy2Day";

        public FeatureCreationModelMock()
        {

        }


        public DataTable GetFeaturesOnOpKey(int opKey)
        {
            DataTable t;
            OdbcDataAdapter dataAdapter;
            using (OdbcConnection conn = new OdbcConnection(connection_string))
            using (OdbcCommand com = conn.CreateCommand())
            using (OdbcDataAdapter adapter = new OdbcDataAdapter(com))
            {
                string query = "SELECT * FROM ATI_FeatureInspection.dbo.Features WHERE Part_Number_FK = (SELECT Part_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key =  " + opKey + ") AND Operation_Number_FK = (SELECT Operation_Number FROM ATI_FeatureInspection.dbo.Operation WHERE Op_Key = " + opKey + ");";

                dataAdapter = adapter;

                
                com.CommandText = query;
                t = new DataTable();
                dataAdapter.Fill(t);

            }

            return t;

        }
    }
}
