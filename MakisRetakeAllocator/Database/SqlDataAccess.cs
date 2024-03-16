using Dapper;
using MakisRetakeAllocator.Configs;
using MySqlConnector;
using System.Data;
using System.Reflection;

namespace MakisRetakeAllocator.Database;

public class SqlDataAccess {
    private readonly DatabaseConfig theConfig;
    private readonly Assembly theAssembly;

    public SqlDataAccess(DatabaseConfig aConfig) {
        theConfig = aConfig;
        theAssembly = Assembly.GetExecutingAssembly();
    }

    //TODO add checks and Errors

    public async Task<T> loadDataAsync<T, U, V, W>(string aSql, U aParameters, Func<IDataReader, V, W, T> aMapper, V aTeam, W aRoundType) {
        using (MySqlConnection myConnection = createConnection()) {
            IDataReader myRows = await myConnection.ExecuteReaderAsync(aSql, aParameters);
            return aMapper(myRows, aTeam, aRoundType);
        }
    }

    public async Task saveDataAsync<T>(string aSql, T aParameters) {
        MySqlConnection myConnection = createConnection();
        await myConnection.ExecuteAsync(aSql, aParameters);
    }

    public async Task executeSql(string aSql) {
        MySqlConnection myConnection = createConnection();
        await myConnection.ExecuteAsync(aSql);
    }

    public string readEmbeddedSqlProcedure(string aResourceName) {
        string myResourceName = theAssembly.GetManifestResourceNames().SingleOrDefault(r => r.EndsWith(aResourceName));

        if (myResourceName == null) {
            throw new InvalidOperationException($"Failed to find embedded resource matching '{aResourceName}'.");
        }

        using (Stream myStream = theAssembly.GetManifestResourceStream(myResourceName)) {
            using (StreamReader myReader = new StreamReader(myStream)) {
                return myReader.ReadToEnd();
            }
        }
    }

    private MySqlConnection createConnection() {
        String myConnectionString = $"Server={theConfig.theServer}; Database={theConfig.theDatabase}; Uid={theConfig.theUserId}; Pwd={theConfig.thePassword}";
        return new MySqlConnection(myConnectionString);
    }
}