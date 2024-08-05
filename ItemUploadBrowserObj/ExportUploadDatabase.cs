using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Data.SqlClient;

// need to install SSIS on the app server to get the 4 required dlls into the gac
//using Microsoft.SqlServer.Management.IntegrationServices;
//using System.Collections.ObjectModel;
using VA.NAC.Services.NACCMExportUploadSharedObj;


namespace VA.NAC.ItemExportUploadBrowser
{
    [Serializable]
    public class ExportUploadDatabase : DBCommon
    {
        public const string ModificationStatusTableName = "ItemExportUploadActivityTable";
        public const string ExportUploadRequestStatusTableName = "ExportUploadStatusRequestTable";

        // these must match the access point description field in the NACSEC database
        public const string MedSurgItemUploadAccessPointDescription = "MedSurgItemUpload";
        public const string PharmItemUploadAccessPointDescription = "PharmItemUpload";
        public const string MedSurgItemExportAccessPointDescription = "MedSurgItemExport";
        public const string PharmItemExportAccessPointDescription = "PharmItemExport";


        public ExportUploadDatabase( ref UserInfo userInfo )
            : base( ref userInfo )
        {
        }
 
        // version used by separate http handler which doesn't have access to session variables
        public ExportUploadDatabase()
            : base()
        {

        }

        // old way
        //public void InitCurrentModification()
        //{
        //    _guidForCurrentModification = Guid.NewGuid();
        //}


