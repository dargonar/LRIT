using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.SqlClient;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Clase de ayuda para regenerar la base de datos del datacenter
  /// </summary>
  public class DBCreate
  {
    /// <summary>
    /// Esta funcion regenera la base de datos del data center
    /// en funcion de la informacion en el DBML y las tablas 
    /// </summary>
    static public void Run(string connectionString)
    {
      try
      {
        DataCenterDataAccess.DBDataContext db = new DataCenterDataAccess.DBDataContext(connectionString);
        db.DeleteDatabase();
        db.CreateDatabase();
        
        SqlConnection conn = new SqlConnection(connectionString);
        conn.Open();

        /*drop "views"*/
        string query = @"DROP TABLE DayStats;";
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        
        query = @"DROP TABLE WeekStats;";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        
        query = @"DROP TABLE MonthStats;";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();

        query = @"DROP TABLE DDPData;";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();

        query = @"DROP TABLE LRIT_SIGO;";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();

        query = @"DROP TABLE LRIT_SIGO_SO;";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        /*-----------------------------------*/

        query = @"INSERT INTO ASP (Name, ASPID, CSPID ) VALUES (0,0,0);";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        
        
        
        query = @"INSERT INTO Configuration (DataCenterID,DDPVerision,SchemaVersion,ASPId) VALUES (6666,0,1.0,1);";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();

        query = @"INSERT INTO MessageId (nextId) VALUES (2);";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        

        query = @"INSERT INTO Ship (AspId, EquipID, IMONum, MMSINum, Name, DNID, Member, Mobile, EstadoAsp, EstadoAspFecha) VALUES (1,0,1,1,'San Julian',1477,5,470181671,'Ok',GETUTCDATE());";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        query = @"INSERT INTO Ship (AspId, EquipID, IMONum, MMSINum, Name, DNID, Member, Mobile, EstadoAsp, EstadoAspFecha) VALUES (1,0,2,2,'San Matias I',1477,7,470105111,'Ok',GETUTCDATE());";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        query = @"INSERT INTO Ship (AspId, EquipID, IMONum, MMSINum, Name, DNID, Member, Mobile, EstadoAsp, EstadoAspFecha) VALUES (1,0,3,3,'Argenmar Austral',1477,8,470100189,'Ok',GETUTCDATE());";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        query = @"INSERT INTO Ship (AspId, EquipID, IMONum, MMSINum, Name, DNID, Member, Mobile, EstadoAsp, EstadoAspFecha) VALUES (1,0,4,4,'Recoleta',1477,9,470181547,'Ok',GETUTCDATE());";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        query = @"INSERT INTO Ship (AspId, EquipID, IMONum, MMSINum, Name, DNID, Member, Mobile, EstadoAsp, EstadoAspFecha) VALUES (1,0,5,5,'Caleta Rosario',1477,10,470100144,'Ok',GETUTCDATE());";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        query = @"INSERT INTO Ship (AspId, EquipID, IMONum, MMSINum, Name, DNID, Member, Mobile, EstadoAsp, EstadoAspFecha) VALUES (1,0,6,6,'Ona Tridente',1477,6,470100161,'Ok',GETUTCDATE());";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        query = @"INSERT INTO Ship (AspId, EquipID, IMONum, MMSINum, Name, DNID, Member, Mobile, EstadoAsp, EstadoAspFecha) VALUES (1,0,7,7,'SinNombre',1477,3,000000000,'Ok',GETUTCDATE());";
        cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();

     

        conn.Close();
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.ToString());
        throw ex;
      }
    }
  }
}
