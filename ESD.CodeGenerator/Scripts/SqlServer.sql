 SELECT COLUMN_NAME AS Name 
    ,prop.value AS Title, 
	case 
		when CHARACTER_MAXIMUM_LENGTH = -1 then DATA_TYPE +'(MAX)'
		when ISNULL(convert(varchar(20),CHARACTER_MAXIMUM_LENGTH),0) = 0  
		then  DATA_TYPE else DATA_TYPE +'(' + convert(varchar(20), CHARACTER_MAXIMUM_LENGTH) +')'
	END AS DataType, 
	case when col.IS_NULLABLE  = 'NO' then 'false' else 'true' end as Nullable 

FROM INFORMATION_SCHEMA.TABLES AS tbl
INNER JOIN INFORMATION_SCHEMA.COLUMNS AS col ON col.TABLE_NAME = tbl.TABLE_NAME
INNER JOIN sys.columns AS sc ON sc.object_id = object_id(tbl.table_schema + '.' + tbl.table_name)
    AND sc.NAME = col.COLUMN_NAME
LEFT JOIN sys.extended_properties prop ON prop.major_id = sc.object_id
    AND prop.minor_id = sc.column_id
    AND prop.NAME = 'MS_Description'
WHERE tbl.TABLE_NAME  = 'TableName' 
