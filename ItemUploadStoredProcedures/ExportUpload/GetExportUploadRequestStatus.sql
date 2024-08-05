IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'GetExportUploadRequestStatus')
	BEGIN
		DROP  Procedure  GetExportUploadRequestStatus
	END
GO

CREATE Procedure GetExportUploadRequestStatus
(
@RequestId int
)

AS

Declare 	@error int,
		@rowCount int,
		@errorMsg nvarchar(1000)
		

BEGIN TRANSACTION

		select r.RequestId, r.RequestStatus, r.RequestingServerName, r.RequestType, r.RequestSubType, r.CreatedBy, r.CreationDate, r.ApplicationVersion, r.ActivityId, r.ActivityDetailsId, 
				r.SalesExportUploadId, r.SelectedFiscalYear, r.SelectedFiscalQuarter, r.GuidForCurrentModification, r.ContractNumber, r.ContractId, r.ScheduleNumber, 
				r.StartDateCriteria, r.EndDateCriteria, r.EncodedCriteria, r.SourceFileType, r.SourceFileExt, r.SourceFileName, r.SourceFilePathName, 
				r.DestFileType, r.DestFileExt, r.DestFileName, r.DestFilePathName, r.StatusMessage, r.CompletionLevel, r.LastModificationDate
		
		from EU_Requests r 
		where r.RequestId = @RequestId

		select @error = @@ERROR, @rowCount = @@ROWCOUNT
		if @error <> 0 or @rowCount <> 1
		BEGIN
			select @errorMsg = 'Error retrieving export/upload request status for RequestId = ' + convert( nvarchar(20), @RequestId )
			goto ERROREXIT
		END


goto OKEXIT

ERROREXIT:

	raiserror( @errorMsg, 16, 1 )
	if @@TRANCOUNT > 1
	BEGIN
		COMMIT TRANSACTION
	END
	Else if @@TRANCOUNT = 1
	BEGIN
		/* only rollback iff this is the highest level */
		ROLLBACK TRANSACTION
	END

	RETURN( -1 )

OKEXIT:

	If @@TRANCOUNT > 0
	BEGIN
		COMMIT TRANSACTION
	END
	RETURN( 0 )



