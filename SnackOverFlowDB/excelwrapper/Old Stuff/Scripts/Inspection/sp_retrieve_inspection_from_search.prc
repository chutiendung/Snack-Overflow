USE [SnackOverflowDB]
GO
IF EXISTS(SELECT * FROM sys.objects WHERE type = 'P' AND  name = 'sp_retrieve_inspection_from_search')
BEGIN
Drop PROCEDURE sp_retrieve_inspection_from_search
Print '' print  ' *** dropping procedure sp_retrieve_inspection_from_search'
End
GO

Print '' print  ' *** creating procedure sp_retrieve_inspection_from_search'
GO
Create PROCEDURE sp_retrieve_inspection_from_search
(
@INSPECTION_ID[INT]=NULL,
@EMPLOYEE_ID[INT]=NULL,
@PRODUCT_LOT_ID[INT]=NULL,
@GRADE_ID[NVARCHAR](250)=NULL,
@DATE_PERFORMED[DATETIME]=NULL,
@EXPIRATION_DATE[DATETIME]=NULL
)
AS
BEGIN
Select INSPECTION_ID, EMPLOYEE_ID, PRODUCT_LOT_ID, GRADE_ID, DATE_PERFORMED, EXPIRATION_DATE
FROM INSPECTION
WHERE (INSPECTION.INSPECTION_ID=@INSPECTION_ID OR @INSPECTION_ID IS NULL)
AND (INSPECTION.EMPLOYEE_ID=@EMPLOYEE_ID OR @EMPLOYEE_ID IS NULL)
AND (INSPECTION.PRODUCT_LOT_ID=@PRODUCT_LOT_ID OR @PRODUCT_LOT_ID IS NULL)
AND (INSPECTION.GRADE_ID=@GRADE_ID OR @GRADE_ID IS NULL)
AND (INSPECTION.DATE_PERFORMED=@DATE_PERFORMED OR @DATE_PERFORMED IS NULL)
AND (INSPECTION.EXPIRATION_DATE=@EXPIRATION_DATE OR @EXPIRATION_DATE IS NULL)
END