        // Connection to NACSEC; SP is general purpose security proc stored under NACCMBrowserStoredProcedures
        public bool GetUserInfo( string userName, ref Guid userGuid, ref string firstName, ref string lastName, ref string fullName, ref string email, ref string phone, ref string status, ref int oldUserId, ref int userDivision )
        {
            bool bSuccess = true;
            SqlConnection dbConnection = null;
            SqlDataAdapter daUserInfo = null;

            try
            {
                SetConnectionString( TargetDatabases.Security );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                // set up the call to the stored procedure
                //GetUserInfo2
                //(
                //@LoginId nvarchar(120)
                //select UserId as 'UserGuid',
                //FirstName, LastName, FullName, UserName as 'LoginId', User_Email as 'Email', User_Phone as 'Phone', 
                //case Inactive when 1 then 'INACTIVE' else 'ACTIVE' end as 'Status', CO_ID as 'oldUserId', Division

                SqlCommand cmdSelectUser = new SqlCommand( "GetUserInfo2", dbConnection );
                cmdSelectUser.CommandType = CommandType.StoredProcedure;
                cmdSelectUser.CommandTimeout = 30;

                SqlParameter loginIdParm = new SqlParameter( "@LoginId", SqlDbType.NVarChar, 120 );
                loginIdParm.Direction = ParameterDirection.Input;
                loginIdParm.Value = userName;
                cmdSelectUser.Parameters.Add( loginIdParm );

                // create a data adapter and dataset to 
                // run the query and hold the results
                daUserInfo = new SqlDataAdapter();
                daUserInfo.SelectCommand = cmdSelectUser;

                DataSet dsUsers = new DataSet( "Users" );
                DataTable dtUsers = dsUsers.Tables.Add( "UserTable" );

                DataColumn userGuidColumn = new DataColumn( "UserGuid", typeof( Guid ) );

                dtUsers.Columns.Add( userGuidColumn );
                dtUsers.Columns.Add( "FirstName", typeof( string ) );
                dtUsers.Columns.Add( "LastName", typeof( string ) );
                dtUsers.Columns.Add( "FullName", typeof( string ) );
                dtUsers.Columns.Add( "LoginId", typeof( string ) ); // with domain
                dtUsers.Columns.Add( "Email", typeof( string ) );
                dtUsers.Columns.Add( "Phone", typeof( string ) );
                dtUsers.Columns.Add( "Status", typeof( string ) );
                dtUsers.Columns.Add( "OldUserId", typeof( int ) );
                dtUsers.Columns.Add( "Division", typeof( int ) );

                // create array of primary key columns
                DataColumn[] primaryKeyColumns = new DataColumn[ 1 ];
                primaryKeyColumns[ 0 ] = userGuidColumn;

                // add the keys to the table
                dtUsers.PrimaryKey = primaryKeyColumns;

                dtUsers.Clear();

                // connect
                dbConnection.Open();

                // run
                daUserInfo.Fill( dsUsers, "UserTable" );

                userGuid = Guid.Empty;
                firstName = "";
                lastName = "";
                fullName = "";
                email = "";
                phone = "";
                status = "";
                oldUserId = -1;
                userDivision = -1;

                int tableCount = dsUsers.Tables.Count;
                int rowCount;
                DataRow userInfoRow = null;

                // user had > 1 entry, so the second result set contains the active instance of the user
                if( tableCount > 1 )
                {
                    rowCount = dsUsers.Tables[ "UserTable1" ].Rows.Count;

                    if( rowCount == 1 )
                    {
                        userInfoRow = dsUsers.Tables[ "UserTable1" ].Rows[ 0 ];
                    }
                }
                else // use the first and only result set
                {
                    rowCount = dsUsers.Tables[ "UserTable" ].Rows.Count;

                    if( rowCount == 1 )
                    {
                        userInfoRow = dsUsers.Tables[ "UserTable" ].Rows[ 0 ];
                    }
                }

                if( userInfoRow != null )
                {
                    userGuid = ( Guid )userInfoRow[ "UserGuid" ];
                    firstName = ( string )userInfoRow[ "FirstName" ];
                    lastName = ( string )userInfoRow[ "LastName" ];
                    fullName = ( string )userInfoRow[ "FullName" ];
                    email = ( string )userInfoRow[ "Email" ];
                    phone = ( string )userInfoRow[ "Phone" ];
                    status = ( string )userInfoRow[ "Status" ];
                    oldUserId = int.Parse( userInfoRow[ "OldUserId" ].ToString() );
                    userDivision = int.Parse( userInfoRow[ "Division" ].ToString() );
                }

                // throws if not found
            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ItemDatabase.GetUserInfo(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }

        // Connection to NACSEC; SP is general purpose security proc stored under NACCMBrowserStoredProcedures
        // this version takes a guid as the lookup parm
        public bool GetUserInfo( Guid userGuid, ref string userName, ref string firstName, ref string lastName, ref string fullName, ref string email, ref string phone, ref string status, ref int oldUserId, ref int userDivision )
        {
            bool bSuccess = true;
            SqlConnection dbConnection = null;
            SqlDataAdapter daUserInfo = null;

            try
            {
                SetConnectionString( TargetDatabases.Security );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                // set up the call to the stored procedure
                //GetUserInfo3
                //(
                //@UserId uniqueidentifier
                //
                //select UserName as 'LoginId',
                //FirstName, LastName, FullName, UserId, User_Email as 'Email', User_Phone as 'Phone', 
                //case Inactive when 1 then 'INACTIVE' else 'ACTIVE' end as 'status', CO_ID as 'oldUserId', Division
                //from SEC_UserProfile
                //where UserId = @UserId	

                SqlCommand cmdSelectUserByGuid = new SqlCommand( "GetUserInfo3", dbConnection );
                cmdSelectUserByGuid.CommandType = CommandType.StoredProcedure;
                cmdSelectUserByGuid.CommandTimeout = 30;

                SqlParameter userIdParm = new SqlParameter( "@UserId", SqlDbType.UniqueIdentifier );
                userIdParm.Direction = ParameterDirection.Input;
                userIdParm.Value = userGuid;
                cmdSelectUserByGuid.Parameters.Add( userIdParm );

                // create a data adapter and dataset to 
                // run the query and hold the results
                daUserInfo = new SqlDataAdapter();
                daUserInfo.SelectCommand = cmdSelectUserByGuid;

                DataSet dsUsers = new DataSet( "Users" );
                DataTable dtUsers = dsUsers.Tables.Add( "UserTable" );

                DataColumn userGuidColumn = new DataColumn( "UserId", typeof( Guid ) );

                dtUsers.Columns.Add( userGuidColumn );
                dtUsers.Columns.Add( "FirstName", typeof( string ) );
                dtUsers.Columns.Add( "LastName", typeof( string ) );
                dtUsers.Columns.Add( "FullName", typeof( string ) );
                dtUsers.Columns.Add( "LoginId", typeof( string ) ); // with domain
                dtUsers.Columns.Add( "Email", typeof( string ) );
                dtUsers.Columns.Add( "Phone", typeof( string ) );
                dtUsers.Columns.Add( "Status", typeof( string ) );
                dtUsers.Columns.Add( "OldUserId", typeof( int ) );
                dtUsers.Columns.Add( "Division", typeof( int ) );

                // create array of primary key columns
                DataColumn[] primaryKeyColumns = new DataColumn[ 1 ];
                primaryKeyColumns[ 0 ] = userGuidColumn;

                // add the keys to the table
                dtUsers.PrimaryKey = primaryKeyColumns;

                dtUsers.Clear();

                // connect
                dbConnection.Open();

                // run
                daUserInfo.Fill( dsUsers, "UserTable" );

                userGuid = Guid.Empty;
                userName = "";
                firstName = "";
                lastName = "";
                fullName = "";
                email = "";
                phone = "";
                status = "";
                oldUserId = -1;
                userDivision = -1;

                int tableCount = dsUsers.Tables.Count;
                int rowCount;
                DataRow userInfoRow = null;

                // user had > 1 entry, so the second result set contains the active instance of the user
                if( tableCount > 1 )
                {
                    rowCount = dsUsers.Tables[ "UserTable1" ].Rows.Count;

                    if( rowCount == 1 )
                    {
                        userInfoRow = dsUsers.Tables[ "UserTable1" ].Rows[ 0 ];
                    }
                }
                else // use the first and only result set
                {
                    rowCount = dsUsers.Tables[ "UserTable" ].Rows.Count;

                    if( rowCount == 1 )
                    {
                        userInfoRow = dsUsers.Tables[ "UserTable" ].Rows[ 0 ];
                    }
                }

                if( userInfoRow != null )
                {
                    userGuid = ( Guid )userInfoRow[ "UserGuid" ];
                    userName = ( string )userInfoRow[ "LoginId" ];
                    firstName = ( string )userInfoRow[ "FirstName" ];
                    lastName = ( string )userInfoRow[ "LastName" ];
                    fullName = ( string )userInfoRow[ "FullName" ];
                    email = ( string )userInfoRow[ "Email" ];
                    phone = ( string )userInfoRow[ "Phone" ];
                    status = ( string )userInfoRow[ "Status" ];
                    oldUserId = int.Parse( userInfoRow[ "OldUserId" ].ToString() );
                    userDivision = int.Parse( userInfoRow[ "Division" ].ToString() );
                }

                // throws if not found
            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ItemDatabase.GetUserInfo(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }

        // connection to NACSEC - new version, the SP is a general security SP stored under NACCMBrowserApplicationStoredProcedures
        public bool AuthorizeUser( int scheduleNumber, ExportUploadGlobals.ExportUploadTypes exportUploadType, ExportUploadGlobals.ActionTypes actionType, ref bool bIsAuthorized )
        {
            bool bSuccess = true;

            SqlConnection dbConnection = null;
            bIsAuthorized = false;

            try
            {
                SetConnectionString( TargetDatabases.Security );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                //AuthorizeUser
                //(
                //@UserId uniqueidentifier,
                //@AccessPointDescription nvarchar(200),
                //@ScheduleNumber int,
                //@IsAuthorized bit OUTPUT

                SqlCommand cmdAuthorizeUserQuery = new SqlCommand( "AuthorizeUser", dbConnection );
                cmdAuthorizeUserQuery.CommandType = CommandType.StoredProcedure;
                cmdAuthorizeUserQuery.CommandTimeout = 30;

                SqlParameter parmUserId = new SqlParameter( "@UserId", SqlDbType.UniqueIdentifier );
                SqlParameter parmAccessPointDescription = new SqlParameter( "@AccessPointDescription", SqlDbType.NVarChar, 200 );
                SqlParameter parmScheduleNumber = new SqlParameter( "@ScheduleNumber", SqlDbType.Int );
                SqlParameter parmIsAuthorized = new SqlParameter( "@IsAuthorized", SqlDbType.Bit );
                parmIsAuthorized.Direction = ParameterDirection.Output;

                cmdAuthorizeUserQuery.Parameters.Add( parmUserId );
                cmdAuthorizeUserQuery.Parameters.Add( parmAccessPointDescription );
                cmdAuthorizeUserQuery.Parameters.Add( parmScheduleNumber );
                cmdAuthorizeUserQuery.Parameters.Add( parmIsAuthorized );

                parmUserId.Value = CurrentUserId;
                parmScheduleNumber.Value = scheduleNumber;

                if( exportUploadType == ExportUploadGlobals.ExportUploadTypes.MedSurg )
                {
                    if( actionType == ExportUploadGlobals.ActionTypes.Export )
                    {
                        parmAccessPointDescription.Value = MedSurgItemExportAccessPointDescription;
                    }
                    else if( actionType == ExportUploadGlobals.ActionTypes.Upload )
                    {
                        parmAccessPointDescription.Value = MedSurgItemUploadAccessPointDescription;
                    }
                    else
                    {
                        throw new Exception( "Unknown access type." );
                    }
                }
                else if( exportUploadType == ExportUploadGlobals.ExportUploadTypes.Pharmaceutical )
                {
                    if( actionType == ExportUploadGlobals.ActionTypes.Export )
                    {
                        parmAccessPointDescription.Value = PharmItemExportAccessPointDescription;
                    }
                    else if( actionType == ExportUploadGlobals.ActionTypes.Upload )
                    {
                        parmAccessPointDescription.Value = PharmItemUploadAccessPointDescription;
                    }
                    else
                    {
                        throw new Exception( "Unknown access type." );
                    }
                }
                else
                {
                    throw new Exception( "Unknown export/upload type." );
                }


                dbConnection.Open();

                // get the count
                cmdAuthorizeUserQuery.ExecuteScalar();

                // get the authorization result
                bIsAuthorized = bool.Parse( ( cmdAuthorizeUserQuery.Parameters[ "@IsAuthorized" ].Value ).ToString() );

            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ItemDatabase.AuthorizeUser(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }


        // check to see if any recent activity has occurred with the current contract
        public bool CheckForPreviousUpload( string contractNumber, DateTime startDate, DateTime endDate, ref DataSet dsModificationStatus, ref int rowCount )
        {
            bool bSuccess = true;
            SqlDataAdapter daModificationStatus = null;
            SqlConnection dbConnection = null;
            dsModificationStatus = null;
            rowCount = 0;

            try
            {

                SetConnectionString( TargetDatabases.ExportUpload );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                //GetItemUploadHistoryForUpload
                //(
                //@UserLogin nvarchar(120),
                //@UserId uniqueidentifier,
                //@ContractNumber nvarchar(20),
                //@StartDate datetime,
                //@EndDate datetime

                //select ActivityId, UserId, CreatedBy, CreationDate, ActivityType, ActivityDataType, ContractNumber, SpreadsheetFileName, ExportUploadStatus
                //from EU_Activity
                //where ContractNumber = @ContractNumber
                //and CreationDate between @StartDate and @EndDate
                //and ActivityType = 'U'

                SqlCommand cmdGetItemUploadHistoryQuery = new SqlCommand( "GetItemUploadHistoryForUpload", dbConnection );
                cmdGetItemUploadHistoryQuery.CommandType = CommandType.StoredProcedure;
                cmdGetItemUploadHistoryQuery.CommandTimeout = 30;

                SqlParameter parmUserName = new SqlParameter( "@UserLogin", SqlDbType.NVarChar, 120 );
                SqlParameter parmUserId = new SqlParameter( "@UserId", SqlDbType.UniqueIdentifier, 16 );
                SqlParameter parmContractNumber = new SqlParameter( "@ContractNumber", SqlDbType.NVarChar, 20 );
                SqlParameter parmStartDate = new SqlParameter( "@StartDate", SqlDbType.DateTime );
                SqlParameter parmEndDate = new SqlParameter( "@EndDate", SqlDbType.DateTime );

                cmdGetItemUploadHistoryQuery.Parameters.Add( parmUserName );
                cmdGetItemUploadHistoryQuery.Parameters.Add( parmUserId );
                cmdGetItemUploadHistoryQuery.Parameters.Add( parmContractNumber );
                cmdGetItemUploadHistoryQuery.Parameters.Add( parmStartDate );
                cmdGetItemUploadHistoryQuery.Parameters.Add( parmEndDate );

                parmUserName.Value = UserName;
                parmUserId.Value = CurrentUserId;
                parmContractNumber.Value = contractNumber;
                parmStartDate.Value = startDate;
                parmEndDate.Value = endDate;

                daModificationStatus = new SqlDataAdapter();
                daModificationStatus.SelectCommand = cmdGetItemUploadHistoryQuery;

                dsModificationStatus = new DataSet( "ModificationStatus" );
                DataTable dtModificationStatus = dsModificationStatus.Tables.Add( ModificationStatusTableName );

                DataColumn activityIdColumn = new DataColumn( "ActivityId", typeof( int ) );

                dtModificationStatus.Columns.Add( activityIdColumn );
                dtModificationStatus.Columns.Add( "UserId", typeof( SqlGuid ) );
                dtModificationStatus.Columns.Add( "CreatedBy", typeof( string ) );
                dtModificationStatus.Columns.Add( "CreationDate", typeof( DateTime ) );
                dtModificationStatus.Columns.Add( "ActivityType", typeof( string ) );
                dtModificationStatus.Columns.Add( "ActivityDataType", typeof( string ) );
                dtModificationStatus.Columns.Add( "ContractNumber", typeof( string ) );
                dtModificationStatus.Columns.Add( "SpreadsheetFileName", typeof( string ) );
                dtModificationStatus.Columns.Add( "ExportUploadStatus", typeof( string ) );
                
                // create array of primary key columns
                DataColumn[] primaryKeyColumns = new DataColumn[ 1 ];
                primaryKeyColumns[ 0 ] = activityIdColumn;

                // add the keys to the table
                dtModificationStatus.PrimaryKey = primaryKeyColumns;

                dtModificationStatus.Clear();

                dbConnection.Open();

                daModificationStatus.Fill( dsModificationStatus, ModificationStatusTableName );
                rowCount = dtModificationStatus.Rows.Count;

            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ExportUploadDatabase.CheckForPreviousUpload(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }

  
        // makes an entry in the EU_Activity table to indicate the modification is in progress
        public bool InitCurrentActivity( ExportUploadGlobals.ActionTypes actionType, ExportUploadGlobals.ExportUploadTypes exportUploadType, string contractNumber, string modificationNumber, string spreadsheetFileName, ref int currentActivityId, ref int currentActivityDetailsId, ref Guid guidForCurrentModification )
        {
            currentActivityId = -1;
            currentActivityDetailsId = -1;

            if( actionType == ExportUploadGlobals.ActionTypes.Upload )
            {
                guidForCurrentModification = Guid.NewGuid();
                base.GuidForCurrentModification = guidForCurrentModification;
            }
            else
            {
                guidForCurrentModification = Guid.Empty;
            }

            bool bSuccess = true;

            SqlConnection dbConnection = null;

            try
            {
                SetConnectionString( TargetDatabases.ExportUpload );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                //stored procedure
                // CreateExportUploadActivity
                //(
                //@UserLogin nvarchar(120),
                //@UserId uniqueidentifier,
                //@ContractNumber nvarchar(20),
                //@ActivityType nchar(1),
                //@ActivityDataType nchar(1),
                //@SpreadsheetFileName nvarchar(255),
                //@ChangeId uniqueidentifier = null,
                //@ModificationNumber nvarchar(20) = null,
                //@ActivityId int OUTPUT,
                //@ActivityDetailsId int OUTPUT

                SqlCommand cmdCreateExportUploadActivityQuery = new SqlCommand( "CreateExportUploadActivity", dbConnection );
                cmdCreateExportUploadActivityQuery.CommandType = CommandType.StoredProcedure;
                cmdCreateExportUploadActivityQuery.CommandTimeout = 30;

                SqlParameter parmUserName = new SqlParameter( "@UserLogin", SqlDbType.NVarChar, 120 );
                SqlParameter parmUserId = new SqlParameter( "@UserId", SqlDbType.UniqueIdentifier, 16 );
                SqlParameter parmContractNumber = new SqlParameter( "@ContractNumber", SqlDbType.NVarChar, 20 );
                SqlParameter parmActivityType = new SqlParameter( "@ActivityType", SqlDbType.NChar, 2 );
                SqlParameter parmActivityDataType = new SqlParameter( "@ActivityDataType", SqlDbType.NChar, 2 );
                SqlParameter parmChangeId = new SqlParameter( "@ChangeId", SqlDbType.UniqueIdentifier, 16 );
                SqlParameter parmModificationNumber = new SqlParameter( "@ModificationNumber", SqlDbType.NVarChar, 20 );
                SqlParameter parmSpreadsheetFileName = new SqlParameter( "@SpreadsheetFileName", SqlDbType.NVarChar, 256 );
                SqlParameter parmActivityId = new SqlParameter( "@ActivityId", SqlDbType.Int, 4 );
                parmActivityId.Direction = ParameterDirection.Output;
                SqlParameter parmActivityDetailsId = new SqlParameter( "@ActivityDetailsId", SqlDbType.Int, 4 );
                parmActivityDetailsId.Direction = ParameterDirection.Output;

                cmdCreateExportUploadActivityQuery.Parameters.Add( parmUserName );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmUserId );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmContractNumber );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmActivityType );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmActivityDataType );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmChangeId );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmModificationNumber );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmSpreadsheetFileName );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmActivityId );
                cmdCreateExportUploadActivityQuery.Parameters.Add( parmActivityDetailsId );

                parmUserName.Value = UserName;
                parmUserId.Value = CurrentUserId;
                parmContractNumber.Value = contractNumber;
                parmActivityType.Value = actionType;
                parmActivityDataType.Value = exportUploadType;
                parmChangeId.Value = GuidForCurrentModification;
                parmModificationNumber.Value = modificationNumber;
                parmSpreadsheetFileName.Value = spreadsheetFileName;

                dbConnection.Open();

                // create the entry
                cmdCreateExportUploadActivityQuery.ExecuteScalar();

                // get the ids
                currentActivityId = int.Parse( ( cmdCreateExportUploadActivityQuery.Parameters[ "@ActivityId" ].Value ).ToString() );
                currentActivityDetailsId = int.Parse( ( cmdCreateExportUploadActivityQuery.Parameters[ "@ActivityDetailsId" ].Value ).ToString() ); 
            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ExportUploadDatabase.InitCurrentActivity(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }


        // updates the status of the current activity
        public bool UpdateExportUploadActivityStatus( int currentActivityId, string status )
        {
            bool bSuccess = true;

            SqlConnection dbConnection = null;

            try
            {
                SetConnectionString( TargetDatabases.ExportUpload );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                // *** this SP stored in shared directory under the service project as it is shared by the gui and the service *//
                //UpdateExportUploadActivityStatus
                //(
                //@ActivityId int,               
                //@ExportUploadStatus nchar(2)
                //)

                SqlCommand cmdUpdateActivityStatusQuery = new SqlCommand( "UpdateExportUploadActivityStatus", dbConnection );
                cmdUpdateActivityStatusQuery.CommandType = CommandType.StoredProcedure;
                cmdUpdateActivityStatusQuery.CommandTimeout = 30;

                SqlParameter parmActivityId = new SqlParameter( "@ActivityId", SqlDbType.Int, 4 );             
                SqlParameter parmExportUploadStatus = new SqlParameter( "@ExportUploadStatus", SqlDbType.NChar, 2 );


                cmdUpdateActivityStatusQuery.Parameters.Add( parmActivityId );            
                cmdUpdateActivityStatusQuery.Parameters.Add( parmExportUploadStatus );

                parmActivityId.Value =  currentActivityId;              
                parmExportUploadStatus.Value = status;

                dbConnection.Open();

                // update the completion status of the modification
                cmdUpdateActivityStatusQuery.ExecuteNonQuery();
            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ExportUploadDatabase.UpdateExportUploadActivityStatus(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }


        // updates the target spreadsheet name of the current activity
        public bool UpdateExportUploadActivityInfo( int currentActivityId, string spreadsheetFileName )
        {
            bool bSuccess = true;

            SqlConnection dbConnection = null;

            try
            {
                SetConnectionString( TargetDatabases.ExportUpload );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                //UpdateExportUploadActivityInfo
                //(
                //@ActivityId int,
                //@SpreadsheetFileName nvarchar(255)

                SqlCommand cmdUpdateActivityInfoQuery = new SqlCommand( "UpdateExportUploadActivityInfo", dbConnection );
                cmdUpdateActivityInfoQuery.CommandType = CommandType.StoredProcedure;
                cmdUpdateActivityInfoQuery.CommandTimeout = 30;

                SqlParameter parmActivityId = new SqlParameter( "@ActivityId", SqlDbType.Int, 4 );
                SqlParameter parmSpreadsheetFileName = new SqlParameter( "@SpreadsheetFileName", SqlDbType.NVarChar, 255 );


                cmdUpdateActivityInfoQuery.Parameters.Add( parmActivityId );
                cmdUpdateActivityInfoQuery.Parameters.Add( parmSpreadsheetFileName );

                parmActivityId.Value =  currentActivityId;
                parmSpreadsheetFileName.Value = spreadsheetFileName;

                dbConnection.Open();

                // update the target spreadsheet of the export upload activity
                cmdUpdateActivityInfoQuery.ExecuteNonQuery();
            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ExportUploadDatabase.UpdateExportUploadActivityInfo(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }


     


        // copied from ExportDB object in NACCM which will be retired.
        //public bool ExportItems( string contractNumber, string destinationPath, ExportUploadGlobals.ExportContents exportContents, int currentActivityId, string startDateString, string endDateString, ref string actualDestinationFilePathAndName )
        //{
        //    bool bSuccess = true;
        //    SqlConnection dbConnection = null;

        //    actualDestinationFilePathAndName = "";

        //    try
        //    {
        //        SetConnectionString( TargetDatabases.ExportUpload );

        //        dbConnection = CreateSqlConnection();
        //        if( dbConnection == null )
        //            return ( false );

        //        // set up the call to the stored procedure
        //        //CreateNACCMPriceListExportSpreadsheet
        //        //(
        //        //@currentUser uniqueidentifier ,
        //        //@ContractNumber nvarchar(20)  ,
        //        //@DestinationPath nvarchar(500),
        //        //@ExportType nchar(1), 
        //        //@ActivityId int,
        //        //@StartDate nvarchar(10),
        //        //@EndDate nvarchar(10),
        //        //@ExportUploadServerName nvarchar(30),
        //        //@MedSurgTemplatePathAndFile nvarchar(512),
        //        //@PharmaceuticalTemplatePathAndFile nvarchar(512),
        //        //@filepath nvarchar(1000) output
        //        //)

        //        SqlCommand cmdCreateSpreadsheetQuery = new SqlCommand( "CreateNACCMPriceListExportSpreadsheet", dbConnection );
        //        cmdCreateSpreadsheetQuery.CommandType = CommandType.StoredProcedure;
        //        cmdCreateSpreadsheetQuery.CommandTimeout = 0; /* refresh for drug item export can take a while */


        //        SqlParameter parmCurrentUser = new SqlParameter( "@CurrentUser", SqlDbType.UniqueIdentifier );
        //        SqlParameter parmContractNumber = new SqlParameter( "@ContractNumber", SqlDbType.NVarChar, 20 );
        //        SqlParameter parmDestinationPath = new SqlParameter( "@DestinationPath", SqlDbType.NVarChar, 500 );
        //        SqlParameter parmExportType = new SqlParameter( "@ExportType", SqlDbType.NVarChar, 1 );
        //        SqlParameter parmActivityId = new SqlParameter( "@ActivityId", SqlDbType.Int );
        //        SqlParameter parmStartDateString = new SqlParameter( "@StartDate", SqlDbType.NVarChar, 10 );
        //        SqlParameter parmEndDateString = new SqlParameter( "@EndDate", SqlDbType.NVarChar, 10 );
        //        SqlParameter parmExportUploadServerName = new SqlParameter( "@ExportUploadServerName", SqlDbType.NVarChar, 30 );
        //        SqlParameter parmMedSurgTemplatePathAndFile = new SqlParameter( "@MedSurgTemplatePathAndFile", SqlDbType.NVarChar, 512 );
        //        SqlParameter parmPharmaceuticalTemplatePathAndFile = new SqlParameter( "@PharmaceuticalTemplatePathAndFile", SqlDbType.NVarChar, 512 );

        //        SqlParameter parmOutputFileAndPath = new SqlParameter( "@filepath", SqlDbType.NVarChar, 1000 );
        //        parmOutputFileAndPath.Direction = ParameterDirection.Output;

        //        parmCurrentUser.Value = CurrentUserId;
        //        parmContractNumber.Value = contractNumber;
        //        parmDestinationPath.Value = destinationPath;
        //        parmExportType.Value = ExportUploadGlobals.GetExportContentsFromEnum( exportContents );
        //        parmActivityId.Value = currentActivityId;
        //        parmStartDateString.Value = startDateString;
        //        parmEndDateString.Value = endDateString;
        //        parmExportUploadServerName.Value = ItemExportUploadConfiguration.ExportUploadDatabaseServerName;
        //        parmMedSurgTemplatePathAndFile.Value = ItemExportUploadConfiguration.MedSurgItemExportTemplatePathAndFile;
        //        parmPharmaceuticalTemplatePathAndFile.Value = ItemExportUploadConfiguration.PharmaceuticalItemExportTemplatePathAndFile;

        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmCurrentUser );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmContractNumber );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmDestinationPath );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmExportType );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmActivityId );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmStartDateString );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmEndDateString );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmExportUploadServerName );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmMedSurgTemplatePathAndFile );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmPharmaceuticalTemplatePathAndFile );
        //        cmdCreateSpreadsheetQuery.Parameters.Add( parmOutputFileAndPath );

        //        // connect
        //        dbConnection.Open();

        //        cmdCreateSpreadsheetQuery.ExecuteNonQuery();

        //        if( cmdCreateSpreadsheetQuery.Parameters[ "@filepath" ].Value != DBNull.Value )
        //        {
        //            actualDestinationFilePathAndName = cmdCreateSpreadsheetQuery.Parameters[ "@filepath" ].Value.ToString();
        //        }
        //    }
        //    catch( Exception ex )
        //    {
        //        ErrorMessage = String.Format( "The following exception was encountered in ExportDB.ExportItems(): {0}", ex.Message );
        //        bSuccess = false;
        //    }
        //    finally
        //    {
        //        if( dbConnection != null )
        //            dbConnection.Close();
        //    }

        //    return ( bSuccess );
        //}

        public bool ExportItems2( string contractNumber, int contractId, int scheduleNumber, ExportUploadGlobals.ExportContents exportContents, string requestingServerName, int currentActivityId, string startDateString, string endDateString, string applicationVersion, ref string destinationFileType, ref string destinationFileName, ref int requestId, ref DateTime creationDate )
        {
            bool bSuccess = true;
            SqlConnection dbConnection = null;

            destinationFileName = "";

            try
            {
                SetConnectionString(TargetDatabases.ExportUpload);

                dbConnection = CreateSqlConnection();
                if (dbConnection == null)
                    return (false);
             
                // set up the call to the stored procedure              
                //CreateNACCMPriceListExportSpreadsheetRequest
                //(
                //@CurrentUser uniqueidentifier,
                //@ContractNumber nvarchar(20),
                //@ContractId int,
                //@ScheduleNumber int,
                //@ExportType nchar(1),  --  'M' or 'P'									              
                //@RequestingServerName nvarchar(40),
                //@ActivityId int,               
                //@StartDate nvarchar(10),
                //@EndDate nvarchar(10),
                //@EncodedCriteria char(20),
                //@ApplictionVersion char(8),
                //@DestFileType nchar(2) output,
                //@DestFileName nvarchar(255) output,
                //@RequestId int output,
                //@CreationDate datetime output



                SqlCommand cmdExportItemsToSpreadsheetQuery = new SqlCommand("CreateNACCMPriceListExportSpreadsheetRequest", dbConnection);
                cmdExportItemsToSpreadsheetQuery.CommandType = CommandType.StoredProcedure;
                cmdExportItemsToSpreadsheetQuery.CommandTimeout = 30; 

                SqlParameter parmCurrentUser = new SqlParameter("@CurrentUser", SqlDbType.UniqueIdentifier);
                SqlParameter parmContractNumber = new SqlParameter("@ContractNumber", SqlDbType.NVarChar, 20);
                SqlParameter parmContractId = new SqlParameter( "@ContractId", SqlDbType.Int );
                SqlParameter parmScheduleNumber = new SqlParameter( "@ScheduleNumber", SqlDbType.Int );                
                SqlParameter parmExportType = new SqlParameter("@ExportType", SqlDbType.NChar, 1);
                SqlParameter parmRequestingServerName = new SqlParameter("@RequestingServerName", SqlDbType.NVarChar, 40);
                SqlParameter parmActivityId = new SqlParameter("@ActivityId", SqlDbType.Int);               
                SqlParameter parmStartDateString = new SqlParameter("@StartDate", SqlDbType.NVarChar, 10);
                SqlParameter parmEndDateString = new SqlParameter("@EndDate", SqlDbType.NVarChar, 10);
                SqlParameter parmEncodedCriteria = new SqlParameter("@EncodedCriteria", SqlDbType.Char, 20);
                SqlParameter parmApplicationVersion = new SqlParameter( "@ApplicationVersion", SqlDbType.Char, 8 );

                SqlParameter parmOutputDestFileType = new SqlParameter( "@DestFileType", SqlDbType.NChar, 2 );
                parmOutputDestFileType.Direction = ParameterDirection.Output;
                
                SqlParameter parmOutputDestFileName = new SqlParameter( "@DestFileName", SqlDbType.NVarChar, 255 );            
                parmOutputDestFileName.Direction = ParameterDirection.Output;
  
                SqlParameter parmOutputRequestId = new SqlParameter( "@RequestId", SqlDbType.Int );
                parmOutputRequestId.Direction = ParameterDirection.Output;

                SqlParameter parmOutputCreationDate = new SqlParameter( "@CreationDate", SqlDbType.DateTime );
                parmOutputCreationDate.Direction = ParameterDirection.Output;

                parmCurrentUser.Value = CurrentUserId;
                parmContractNumber.Value = contractNumber;
                parmContractId.Value = contractId;
                parmScheduleNumber.Value = scheduleNumber;


                string exportType = "";
                string encodedCriteria = "";

                if (exportContents == ExportUploadGlobals.ExportContents.MedSurg)
                {
                    exportType = "M";
                }
                else
                {
                    exportType = "P";

                    encodedCriteria =  ExportUploadGlobals.GetExportContentsFromEnum( exportContents ); // (M), C, B, U
                }
                parmExportType.Value = exportType;
                parmEncodedCriteria.Value = encodedCriteria;
                parmStartDateString.Value = startDateString;
                parmEndDateString.Value = endDateString;

                parmRequestingServerName.Value = requestingServerName;
                parmActivityId.Value = currentActivityId;
                parmApplicationVersion.Value = applicationVersion;

                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmCurrentUser );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmContractNumber );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmContractId );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmScheduleNumber );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmExportType );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmRequestingServerName );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmActivityId );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmStartDateString );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmEndDateString );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmEncodedCriteria );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmApplicationVersion );

                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmOutputDestFileType );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmOutputDestFileName );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmOutputRequestId );
                cmdExportItemsToSpreadsheetQuery.Parameters.Add( parmOutputCreationDate );

                // connect
                dbConnection.Open();

                cmdExportItemsToSpreadsheetQuery.ExecuteNonQuery();

                if( cmdExportItemsToSpreadsheetQuery.Parameters[ "@DestFileType" ].Value != DBNull.Value )
                {
                    destinationFileType = cmdExportItemsToSpreadsheetQuery.Parameters[ "@DestFileType" ].Value.ToString();
                }

                if( cmdExportItemsToSpreadsheetQuery.Parameters[ "@DestFileName" ].Value != DBNull.Value )
                {
                    destinationFileName = cmdExportItemsToSpreadsheetQuery.Parameters[ "@DestFileName" ].Value.ToString();
                }

                if (cmdExportItemsToSpreadsheetQuery.Parameters["@RequestId"].Value != DBNull.Value)
                {
                    requestId = int.Parse( cmdExportItemsToSpreadsheetQuery.Parameters["@RequestId"].Value.ToString() );
                }

                if( cmdExportItemsToSpreadsheetQuery.Parameters[ "@CreationDate" ].Value != DBNull.Value )
                {
                    creationDate = DateTime.Parse( cmdExportItemsToSpreadsheetQuery.Parameters[ "@CreationDate" ].Value.ToString() );
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = String.Format("The following exception was encountered in ExportDB.ExportItems2(): {0}", ex.Message);
                bSuccess = false;
            }
            finally
            {
                if (dbConnection != null)
                    dbConnection.Close();
            }

            return (bSuccess);
        }

        public bool UploadItems2( string contractNumber, int contractId, int scheduleNumber, bool bIsBPA, string uploadType, string sourceFileType, string sourceFileExt, string sourceFileName, string sourceFilePrefix, string sourceFilePathOnly, string sourceFilePathAndName, string requestingServerName, int currentActivityId, int currentActivityDetailsId, Guid guidForCurrentModification, string applicationVersion, ref string potentialErrorFilePathAndName, ref string destinationFileType, ref int requestId, ref DateTime creationDate )
        {
            bool bSuccess = true;
            SqlConnection dbConnection = null;

            potentialErrorFilePathAndName = "";
            destinationFileType = "";
            requestId = -1;
            creationDate = DateTime.Now;
            string encodedCriteria = ( bIsBPA == true ) ? "BPA;" : "";

            try
            {
                SetConnectionString( TargetDatabases.ExportUpload );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                // set up the call to the stored procedure
                //CreateNACCMPriceListUploadSpreadsheetRequest
                //(
                //@CurrentUser uniqueidentifier,
                //@ContractNumber nvarchar(20),
                //@ContractId int,
                //@ScheduleNumber int,
                //@EncodedCriteria char( 20 ),
                //@SourceFileType nchar(2),
                //@SourceFileExt nvarchar(10),
                //@SourceFileName nvarchar(255),
                //@SourceFilePrefix nvarchar(255),
                //@SourceFilePathOnly nvarchar(255),
                //@SourceFilePathAndName nvarchar(255),
                //@UploadType nchar(1),  --  'M' or 'P'
                //@RequestingServerName nvarchar(40),
                //@ActivityId int,
                //@ActivityDetailsId int,
                //@ApplicationVersion char(8),
                //@GuidForCurrentModification uniqueidentifier,
                //@DestFileType nchar(2) output,
                //@PotentialErrorFilePathAndName  nvarchar(255) output,                 
                //@RequestId int output,
                //@CreationDate datetime output


                SqlCommand cmdUploadItemsQuery = new SqlCommand( "CreateNACCMPriceListUploadSpreadsheetRequest", dbConnection );
                cmdUploadItemsQuery.CommandType = CommandType.StoredProcedure;
                cmdUploadItemsQuery.CommandTimeout = 30; 

                SqlParameter parmCurrentUser = new SqlParameter( "@CurrentUser", SqlDbType.UniqueIdentifier );
                SqlParameter parmContractNumber = new SqlParameter( "@ContractNumber", SqlDbType.NVarChar, 20 );
                SqlParameter parmContractId = new SqlParameter( "@ContractId", SqlDbType.Int );
                SqlParameter parmScheduleNumber = new SqlParameter( "@ScheduleNumber", SqlDbType.Int );  
                SqlParameter parmEncodedCriteria = new SqlParameter( "@EncodedCriteria", SqlDbType.Char, 20 );
                SqlParameter parmSourceFileType = new SqlParameter( "@SourceFileType", SqlDbType.NChar, 2 );
                SqlParameter parmSourceFileExt = new SqlParameter( "@SourceFileExt", SqlDbType.NVarChar, 10 );
                SqlParameter parmSourceFileName = new SqlParameter( "@SourceFileName", SqlDbType.NVarChar, 255 );
                SqlParameter parmSourceFilePrefix = new SqlParameter( "@SourceFilePrefix", SqlDbType.NVarChar, 255 );
                SqlParameter parmSourceFilePathOnly = new SqlParameter( "@SourceFilePathOnly", SqlDbType.NVarChar, 255 );
                SqlParameter parmSourceFilePathAndName = new SqlParameter( "@SourceFilePathAndName", SqlDbType.NVarChar, 255 );
                SqlParameter parmUploadType = new SqlParameter( "@UploadType", SqlDbType.NVarChar, 1 );

                SqlParameter parmRequestingServerName = new SqlParameter( "@RequestingServerName", SqlDbType.NVarChar, 40 );
                SqlParameter parmActivityId = new SqlParameter( "@ActivityId", SqlDbType.Int );
                SqlParameter parmActivityDetailsId = new SqlParameter( "@ActivityDetailsId", SqlDbType.Int );
                SqlParameter parmApplicationVersion = new SqlParameter( "@ApplicationVersion", SqlDbType.Char, 8 );
                SqlParameter parmGuidForCurrentModification = new SqlParameter( "@GuidForCurrentModification", SqlDbType.UniqueIdentifier );
                
                SqlParameter parmOutputDestFileType = new SqlParameter( "@DestFileType", SqlDbType.NChar, 2 );
                parmOutputDestFileType.Direction = ParameterDirection.Output;

                SqlParameter parmOutputPotentialErrorFilePathAndName = new SqlParameter( "@PotentialErrorFilePathAndName", SqlDbType.NVarChar, 255 );
                parmOutputPotentialErrorFilePathAndName.Direction = ParameterDirection.Output;

                SqlParameter parmOutputRequestId = new SqlParameter( "@RequestId", SqlDbType.Int );
                parmOutputRequestId.Direction = ParameterDirection.Output;

                SqlParameter parmOutputCreationDate = new SqlParameter( "@CreationDate", SqlDbType.DateTime );
                parmOutputCreationDate.Direction = ParameterDirection.Output;

                parmCurrentUser.Value = CurrentUserId;
                parmContractNumber.Value = contractNumber;
                parmContractId.Value = contractId;
                parmScheduleNumber.Value = scheduleNumber;
                parmEncodedCriteria.Value = encodedCriteria;


                parmSourceFileType.Value = sourceFileType;
                parmSourceFileExt.Value = sourceFileExt;
                parmSourceFileName.Value = sourceFileName;
                parmSourceFilePrefix.Value = sourceFilePrefix;
                parmSourceFilePathOnly.Value = sourceFilePathOnly;
                parmSourceFilePathAndName.Value = sourceFilePathAndName;
                parmUploadType.Value = uploadType;

                parmRequestingServerName.Value = requestingServerName;
                parmActivityId.Value = currentActivityId;
                parmActivityDetailsId.Value = currentActivityDetailsId;
                parmApplicationVersion.Value = applicationVersion;
                parmGuidForCurrentModification.Value = guidForCurrentModification;
                
                cmdUploadItemsQuery.Parameters.Add( parmCurrentUser );
                cmdUploadItemsQuery.Parameters.Add( parmContractNumber );
                cmdUploadItemsQuery.Parameters.Add( parmContractId );
                cmdUploadItemsQuery.Parameters.Add( parmScheduleNumber );
                cmdUploadItemsQuery.Parameters.Add( parmEncodedCriteria );

                cmdUploadItemsQuery.Parameters.Add( parmSourceFileType );
                cmdUploadItemsQuery.Parameters.Add( parmSourceFileExt );
                cmdUploadItemsQuery.Parameters.Add( parmSourceFileName );
                cmdUploadItemsQuery.Parameters.Add( parmSourceFilePrefix );
                cmdUploadItemsQuery.Parameters.Add( parmSourceFilePathOnly );
                cmdUploadItemsQuery.Parameters.Add( parmSourceFilePathAndName );
                cmdUploadItemsQuery.Parameters.Add( parmUploadType );

                cmdUploadItemsQuery.Parameters.Add( parmRequestingServerName );
                cmdUploadItemsQuery.Parameters.Add( parmActivityId );
                cmdUploadItemsQuery.Parameters.Add( parmActivityDetailsId );
                cmdUploadItemsQuery.Parameters.Add( parmApplicationVersion );
                cmdUploadItemsQuery.Parameters.Add( parmGuidForCurrentModification );
                
                cmdUploadItemsQuery.Parameters.Add( parmOutputPotentialErrorFilePathAndName );
                cmdUploadItemsQuery.Parameters.Add( parmOutputDestFileType );
                cmdUploadItemsQuery.Parameters.Add( parmOutputRequestId );
                cmdUploadItemsQuery.Parameters.Add( parmOutputCreationDate );

                // connect
                dbConnection.Open();

                cmdUploadItemsQuery.ExecuteNonQuery();

                if( cmdUploadItemsQuery.Parameters[ "@PotentialErrorFilePathAndName" ].Value != DBNull.Value )
                {
                    potentialErrorFilePathAndName = cmdUploadItemsQuery.Parameters[ "@PotentialErrorFilePathAndName" ].Value.ToString();
                }

                if( cmdUploadItemsQuery.Parameters[ "@DestFileType" ].Value != DBNull.Value )
                {
                    destinationFileType = cmdUploadItemsQuery.Parameters[ "@DestFileType" ].Value.ToString();
                }
            
                if( cmdUploadItemsQuery.Parameters[ "@RequestId" ].Value != DBNull.Value )
                {
                    requestId = int.Parse( cmdUploadItemsQuery.Parameters[ "@RequestId" ].Value.ToString() );
                }

                if( cmdUploadItemsQuery.Parameters[ "@CreationDate" ].Value != DBNull.Value )
                {
                    creationDate = DateTime.Parse( cmdUploadItemsQuery.Parameters[ "@CreationDate" ].Value.ToString() );
                }

            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ExportDB.UploadItems2(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }


        public bool GetExportUploadRequestStatus( int requestId, ref DataSet dsExportUploadRequestStatus, ref int rowCount )
        {
            bool bSuccess = true;
            SqlDataAdapter daExportUploadRequestStatus = null;
            SqlConnection dbConnection = null;
            dsExportUploadRequestStatus = null;
            rowCount = 0;

            try
            {
                SetConnectionString( TargetDatabases.ExportUpload );

                dbConnection = CreateSqlConnection();
                if( dbConnection == null )
                    return ( false );

                // set up the call to the stored procedure
                //GetExportUploadRequestStatus
                //(
                //@RequestId int
                //select r.RequestId, r.RequestStatus, r.RequestingServerName, r.RequestType, r.RequestSubType, r.CreatedBy, r.CreationDate, r.ApplicationVersion, r.ActivityId, r.ActivityDetailsId, 
				//r.SalesExportUploadId, r.SelectedFiscalYear, r.SelectedFiscalQuarter, r.GuidForCurrentModification, r.ContractNumber, r.ContractId, r.ScheduleNumber, 
				//r.StartDateCriteria, r.EndDateCriteria, r.EncodedCriteria, r.SourceFileType, r.SourceFileExt, r.SourceFileName, r.SourceFilePathName, 
				//r.DestFileType, r.DestFileExt, r.DestFileName, r.DestFilePathName, r.StatusMessage, r.CompletionLevel, r.LastModificationDate


                SqlCommand cmdGetExportUploadRequestStatusQuery = new SqlCommand( "GetExportUploadRequestStatus", dbConnection );
                cmdGetExportUploadRequestStatusQuery.CommandType = CommandType.StoredProcedure;
                cmdGetExportUploadRequestStatusQuery.CommandTimeout = 30; 

                SqlParameter parmRequestId = new SqlParameter( "@RequestId", SqlDbType.Int );                         
                parmRequestId.Value = requestId;
                cmdGetExportUploadRequestStatusQuery.Parameters.Add( parmRequestId );

                daExportUploadRequestStatus = new SqlDataAdapter();
                daExportUploadRequestStatus.SelectCommand = cmdGetExportUploadRequestStatusQuery;

                dsExportUploadRequestStatus = new DataSet( "ExportUploadRequestStatus" );
                DataTable dtExportUploadRequestStatus = dsExportUploadRequestStatus.Tables.Add( ExportUploadRequestStatusTableName );

                DataColumn requestIdColumn = new DataColumn( "RequestId", typeof( int ) );

                dtExportUploadRequestStatus.Columns.Add( requestIdColumn );
                dtExportUploadRequestStatus.Columns.Add( "RequestStatus", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "RequestingServerName", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "RequestType", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "RequestSubType", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "CreatedBy", typeof( Guid ) );
                dtExportUploadRequestStatus.Columns.Add( "CreationDate", typeof( DateTime ) );
                dtExportUploadRequestStatus.Columns.Add( "ApplicationVersion", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "ActivityId", typeof( int ) );
                dtExportUploadRequestStatus.Columns.Add( "ActivityDetailsId", typeof( int ) );
                dtExportUploadRequestStatus.Columns.Add( "GuidForCurrentModification", typeof( Guid ) );
                dtExportUploadRequestStatus.Columns.Add( "ContractNumber", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "ContractId", typeof( int ) );
                dtExportUploadRequestStatus.Columns.Add( "ScheduleNumber", typeof( int ) );
                dtExportUploadRequestStatus.Columns.Add( "StartDateCriteria", typeof( DateTime ) );
                dtExportUploadRequestStatus.Columns.Add( "EndDateCriteria", typeof( DateTime ) );
                dtExportUploadRequestStatus.Columns.Add( "EncodedCriteria", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "SourceFileType", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "SourceFileExt", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "SourceFileName", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "SourceFilePathName", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "DestFileType", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "DestFileExt", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "DestFileName", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "DestFilePathName", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "StatusMessage", typeof( string ) );
                dtExportUploadRequestStatus.Columns.Add( "CompletionLevel", typeof( int ) );
                dtExportUploadRequestStatus.Columns.Add( "LastModificationDate", typeof( DateTime ) );

                // create array of primary key columns
                DataColumn[] primaryKeyColumns = new DataColumn[ 1 ];
                primaryKeyColumns[ 0 ] = requestIdColumn;

                // add the keys to the table
                dtExportUploadRequestStatus.PrimaryKey = primaryKeyColumns;

                dtExportUploadRequestStatus.Clear();

                dbConnection.Open();

                daExportUploadRequestStatus.Fill( dsExportUploadRequestStatus, ExportUploadRequestStatusTableName );
                rowCount = dtExportUploadRequestStatus.Rows.Count;             
            }
            catch( Exception ex )
            {
                ErrorMessage = String.Format( "The following exception was encountered in ExportDB.GetExportUploadRequestStatus(): {0}", ex.Message );
                bSuccess = false;
            }
            finally
            {
                if( dbConnection != null )
                    dbConnection.Close();
            }

            return ( bSuccess );
        }
    }
}
