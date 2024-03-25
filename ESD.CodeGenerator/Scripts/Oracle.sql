select 
       col.column_name as Name, 
       col.column_name as Title, --todo 
       col.data_type, 
       --col.data_length,  
       case when col.nullable  = 'N' then 'false' else 'true' end as Nullable 
from sys.all_tab_columns col
inner join sys.all_tables t on col.owner = t.owner 
                              and col.table_name = t.table_name
where col.owner = 'SchemaName'
and col.table_name = 'TableName'
order by col.column_id;
